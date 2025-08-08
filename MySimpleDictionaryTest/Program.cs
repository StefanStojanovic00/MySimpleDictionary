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

            var dict = new MySimpleDictionary<int, string>();

            Console.WriteLine("=== TEST 1: Add, Indexer i Print ===");
            dict.Add(1, "jedan");
            dict.Add(2, "dva");
            dict.Add(3, "tri");
            dict[4] = "četiri"; // test indexera
            dict.PrintAll();

            Console.WriteLine("\n=== TEST 2: TryGetValue ===");
            if (dict.TryGetValue(2, out string value))
                Console.WriteLine($"TryGetValue(2): {value}");

            Console.WriteLine("\n=== TEST 3: ContainsKey i ContainsValue ===");
            Console.WriteLine($"ContainsKey(1): {dict.ContainsKey(1)}");
            Console.WriteLine($"ContainsKey(99): {dict.ContainsKey(99)}");
            Console.WriteLine($"ContainsValue(\"tri\"): {dict.ContainsValue("tri")}");
            Console.WriteLine($"ContainsValue(\"pet\"): {dict.ContainsValue("pet")}");

            Console.WriteLine("\n=== TEST 4: Indexer get/set ===");
            dict[3] = "tri-modifikovano";
            Console.WriteLine($"dict[3]: {dict[3]}");

            Console.WriteLine("\n=== TEST 5: Remove ===");
            dict.Remove(2);
            Console.WriteLine("Nakon uklanjanja ključa 2:");
            dict.PrintAll();

            Console.WriteLine("\n=== TEST 6: Count ===");
            Console.WriteLine($"Ukupno elemenata: {dict.Count}");

            Console.WriteLine("\n=== TEST 7: Keys i Values ===");
            Console.WriteLine("Keys: " + string.Join(", ", dict.Keys));
            Console.WriteLine("Values: " + string.Join(", ", dict.Values));

            Console.WriteLine("\n=== TEST 8: Clear ===");
            dict.Clear();
            Console.WriteLine($"Nakon Clear: Count = {dict.Count}");

            Console.WriteLine("\n=== TEST 9: Enumeracija (foreach) ===");
            dict[10] = "deset";
            dict[20] = "dvadeset";
            dict[30] = "trideset";
            foreach (var kv in dict)
            {
                Console.WriteLine($"Key: {kv.Key}, Value: {kv.Value}");
            }

            Console.WriteLine("\n=== TEST 10: Kolizije ===");
            var kolDict = new MySimpleDictionary<int, string>();
            kolDict.Add(1, "prvi");
            kolDict.Add(5, "drugi");
            kolDict.Add(9, "treći");
            kolDict.PrintAll();

            Console.WriteLine("\n=== TEST 11: Performanse (insert + lookup) ===");
            const int N = 100_000;
            var bigDict = new MySimpleDictionary<int, int>();

            var sw = Stopwatch.StartNew();
            for (int i = 0; i < N; i++)
                bigDict[i] = i * 2;
            sw.Stop();
            Console.WriteLine($"Dodavanje {N} elemenata: {sw.ElapsedMilliseconds} ms");

            sw.Restart();
            int found = 0;
            for (int i = 0; i < N; i++)
            {
                if (bigDict.TryGetValue(i, out int v) && v == i * 2)
                    found++;
            }
            sw.Stop();
            Console.WriteLine($"Pronađeno {found}/{N} elemenata: {sw.ElapsedMilliseconds} ms");

            Console.WriteLine("\n=== TEST 12: string -> int ===");
            var dictStrInt = new MySimpleDictionary<string, int>();
            dictStrInt.Add("jedan", 1);
            dictStrInt.Add("dva", 2);
            dictStrInt.Add("tri", 3);
            dictStrInt.PrintAll();

            Console.WriteLine("\n=== TEST 13: Guid -> string ===");
            var dictGuid = new MySimpleDictionary<Guid, string>();
            Guid g1 = Guid.NewGuid();
            Guid g2 = Guid.NewGuid();
            dictGuid.Add(g1, "korisnik1");
            dictGuid.Add(g2, "korisnik2");
            dictGuid.PrintAll();

            Console.WriteLine("\n=== TEST 14: Tuple<int,int> -> string ===");
            var dictTuple = new MySimpleDictionary<Tuple<int, int>, string>();
            dictTuple.Add(Tuple.Create(1, 2), "par1");
            dictTuple.Add(Tuple.Create(3, 4), "par2");
            dictTuple.PrintAll();

            Console.WriteLine("\n=== TEST 15: int -> List<string> ===");
            var dictList = new MySimpleDictionary<int, List<string>>();
            dictList.Add(1, new List<string> { "prvi", "element" });
            dictList.Add(2, new List<string> { "drugi" });
            foreach (var kv in dictList)
            {
                Console.WriteLine($"Key: {kv.Key}, Values: [{string.Join(", ", kv.Value)}]");
            }

            Console.WriteLine("\n=== TEST 16: string -> List<int> ===");
            var dictList2 = new MySimpleDictionary<string, List<int>>();
            dictList2.Add("grupaA", new List<int> { 1, 2, 3 });
            dictList2.Add("grupaB", new List<int> { 10, 20 });
            foreach (var kv in dictList2)
            {
                Console.WriteLine($"Key: {kv.Key}, Values: [{string.Join(", ", kv.Value)}]");
            }

            

            // TIME COMPLEXITY
            const int brojElemenata = 1_000_000;
            var mojaMapa = new MySimpleDictionary<int, int>();

            // Vreme dodavanja (O(1))
            var tajmer = Stopwatch.StartNew();
            for (int kljuc = 0; kljuc < brojElemenata; kljuc++)
            {
                mojaMapa.Add(kljuc, kljuc);
            }
            tajmer.Stop();
            Console.WriteLine($"Dodavanje {brojElemenata} elemenata: {tajmer.ElapsedMilliseconds} ms");

            // Vreme pretrage (O(1))
            tajmer.Restart();
            int pronadjeno = 0;
            for (int kljuc = 0; kljuc < brojElemenata; kljuc++)
            {
                if (mojaMapa.TryGetValue(kljuc, out int vrednost))
                    pronadjeno++;
            }
            tajmer.Stop();
            Console.WriteLine($"Pretraga {pronadjeno} elemenata: {tajmer.ElapsedMilliseconds} ms");

            // ContainsValue (O(n))
            tajmer.Restart();
            bool sadrziVrednost = mojaMapa.ContainsValue(brojElemenata / 2);
            tajmer.Stop();
            Console.WriteLine($"ContainsValue (linearna pretraga): {tajmer.ElapsedMilliseconds} ms");

            Console.WriteLine("\n=== TEST 17: Update postojećih vrednosti više puta ===");
            var updateDict = new MySimpleDictionary<int, string>();
            updateDict.Add(1, "start");
            for (int i = 0; i < 5; i++)
            {
                updateDict[1] = $"update_{i}";
                Console.WriteLine($"Iteration {i}: {updateDict[1]}");
            }


            Console.WriteLine("\n=== TEST 18: Ključevi koji su reference tipovi ===");
            var objKeyDict = new MySimpleDictionary<object, string>();
            object o1 = new object();
            object o2 = new object();
            objKeyDict.Add(o1, "objekat1");
            objKeyDict.Add(o2, "objekat2");

            Console.WriteLine($"Value za o1: {objKeyDict[o1]}");
            Console.WriteLine($"Value za o2: {objKeyDict[o2]}");

            Console.WriteLine($"ContainsKey(o1): {objKeyDict.ContainsKey(o1)}");
            Console.WriteLine($"ContainsKey(new object()): {objKeyDict.ContainsKey(new object())}"); // očekuje se false


            /* Console.WriteLine("\n=== TEST 19: Null kao ključ i/ili vrednost ===");
             var nullDict = new MySimpleDictionary<string, string>();
             nullDict.Add(null, "nullKey");
             nullDict.Add("nekiKey", null);

             Console.WriteLine($"Value za null ključ: {nullDict[null]}");
             Console.WriteLine($"Value za 'nekiKey': {nullDict["nekiKey"]}");

             Console.WriteLine($"ContainsKey(null): {nullDict.ContainsKey(null)}");
             Console.WriteLine($"ContainsValue(null): {nullDict.ContainsValue(null)}");*/

            Console.WriteLine("\n=== TEST 20: Veštačke kolizije ===");
            var kolizije = new MySimpleDictionary<int, string>();

            for (int i = 0; i < 100; i++)
            {
                // Pravimo kolizije ako buckets.Length je npr. 5, a koristimo ključeve koji daju isti mod
                int key = i * 5;
                kolizije.Add(key, $"vrednost_{key}");
            }

            Console.WriteLine("Provera vrednosti iz kolizionih slotova:");
            for (int i = 0; i < 100; i++)
            {
                int key = i * 5;
                if (kolizije.TryGetValue(key, out string val))
                {
                    Console.WriteLine($"Key {key} -> {val}");
                }
                else
                {
                    Console.WriteLine($"Key {key} NIJE pronađen!");
                }
            }


            Console.WriteLine("\n=== TEST ZAVRŠEN ===");

        }
    }
}
