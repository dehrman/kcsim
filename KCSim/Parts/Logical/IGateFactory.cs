using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Parts.Logical
{
    public interface IGateFactory
    {
        public AndGate CreateNewAndGate();
    }
}
