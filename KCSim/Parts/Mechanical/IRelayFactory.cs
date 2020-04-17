using System;
using System.Collections.Generic;
using System.Text;
using static KCSim.Parts.Mechanical.Paddle;

namespace KCSim.Parts.Mechanical
{
    public interface IRelayFactory
    {
        Relay CreateNew(bool isControlPositiveDirection = true, bool isInputPositiveDirection = true, string name = "");
    }
}
