using System;
using System.Collections.Generic;
using System.Text;

namespace Guardian_Roguelike.Utilities
{
    class InterStateResources
    {
        private static InterStateResources instance;
        public static InterStateResources Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new InterStateResources();
                }
                return instance;
            }
        }

        public Dictionary<string, object> Resources;

        private InterStateResources()
        {
            Resources = new Dictionary<string, object>();
        }
    }
}
