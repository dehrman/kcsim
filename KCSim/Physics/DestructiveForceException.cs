using System;
namespace KCSim.Physics
{
    public class DestructiveForceException : Exception
    {
        public DestructiveForceException()
        {
        }

        public DestructiveForceException(string message) : base(message)
        {
        }

        public DestructiveForceException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
