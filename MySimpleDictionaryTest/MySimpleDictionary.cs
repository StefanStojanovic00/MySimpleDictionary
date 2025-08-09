using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySimpleDictionaryTest
{
    public class MySimpleDictionary<TKey, TValue>
    {
        private struct Entry
        {
            public int HashCode;    // hash code ključa
            public int Next;        // indeks sledećeg Entry u lancu (-1 ako nema)
            public TKey Key;
            public TValue Value;
        }

        private int[] Buckets;         // niz indeksa u entries (buckets[i] = index + 1 ili 0 ako prazno)
        private Entry[] Entries;
        private int count;             // broj zauzetih elemenata
        private int FreeList;          // indeks prvog slobodnog Entry (-1 ako nema)
        private int FreeCount;         // broj slobodnih mesta
        private const int InitialCapacity = 4;

        public MySimpleDictionary()
        {
            Buckets = new int[InitialCapacity];
            Entries = new Entry[InitialCapacity];
            FreeList = -1;
        }


        private static void ValidateKey(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
        }

        private int GetBucketIndex(int hashCode, int length) => hashCode % length;
        private int GetBucketIndex(int hashCode) => GetBucketIndex(hashCode, Buckets.Length);

        private int ComputeHashCode(TKey key) => key.GetHashCode() & 0x7FFFFFFF;
       
       private int FindEntryIndex(TKey key)
        {
            ValidateKey(key);

            int hashCode = ComputeHashCode(key);
            int bucketIndex = GetBucketIndex(hashCode);

            for (int i = Buckets[bucketIndex] - 1; i >= 0; i = Entries[i].Next)
            {
                if (Entries[i].HashCode == hashCode && EqualityComparer<TKey>.Default.Equals(Entries[i].Key, key))
                    return i;
            }

            return -1;
        }

 

        public void Add(TKey key, TValue value)
        {
            ValidateKey(key);

            if (FindEntryIndex(key) >= 0)
            {
                throw new ArgumentException("Ključ već postoji");
            }

            int hashCode = ComputeHashCode(key);
            int bucketIndex = GetBucketIndex(hashCode);

            int index;
            if (FreeCount > 0)
            {
                index = FreeList;
                FreeList = Entries[index].Next;
                FreeCount--;
            }
            else
            {
                if (count == Entries.Length)
                {
                    Resize();
                    bucketIndex = GetBucketIndex(hashCode);
                }

                index = count;
                count++;
            }

            Entries[index].HashCode = hashCode;
            Entries[index].Key = key;
            Entries[index].Value = value;
            Entries[index].Next = Buckets[bucketIndex] - 1;
            Buckets[bucketIndex] = index + 1;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            int index = FindEntryIndex(key);
            if (index >= 0)
            {
                value = Entries[index].Value;
                return true;
            }

            //value = default!;
            value = default(TValue);
            return false;
        }

        public bool Remove(TKey key)
        {
            ValidateKey(key);

            int hashCode = ComputeHashCode(key);
            int bucketIndex = GetBucketIndex(hashCode);
            int last = -1;

            for (int i = Buckets[bucketIndex] - 1; i >= 0; last = i, i = Entries[i].Next)
            {
                if (Entries[i].HashCode == hashCode &&
                    EqualityComparer<TKey>.Default.Equals(Entries[i].Key, key))
                {
                    if (last < 0)
                    {
                        Buckets[bucketIndex] = Entries[i].Next + 1;
                    }
                    else
                    {
                        Entries[last].Next = Entries[i].Next;
                    }

                    Entries[i].HashCode = -1;
                    Entries[i].Next = FreeList;
                    Entries[i].Key = default!;
                    Entries[i].Value = default!;
                    FreeList = i;
                    FreeCount++;
                    return true;
                }
            }

            return false;
        }

        private void Resize()
        {
            int newSize = Entries.Length * 2;
            int[] newBuckets = new int[newSize];
            Entry[] newEntries = new Entry[newSize];

            Array.Copy(Entries, newEntries, count);

            for (int i = 0; i < count; i++)
            {
                if (newEntries[i].HashCode >= 0)
                {
                    int bucketIndex = newEntries[i].HashCode % newSize;
                    newEntries[i].Next = newBuckets[bucketIndex] - 1;
                    newBuckets[bucketIndex] = i + 1;
                }
            }

            Buckets = newBuckets;
            Entries = newEntries;
        }

        public void PrintAll()
        {
            Console.WriteLine("Svi unosi u Dictionary:");

            for (int i = 0; i < count; i++)
            {
                if (Entries[i].HashCode >= 0)
                {
                    var key = Entries[i].Key;
                    var value = Entries[i].Value;

                    Console.Write($"Key: {key}, Value: ");

                    if (value is IEnumerable enumerable && !(value is string))
                    {
                        Console.Write("[");
                        bool first = true;
                        foreach (var item in enumerable)
                        {
                            if (!first) Console.Write(", ");
                            Console.Write(item);
                            first = false;
                        }
                        Console.WriteLine("]");
                    }
                    else
                    {
                        Console.WriteLine(value);
                    }
                }
            }
        }

        public bool ContainsKey(TKey key)
        {
            return FindEntryIndex(key) >= 0;
        }

        public bool ContainsValue(TValue value)
        {
            EqualityComparer<TValue> comparer = EqualityComparer<TValue>.Default;

            for (int i = 0; i < count; i++)
            {
                if (Entries[i].HashCode >= 0 && comparer.Equals(Entries[i].Value, value))
                    return true;
            }

            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < Buckets.Length; i++)
                Buckets[i] = 0;

            Array.Clear(Entries, 0, count);
            count = 0;
            FreeList = -1;
            FreeCount = 0;
        }

        public TValue this[TKey key]
        {
            get
            {
                if (TryGetValue(key, out TValue value))
                    return value;

                throw new KeyNotFoundException();
            }

            set
            {
                ValidateKey(key);

                int index = FindEntryIndex(key);
                if (index >= 0)
                {
                    Entries[index].Value = value;
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        public int Count => count - FreeCount;

        public IEnumerable<TKey> Keys
        {
            get
            {
                for (int i = 0; i < count; i++)
                {
                    if (Entries[i].HashCode >= 0)
                        yield return Entries[i].Key;
                }
            }
        }

        public IEnumerable<TValue> Values
        {
            get
            {
                for (int i = 0; i < count; i++)
                {
                    if (Entries[i].HashCode >= 0)
                        yield return Entries[i].Value;
                }
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
            {
                if (Entries[i].HashCode >= 0)
                    yield return new KeyValuePair<TKey, TValue>(Entries[i].Key, Entries[i].Value);
            }
        }
    }
}
