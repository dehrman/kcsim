using System;
namespace KCSim.Parts.Mechanical
{
    /*
     * An arm rotates about an axle.
     *
     * If there is an opposing end of the arm (180 degrees rotated about the axle), that arm ought to be represented as
     * a separate arm, with both arms being locked to the same axle, 180 degrees out of phase.
     */
    public class Arm
    {
        public Axle EndAxle { get; set; }
        public Axle FulcrumAxle { get; set; }

        private readonly string name;

        public Arm(string name = "default arm name")
        {
            this.name = name;

            this.EndAxle = new Axle();
            this.FulcrumAxle = new Axle();
        }

        public override string ToString()
        {
            return "Arm: \"" + name + "\"";
        }
    }
}
