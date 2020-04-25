using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Parts.Logical
{
    public class UnpoweredGateException : Exception
    {
        public UnpoweredGateException()
        {
        }

        public UnpoweredGateException(string message) : base(message)
        {
        }

        public UnpoweredGateException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
