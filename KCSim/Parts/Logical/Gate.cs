using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public abstract class Gate
    {
        private readonly string name;

        public readonly Axle Power;
        public readonly Axle InputA;
        public readonly Axle Input;
        public readonly Axle InputB;
        public readonly Axle Output;
        public readonly Axle OutputInverse;

        public Gate(string name)
        {
            this.name = name;

            Power = new Axle(name + " power");
            InputA = new Axle(name + " input A");
            Input = InputA;
            InputB = new Axle(name + " input B");
            Output = new Axle(name + " output");
            OutputInverse = new Axle(name + " output inverse");
        }

        public override string ToString()
        {
            return name;
        }

        public abstract bool RequiresPower();
    }
}
