using System;

namespace IntegerDataStructures
{
    public class VanEmdeBoasTree<T>
    {
        public int Capacity { get; }
        public int Count { get; private set; }

        private readonly VanEmdeBoasNode<T> node;

        public VanEmdeBoasTree(int capacity)
        {
            Count = 0;
            Capacity = GetCapacity(capacity);
            node = new VanEmdeBoasNode<T>(Capacity);
        }

        public bool Insert(int key, T val)
        {
            CheckKey(key);

            bool inserted = node.Insert(key, val);
            if (inserted)
            {
                ++Count;
            }

            return inserted;
        }

        public T GetValue(int key)
        {
            CheckKey(key);

            return node.GetValue(key);
        }

        public bool Contains(int key)
        {
            CheckKey(key);

            return node.Contains(key);
        }

        public int? MaximumKey => node.MaximumKey();

        public int? MinimumKey => node.MinimumKey();

        public bool Delete(int key)
        {
            CheckKey(key);

            bool deleted = node.Delete(key);
            if (deleted)
            {
                --Count;
            }

            return deleted;
        }

        private void CheckKey(int key)
        {
            if (key < 0 || key >= Capacity)
            {
                throw new ArgumentOutOfRangeException($"Key must be in range: [{0}, {Capacity})");
            }
        }

        private static int GetCapacity(int requiredCapacity)
        {
            int capacity = 2;
            for (; capacity < requiredCapacity; capacity <<= 1) ;
            return capacity;
        }
    }
}
