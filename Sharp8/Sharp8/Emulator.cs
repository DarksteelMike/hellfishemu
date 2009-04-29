using System;
using System.Collections.Generic;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

using SdlDotNet.Core;
using SdlDotNet.Graphics;

namespace Sharp8_V3
{
    public class Emulator
    {
        #region Constants
        public const int OPCODESPERSECOND = 600;
        public const int OPCODESPERFRAME = OPCODESPERSECOND / 60;
        #endregion

        #region Properties
        /// <summary>
        /// The CPU registers
        /// </summary>
        private byte[] registers;
        public byte[] Registers
        {
            get { return registers; }
        }

        /// <summary>
        /// The program counter
        /// </summary>
        private ushort programCounter;
        public ushort ProgramCounter
        {
            get { return programCounter; }
        }

        /// <summary>
        /// The address register I
        /// </summary>
        private ushort addressRegister;
        public ushort AddressRegister
        {
            get { return addressRegister; }
        }

        /// <summary>
        /// The sound timer
        /// </summary>
        private byte soundTimer;
        public byte SoundTimer
        {
            get { return soundTimer; }
        }

        /// <summary>
        /// The delay timer
        /// </summary>
        private byte delayTimer;
        public byte DelayTimer
        {
            get { return delayTimer; }
        }

        /// <summary>
        /// Wether or not the emulator should do it's stuff
        /// </summary>
        private bool isPaused;
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        /// <summary>
        /// The memory handler
        /// </summary>
        private MemoryHandler memory;
        public MemoryHandler Memory
        {
            get { return memory; }
        }

        /// <summary>
        /// The input handler
        /// </summary>
        private InputHandler input;
        public InputHandler Input
        {
            get { return input; }
        }

        /// <summary>
        /// The currently loaded ROM
        /// </summary>
        private byte[] currentROM;
        public byte[] CurrentROM
        {
            get { return currentROM; }
        }

        /// <summary>
        /// The stack used for subroutine calls
        /// </summary>
        private Stack<ushort> programStack;
        public Stack<ushort> ProgramStack
        {
            get { return programStack; }
        }

        /// <summary>
        /// If we should use the higher precision graphics
        /// </summary>
        private bool isInSCHIPMode;
        public bool IsInSCHIPMode
        {
            get { return isInSCHIPMode; }
        }

        /// <summary>
        /// The output buffer
        /// </summary>
        private Surface surfaceOut;
        public Surface SurfaceOut
        {
            get { return surfaceOut; }
        }

        /// <summary>
        /// Wether a screen refresh should be performed every frame
        /// </summary>
        private bool forceUpdate;
        public bool ForceUpdate
        {
            get { return forceUpdate; }
            set { forceUpdate = value; }
        }

        /// <summary>
        /// Wether sound should be played
        /// </summary>
        private bool useSound;
        public bool UseSound
        {
            get { return useSound; }
            set { useSound = value; }
        }

        #endregion

        #region Fields
        private ushort CurrentOpcode;
        private byte FirstByte,SecondByte;
        private byte FirstNibble, SecondNibble, ThirdNibble, FourthNibble;
        private ushort LastThreeNibbles;
        
        private bool[,] ScreenData;
        private int TimeUntilTimerUpdate;
        private Random RandGen;
        private Surface PixelB, PixelW, SCHIPPixelB, SCHIPPixelW;
        private SdlDotNet.Audio.Sound BeepSound;
        private SdlDotNet.Audio.Channel BeepChan;
        private bool SoundStartedThisFrame;
        #endregion

        #region Constructors
        public Emulator(byte[] ROM,InputHandler IH)
        {
            Init();
            LoadROM(ROM);
            AttachInputHandler(IH);
            Reset();
        }
        public Emulator(byte[] ROM)
        {
            Init();
            LoadROM(ROM);
            Reset();
        }
        public Emulator()
        {
            Init();
            Reset();
        }
        #endregion

