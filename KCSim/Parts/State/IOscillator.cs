using System;
using System.Collections.Generic;
using System.Text;
using KCSim.Parts.Mechanical.Atomic;
using KCSim.Physics;

namespace KCSim.Parts.State
{
    public interface IOscillator
    {
        public delegate void OnQChangedDelegate(Force oldQ, Force newQ);

        public ISet<OnQChangedDelegate> GetOnQChangedDelegateSet();

        public void CouplePowerInput(Torqueable powerInput);

        public Axle GetOutput();
    }
}
