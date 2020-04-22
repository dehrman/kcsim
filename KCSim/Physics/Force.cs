using System;
namespace KCSim.Physics
{
    /*
     * Here force is represented as an angular velocity. Yes, force is mass times acceleration, but in practice, actual
     * acceleration is negligible, and motors yield constant velocity, so it's more useful to think of force in this
     * context as an angular velocity.
     */
    public class Force : IComparable
    {
        public static Force ZeroForce = new Force(0);

        public double Velocity { get; }

        public Force(double velocity)
        {
            Velocity = velocity;
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Force other = obj as Force;

            if (other == null)
            {
                throw new ArgumentException("Object is not a Force");
            }

            return Math.Abs(this.Velocity).CompareTo(Math.Abs(other.Velocity));
        }

        public override bool Equals(object obj)
        {
            return obj is Force force &&
                   Velocity == force.Velocity;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Velocity);
        }

        public override string ToString()
        {
            return Velocity.ToString();
        }
    }
}
