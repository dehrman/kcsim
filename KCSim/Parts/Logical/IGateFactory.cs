using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Parts.Logical
{
    public interface IGateFactory
    {
        AndGate CreateNewAndGate();

        OrGate CreateNewOrGate();

        NotGate CreateNewNotGate();

        NandGate CreateNewNandGate();

        XorGate CreateNewXorGate();

        NorGate CreateNewNorGate();

        Buffer CreateNewBuffer();
    }
}
