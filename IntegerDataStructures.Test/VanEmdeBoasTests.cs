using System;
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
            var tree = new VanEmdeBoasTree<string>(universeSize);

            Assert.Equal(universeSize, tree.Capacity);

            for (int i = 0; i < universeSize; ++i)
            {
                Assert.False(tree.Contains(i));
            }

            for (int i = 0; i < universeSize; ++i)
            {
                tree.Insert(i, i.ToString());
            }

            Assert.Equal(universeSize, tree.Count);

            for (int i = universeSize-1; i > 0; i-=3)
            {
                Assert.True(tree.Contains(i));
                Assert.Equal(i.ToString(), tree.GetValue(i));
            }

            for (int i = 0; i < universeSize; ++i)
            {
                Assert.True(tree.Contains(i));
                Assert.Equal(i.ToString(), tree.GetValue(i));
            }
        }

        [Fact]
        public void VanEmdeBoasNodeInsertTest()
        {
            var tree = new VanEmdeBoasTree<int>(64);
            Assert.Equal(0, tree.Count);

            Assert.False(tree.Contains(1));
            Assert.True(tree.Insert(1, 11));
            Assert.Equal(11, tree.GetValue(1));
            Assert.True(tree.Contains(1));

            Assert.False(tree.Insert(1, 10));
            Assert.Equal(10, tree.GetValue(1));
            Assert.True(tree.Contains(1));

            Assert.False(tree.Contains(50));
            Assert.True(tree.Insert(50, 101));
            Assert.Equal(101, tree.GetValue(50));
            Assert.True(tree.Contains(50));

            Assert.Equal(2, tree.Count);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(10, 16)]
        [InlineData(50, 64)]
        [InlineData(100, 128)]
        public void VanEmdeBoasNodeDeleteTest(int capacity, int expectedCapacity)
        {
            var tree = new VanEmdeBoasTree<int>(capacity);
            Assert.Equal(expectedCapacity, tree.Capacity);

            for (int i = 0; i < tree.Capacity; ++i)
            {
                tree.Insert(i, 100*i);
            }

            int numDeleted = 0;
            for (int i = 0; i < tree.Capacity; i += 3)
            {
                ++numDeleted;
                Assert.True(tree.Delete(i));
                Assert.Equal(tree.Capacity - numDeleted, tree.Count);
            }

            for (int i = tree.Capacity - 2; i >= 0; i -= 2)
            {
                bool toDelete = i % 3 != 0;
                
                Assert.Equal(toDelete, tree.Delete(i));

                if (toDelete)
                {
                    ++numDeleted;
                }
            }

            Assert.Equal(tree.Capacity - numDeleted, tree.Count);

            for (int i = 0; i < tree.Capacity; i += 2)
            {
                bool deleted = (i % 2 == 0) || (i % 3 == 0);
                Assert.Equal(!deleted, tree.Contains(i));
                Assert.Equal(deleted ? 0 : i * 100, tree.GetValue(i));
            }
        }

        [Fact]
        public void OutOfRanngeKeyTest()
        {
            var tree = new VanEmdeBoasTree<int>(64);

            Assert.Throws<ArgumentOutOfRangeException>(() => tree.GetValue(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.GetValue(100));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.Contains(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.Contains(100));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.Delete(-1));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.Delete(100));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.Insert(-1, 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => tree.Insert(100, 1));
        }

        [Fact]
        public void NextKeyTest()
        {
            var tree = new VanEmdeBoasTree<int>(111);
            tree.Insert(100, 1100);
            tree.Insert(77, 1077);
            tree.Insert(33, 1033);
            tree.Insert(55, 1055);

            Assert.Null(tree.NextKey(100));
            Assert.Equal(100, tree.NextKey(77));
            Assert.Equal(33, tree.NextKey(10));
            Assert.Equal(55, tree.NextKey(33));
            Assert.Equal(100, tree.NextKey(77));

            Assert.True(tree.Delete(33));
            Assert.Equal(55, tree.NextKey(10));

            for (int i = 0; i < tree.Capacity; ++i)
            {
                tree.Insert(i, i * 100);
            }

            for (int i = 0; i < tree.Capacity - 1; ++i)
            {
                Assert.Equal(i + 1, tree.NextKey(i));
            }

            Assert.Null(tree.NextKey(tree.Capacity - 1));
        }

        [Fact]
        public void PreviousKeyTest()
        {
            var tree = new VanEmdeBoasTree<int>(111);
            tree.Insert(100, 1100);
            tree.Insert(77, 1077);
            tree.Insert(33, 1033);
            tree.Insert(55, 1055);

            Assert.Null(tree.PreviousKey(33));
            Assert.Equal(77, tree.PreviousKey(100));
            Assert.Equal(33, tree.PreviousKey(55));
            Assert.Equal(100, tree.PreviousKey(110));

            for (int i = 0; i < tree.Capacity; ++i)
            {
                tree.Insert(i, i * 100);
            }

            for (int i = 1; i < tree.Capacity; ++i)
            {
                Assert.Equal(i - 1, tree.PreviousKey(i));
            }

            Assert.Null(tree.PreviousKey(0));
        }

    }
}