        private void Init()
        {
            useSound = true;
            byte[] BeepData = new byte[Sharp8.Properties.Resources.beep.Length];
            Sharp8.Properties.Resources.beep.Read(BeepData, 0, (int)Sharp8.Properties.Resources.beep.Length);
            BeepSound = new SdlDotNet.Audio.Sound(BeepData);
            BeepChan = BeepSound.Play(true);
            BeepChan.Pause();
            memory = new MemoryHandler();
            registers = new byte[0x10];
            RandGen = new Random();
            surfaceOut = Video.CreateRgbSurface(640, 320);
            PixelB = Video.CreateRgbSurface(10, 10);
            PixelW = Video.CreateRgbSurface(10, 10);
            SCHIPPixelB = Video.CreateRgbSurface(5, 5);
            SCHIPPixelW = Video.CreateRgbSurface(5, 5);
            PixelB.Fill(Color.Black);
            PixelW.Fill(Color.White);
            SCHIPPixelB.Fill(Color.Black);
            SCHIPPixelW.Fill(Color.White);
            ScreenData = new bool[128, 64];
            
            programStack = new Stack<ushort>(16);
        }

        public void LoadROM(byte[] ROM)
        {
            currentROM = ROM;
            memory.Write((ushort)0x200, currentROM);
        }

        public void AttachInputHandler(InputHandler IH)
        {
            input = IH;            
        }

        public void Reset()
        {
            SoundStartedThisFrame = false;
            memory.Clear();
            if (currentROM != null)
            {
                memory.Write((ushort)0x200, currentROM);
            }
            if (input != null)
            {
                input.ClearStates();
            }
            isInSCHIPMode = false;
            TimeUntilTimerUpdate = OPCODESPERFRAME;

            programCounter = (ushort)0x200;
            addressRegister = 0;
            soundTimer = delayTimer = 0;
            programStack.Clear();
            for (int i = 0; i < 0x10; i++)
            {
                registers[i] = 0;
            }
            for (int x = 0; x < 128; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    ScreenData[x, y] = false;
                }
            }
            surfaceOut.Fill(Color.Black);
        }

        /// <summary>
        /// This function runs one frame's worth of emulation
        /// </summary>
        /// <returns>True if the display surface has changed and needs updating. False otherwise.</returns>
        public bool DoOneFrame()
        {
            SoundStartedThisFrame = false;
            bool DoSurfaceUpdate = false;
            for (int i = 0; i < OPCODESPERFRAME; i++)
            {
                if (DoNextOpcode())
                {
                    DoSurfaceUpdate = true;
                    if (isPaused)
                    {
                       return DoSurfaceUpdate;
                    }
                }
            }

            return DoSurfaceUpdate;
        }

        #region Opcode functions
        public bool DoNextOpcode()
        {
            //Update timers if time is right
            if(TimeUntilTimerUpdate == 0)
            {
                if (soundTimer > 0)
                {
                    soundTimer--;
                    if (useSound && !SoundStartedThisFrame)
                    {
                        BeepChan.Play(BeepSound);
                        SoundStartedThisFrame = true;
                    }
                }
                if (delayTimer > 0)
                {
                    delayTimer--;
                }
                TimeUntilTimerUpdate = OPCODESPERFRAME;
            }
            else
            {
                TimeUntilTimerUpdate--;
            }

            CurrentOpcode = memory.ReadWord(programCounter);
            FirstByte = (byte)((CurrentOpcode & 0xFF00) >> 8);
            SecondByte = (byte)(CurrentOpcode & 0x00FF);
            FirstNibble = (byte)((CurrentOpcode & 0xF000) >> 12);
            SecondNibble = (byte)((CurrentOpcode & 0x0F00) >> 8);
            ThirdNibble = (byte)((CurrentOpcode & 0x00F0) >> 4);
            FourthNibble = (byte)(CurrentOpcode & 0x000F);
            LastThreeNibbles = (ushort)(CurrentOpcode & 0x0FFF);

            programCounter += 2;
            bool ReturnValue = false;
            switch (FirstNibble)
            {
                case(0x0):
                    ReturnValue = DoSubset0XXX();
                    break;
                case(0x1):
                    ReturnValue = Do1NNN();
                    break;
                case(0x2):
                    ReturnValue = Do2NNN();
                    break;
                case(0x3):
                    ReturnValue = Do3XKK();
                    break;
                case(0x4):
                    ReturnValue = Do4XKK();
                    break;
                case(0x5):
                    ReturnValue = Do5XY0();
                    break;
                case(0x6):
                    ReturnValue = Do6XKK();
                    break;
                case(0x7):
                    ReturnValue = Do7XKK();
                    break;
                case(0x8):
                    ReturnValue = DoSubset8XXX();
                    break;
                case(0x9):
                    ReturnValue = Do9XY0();
                    break;
                case(0xA):
                    ReturnValue = DoANNN();
                    break;
                case(0xB):
                    ReturnValue = DoBNNN();
                    break;
                case(0xC):
                    ReturnValue = DoCXKK();
                    break;
                case(0xD):
                    ReturnValue = DoDXYN();
                    break;
                case(0xE):
                    ReturnValue = DoSubsetEXXX();
                    break;
                case(0xF):
                    ReturnValue = DoSubsetFXXX();
                    break;

            }

            return ReturnValue;
        }

