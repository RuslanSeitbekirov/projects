using System;
using System.Collections.Generic;

namespace DictionariesDemo
{
    // =========================================================================
    //  Пункт 1. Свой словарь.
    //
    //  Словарь хранит пары "ключ — значение" и очень быстро находит значение
    //  по ключу. Как он это делает, я понял не сразу, поэтому записываю подробно.
    //
    //  Идея называется ХЕШ-ТАБЛИЦА:
    //
    //    1) Внутри словаря есть массив "корзин" (buckets).
    //    2) У любого объекта в C# есть метод GetHashCode() — он превращает
    //       объект в целое число (хеш-код). Для одинаковых ключей число
    //       всегда одинаковое.
    //    3) По хеш-коду вычисляется номер корзины:
    //           номер = хеш-код % количество_корзин
    //    4) Пара кладётся именно в эту корзину.
    //
    //  При поиске делаем то же самое: считаем номер корзины по ключу и смотрим
    //  ТОЛЬКО в неё. Не нужно перебирать все элементы — поэтому поиск почти
    //  мгновенный, сколько бы элементов ни было.
    //
    //  Иногда у разных ключей получается одинаковый номер корзины — это
    //  называется КОЛЛИЗИЯ. Поэтому каждая корзина — это маленький список:
    //  в ней может лежать несколько пар, и внутри корзины ключи сравниваются
    //  уже обычным перебором.
    //
    //  Чтобы корзины не переполнялись, при росте словаря массив корзин
    //  увеличивается, и все пары раскладываются заново.
    // =========================================================================

    /// <summary>
    /// Мой словарь. TKey — тип ключа, TValue — тип значения
    /// (подставляются при создании: MyDictionary&lt;string, int&gt;).
    /// </summary>
    public class MyDictionary<TKey, TValue>
    {
        /// <summary>Одна пара "ключ — значение".</summary>
        private class Entry
        {
            public TKey Key;
            public TValue Value;
        }

        // Массив корзин. Каждая корзина — список пар (или null, если она пуста).
        private List<Entry>[] buckets = new List<Entry>[16];

        // Сколько всего пар в словаре
        private int count;

        /// <summary>Количество пар.</summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>Вычисляет номер корзины для ключа.</summary>
        private int GetBucketIndex(TKey key)
        {
            // Хеш-код бывает отрицательным, а номер корзины — нет.
            // "& 0x7FFFFFFF" обнуляет знаковый бит, то есть делает число неотрицательным.
            int hash = key.GetHashCode() & 0x7FFFFFFF;

            // Остаток от деления всегда в диапазоне 0 .. (число корзин - 1)
            return hash % buckets.Length;
        }

        /// <summary>Ищет пару с таким ключом. Возвращает null, если её нет.</summary>
        private Entry FindEntry(TKey key)
        {
            List<Entry> bucket = buckets[GetBucketIndex(key)];
            if (bucket == null)
            {
                return null;   // корзина пуста — ключа точно нет
            }

            // В корзине обычно 0–2 пары, так что этот перебор очень короткий
            foreach (Entry entry in bucket)
            {
                if (entry.Key.Equals(key))
                {
                    return entry;
                }
            }
            return null;
        }

        /// <summary>Добавляет пару. Ключи в словаре не повторяются.</summary>
        public void Add(TKey key, TValue value)
        {
            if (FindEntry(key) != null)
            {
                // Так же ведёт себя и стандартный Dictionary
                throw new ArgumentException("Такой ключ уже есть: " + key);
            }

            // Если пар стало почти столько же, сколько корзин, — расширяемся
            if (count >= buckets.Length * 3 / 4)
            {
                Resize();
            }

            int index = GetBucketIndex(key);
            if (buckets[index] == null)
            {
                buckets[index] = new List<Entry>();
            }

            Entry entry = new Entry();
            entry.Key = key;
            entry.Value = value;
            buckets[index].Add(entry);
            count++;
        }

        /// <summary>Увеличивает число корзин вдвое и раскладывает пары заново.</summary>
        private void Resize()
        {
            List<Entry>[] oldBuckets = buckets;
            buckets = new List<Entry>[oldBuckets.Length * 2];

            // Номер корзины зависит от количества корзин, поэтому после
            // увеличения массива каждую пару нужно положить на новое место.
            foreach (List<Entry> bucket in oldBuckets)
            {
                if (bucket == null)
                {
                    continue;
                }

                foreach (Entry entry in bucket)
                {
                    int index = GetBucketIndex(entry.Key);
                    if (buckets[index] == null)
                    {
                        buckets[index] = new List<Entry>();
                    }
                    buckets[index].Add(entry);
                }
            }
        }

        /// <summary>Есть ли такой ключ.</summary>
        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) != null;
        }

        /// <summary>
        /// Безопасное чтение: не бросает исключение, а возвращает true/false.
        /// Найденное значение отдаётся через out-параметр.
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            Entry entry = FindEntry(key);
            if (entry == null)
            {
                value = default(TValue);   // "пустое" значение типа (0, null...)
                return false;
            }

            value = entry.Value;
            return true;
        }

        /// <summary>
        /// ИНДЕКСАТОР — позволяет писать dictionary[key], как у массива,
        /// только в квадратных скобках не номер, а ключ.
        /// Объявляется как свойство с именем this и параметром в [ ].
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                Entry entry = FindEntry(key);
                if (entry == null)
                {
                    throw new KeyNotFoundException("Нет ключа: " + key);
                }
                return entry.Value;
            }
            set
            {
                // value — то, что стоит справа от знака "="
                Entry entry = FindEntry(key);
                if (entry == null)
                {
                    Add(key, value);       // ключа не было — добавляем
                }
                else
                {
                    entry.Value = value;   // ключ был — меняем значение
                }
            }
        }

        /// <summary>Удаляет пару. Возвращает true, если она была.</summary>
        public bool Remove(TKey key)
        {
            List<Entry> bucket = buckets[GetBucketIndex(key)];
            if (bucket == null)
            {
                return false;
            }

            for (int i = 0; i < bucket.Count; i++)
            {
                if (bucket[i].Key.Equals(key))
                {
                    bucket.RemoveAt(i);
                    count--;
                    return true;
                }
            }
            return false;
        }

        /// <summary>Сколько сейчас корзин (для любопытства).</summary>
        public int BucketCount
        {
            get { return buckets.Length; }
        }

        /// <summary>Сколько пар в самой "населённой" корзине (показывает коллизии).</summary>
        public int LongestBucket
        {
            get
            {
                int longest = 0;
                foreach (List<Entry> bucket in buckets)
                {
                    if (bucket != null && bucket.Count > longest)
                    {
                        longest = bucket.Count;
                    }
                }
                return longest;
            }
        }
    }
}
