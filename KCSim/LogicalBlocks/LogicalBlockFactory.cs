using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Logical;

namespace KCSim.LogicalBlocks
{
    public class LogicalBlockFactory
    {
        private readonly ICouplingService couplingService;
        private readonly IGateFactory gateFactory;

        public LogicalBlockFactory(
            ICouplingService couplingService,
            IGateFactory gateFactory)
        {
            this.couplingService = couplingService;
            this.gateFactory = gateFactory;
        }

        public Mux CreateNewMux(int numInputs, string name = "mux")
        {
            return new Mux(couplingService, gateFactory, numInputs, name);
        }
    }
}
