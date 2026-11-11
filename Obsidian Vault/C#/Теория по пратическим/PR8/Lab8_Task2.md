# Практическая работа №8. Задание 2 — Словари и индексаторы

Папка проекта: `Lab8_Task2/DictionariesDemo/` — консольное приложение, три файла.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки, комментарии и строки, которые только печатают заголовок, отдельно не расписываются.
> WPF в этой работе нет.
>
> | Пункт задания | Файл |
> |---|---|
> | 1. Свой словарь, скорость выборки по ключу | `MyDictionary.cs`, метод `SpeedTest` в `Program.cs` |
> | 2. «Умный массив» с помощью индексатора | `SmartArray.cs`, метод `SmartArrayDemo` в `Program.cs` |

---

## 1. Немного теории

### 1.1. Словарь и хеш-таблица

**Словарь** хранит пары «ключ — значение» и быстро находит значение по ключу. Внутри он устроен как **хеш-таблица**:

1. Есть массив «корзин» (buckets).
2. У любого объекта есть метод `GetHashCode()` — он превращает объект в целое число (**хеш-код**). Для равных ключей число всегда одинаковое.
3. Номер корзины: `хеш-код % число_корзин`.
4. Пара кладётся в эту корзину.

При поиске вычисляется та же корзина, и просматривается **только она** (числа в примере условные):

```
ключ "Москва" → GetHashCode() → 1 846 213 907 → % 16 → корзина №3 → ищем среди 1–2 пар
```

Поэтому время поиска почти не зависит от количества элементов — это записывают как **O(1)**. Для сравнения, поиск перебором в списке — **O(n)**: в среднем просматривается половина элементов.

**Коллизия** — разные ключи попали в одну корзину. Поэтому корзина — это маленький список, и внутри неё ключи сравниваются перебором через `Equals`.

**Расширение.** Когда пар становится много, корзины переполняются и поиск замедляется. Тогда массив корзин увеличивают и раскладывают все пары заново.

### 1.2. Индексатор

**Индексатор** — особое свойство, которое позволяет обращаться к объекту через квадратные скобки, как к массиву:

```csharp
public int this[int index]      // имя — this, параметр — в квадратных скобках
{
    get { ... }                 // x = obj[2];
    set { ... }                 // obj[2] = 7;   значение — в переменной value
}
```

Параметр может быть любого типа — у словаря это ключ (`codes["Москва"]`). Внутри `get` и `set` можно писать любые проверки — именно этим «умный массив» отличается от обычного.

---

## 2. `MyDictionary.cs` — свой словарь

### 2.1. Заголовок и поля

```csharp
1   using System;
2   using System.Collections.Generic;
4   namespace DictionariesDemo
39      public class MyDictionary<TKey, TValue>
42          private class Entry
44              public TKey Key;
45              public TValue Value;
49          private List<Entry>[] buckets = new List<Entry>[16];
52          private int count;
55          public int Count
57              get { return count; }
```

- **стр. 1** — `ArgumentException`; **стр. 2** — `List<T>`, `KeyNotFoundException`.
- **стр. 39** — обобщённый класс с **двумя** параметрами типа: тип ключа и тип значения.
- **стр. 42–45** — **вложенный** закрытый класс: одна пара «ключ — значение». Снаружи словаря он не виден.
- **стр. 49** — массив корзин: 16 штук, каждая — список пар или `null`, если пуста.
- **стр. 52–57** — количество пар и свойство для чтения.

### 2.2. Номер корзины и поиск пары

```csharp
61          private int GetBucketIndex(TKey key)
65              int hash = key.GetHashCode() & 0x7FFFFFFF;
68              return hash % buckets.Length;
```

- **стр. 65** — хеш-код ключа. Он бывает отрицательным, а индекс массива — нет; `& 0x7FFFFFFF` — побитовое «И» с числом, у которого все биты единицы, кроме знакового, — обнуляет знак.
- **стр. 68** — остаток от деления всегда в диапазоне `0 … число_корзин − 1`.

