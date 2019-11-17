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
                clusters = new ProtoVanEmdeBoasNode<T>[childCapacity];
                for (int i = 0; i < childCapacity; ++i)
                {
                    clusters[i] = new ProtoVanEmdeBoasNode<T>(childCapacity);
                }
            }

            this.size = 0;
        }

        public int Count
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

        public int Capacity
        {
            get
            {
                if (clusters != null)
                {
                    return clusters.Length * clusters.Length;
                }
                else  // Leaf case
                {
                    return 2;
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
                bool toBeInserted = !ContainsInLeaf(key);
                data[key] = val;
                size |= LeafSizeMasks[key];
                return toBeInserted;
            }
            else if (clusters != null && summary != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key, clusters.Length);
                summary.Insert(clusterIndex, 1);
                bool insered = clusters[clusterIndex].Insert(keyInCluster, val);
                if (insered)
                {
                    size++;
                }

                return insered;
            }

            throw new ContractFailedException("ProtoVanEmdeBoasNode is invalid");
        }

        public T GetValue(int key)
        {
            if (data != null)   // Leaf case
            {
                return data[key];
            }
            else if (clusters != null && summary != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key, clusters.Length);
                return clusters[clusterIndex].GetValue(keyInCluster);
            }

            throw new ContractFailedException("ProtoVanEmdeBoasNode is invalid");
        }

        public bool Delete(int key)
        {
            if (data != null)   // Leaf case
            {
                bool toBeDeleted = ContainsInLeaf(key);
                if (toBeDeleted)
                {
                    data[key] = default!;
                    size &= ~LeafSizeMasks[key];
                }

                return toBeDeleted;
            }
            else if (clusters != null && summary != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key, clusters.Length);
                bool deleted = clusters[clusterIndex].Delete(keyInCluster);
                if (deleted)
                {
                    size--;

                    if (clusters[clusterIndex].Count == 0)
                    {
                        summary.Delete(clusterIndex);
                    }
                }

                return deleted;
            }

            throw new ContractFailedException("ProtoVanEmdeBoasNode is invalid");
        }

        public int? MinimumKey()
        {
            if (data != null)   // Leaf case
            {
                if (ContainsInLeaf(0))
                {
                    return 0;
                }
                else if (ContainsInLeaf(1))
                {
                    return 1;
                }
                else
                {
                    return null;
                }
            }
            else if (clusters != null && summary != null)
            {
                var clusterIndex = summary.MinimumKey();
                if (clusterIndex != null)
                {
                    var offset = clusters[clusterIndex.Value].MinimumKey();
                    if (offset != null)  // must be not null
                    {
                        return ConstructKey(clusterIndex.Value, offset.Value, clusters.Length);
                    }
                }

                return null;
            }

            throw new ContractFailedException("ProtoVanEmdeBoasNode is invalid");
        }

        public int? MaximumKey()
        {
            if (data != null)   // Leaf case
            {
                if (ContainsInLeaf(1))
                {
                    return 1;
                }
                else if (ContainsInLeaf(0))
                {
                    return 0;
                }
                else
                {
                    return null;
                }
            }
            else if (clusters != null && summary != null)
            {
                var clusterIndex = summary.MaximumKey();
                if (clusterIndex == null)
                {
                    return null;
                }

                var offset = clusters[clusterIndex.Value].MaximumKey();
                if (offset == null)
                {
                    throw new ContractFailedException("ProtoVanEmdeBoasNode.Summary is invalid");
                }

                return ConstructKey(clusterIndex.Value, offset.Value, clusters.Length);
            }

            throw new ContractFailedException("ProtoVanEmdeBoasNode is invalid");
        }

        public int? NextKey(int key)
        {
            if (data != null)   // Leaf case
            {
                if (key == 0 && ContainsInLeaf(1))
                {
                    return 1;
                }
                else
                {
                    return null;
                }
            }
            else if (clusters != null && summary != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key, clusters.Length);
                var offset = clusters[clusterIndex].NextKey(keyInCluster);
                if (offset != null)
                {
                    return ConstructKey(clusterIndex, offset.Value, clusters.Length);
                }
                else
                {
                    var nextCluster = summary.NextKey(clusterIndex);
                    if (nextCluster != null)
                    {
                        offset = clusters[nextCluster.Value].MinimumKey();
                        if (offset == null)
                        {
                            throw new ContractFailedException("ProtoVanEmdeBoasNode.Summary is invalid");
                        }

                        return ConstructKey(nextCluster.Value, offset.Value, clusters.Length);
                    }

                    return null;
                }
            }

            throw new ContractFailedException("ProtoVanEmdeBoasNode is invalid");
        }

        public int? PreviousKey(int key)
        {
            if (data != null)   // Leaf case
            {
                if (key == 1 && ContainsInLeaf(0))
                {
                    return 0;
                }

                return null;
            }
            else if (clusters != null && summary != null)
            {
                var (clusterIndex, keyInCluster) = DeconstructKey(key, clusters.Length);
                var offset = clusters[clusterIndex].PreviousKey(keyInCluster);

                if (offset != null)
                {
                    return ConstructKey(clusterIndex, offset.Value, clusters.Length);
                }

                var prevCluster = summary.PreviousKey(clusterIndex);
                if (prevCluster == null)
                {
                    return null;
                }

                offset = clusters[prevCluster.Value].MaximumKey();
                if (offset == null)
                {
                    throw new ContractFailedException("ProtoVanEmdeBoasNode.Summary is invalid");
                }

                return ConstructKey(prevCluster.Value, offset.Value, clusters.Length);
            }

            throw new ContractFailedException("ProtoVanEmdeBoasNode is invalid");
        }

        private bool ContainsInLeaf(int key)
        {
            return (size & LeafSizeMasks[key]) == LeafSizeMasks[key];
        }

        private readonly ProtoVanEmdeBoasNode<byte>? summary;
        private readonly ProtoVanEmdeBoasNode<T>[]? clusters;
        private readonly T[]? data;
        private int size;

        public static (int clusterIndex, int keyInCluster) DeconstructKey(int key, int universeSqrtSize)
        {
            int high = key / universeSqrtSize;
            int low = key % universeSqrtSize;
            return (high, low);
        }

        public static int ConstructKey(int clusterIndex, int keyInCluster, int universeSqrtSize)
        {
            return clusterIndex * universeSqrtSize + keyInCluster;
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
                throw new System.ArgumentException("Values bigger than 65536 are not supported");
            }
        }
    }
}
