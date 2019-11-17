using Xunit;

namespace IntegerDataStructures.Test
{
    public class ProtoVanEmdeBoas
    {
        [Fact]
        public void DeconstructKeyTest()
        {
            var node = new ProtoVanEmdeBoasNode<int>(10);
            Assert.Equal((0, 0), ProtoVanEmdeBoasNode<string>.DeconstructKey(0));
            Assert.Equal((0, 1), ProtoVanEmdeBoasNode<string>.DeconstructKey(1));
            Assert.Equal((1, 0), ProtoVanEmdeBoasNode<string>.DeconstructKey(2));
            Assert.Equal((2, 3), ProtoVanEmdeBoasNode<string>.DeconstructKey(11));
            Assert.Equal((3, 2), ProtoVanEmdeBoasNode<string>.DeconstructKey(14));
            Assert.Equal((3, 3), ProtoVanEmdeBoasNode<string>.DeconstructKey(15));
            Assert.Equal((4, 0), ProtoVanEmdeBoasNode<string>.DeconstructKey(16));
            Assert.Equal((2, 0), ProtoVanEmdeBoasNode<string>.DeconstructKey(32));
            Assert.Equal((15, 15), ProtoVanEmdeBoasNode<string>.DeconstructKey(255));
            Assert.Equal((16, 0), ProtoVanEmdeBoasNode<string>.DeconstructKey(256));
            Assert.Equal((1, 1), ProtoVanEmdeBoasNode<string>.DeconstructKey(257));
        }

        [Fact]
        public void ProtoVanEmdeBoasSmokeTest()
        {
            var tree = new ProtoVanEmdeBoasTree<string>(111);

            Assert.True(tree.Insert(79, "value 79"));
            Assert.Equal("value 79", tree.Find(79));

            Assert.False(tree.Insert(79, "value 79 - 2"));
            Assert.Equal("value 79 - 2", tree.Find(79));

            Assert.True(tree.Insert(16, "value 16"));
            Assert.Equal("value 16", tree.Find(16));
        }
    }
}