```csharp
72          private Entry FindEntry(TKey key)
74              List<Entry> bucket = buckets[GetBucketIndex(key)];
75              if (bucket == null)
77                  return null;
81              foreach (Entry entry in bucket)
83                  if (entry.Key.Equals(key))
85                      return entry;
88              return null;
```

- **стр. 74** — сразу берём нужную корзину — в этом вся скорость словаря.
- **стр. 75–77** — корзина пуста: ключа точно нет.
- **стр. 81–85** — перебор внутри корзины (обычно 0–2 пары); ключи сравниваются через `Equals`.
- **стр. 88** — в корзине есть пары, но с другими ключами (коллизия).

На этом методе построены все остальные.

### 2.3. `Add` и `Resize`

```csharp
92          public void Add(TKey key, TValue value)
94              if (FindEntry(key) != null)
97                  throw new ArgumentException("Такой ключ уже есть: " + key);
101             if (count >= buckets.Length * 3 / 4)
103                 Resize();
106             int index = GetBucketIndex(key);
107             if (buckets[index] == null)
109                 buckets[index] = new List<Entry>();
112             Entry entry = new Entry();
113             entry.Key = key;
114             entry.Value = value;
115             buckets[index].Add(entry);
116             count++;
```

- **стр. 94–97** — ключи в словаре не повторяются; так же ведёт себя стандартный `Dictionary`.
- **стр. 101–103** — словарь заполнен на 75% — расширяемся (это называют коэффициентом заполнения).
- **стр. 106** — номер корзины считается **после** возможного расширения: он зависит от числа корзин.
- **стр. 107–109** — корзина создаётся при первой надобности.
- **стр. 112–116** — создать пару, положить в корзину, увеличить счётчик.

```csharp
120         private void Resize()
122             List<Entry>[] oldBuckets = buckets;
123             buckets = new List<Entry>[oldBuckets.Length * 2];
127             foreach (List<Entry> bucket in oldBuckets)
129                 if (bucket == null)
131                     continue;
134                 foreach (Entry entry in bucket)
136                     int index = GetBucketIndex(entry.Key);
137                     if (buckets[index] == null)
139                         buckets[index] = new List<Entry>();
141                     buckets[index].Add(entry);
```

- **стр. 122–123** — запомнить старый массив, создать новый вдвое больше.
- **стр. 127–131** — пустые корзины пропускаются (`continue` — перейти к следующему шагу цикла).
- **стр. 134–141** — каждую пару нужно положить **на новое место**: номер корзины зависит от их количества.

### 2.4. `ContainsKey` и `TryGetValue`

```csharp
147         public bool ContainsKey(TKey key)
149             return FindEntry(key) != null;
156         public bool TryGetValue(TKey key, out TValue value)
158             Entry entry = FindEntry(key);
159             if (entry == null)
161                 value = default(TValue);
162                 return false;
165             value = entry.Value;
166             return true;
```

- **стр. 149** — ключ есть, если пара найдена.
- **стр. 156** — `out` — параметр, через который метод **возвращает** значение; метод обязан его присвоить.
- **стр. 161** — `default(TValue)` — «пустое» значение типа: 0 для чисел, `null` для ссылочных типов.
- **стр. 165–166** — найденное значение и `true`.

### 2.5. Индексатор по ключу

```csharp
174         public TValue this[TKey key]
178                 Entry entry = FindEntry(key);
179                 if (entry == null)
181                     throw new KeyNotFoundException("Нет ключа: " + key);
183                 return entry.Value;
188                 Entry entry = FindEntry(key);
189                 if (entry == null)
191                     Add(key, value);
195                     entry.Value = value;
```

- **стр. 174** — индексатор: в скобках не номер, а **ключ**.
- **стр. 178–183** — блок `get`: чтение несуществующего ключа — исключение, как у стандартного словаря.
- **стр. 188–195** — блок `set`: `value` — то, что стоит справа от `=`. Ключа не было — добавляем, был — меняем значение. Поэтому `dict[key] = x` никогда не бросает исключение, в отличие от `Add`.

### 2.6. `Remove` и статистика

