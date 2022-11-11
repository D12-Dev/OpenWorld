using System;
using System.Collections.Generic;
using System.Text;

namespace OpenWorldServer
{
    public static class MathUtils
    {

        /**
         * Similar to Unity's Mathf.Approximately
         */ 
        public static bool Approximately(float a, float b)
        {
            return Math.Abs(a - b) < 0.0001f;
        }
    }
}
