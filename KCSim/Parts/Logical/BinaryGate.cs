using KCSim.Parts.Mechanical.Atomic;

namespace KCSim.Parts.Logical
{
    public abstract class BinaryGate
    {
        private readonly string name;

        public BinaryGate(string name = "")
        {
            this.name = name;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