```csharp
201         public bool Remove(TKey key)
203             List<Entry> bucket = buckets[GetBucketIndex(key)];
204             if (bucket == null)
206                 return false;
209             for (int i = 0; i < bucket.Count; i++)
211                 if (bucket[i].Key.Equals(key))
213                     bucket.RemoveAt(i);
214                     count--;
215                     return true;
218             return false;
```

- **стр. 203–206** — корзина пуста — удалять нечего.
- **стр. 209–215** — цикл `for` с индексом (а не `foreach`), потому что удалять нужно по номеру; после удаления сразу `return`.

```csharp
222         public int BucketCount
224             get { return buckets.Length; }
228         public int LongestBucket
232                 int longest = 0;
233                 foreach (List<Entry> bucket in buckets)
235                     if (bucket != null && bucket.Count > longest)
237                         longest = bucket.Count;
240                 return longest;
```

- **стр. 222–224** — сколько сейчас корзин.
- **стр. 228–240** — сколько пар в самой полной корзине: показывает, насколько сильны коллизии.

---

## 3. `SmartArray.cs` — «умный массив»

```csharp
1   using System;
3   namespace DictionariesDemo
34      public class SmartArray
38          private readonly int[] data;
41          public SmartArray(int size)
43              if (size < 0)
45                  size = 0;
47              data = new int[size];
51          public int Length
53              get { return data.Length; }
60          public string LastError { get; private set; }
63          private bool IsValidIndex(int index)
65              return index >= 0 && index < data.Length;
```

- **стр. 38** — настоящий массив **спрятан** (`private`): добраться до него можно только через индексатор с проверками.
- **стр. 41–47** — конструктор; отрицательный размер заменяется нулём.
- **стр. 51–53** — длина.
- **стр. 60** — текст последней ошибки или `null`. `private set` — менять может только сам класс.
- **стр. 63–65** — проверка, что индекс попадает в границы.

```csharp
69          public int this[int index]
73                  if (!IsValidIndex(index))
75                      LastError = "чтение: индекс " + index + " вне границ 0.." + (data.Length - 1);
76                      return 0;
79                  LastError = null;
80                  return data[index];
85                  if (!IsValidIndex(index))
87                      LastError = "запись: индекс " + index + " вне границ 0.." + (data.Length - 1);
88                      return;
91                  if (value < 0)
93                      LastError = "запись: отрицательное число " + value + " не принимается";
94                      return;
97                  LastError = null;
98                  data[index] = value;
```

- **стр. 69** — **индексатор**: благодаря ему пишут `smart[2]`.
- **стр. 73–76** — блок `get`: индекс вне границ — запоминаем ошибку и возвращаем безопасный 0 вместо падения.
- **стр. 79–80** — всё в порядке: ошибки нет, возвращаем элемент.
- **стр. 85–88** — блок `set`: индекс вне границ — ничего не записываем.
- **стр. 91–94** — отрицательное число — не принимаем, старое значение остаётся.
- **стр. 97–98** — запись.

```csharp
103         public override string ToString()
105             return "[" + string.Join(", ", data) + "]";
```

- **стр. 103–105** — все элементы одной строкой в квадратных скобках.

**Моё решение по поведению.** В задании сказано только «не позволяет выйти за границы и использовать отрицательные числа». Здесь массив **не роняет программу**: неверная запись игнорируется, неверное чтение даёт 0, причина — в `LastError`. Альтернатива — бросать исключение (`ArgumentOutOfRangeException`); для этого достаточно заменить строки с `LastError = ...; return ...;` на `throw`.

---

## 4. `Program.cs`

### 4.1. `Main`

```csharp
1   using System;
2   using System.Collections.Generic;
3   using System.Diagnostics;
4   using System.Text;
15      internal class Program
17          private static void Main()
19              Console.OutputEncoding = Encoding.UTF8;
22              DictionaryBasics();
26              SpeedTest();
30              SmartArrayDemo();
32              if (!Console.IsInputRedirected)
36                  Console.ReadKey();
```

