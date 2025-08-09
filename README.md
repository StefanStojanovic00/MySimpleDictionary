# MySimpleDictionary<TKey, TValue>

`MySimpleDictionary` je implementacija generičkog haš rečnika (Dictionary) u C#-u, sa osnovnim funkcionalnostima koje imaju i standardni .NET rečnici.
`MySimpleDictionary` je implementiran kao **hash tabela** koja koristi tehniku **Separate Chaining** za efikasno rešavanje kolizija. Ova metoda omogućava skladištenje višestrukih elemenata sa istim hash kodom u povezane liste unutar pojedinačnih bucket-a.

Implementacija i podešavanje `MySimpleDictionary` zasnovani su na članku sa sajta [Dotnetos](https://dotnetos.org/blog/2022-03-28-dictionary-implementation/).
## Opis

Ova klasa predstavlja osnovni dictionary sa sledećim funkcionalnostima:

- **Dodavanje elemenata** sa ključem i vrednošću.
- **Pristup vrednosti** po ključu koristeći Indexer ili metodu `TryGetValue`.
- **Provera postojanja ključeva i vrednosti** (`ContainsKey`, `ContainsValue`).
- **Uklanjanje pojedinačnih elemenata** (`Remove`) i **brisanje celog rečnika** (`Clear`).
- **Iteracija kroz sve elemente** (implementiran enumerator).
- **Pristup listama svih ključeva i vrednosti** (`Keys`, `Values`).
- **Automatsko proširenje unutrašnjih struktura** (metoda `Resize`) kad kapacitet postane premali.
- Podržava rad sa **kompleksnim tipovima kao ključevima i vrednostima**.
- Implementirana je osnovna **obrada kolizija heša** kroz povezane liste (chaining).

  Najvažniji elementi implementacije `MySimpleDictionary<TKey, TValue>` su:

- **buckets** – skup elemenata sa sličnim hash vrednostima  
- **entries** – elementi samog rečnika  
- **freeList** – indeks prve slobodne pozicije  
- **freeCount** – broj praznih mesta u nizu koja nisu na kraju  
- **count** – trenutni broj elemenata u rečniku 

---

## Funkcionalnosti

- Konstruktor sa inicijalnim kapacitetom.
- `Add(TKey key, TValue value)` — dodaje novi par ključ-vrednost.
- Indexer `this[TKey key]` — pristupa vrednosti po ključu.
- `TryGetValue(TKey key, TValue value)` — pokušava dohvatiti vrednost bez exceptiona.
- `Remove(TKey key)` — uklanja element sa zadatim ključem.
- `Clear()` — briše ceo rečnik.
- `ContainsKey(TKey key)` i `ContainsValue(TValue value)` — proverava postojanje ključeva i vrednosti.
- `Count` — broj elemenata u rečniku.
- `Keys` i `Values` — enumeracije svih ključeva i vrednosti.
- Enumeracija `IEnumerable<KeyValuePair<TKey, TValue>>` — omogućava `foreach` petlju.

---

### Pregled testova:

- **Test 1: Dodavanje, pristup i indeksiranje**  
  Proverava osnovne operacije dodavanja elemenata (`Add`), pristupa vrednostima preko indeksa (`Indexer get/set`) i izmenu vrednosti postojećeg ključa.

- **Test 2: Uklanjanje i provera prisutnosti ključeva**  
  Testira uklanjanje ključeva (`Remove`), proveru postojanja ključa (`ContainsKey`) i ispravno bacanje izuzetka pri pristupu nepostojećem ključu.

- **Test 3: Kolizije sa namernim sudaranjem u isti bucket**  
  Cilj je da se testira ponašanje rešenja pri velikom broju kolizija, gde se svi ključevi namerno mapiraju u isti hash bucket, kako bi se proverila ispravnost pronalaska i dodavanja u ovakvim uslovima.

- **Test 4: Performanse za veliki broj elemenata**  
  Mere se brzine dodavanja i pronalaženja milion elemenata, radi ocene efikasnosti implementacije na velikom opterećenju.

- **Test 5: Enumeracija i rad sa kolekcijama ključeva i vrednosti**  
  Testira enumeraciju svih parova ključ-vrednost, kao i pristup kolekcijama ključeva (`Keys`) i vrednosti (`Values`).

- **Test 6: Rad sa kompleksnim i referentnim tipovima**  
  Proverava da li kolekcija pravilno funkcioniše sa složenim tipovima ključeva (npr. tuple) i vrednosti koje su referentni tipovi (npr. lista stringova).

- **Test 7: Brisanje svih elemenata i ponovno korišćenje kolekcije**  
  Testira `Clear` metodu i dodavanje novih elemenata nakon brisanja, kako bi se osigurala ispravna ponovna inicijalizacija.

- **Test 8: Benchmark poređenje sa standardnim Dictionary**  
  Izvršava seriju benchmark testova upoređujući brzine dodavanja, pronalaženja, uklanjanja i enumeracije elemenata između `MySimpleDictionary` i `System.Collections.Generic.Dictionary`. Ovo omogućava realnu procenu performansi i optimizacija.

![Testovi i Benchmark MySimpleDictionary](https://github.com/user-attachments/assets/dc4ad078-3733-439f-a238-4e87b2eca17c)

*Prikaz rezultata testova i benchmark-a za implementaciju `MySimpleDictionary`. Mogu se videti osnovni testovi, testovi kolizija, performansi, kao i poređenje sa .NET `Dictionary` klasom.*


---


