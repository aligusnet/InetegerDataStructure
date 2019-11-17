using Xunit;

namespace IntegerDataStructures.Test
{
    public class ProtoVanEmdeBoasTests
    {
        [Fact]
        public void DeconstructKeyTest()
        {
            Assert.Equal((0, 0), ProtoVanEmdeBoasNode<string>.DeconstructKey(0, 256));
            Assert.Equal((0, 1), ProtoVanEmdeBoasNode<string>.DeconstructKey(1, 256));
            Assert.Equal((1, 0), ProtoVanEmdeBoasNode<string>.DeconstructKey(2, 2));
            Assert.Equal((2, 3), ProtoVanEmdeBoasNode<string>.DeconstructKey(11, 4));
            Assert.Equal((3, 2), ProtoVanEmdeBoasNode<string>.DeconstructKey(14, 4));
            Assert.Equal((3, 3), ProtoVanEmdeBoasNode<string>.DeconstructKey(15, 4));
        }

        [Fact]
        public void ProtoVanEmdeBoasSmokeTest()
        {
            var tree = new ProtoVanEmdeBoasTree<string>(111);

            Assert.True(tree.Insert(79, "value 79"));
            Assert.Equal("value 79", tree.GetValue(79));

            Assert.False(tree.Insert(79, "value 79 - 2"));
            Assert.Equal("value 79 - 2", tree.GetValue(79));

            Assert.True(tree.Insert(16, "value 16"));
            Assert.Equal("value 16", tree.GetValue(16));

            Assert.Equal(2, tree.Count);

            Assert.False(tree.Delete(20));
            Assert.Equal(2, tree.Count);

            Assert.True(tree.Delete(16));
            Assert.Equal(1, tree.Count);

            Assert.False(tree.Delete(16));
            Assert.Equal(1, tree.Count);

            Assert.True(tree.Delete(79));
            Assert.Equal(0, tree.Count);

            Assert.False(tree.Delete(79));
            Assert.Equal(0, tree.Count);
        }

        [Fact]
        public void TrivialTreeTest()
        {
            var tree = new ProtoVanEmdeBoasTree<int>(2);

            Assert.Equal(0, tree.Count);

            Assert.True(tree.Insert(0, 100));
            Assert.Equal(100, tree.GetValue(0));
            Assert.Equal(1, tree.Count);

            Assert.False(tree.Insert(0, 10));
            Assert.Equal(10, tree.GetValue(0));
            Assert.Equal(1, tree.Count);

            Assert.True(tree.Insert(1, 20));
            Assert.Equal(20, tree.GetValue(1));
            Assert.Equal(2, tree.Count);
        }

        [Fact]
        public void MinimumKeyTrivialTest()
        {
            var tree = new ProtoVanEmdeBoasTree<int>(2);
            Assert.Null(tree.MinimumKey);

            tree.Insert(1, 100);
            Assert.Equal(1, tree.MinimumKey);

            tree.Insert(0, 200);
            Assert.Equal(0, tree.MinimumKey);
        }

        [Fact]
        public void MinimumKeyTest()
        {
            var tree = new ProtoVanEmdeBoasTree<int>(111);
            Assert.Null(tree.MinimumKey);

            tree.Insert(100, 1100);
            Assert.Equal(100, tree.MinimumKey);

            tree.Insert(77, 1077);
            Assert.Equal(77, tree.MinimumKey);

            tree.Insert(33, 1033);
            Assert.Equal(33, tree.MinimumKey);

            tree.Insert(55, 1055);
            Assert.Equal(33, tree.MinimumKey);
        }

        [Fact]
        public void MaximumKeyTrivialTest()
        {
            var tree = new ProtoVanEmdeBoasTree<int>(2);
            Assert.Null(tree.MaximumKey);

            tree.Insert(0, 200);
            Assert.Equal(0, tree.MaximumKey);

            tree.Insert(1, 100);
            Assert.Equal(1, tree.MaximumKey);
        }

        [Fact]
        public void MaximumKeyTest()
        {
            var tree = new ProtoVanEmdeBoasTree<int>(111);
            Assert.Null(tree.MaximumKey);

            tree.Insert(55, 1055);
            Assert.Equal(55, tree.MaximumKey);

            tree.Insert(33, 1033);
            Assert.Equal(55, tree.MaximumKey);

            tree.Insert(100, 1100);
            Assert.Equal(100, tree.MaximumKey);

            tree.Insert(77, 1077);
            Assert.Equal(100, tree.MaximumKey);
        }

        [Fact]
        public void NextKeyTest()
        {
            var tree = new ProtoVanEmdeBoasTree<int>(111);
            tree.Insert(100, 1100);
            tree.Insert(77, 1077);
            tree.Insert(33, 1033);
            tree.Insert(55, 1055);

            Assert.Null(tree.NextKey(100));
            Assert.Equal(100, tree.NextKey(77));
            Assert.Equal(33, tree.NextKey(10));
            Assert.Equal(55, tree.NextKey(33));
            Assert.Equal(100, tree.NextKey(77));

            for (int i = 0; i < tree.Capacity; ++i)
            {
                tree.Insert(i, i*100);
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
            var tree = new ProtoVanEmdeBoasTree<int>(111);
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
