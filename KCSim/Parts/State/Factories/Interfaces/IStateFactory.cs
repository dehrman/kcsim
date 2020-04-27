using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;

namespace KCSim.Parts.State
{
    public interface IStateFactory
    {
        SRLatch CreateNewSRLatch(string name = null);

        GatedDLatch CreateNewGatedDLatch(string name = null);
    }
}
