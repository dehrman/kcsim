using System;
using System.Collections.Generic;
using System.Text;
using static KCSim.Parts.Mechanical.Paddle;

namespace KCSim.Parts.Mechanical
{
    public interface IBidirectionalLatchFactory
    {
        public BidirectionalLatch CreateNew(string name = "");
    }
}
