using Xunit;

namespace IntegerDataStructures.Test
{
    public class VanEmdeBoasTests
    {
        [Theory]
        [InlineData(2)]
        [InlineData(16)]
        [InlineData(128)]
        public void VanEmdeBoasNodeTest(int universeSize)
        {
            var node = new VanEmdeBoasNode<string>(universeSize);

            for (int i = 0; i < universeSize; ++i)
            {
                Assert.False(node.Contains(i));
            }

            for (int i = 0; i < universeSize; ++i)
            {
                node.Insert(i, i.ToString());
            }

            Assert.Equal(universeSize, node.Count);

            for (int i = universeSize-1; i > 0; i-=3)
            {
                Assert.True(node.Contains(i));
                Assert.Equal(i.ToString(), node.GetValue(i));
            }

            for (int i = 0; i < universeSize; ++i)
            {
                Assert.True(node.Contains(i));
                Assert.Equal(i.ToString(), node.GetValue(i));
            }
        }

        [Fact]
        public void VanEmdeBoasNodeInsertTest()
        {
            var node = new VanEmdeBoasNode<int>(64);
            Assert.Equal(0, node.Count);

            Assert.False(node.Contains(1));
            Assert.True(node.Insert(1, 11));
            Assert.Equal(11, node.GetValue(1));
            Assert.True(node.Contains(1));

            Assert.False(node.Insert(1, 10));
            Assert.Equal(10, node.GetValue(1));
            Assert.True(node.Contains(1));

            Assert.False(node.Contains(50));
            Assert.True(node.Insert(50, 101));
            Assert.Equal(101, node.GetValue(50));
            Assert.True(node.Contains(50));

            Assert.Equal(2, node.Count);
        }
    }
}
