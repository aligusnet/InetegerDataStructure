using System;

namespace IntegerDataStructures
{
    public class ProtoVanEmdeBoasNode<T>
    {
        private static readonly int[] LeafSizeMasks = new int[] { 0b01, 0b10};
        private const int LeafIsFull = 0b11;

        public ProtoVanEmdeBoasNode(int capacity)
        {
            if (capacity == 2)  // Leaf case
            {
                data = new T[capacity];
            }
            else
            {
                int childCapacity = Sqrt(capacity);
                summary = new ProtoVanEmdeBoasNode<byte>(childCapacity);
                clusters = new ProtoVanEmdeBoasNode<T>[capacity];
                for (int i = 0; i < childCapacity; ++i)
                {
                    clusters[i] = new ProtoVanEmdeBoasNode<T>(childCapacity);
                }
            }

            this.size = 0;
        }

        public int Size
        {
            get
            {
                if (data == null)
                {
                    return size;
                }
                else  // Leaf case
                {
                    return size switch
                    {
                        0 => 0,
                        LeafIsFull => 2,
                        _ => 1
                    };
                }
            }
        }

        /// <summary>
        /// Insert new value into the tree
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="val">The value</param>
        /// <returns>True if the value was inserted, false if it replaces old value</returns>
        public bool Insert(int key, T val)
        {
            if (data != null)  // Leaf case
            {
                bool inserted = (size & LeafSizeMasks[key]) != LeafSizeMasks[key];
                data[key] = val;
                size |= LeafSizeMasks[key];
                return inserted;
            }
            else if (clusters != null && summary != null)
            {
                var (ClusterIndex, KeyInCluster) = DeconstructKey(key);
                summary.Insert(KeyInCluster, 1);
                bool insered = clusters[ClusterIndex].Insert(KeyInCluster, val);
                if (insered)
                {
                    size++;
                }

                return insered;
            }

            throw new Exception("ProtoVanEmdeBoasNode is invalid");
        }

        public T Find(int key)
        {
            if (data != null)   // Leaf case
            {
                return data[key];
            }
            else if (clusters != null && summary != null)
            {
                var (ClusterIndex, KeyInCluster) = DeconstructKey(key);
                return clusters[ClusterIndex].Find(KeyInCluster);
            }

            throw new Exception("ProtoVanEmdeBoasNode is invalid");
        }

        private ProtoVanEmdeBoasNode<byte>? summary;
        private ProtoVanEmdeBoasNode<T>[]? clusters;
        private T[]? data;
        private int size;

        public static (int ClusterIndex, int KeyInCluster) DeconstructKey(int key)
        {
            int sqrt = Sqrt(key);
            int high = key / sqrt;
            int low = key % sqrt;
            return (high, low);
        }

        public static int Sqrt(int value)
        {
            if (value <= 4)
            {
                return 2;
            }
            else if (value <= 16)
            {
                return 4;
            }
            else if (value <= 256)
            {
                return 16;
            }
            else if (value <= 65536)
            {
                return 256;
            }
            else
            {
                throw new ArgumentException("Values bigger than 65536 are not supported");
            }
        }
    }
}
