using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public abstract class Gate
    {
        private readonly string name;

        public Gate(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
