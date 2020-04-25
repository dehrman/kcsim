using System.Diagnostics.Contracts;

namespace KCSim
{
    public class BitMath
    {
        public static int GetNumSelectBitsRequired(int numInputs)
        {
            Contract.Requires(numInputs > 1, "Only valid for numInputs > 1");
            int numSelectBits = 1;
            int maxSelectableInputs = 1 << numSelectBits;
            while (maxSelectableInputs < numInputs)
            {
                maxSelectableInputs <<= 1;
                numSelectBits++;
            }
            return numSelectBits;
        }

        /// <summary>
        /// Convert a number into a bit vector represented as an array of boolean values.
        ///
        /// For example, 9 will result in the boolean array, {true, false, false, true}.
        /// </summary>
        /// <param name="numBits">the number of bits in the resulting bit vector</param>
        /// <param name="number">the number to be converted into the bit vector</param>
        /// <returns>a new boolean array containing the number represented in binary format
        /// using true and false for 0 and 1</returns>
        public static bool[] GetBitVector(int numBits, int number)
        {
            bool[] bitVector = new bool[numBits];
            for (int i = 0; i < numBits; i++)
            {
                bitVector[i] = (number & 1) != 0;
                number >>= 1;
            }
            return bitVector;
        }
    }
}
