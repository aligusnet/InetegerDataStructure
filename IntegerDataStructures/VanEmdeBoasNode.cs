using System.Diagnostics;

namespace IntegerDataStructures
{
    internal class VanEmdeBoasNode<T>
    {
        private const int InvalidKey = -1;

        public VanEmdeBoasNode(int universeSize)
        {
            minKey = InvalidKey;
            maxKey = InvalidKey;
            minValue = default!;
            maxValue = default!;

            if (universeSize != 2)
            {
                var (floor, ceiling) = Sqrt(universeSize);
                Debug.Assert(floor * ceiling == universeSize, "Sqrt is wrong!");

                childUniverseSize = floor;
                summary = new VanEmdeBoasNode<byte>(ceiling);
                cluster = new VanEmdeBoasNode<T>[ceiling];
            }
        }

        public int Capacity
        {
            get
            {
                if (cluster != null)
                {
                    return cluster.Length * childUniverseSize;
                }
                else
                {
                    return 2;
                }
            }
        }

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

        public int? NextKey(int key)
        {
            if (cluster == null || summary == null)
            {
                if (key == 0 && maxKey == 1)
                {
                    return 1;
                }
                else
                {
                    return null;
                }
            }
            else if (minKey != InvalidKey && key < minKey)
            {
                return minKey;
            }
            else
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key);

                var maxLow = cluster[clusterIndex]?.MaximumKey();
                if (maxLow.HasValue && keyInCluster < maxLow.Value)
                {
                    var offset = cluster[clusterIndex].NextKey(keyInCluster);
                    if (offset == null)
                    {
                        throw new ContractFailedException("VanEmdeBoasNode.Summary is invalid");
                    }

                    return ConstructKey(clusterIndex, offset.Value);
                }
                else
                {
                    var nextClusterIndex = summary.NextKey(clusterIndex);
                    if (nextClusterIndex != null)
                    {
                        var offset = cluster[nextClusterIndex.Value].MinimumKey();
                        if (offset == null)
                        {
                            throw new ContractFailedException("VanEmdeBoasNode.Summary is invalid");
                        }

                        return ConstructKey(nextClusterIndex.Value, offset.Value);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public int? PreviousKey(int key)
        {
            if (cluster == null || summary == null)
            {
                if (key == 1 && minKey == 0)
                {
                    return 0;
                }
                else
                {
                    return null;
                }
            }
            else if (maxKey != InvalidKey && key > maxKey)
            {
                return maxKey;
            }
            else
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key);
                var minLow = cluster[clusterIndex]?.MinimumKey();
                if (minLow.HasValue && keyInCluster > minLow.Value)
                {
                    var offset = cluster[clusterIndex]?.PreviousKey(keyInCluster);
                    if (offset == null)
                    {
                        throw new ContractFailedException("VanEmdeBoasNode.Summary is invalid");
                    }

                    return ConstructKey(clusterIndex, offset.Value);
                }
                else
                {
                    var prevClusterIndex = summary.PreviousKey(clusterIndex);
                    if (prevClusterIndex != null)
                    {
                        var offset = cluster[prevClusterIndex.Value]?.MaximumKey();
                        if (offset == null)
                        {
                            throw new ContractFailedException("VanEmdeBoasNode.Summary is invalid");
                        }

                        return ConstructKey(prevClusterIndex.Value, offset.Value);
                    }
                    else
                    {
                        return (minKey != InvalidKey && key > minKey) ? (int?)minKey : null;
                    }
                }
            }
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

            return inserted;
        }

        private bool InsertIntoEmpty(int key, T value)
        {
            minKey = maxKey = key;
            minValue = maxValue = value;
            return true;
        }

        public bool Delete(int key)
        {
            if (Contains(key))
            {
                InternalDelete(key);
                return true;
            }

            return false;
        }

        private bool InternalDelete(int key)
        {
            if (minKey == InvalidKey && maxKey == InvalidKey)
            {
                return false;
            }

            if (minKey == maxKey)
            {
                minKey = InvalidKey;
                maxKey = InvalidKey;
                minValue = default!;
                maxValue = default!;

                return true;
            }

            if (cluster == null || summary == null)
            {
                if (key == 0)
                {
                    minKey = 1;
                    minValue = maxValue;
                }
                else
                {
                    minKey = 0;
                    maxValue = minValue;
                }
                maxKey = minKey;

                return true;
            }

            if (key == minKey)
            {
                var firstCluster = summary.MinimumKey();
                if (!firstCluster.HasValue)
                {
                    throw new ContractFailedException("The node must not be empty");
                }

                var keyInCluster = cluster[firstCluster.Value].MinimumKey();
                if (!keyInCluster.HasValue)
                {
                    throw new ContractFailedException("The child node must not be empty");
                }

                key = ConstructKey(firstCluster.Value, keyInCluster.Value);


                minValue = GetValue(key);
                minKey = key;

            }

            var k = DeconstructKey(key);
            if (cluster[k.clusterIndex] == null)
            {
                return false;
            }

            bool deleted = cluster[k.clusterIndex].InternalDelete(k.keyInCluster);
            var maximum = cluster[k.clusterIndex].MaximumKey();
            if (maximum == null)
            {
                if (!summary.InternalDelete(k.clusterIndex))
                {
                    throw new ContractFailedException("The summary is broken");
                }

                cluster[k.clusterIndex] = default!;

                if (key == maxKey)
                {
                    var summaryMax = summary.MaximumKey();
                    if (summaryMax == null)
                    {
                        maxKey = minKey;
                        maxValue = minValue;
                    }
                    else
                    {
                        var maxMaxKey = cluster[summaryMax.Value].MaximumKey();
                        if (maxMaxKey == null)
                        {
                            throw new ContractFailedException("The child node must be non-empty");
                        }

                        var tmpKey = ConstructKey(summaryMax.Value, maxMaxKey.Value);
                        maxValue = GetValue(tmpKey);
                        maxKey = tmpKey;
                    }
                }
            }
            else if (maxKey == key)
            {
                var tmpKey = ConstructKey(k.clusterIndex, maximum.Value);
                maxValue = GetValue(tmpKey);
                maxKey = tmpKey;
            }

            return deleted;
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

        private int ConstructKey(int clusterIndex, int keyInCluster)
        {
            return clusterIndex * childUniverseSize + keyInCluster;
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
