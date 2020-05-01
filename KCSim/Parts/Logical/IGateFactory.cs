using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Parts.Logical
{
    public interface IGateFactory
    {
        AndGate CreateNewAndGate(bool doMonitor = true, string name = null);

        OrGate CreateNewOrGate(bool doMonitor = true, string name = null);

        NotGate CreateNewNotGate(bool doMonitor = true, string name = null);

        NandGate CreateNewNandGate(bool doMonitor = true, string name = null);

        XorGate CreateNewXorGate(bool doMonitor = true, string name = null);

        NorGate CreateNewNorGate(bool doMonitor = true, string name = null);

        Buffer CreateNewBuffer(bool doMonitor = true, string name = null);

        MultiInputGate<AndGate> CreateNewMultiInputAndGate(int numInputs, bool doMonitor = true);

        MultiInputGate<OrGate> CreateNewMultiInputOrGate(int numInputs, bool doMonitor = true);
    }
}