        private bool DoSubset0XXX()
        {
            //Subset
            bool ReturnValue = false; ;
            switch (ThirdNibble)
            {
                case (0xC):
                    ReturnValue = Do00CN();
                    break;

                case (0xE):
                    ReturnValue = DoSubset00EX();
                    break;

                case (0xF):
                    ReturnValue = DoSubset00FX();
                    break;
            }

            return ReturnValue;
        }

        private bool DoSubset00EX()
        {
            bool ReturnValue = false;
            //Subset
            switch (FourthNibble)
            {
                case (0x0):
                    ReturnValue = Do00E0();
                    break;
                case (0xE):
                    ReturnValue = Do00EE();
                    break;
            }

            return ReturnValue;
        }
        private bool DoSubset00FX()
        {
            bool ReturnValue = false;
            //Subset
            switch (FourthNibble)
            {
                case (0xB):
                    ReturnValue = Do00FB();
                    break;
                case (0xC):
                    ReturnValue = Do00FC();
                    break;
                case (0xD):
                    ReturnValue = Do00FD();
                    break;
                case (0xE):
                    ReturnValue = Do00FE();
                    break;
                case (0xF):
                    ReturnValue = Do00FF();
                    break;

            }

            return ReturnValue;
        }
        private bool DoSubset8XXX()
        {
            //Subset
            bool ReturnValue = false;
            switch (FourthNibble)
            {
                case(0x0):
                    ReturnValue = Do8XY0();
                    break;
                case (0x1):
                    ReturnValue = Do8XY1();
                    break;
                case (0x2):
                    ReturnValue = Do8XY2();
                    break;
                case (0x3):
                    ReturnValue = Do8XY3();
                    break;
                case (0x4):
                    ReturnValue = Do8XY4();
                    break;
                case (0x5):
                    ReturnValue = Do8XY5();
                    break;
                case (0x6):
                    ReturnValue = Do8XY6();
                    break;
                case (0x7):
                    ReturnValue = Do8XY7();
                    break;
                case (0xE):
                    ReturnValue = Do8XYE();
                    break;
            }

            return ReturnValue;
        }
        private bool DoSubsetEXXX()
        {
            bool ReturnValue = false;
            //Subset
            switch (SecondByte)
            {
                case(0x9E):
                    ReturnValue = DoEX9E();
                    break;
                case(0xA1):
                    ReturnValue = DoEXA1();
                    break;
            }

            return ReturnValue;
        }
        private bool DoSubsetFXXX()
        {
            bool ReturnValue = false;
            //Subset
            switch (SecondByte)
            {
                case(0x07):
                    ReturnValue = DoFX07();
                    break;
                case(0x0A):
                    ReturnValue = DoFX0A();
                    break;
                case(0x15):
                    ReturnValue = DoFX15();
                    break;
                case(0x18):
                    ReturnValue = DoFX18();
                    break;
                case(0x1E):
                    ReturnValue = DoFX1E();
                    break;
                case(0x29):
                    ReturnValue = DoFX29();
                    break;
                case(0x30):
                    ReturnValue = DoFX30();
                    break;
                case (0x33):
                    ReturnValue = DoFX33();
                    break;
                case(0x55):
                    ReturnValue = DoFX55();
                    break;
                case(0x65):
                    ReturnValue = DoFX65();
                    break;
            }

            return ReturnValue;
        }

