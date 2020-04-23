using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.State;

namespace KCSim
{
    public interface IStateMonitor
    {
        T RegisterStatefulGate<T>(T gate) where T : StatefulGate;
    }
}
