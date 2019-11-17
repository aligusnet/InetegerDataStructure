using System;

namespace IntegerDataStructures
{
    public class ProtoVanEmdeBoasTree<T>
    {
        // universe's size
        private readonly int capacity;
        private readonly ProtoVanEmdeBoasNode<T> node;

        public ProtoVanEmdeBoasTree(int capacity)
        {
            this.capacity = GetCapacity(capacity);
            node = new ProtoVanEmdeBoasNode<T>(this.capacity);
        }

        public bool Insert(int key, T val) => node.Insert(key, val);

        public T GetValue(int key) => node.GetValue(key);

        public bool Delete(int key) => node.Delete(key);

        public int Count => node.Count;

        private static int GetCapacity(int requiredCapacity)
        {
            if (requiredCapacity <= 2)
            {
                return 2;
            }
            else if (requiredCapacity <= 4)
            {
                return 4;
            }
            else if (requiredCapacity <= 16)
            {
                return 16;
            }
            else if (requiredCapacity <= 256)
            {
                return 256;
            }
            else if (requiredCapacity <= 65536)
            {
                return 65536;
            }
            else
            {
                throw new ArgumentOutOfRangeException("ProtoVanEmdeBoas does not support capacities > 65536");
            }
        }
    }
}
