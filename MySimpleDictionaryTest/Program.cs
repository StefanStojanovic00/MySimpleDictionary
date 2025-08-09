using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace MySimpleDictionaryTest
{
    class Program
    {


        static void Main(string[] args)
        {

            

            Console.WriteLine("=== TEST 1: Dodavanje, dohvat i indeksiranje ===");
            var dict = new MySimpleDictionary<int, string>();
            dict.Add(1, "jedan");
            dict.Add(2, "dva");
            dict[3] = "tri"; // indexer set
            Console.WriteLine(dict[2]); // očekuje "dva"
            dict[2] = "dva-modifikovano"; // indexer update
            Console.WriteLine(dict[2]); // očekuje "dva-modifikovano"

            Console.WriteLine("\n=== TEST 2: Uklanjanje i provjera nakon uklanjanja ===");
            bool removed = dict.Remove(2);
            Console.WriteLine($"Remove(2) uspješno: {removed}");
            Console.WriteLine($"ContainsKey(2): {dict.ContainsKey(2)}"); // očekuje false
            try
            {
                Console.WriteLine(dict[2]); // treba baciti KeyNotFoundException
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("KeyNotFoundException uhvaćen za dict[2]");
            }

            Console.WriteLine("\n=== TEST 3: Kolizije sa namjerno istim hash bucket-om ===");
            var kolDict = new MySimpleDictionary<int, string>();
            int bucketCount = 4; // inicijalni kapacitet u tvom Dictionary-ju
            for (int i = 0; i < 10; i++)
            {
                int key = i * bucketCount; // svi se mapiraju u isti bucket (mod bucketCount == 0)
                kolDict.Add(key, $"value_{key}");
            }
            bool allFound = true;
            for (int i = 0; i < 10; i++)
            {
                int key = i * bucketCount;
                if (!kolDict.TryGetValue(key, out string val) || val != $"value_{key}")
                    allFound = false;
            }
            Console.WriteLine($"Kolizije test, svi elementi pronađeni: {allFound}");

            Console.WriteLine("\n=== TEST 4: Performanse za veliki broj elemenata ===");
            const int N = 1000000;
            var bigDict = new MySimpleDictionary<int, int>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < N; i++)
                bigDict[i] = i * 3;
            sw.Stop();
            Console.WriteLine($"Dodavanje {N} elemenata: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            int foundCount = 0;
            for (int i = 0; i < N; i++)
            {
                if (bigDict.TryGetValue(i, out int v) && v == i * 3)
                    foundCount++;
            }
            sw.Stop();
            Console.WriteLine($"Pronađeno {foundCount} od {N} elemenata za pretragu: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("\n=== TEST 5: Enumeracija i Keys/Values ===");
            int sumKeys = 0, sumValues = 0;
            foreach (var kv in bigDict)
            {
                sumKeys += kv.Key;
                sumValues += kv.Value;
            }
            Console.WriteLine($"Suma ključeva: {sumKeys}, suma vrijednosti: {sumValues}");
            Console.WriteLine($"Keys.Count: {bigDict.Keys.Count()}, Values.Count: {bigDict.Values.Count()}");

            Console.WriteLine("\n=== TEST 6: Ključevi i vrijednosti kao reference tipovi i kompleksni tipovi ===");
            var complexDict = new MySimpleDictionary<(int, string), List<string>>();
            complexDict.Add((1, "A"), new List<string> { "jedan", "dva" });
            complexDict.Add((2, "B"), new List<string> { "tri" });
            foreach (var kv in complexDict)
            {
                Console.WriteLine($"Key: ({kv.Key.Item1}, {kv.Key.Item2}), Values: [{string.Join(", ", kv.Value)}]");
            }

            Console.WriteLine("\n=== TEST 7: Clear i ponovna upotreba ===");
            bigDict.Clear();
            Console.WriteLine($"Count nakon Clear: {bigDict.Count} (očekuje 0)");
            bigDict.Add(100, 1000);
            Console.WriteLine($"Vrijednost za ključ 100 nakon Clear i dodavanja: {bigDict[100]}");

            Console.WriteLine("\n=== TEST 8: Benchmark ===");
            Test8_Benchmark();
        }
        private static void Test8_Benchmark()
        {
            const int N = 1_000_000;
            const int Iterations = 3;

            var myDict = new MySimpleDictionary<int, int>();
            var sysDict = new Dictionary<int, int>();

            double BenchmarkAddMyDict(MySimpleDictionary<int, int> dict, int n, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;

                for (int it = 0; it < iters; it++)
                {
                    dict.Clear();
                    sw.Restart();
                    for (int i = 0; i < n; i++)
                        dict[i] = i;
                    sw.Stop();
                    totalMs += sw.Elapsed.TotalMilliseconds;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                return totalMs / iters;
            }

            double BenchmarkAddSysDict(Dictionary<int, int> dict, int n, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;

                for (int it = 0; it < iters; it++)
                {
                    dict.Clear();
                    sw.Restart();
                    for (int i = 0; i < n; i++)
                        dict[i] = i;
                    sw.Stop();
                    totalMs += sw.Elapsed.TotalMilliseconds;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                return totalMs / iters;
            }

            double BenchmarkFindMyDict(MySimpleDictionary<int, int> dict, int n, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;

                for (int it = 0; it < iters; it++)
                {
                    sw.Restart();
                    int found = 0;
                    for (int i = 0; i < n; i++)
                    {
                        if (dict.TryGetValue(i, out int v) && v == i)
                            found++;
                    }
                    sw.Stop();
                    if (found != n) throw new Exception("MySimpleDictionary: nisu svi elementi pronađeni!");
                    totalMs += sw.Elapsed.TotalMilliseconds;
                }

                return totalMs / iters;
            }

            double BenchmarkFindSysDict(Dictionary<int, int> dict, int n, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;

                for (int it = 0; it < iters; it++)
                {
                    sw.Restart();
                    int found = 0;
                    for (int i = 0; i < n; i++)
                    {
                        if (dict.TryGetValue(i, out int v) && v == i)
                            found++;
                    }
                    sw.Stop();
                    if (found != n) throw new Exception("System Dictionary: nisu svi elementi pronađeni!");
                    totalMs += sw.Elapsed.TotalMilliseconds;
                }

                return totalMs / iters;
            }

            double BenchmarkRemoveMyDict(MySimpleDictionary<int, int> dict, int n, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;

                for (int it = 0; it < iters; it++)
                {
                    dict.Clear();
                    for (int i = 0; i < n; i++)
                        dict[i] = i;

                    sw.Restart();
                    for (int i = 0; i < n; i++)
                        dict.Remove(i);
                    sw.Stop();

                    if (dict.Count != 0) throw new Exception("MySimpleDictionary: Dictionary nije prazan nakon uklanjanja!");

                    totalMs += sw.Elapsed.TotalMilliseconds;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                return totalMs / iters;
            }

            double BenchmarkRemoveSysDict(Dictionary<int, int> dict, int n, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;

                for (int it = 0; it < iters; it++)
                {
                    dict.Clear();
                    for (int i = 0; i < n; i++)
                        dict[i] = i;

                    sw.Restart();
                    for (int i = 0; i < n; i++)
                        dict.Remove(i);
                    sw.Stop();

                    if (dict.Count != 0) throw new Exception("System Dictionary nije prazan nakon uklanjanja!");

                    totalMs += sw.Elapsed.TotalMilliseconds;
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                return totalMs / iters;
            }

            double BenchmarkEnumerateMyDict(MySimpleDictionary<int, int> dict, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;
                int sum = 0;

                for (int it = 0; it < iters; it++)
                {
                    sw.Restart();
                    foreach (var kv in dict)
                        sum += kv.Key + kv.Value;
                    sw.Stop();
                    totalMs += sw.Elapsed.TotalMilliseconds;
                }

                if (sum == 0) Console.WriteLine("MySimpleDictionary: Sum je 0 (ne bi trebalo da se desi)");

                return totalMs / iters;
            }

            double BenchmarkEnumerateSysDict(Dictionary<int, int> dict, int iters)
            {
                var sw = new Stopwatch();
                double totalMs = 0;
                int sum = 0;

                for (int it = 0; it < iters; it++)
                {
                    sw.Restart();
                    foreach (var kv in dict)
                        sum += kv.Key + kv.Value;
                    sw.Stop();
                    totalMs += sw.Elapsed.TotalMilliseconds;
                }

                if (sum == 0) Console.WriteLine("System Dictionary: Sum je 0 (ne bi trebalo da se desi)");

                return totalMs / iters;
            }

            // Pokreni benchmark-e
            double myAdd = BenchmarkAddMyDict(myDict, N, Iterations);
            double sysAdd = BenchmarkAddSysDict(sysDict, N, Iterations);

            double myFind = BenchmarkFindMyDict(myDict, N, Iterations);
            double sysFind = BenchmarkFindSysDict(sysDict, N, Iterations);

            double myRemove = BenchmarkRemoveMyDict(myDict, N, Iterations);
            double sysRemove = BenchmarkRemoveSysDict(sysDict, N, Iterations);

            // Napuni dict za enumeraciju
            myDict.Clear();
            sysDict.Clear();
            for (int i = 0; i < N; i++)
            {
                myDict[i] = i;
                sysDict[i] = i;
            }

            double myEnum = BenchmarkEnumerateMyDict(myDict, Iterations);
            double sysEnum = BenchmarkEnumerateSysDict(sysDict, Iterations);

            // Ispis rezultata
            Console.WriteLine($"Dodavanje {N} elemenata, prosjek {Iterations} iteracija:");
            Console.WriteLine($"MySimpleDictionary: {myAdd:F2} ms");
            Console.WriteLine($"Dictionary        : {sysAdd:F2} ms\n");

            Console.WriteLine($"Traženje {N} elemenata, prosjek {Iterations} iteracija:");
            Console.WriteLine($"MySimpleDictionary: {myFind:F2} ms");
            Console.WriteLine($"Dictionary        : {sysFind:F2} ms\n");

            Console.WriteLine($"Uklanjanje {N} elemenata, prosjek {Iterations} iteracija:");
            Console.WriteLine($"MySimpleDictionary: {myRemove:F2} ms");
            Console.WriteLine($"Dictionary        : {sysRemove:F2} ms\n");

            Console.WriteLine($"Enumeracija {N} elemenata, prosjek {Iterations} iteracija:");
            Console.WriteLine($"MySimpleDictionary: {myEnum:F2} ms");
            Console.WriteLine($"Dictionary        : {sysEnum:F2} ms");
        }
    }
}
