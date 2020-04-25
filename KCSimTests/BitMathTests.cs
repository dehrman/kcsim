using KCSim;
using Xunit;

namespace KCSimTests
{
    public class BitMathTests
    {
        public BitMathTests()
        {

        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(3, 2)]
        [InlineData(4, 2)]
        [InlineData(5, 3)]
        [InlineData(6, 3)]
        [InlineData(7, 3)]
        [InlineData(8, 3)]
        [InlineData(9, 4)]
        public void TestThat_GetNumSelectBitsRequired_YieldsExpectedResults(int numInputs, int expectedNumSelectBits)
        {
            Assert.Equal(expectedNumSelectBits, BitMath.GetNumSelectBitsRequired(numInputs));
        }
    }
}
