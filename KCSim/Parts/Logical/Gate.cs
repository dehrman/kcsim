using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public abstract class Gate
    {
        private readonly string name;

        public readonly Axle Power;

        public Gate(string name)
        {
            this.name = name;

            Power = new Axle(name + " power");
        }

        public override string ToString()
        {
            return name;
        }

        public abstract bool RequiresPower();
    }
}
