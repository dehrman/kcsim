using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;

namespace KCSim
{
    public interface IGateMonitor
    {
        void ValidateNoUnpoweredGates();

        T RegisterGate<T>(T gate) where T : Gate;
    }
}
