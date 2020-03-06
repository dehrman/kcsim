using System;
using static KCSim.Parts.Mechanical.Paddle;

namespace KCSim.Parts.Mechanical
{
    public interface IPaddleFactory
    {
        Paddle CreateNew(Position initialPosition = Position.Positive, string name = "default paddle name");
    }
}
