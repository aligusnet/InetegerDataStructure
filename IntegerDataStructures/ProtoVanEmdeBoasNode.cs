using System;
using System.Collections.Generic;
using System.Text;

namespace IntegerDataStructures
{
    public class ProtoVanEmdeBoasNode<T>
    {
        public ProtoVanEmdeBoasNode(int size)
        {
            if (size == 2)
            {
                data = new T[size];
            }
            else
            {
                int childSize = Sqrt(size);
                summary = new ProtoVanEmdeBoasNode<byte>(childSize);
                clusters = new ProtoVanEmdeBoasNode<T>[size];
                for (int i = 0; i < childSize; ++i)
                {
                    clusters[i] = new ProtoVanEmdeBoasNode<T>(childSize);
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
            if (data != null)
            {
                bool inserted = data[key] == null;
                data[key] = val;
                return inserted;
            }
            else if (clusters != null && summary != null)
            {
                var (ClusterIndex, KeyInCluster) = DeconstructKey(key);
                summary.Insert(KeyInCluster, 1);
                return clusters[ClusterIndex].Insert(KeyInCluster, val);
            }

            throw new Exception("ProtoVanEmdeBoasNode is invalid");
        }

        public T Find(int key)
        {
            if (data != null)
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