- **стр. 2** — `List`, `Dictionary`, `KeyValuePair`; **стр. 3** — `Stopwatch`.
- **стр. 22–30** — три части; перед каждой печатается заголовок.
- **стр. 32–36** — ожидание клавиши в обычной консоли.

### 4.2. `DictionaryBasics` — работа с моим словарём

```csharp
47              MyDictionary<string, int> codes = new MyDictionary<string, int>();
50              codes.Add("Симферополь", 3652);
51              codes.Add("Москва", 495);
54              codes["Казань"] = 843;
56              Console.WriteLine("Пар в словаре: " + codes.Count);
59              Console.WriteLine("Код Симферополя: " + codes["Симферополь"]);
62              codes["Москва"] = 499;
63              Console.WriteLine("Новый код Москвы: " + codes["Москва"]);
```

- **стр. 47** — ключ — название города (`string`), значение — телефонный код (`int`).
- **стр. 50–51** — добавление методом `Add`.
- **стр. 54** — добавление через индексатор (`set`): ключа ещё нет — создаётся.
- **стр. 59** — чтение через индексатор (`get`).
- **стр. 62–63** — запись для существующего ключа меняет значение.

```csharp
66–67           Console.WriteLine("Есть «Казань»: " + codes.ContainsKey("Казань") + ", есть «Париж»: " + codes.ContainsKey("Париж"));
70              int code;
71              if (codes.TryGetValue("Париж", out code))
83                  Console.WriteLine(codes["Париж"]);
85              catch (KeyNotFoundException ex)
87                  Console.WriteLine("codes[«Париж»] → исключение: " + ex.Message);
93                  codes.Add("Казань", 1);
95              catch (ArgumentException ex)
97                  Console.WriteLine("Повторный Add → исключение: " + ex.Message);
101             Console.WriteLine("Remove(«Москва»): " + codes.Remove("Москва") + ", осталось пар: " + codes.Count);
```

- **стр. 66–67** — проверка наличия ключей.
- **стр. 70–71** — безопасное чтение: переменная для результата и вызов с `out`. Для «Парижа» метод вернёт `false` — выполнится ветка `else` (стр. 77).
- **стр. 83–87** — чтение несуществующего ключа через `[ ]` — исключение, оно перехвачено.
- **стр. 93–97** — повторный `Add` того же ключа — тоже исключение.
- **стр. 101** — удаление: `true` и уменьшившееся количество.

### 4.3. `SpeedTest` — замер скорости выборки

```csharp
116             const int Size = 100000;
117             const int Searches = 1000;
121             List<KeyValuePair<string, int>> list = new List<KeyValuePair<string, int>>();
122             MyDictionary<string, int> mine = new MyDictionary<string, int>();
123             Dictionary<string, int> standard = new Dictionary<string, int>();
125             for (int i = 0; i < Size; i++)
127                 string key = "user" + i;
128                 list.Add(new KeyValuePair<string, int>(key, i));
129                 mine.Add(key, i);
130                 standard.Add(key, i);
134             string[] keys = new string[Searches];
135             for (int i = 0; i < Searches; i++)
137                 keys[i] = "user" + (i * (Size / Searches));
```

- **стр. 116–117** — 100 000 пар, 1000 поисков.
- **стр. 121–123** — три хранилища: список пар (`KeyValuePair` — готовая структура «ключ + значение»), мой словарь, стандартный словарь.
- **стр. 125–130** — одинаковые данные во всех трёх: ключи `user0`, `user1`, …
- **стр. 134–137** — ключи для поиска, равномерно по всему диапазону (каждый сотый).

```csharp
143             long sum;
146             Stopwatch watch = Stopwatch.StartNew();
147             sum = 0;
148             foreach (string key in keys)
151                 foreach (KeyValuePair<string, int> pair in list)
153                     if (pair.Key == key)
155                         sum += pair.Value;
156                         break;
160             watch.Stop();
161             double listTime = watch.Elapsed.TotalMilliseconds;
```

