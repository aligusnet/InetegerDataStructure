using System.Diagnostics;

namespace IntegerDataStructures
{
    public class VanEmdeBoasNode<T>
    {
        private const int InvalidKey = -1;

        public VanEmdeBoasNode(int universeSize)
        {
            minKey = InvalidKey;
            maxKey = InvalidKey;
            minValue = default!;
            maxValue = default!;

            Count = 0;

            if (universeSize != 2)
            {
                var (floor, ceiling) = Sqrt(universeSize);
                Debug.Assert(floor * ceiling == universeSize, "Sqrt is wrong!");

                childUniverseSize = floor;
                summary = new VanEmdeBoasNode<byte>(ceiling);
                cluster = new VanEmdeBoasNode<T>[ceiling];
            }
        }

        public int Count { get; private set; }

        public int? MinimumKey()
        {
            if (minKey == InvalidKey)
            {
                return null;
            }

            return minKey;
        }

        public int? MaximumKey()
        {
            if (maxKey == InvalidKey)
            {
                return null;
            }

            return maxKey;
        }

        public bool Contains(int key)
        {
            if (minKey == key || maxKey == key)
            {
                return true;
            }

            if (cluster != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key);
                if (cluster[clusterIndex] != null)
                {
                    return cluster[clusterIndex].Contains(keyInCluster);
                }
            }

            return false;
        }

        public T GetValue(int key)
        {
            if (minKey == key)
            {
                return minValue;
            }

            if (maxKey == key)
            {
                return maxValue;
            }

            if (cluster != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key);
                if (cluster[clusterIndex] != null)
                {
                    return cluster[clusterIndex].GetValue(keyInCluster);
                }
            }

            return default!;
        }

        public bool Insert(int key, T value)
        {
            if (key == minKey)
            {
                minValue = value;
                return false;
            }

            if (key == maxKey)
            {
                maxValue = value;
                return false;
            }

            if (minKey == InvalidKey)
            {
                InsertIntoEmpty(key, value);
                ++Count;
                return true;
            }

            if (key < minKey)
            {
                Swap(ref key, ref minKey);
                Swap(ref value, ref minValue);
            }

            bool inserted = true;

            if (cluster != null && summary != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key);
                var child = GetChild(clusterIndex);
                if (child.MinimumKey() == null)
                {
                    summary.Insert(clusterIndex, 1);
                    child.InsertIntoEmpty(keyInCluster, value);
                }
                else
                {
                    inserted = child.Insert(keyInCluster, value);
                }
            }

            if (key > maxKey)
            {
                maxKey = key;
                maxValue = value;
            }

            if (inserted)
            {
                ++Count;
            }

            return inserted;
        }

        public bool InsertIntoEmpty(int key, T value)
        {
            minKey = key;
            minValue = value;
            return true;
        }

        private VanEmdeBoasNode<T> GetChild(int index)
        {
            if (cluster == null)
            {
                throw new ContractFailedException("VanEmdeBoasNode is broken");
            }

            if (cluster[index] == null)
            {
                cluster[index] = new VanEmdeBoasNode<T>(childUniverseSize);
            }

            return cluster[index];
        }

        private (int clusterIndex, int keyInCluster) DeconstructKey(int key)
        {
            int high = key / childUniverseSize;
            int low = key % childUniverseSize;
            return (high, low);
        }

        private readonly VanEmdeBoasNode<byte>? summary;
        private readonly VanEmdeBoasNode<T>[]? cluster;
        private readonly int childUniverseSize;
        private int minKey;
        private int maxKey;
        private T minValue;
        private T maxValue;

        // the function expects that val is power of 2 and bigger than 2
        private static (int floor, int ceiling) Sqrt(int val)
        {
            int power;
            for (power = 0; val != 1; ++power)
            {
                val >>= 1;
            }

            int floorPower = power / 2;
            int ceilingPower = power - floorPower;
            return (1 << floorPower, 1 << ceilingPower);
        }

        private static void Swap<K>(ref K lhs, ref K rhs)
        {
            K tmp = lhs;
            lhs = rhs;
            rhs = tmp;
        }
    }
}
