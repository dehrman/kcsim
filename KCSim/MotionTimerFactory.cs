using System;
using System.Collections.Generic;
using System.Text;

namespace KCSim
{
    public class MotionTimerFactory
    {
        public IMotionTimer CreateNew()
        {
            return new MotionTimer();
        }
    }
}