- **стр. 143** — контрольная сумма: по ней видно, что все три способа нашли одно и то же.
- **стр. 146** — секундомер создан и запущен.
- **стр. 148–156** — **линейный поиск**: для каждого ключа идём по списку с начала; нашли — прибавили и вышли (`break`).
- **стр. 160–161** — время в миллисекундах с дробной частью.

```csharp
165             watch.Restart();
166             sum = 0;
167             foreach (string key in keys)
169                 sum += mine[key];
171             watch.Stop();
172             double mineTime = watch.Elapsed.TotalMilliseconds;
176–183         (то же для standard[key] → standardTime)
186–191         (вывод: во сколько раз быстрее, число корзин, самая полная корзина)
```

- **стр. 165–172** — то же через индексатор моего словаря.
- **стр. 176–183** — и через стандартный `Dictionary`.
- **стр. 186–191** — итоги: отношение времени, а также `BucketCount` и `LongestBucket`.

При проверке: список — около 700 мс, `MyDictionary` — около 0,3 мс, `Dictionary` — около 0,1 мс; разница между списком и словарём — больше чем в 2000 раз. Стандартный словарь быстрее моего: он хранит пары в одном массиве структур, а не в отдельных списках.

### 4.4. `SmartArrayDemo`

```csharp
200             SmartArray smart = new SmartArray(5);
201             Console.WriteLine("Создан массив длиной " + smart.Length + ": " + smart);
205             smart[0] = 10;
206             smart[4] = 50;
210             Console.WriteLine("smart[4] = " + smart[4]);
213             smart[7] = 99;
214             Console.WriteLine("smart[7] = 99   → " + smart.LastError);
216             smart[-1] = 99;
220             smart[1] = -5;
224             int value = smart[100];
225             Console.WriteLine("smart[100] вернул " + value + " → " + smart.LastError);
228             Console.WriteLine("Массив после всех попыток: " + smart);
231             int[] usual = new int[5];
234                 usual[7] = 99;
236             catch (IndexOutOfRangeException)
```

- **стр. 200–201** — массив из пяти нулей; при склейке со строкой вызывается `ToString`.
- **стр. 205–206** — обычная запись: на самом деле вызывается блок `set` индексатора.
- **стр. 210** — чтение: блок `get`.
- **стр. 213–217** — запись за правой и левой границей — не выполняется, причина в `LastError`.
- **стр. 220–221** — отрицательное число — не принимается.
- **стр. 224–225** — чтение за границей: программа не падает, приходит 0.
- **стр. 228** — массив не испорчен: `[10, 0, 0, 0, 50]`.
- **стр. 231–236** — для сравнения обычный массив в такой же ситуации бросает `IndexOutOfRangeException`.

---

## 5. Важные моменты (вопросы на защите)

1. **Почему поиск в словаре быстрый?** — По хеш-коду ключа сразу вычисляется корзина; перебирать все элементы не нужно.
2. **Что такое коллизия и как она решается?** — Разные ключи попали в одну корзину; корзина — список, внутри неё ключи сравниваются через `Equals`.
3. **Зачем `Resize`?** — Чтобы корзины оставались короткими; после увеличения все пары перекладываются, потому что номер корзины зависит от их числа.
4. **Какие требования к ключу?** — Корректные `GetHashCode` и `Equals`: равные объекты обязаны давать равный хеш-код. Ключ не должен меняться, пока лежит в словаре.
5. **`Add` и `dict[key] = value` — в чём разница?** — `Add` бросает исключение, если ключ уже есть; индексатор перезаписывает значение.
6. **`dict[key]` и `TryGetValue`?** — Индексатор бросает `KeyNotFoundException`, `TryGetValue` возвращает `false`.
7. **Что такое индексатор?** — Свойство с именем `this` и параметром в квадратных скобках; даёт объекту синтаксис массива.
8. **Может ли индексатор принимать не `int`?** — Да, любой тип и даже несколько параметров.
9. **Чем «умный массив» лучше обычного?** — В `get` и `set` индексатора можно проверять индекс и значение; обычный массив так не умеет.
10. **Сохраняет ли словарь порядок элементов?** — Нет, порядок определяется хеш-кодами; для упорядоченности есть `SortedDictionary`.
