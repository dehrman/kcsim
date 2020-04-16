using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Parts.Mechanical
{
    class RelayFactory : IRelayFactory
    {
        private readonly IPaddleFactory paddleFactory;

        public RelayFactory(IPaddleFactory paddleFactory)
        {
            this.paddleFactory = paddleFactory;
        }

        public Relay CreateNew(bool isControlPositiveDirection = true, bool isInputPositiveDirection = true)
        {
            return new Relay(paddleFactory: paddleFactory, isControlPositiveDirection: isControlPositiveDirection, isInputPositiveDirection: isInputPositiveDirection);
        }
    }
}
