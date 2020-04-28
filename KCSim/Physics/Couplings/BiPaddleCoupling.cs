using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim.Physics.Couplings
{
    public class BiPaddleCoupling : BidirectionalSymmetricalCoupling
    {
        public BiPaddleCoupling(
            Torqueable input,
            Torqueable output,
            string name = "") : base(input, output, name)
        {
        }
    }
}