        private bool Do00CN()
        {
            //00CN: Scroll down N lines
            for (int y = 63; y >= 0; y--)
            {
                for (int x = 0; x < 128; x++)
                {
                    if (y >= FourthNibble)
                    {
                        ScreenData[x, y] = ScreenData[x, y - FourthNibble];
                    }
                    else
                    {
                        ScreenData[x, y] = false;
                    }
                }
            }
            ScreenDataToSurface();
            return true;
        }
        private bool Do00E0()
        {
            //00E0: Clear the screen
            surfaceOut.Fill(Color.Black);
            surfaceOut.Update();

            for (int x = 0; x < 128; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    ScreenData[x, y] = false;
                }
            }
            return true;
        }
        private bool Do00EE()
        {
            //00EE: Return from a subroutine
            programCounter = programStack.Pop();
            return false;
        }
        private bool Do00FB()
        {
            //00FC: Scroll 4 pixels right (2 if not in SCHIP mode)
            if (isInSCHIPMode)
            {
                for (int x = 0; x < 128; x++)
                {
                    for (int y = 0; y < 64; y++)
                    {
                        if (x >= 4)
                        {
                            ScreenData[x, y] = ScreenData[x - 4, y];
                        }
                        else
                        {
                            ScreenData[x, y] = false;
                        }
                    }
                }
            }
            else
            {
                for (int x = 63; x >= 0; x--)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (x >= 2)
                        {
                            ScreenData[x, y] = ScreenData[x - 2, y];
                        }
                        else
                        {
                            ScreenData[x, y] = false;
                        }
                    }
                }
            }
            ScreenDataToSurface();
            return true;
        }
        private bool Do00FC()
        {
            //00FB: Scroll 4 pixels left (2 if not in SCHIP mode)
            if (isInSCHIPMode)
            {
                for (int x = 0; x < 128; x++)
                {
                    for (int y = 0; y < 64; y++)
                    {
                        if (x <= 123)
                        {
                            ScreenData[x, y] = ScreenData[x + 4, y];
                        }
                        else
                        {
                            ScreenData[x, y] = false;
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < 64; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (x <= 61)
                        {
                            ScreenData[x, y] = ScreenData[x + 2, y];
                        }
                        else
                        {
                            ScreenData[x, y] = false;
                        }
                    }
                }
            }
            ScreenDataToSurface();
            return true;
        }
        private bool Do00FD()
        {
            //00FD: Quite the emulator
            System.Windows.Forms.MessageBox.Show("Quit by opcode!");
            Reset();
            isPaused = true;
            return false;
        }
        private bool Do00FE()
        {
            //00FE: Set CHIP8 mode
            isInSCHIPMode = false;
            return false;
        }
        private bool Do00FF()
        {
            //00FF: Set SCHIP mode
            isInSCHIPMode = true;
            return false;
        }
        private bool Do1NNN()
        {
            //1NNN: Jump to NNN
            programCounter = LastThreeNibbles;
            return false;
        }
        private bool Do2NNN()
        {
            //2NNN: Call subroutine at NNN
            programStack.Push(programCounter);
            programCounter = LastThreeNibbles;
            return false;
        }
        private bool Do3XKK()
        {
            //3XKK: Skip next instruction if VX == KK
            if (registers[SecondNibble] == SecondByte)
            {
                programCounter += 2;
            }
            return false;
        }
        private bool Do4XKK()
        {
            //4XKK: Skip next instruction if VX != KK
            if (registers[SecondNibble] != SecondByte)
            {
                programCounter += 2;
            }
            return false;
        }
        private bool Do5XY0()
        {
            //5XY0: Skip next instruction if VX == VY
            if (registers[SecondNibble] == registers[ThirdNibble])
            {
                programCounter += 2;
            }
            return false;
        }
        private bool Do6XKK()
        {
            //6XKK: VX = KK
            registers[SecondNibble] = SecondByte;
            return false;
        }
        private bool Do7XKK()
        {
            //7XKK: VX += KK. Ignore carry
            for (int i = 0; i < SecondByte; i++)
            {
                if (registers[SecondNibble] == 255)
                {
                    registers[SecondNibble] = 0;
                    continue;
                }
                registers[SecondNibble]++;
                              
            }
            return false;
        }
        private bool Do8XY0()
        {
            //8XY0: VX = VY
            registers[SecondNibble] = registers[ThirdNibble];
            return false;
        }
        private bool Do8XY1()
        {
            //8XY1: VX |= VY
            registers[SecondNibble] |= registers[ThirdNibble];
            return false;
        }
        private bool Do8XY2()
        {
            //8XY2: VX &= VY
            registers[SecondNibble] &= registers[ThirdNibble];
            return false;
        }
        private bool Do8XY3()
        {
            //8XY3: VX ^= VY
            registers[SecondNibble] ^= registers[ThirdNibble];
            return false;
        }
        private bool Do8XY4()
        {
            //8XY4: VX += VY. VF = Carry
            registers[0xF] = 0;
            byte Limit = registers[ThirdNibble];
            for (int i = 0; i < Limit; i++)
            {
                if (registers[SecondNibble] == 255)
                {
                    registers[SecondNibble] = 0;
                    registers[0xF] = 1;
                }
                else
                {
                    registers[SecondNibble]++;
                }
            }
            return false;
        }
        private bool Do8XY5()
        {
            //8XY5: VX -= VY. VF = Not Borrow
            registers[0xF] = 1;
            byte Limit = registers[ThirdNibble];
            for (int i = 0; i < Limit; i++)
            {
                if (registers[SecondNibble] == 0)
                {
                    registers[SecondNibble] = 255;
                    registers[0xF] = 0;
                }
                else
                {
                    registers[SecondNibble]--;
                }
            }
            return false;
        }
        private bool Do8XY6()
        {
            //8XY6: VX >>= 1. VF = Carry
            registers[0xF] = (byte)(registers[SecondNibble] & 1);
            registers[SecondNibble] >>= 1;
            return false;
        }
        private bool Do8XY7()
        {
            //8XY7: VX = VY - VX. VF = Not Borrow
            byte Limit = registers[SecondNibble];
            registers[SecondNibble] = registers[ThirdNibble];
            registers[0xF] = 1;
            for (int i = 0; i < Limit; i++)
            {
                if (registers[SecondNibble] == 0)
                {
                    registers[SecondNibble] = 255;
                    registers[0xF] = 0;
                }
                else
                {
                    registers[SecondNibble]--;
                }
            }
            return false;
        }
        private bool Do8XYE()
        {
            //8XYE: VX <<= 1. VF = Carry
            registers[0xF] = (byte)((registers[SecondNibble] & 0x80) >> 7);
            registers[SecondNibble] <<= 1;

            return false;
        }
        private bool Do9XY0()
        {
            //9XY0: Skip next instruction if VX != VY
            if (registers[SecondNibble] != registers[ThirdNibble])
            {
                programCounter += 2;
            }
            return false;
        }
        private bool DoANNN()
        {
            //ANNN: I = NNN
            addressRegister = LastThreeNibbles;
            return false;
        }
        private bool DoBNNN()
        {
            //BNNN: Unconditional jump to NNN + V0
            programCounter = (ushort)(LastThreeNibbles + registers[0]);
            return false;
        }
        private bool DoCXKK()
        {
            //CXKK: VX = Random Number & KK
            registers[SecondNibble] = (byte)((byte)RandGen.Next(0, 256) & SecondByte);
            return false;
        }
        private bool DoDXYN()
        {
            registers[0xF] = 0;
            if (isInSCHIPMode)
            {
            }
            else
            {                
                for (int x = 0; x < 8; x++)
                {
                    if (x + registers[SecondNibble] > 63)
                    {
                        break; //Stop if we've drawn off the right of the screen.
                    }
                    for (int y = 0; y < (int)FourthNibble; y++)
                    {
                        if (y + registers[ThirdNibble] > 31)
                        {
                            break; //Stop if we've drawn below the bottom of the screen.
                        }

                        if (((memory.ReadByte((ushort)(addressRegister + y)) & (0x80 >> x)) >> (7 - x)) == 1) //If bit is set (Should toggle)
                        {
                            if (ScreenData[x + registers[SecondNibble], y + registers[ThirdNibble]])
                            {
                                ScreenData[x + registers[SecondNibble], y + registers[ThirdNibble]] = false;
                                registers[0xF] = 1;
                                surfaceOut.Blit(PixelB, new Point((x + registers[SecondNibble]) * 10, (y + registers[ThirdNibble]) * 10));
                            }
                            else
                            {
                                ScreenData[x + registers[SecondNibble], y + registers[ThirdNibble]] = true;
                                surfaceOut.Blit(PixelW, new Point((x + registers[SecondNibble]) * 10, (y + registers[ThirdNibble]) * 10));
                            }
                        }
                    }
                }
            }
            surfaceOut.Update();
            return true;
        }
        private bool DoEX9E()
        {
            //EX9E: Skip next instruction if key in VX is down
            if (input.KeyStates[registers[SecondNibble]])
            {
                programCounter += 2;
            }
            return false;
        }
        private bool DoEXA1()
        {
            //EXA1: Skip next instruction if key in VX is up
            if (!input.KeyStates[registers[SecondNibble]])
            {
                programCounter += 2;
            }
            return false;
        }
        private bool DoFX07()
        {
            //FX07: VX = Delay Timer
            registers[SecondNibble] = delayTimer;
            return false;
        }
        private bool DoFX0A()
        {
            //FX0A: Wait for keypress and store it in VX
            bool keyfound = false;
            for (int i = 0; i < 0x10; i++)
            {
                if (input.KeyStates[i])
                {
                    keyfound = true;
                    registers[SecondNibble] = (byte)i;
                }
            }
            if (!keyfound)
            {
                programCounter -= 2;
            }

            return false;
        }
        private bool DoFX15()
        {
            //FX15: Delay timer = VX
            delayTimer = registers[SecondNibble];
            return false;
        }
        private bool DoFX18()
        {
            //FX18: Sound timer = VX
            soundTimer = registers[SecondNibble];
            return false;
        }
        private bool DoFX1E()
        {
            //FX1E: I += VX
            addressRegister += registers[SecondNibble];
            return false;
        }
        private bool DoFX29()
        {
            //FX29: Set I to address of 4x5 font sprite for character in VX
            addressRegister = (ushort)(registers[SecondNibble] * 5);
            return false;
        }
        private bool DoFX30()
        {
            //FX30: Set I to address of 8x10 font sprite for character in VX
            //TODO: Implement
            return false;
        }
        private bool DoFX33()
        {
            //FX33: Store BCD representation of VX in memory at I,I+1 & I+2
            int hundreds, tens, ones;

            hundreds = (int)(registers[SecondNibble] / 100);
            tens = (int)((registers[SecondNibble] / 10) % 10);
            ones = (int)(registers[SecondNibble] % 10);

            Memory.Write(addressRegister, (byte)hundreds);
            Memory.Write((ushort)(addressRegister+1), (byte)tens);
            Memory.Write((ushort)(addressRegister + 2), (byte)ones);
            //addressRegister += 3;
            return false;
        }
        private bool DoFX55()
        {
            //FX55: Save V0 to VX in memory at I to I+X
            for (int i = 0; i <= SecondNibble; i++)
            {
                memory.Write((ushort)(addressRegister + i), registers[i]);
            }
            return false;
        }
        private bool DoFX65()
        {
            //FX65: Load V0 to VX from memory at I to I+X
            for (int i = 0; i <= SecondNibble; i++)
            {
                registers[i] = memory.ReadByte((ushort)(addressRegister + i));
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Draws the screen data to the output surface.
        /// </summary>
        private void ScreenDataToSurface()
        {
            surfaceOut.Fill(Color.Black);
            if (isInSCHIPMode)
            {
                for (int x = 0; x < 128; x++)
                {
                    for (int y = 0; y < 64; y++)
                    {
                        if (ScreenData[x, y])
                        {
                            surfaceOut.Blit(SCHIPPixelW, new Point(x * 5, y * 5));
                        }
                    }
                }
            }
            else
            {
                for (int x = 0; x < 64; x++)
                {
                    for (int y = 0; y < 32; y++)
                    {
                        if (ScreenData[x, y])
                        {
                            surfaceOut.Blit(PixelW, new Point(x * 10, y * 10));
                        }
                    }
                }
            }

            surfaceOut.Update();
        }
    }
}