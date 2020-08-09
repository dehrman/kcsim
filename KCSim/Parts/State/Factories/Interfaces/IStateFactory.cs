using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;

namespace KCSim.Parts.State
{
    public interface IStateFactory
    {
        SRLatch CreateNewSRLatch(string name = "SR latch");

        GatedDLatch CreateNewGatedDLatch(string name = "gated D latch");

        DFlipFlop CreateNewDFlipFlop(string name = "D flip-flop");
    }
}
