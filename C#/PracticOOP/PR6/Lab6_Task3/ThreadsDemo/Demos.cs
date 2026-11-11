using System;
using System.Diagnostics;        // Stopwatch — секундомер
using System.Threading;          // Thread, Monitor, Mutex, Semaphore, Interlocked
using System.Threading.Tasks;    // Task, Parallel

namespace ThreadsDemo
{
    /// <summary>
    /// Примеры к пунктам 2–6 задания 3. Каждый метод — отдельный небольшой опыт.
    /// Результаты выводятся через параметр log: это делегат Action&lt;string&gt;,
    /// то есть "метод, который принимает строку". Окно передаёт сюда свой метод
    /// Log, и текст попадает в поле внизу окна. Благодаря этому сам класс
    /// ничего не знает про окно и WPF.
    /// </summary>
    public static class Demos
    {
        // =====================================================================
        //  Вспомогательный метод: запустить несколько потоков и дождаться всех
        // =====================================================================

        /// <summary>
        /// Создаёт count потоков, в каждом выполняет work(номер потока)
        /// и ждёт, пока все закончат.
        /// </summary>
        private static void RunThreads(int count, Action<int> work)
        {
            Thread[] threads = new Thread[count];

            for (int i = 0; i < count; i++)
            {
                // Важная тонкость, на которой легко ошибиться: лямбда ниже
                // запоминает не ЗНАЧЕНИЕ переменной, а САМУ переменную.
                // Если написать work(i + 1), то к моменту запуска потока
                // i уже успеет измениться, и номера перепутаются.
                // Поэтому делаю отдельную копию для каждого потока.
                int number = i + 1;

                // Поток создаётся из метода, который он должен выполнить.
                // Здесь метод записан лямбда-выражением.
                threads[i] = new Thread(() => work(number));

                // Пока не вызван Start, поток не работает
                threads[i].Start();
            }

            // Join — "подожди, пока этот поток закончится".
            // Без него метод вернулся бы сразу, не дожидаясь результатов.
            foreach (Thread t in threads)
            {
                t.Join();
            }
        }

        // =====================================================================
        //  Пункт 2. lock и класс Monitor
        // =====================================================================

        /// <summary>
        /// Показывает, зачем нужна блокировка, и три способа её сделать.
        /// </summary>
        public static void LockAndMonitor(Action<string> log)
        {
            const int ThreadCount = 4;
            const int Steps = 1000000;
            int expected = ThreadCount * Steps;

            log("=== 2. lock и Monitor ===");
            log("4 потока по 1 000 000 раз увеличивают общий счётчик. Должно получиться " + expected + ".");

            // Общая переменная, которую меняют все потоки
            int counter = 0;

            // Объект-"замок". Сам по себе он ничего не хранит —
            // он нужен только как то, что можно "запереть".
            object locker = new object();

            // ---- а) Без блокировки ----
            // counter++ кажется одним действием, но на самом деле их три:
            // прочитать значение, прибавить 1, записать обратно.
            // Два потока могут прочитать одно и то же старое значение,
            // и одно из прибавлений потеряется ("состояние гонки").
            RunThreads(ThreadCount, number =>
            {
                for (int i = 0; i < Steps; i++)
                {
                    counter++;
                }
            });
            if (counter == expected)
            {
                log("а) без блокировки:  " + counter + "  (в этот раз повезло — запустите ещё раз)");
            }
            else
            {
                log("а) без блокировки:  " + counter + "  <-- часть прибавлений потерялась!");
            }

            // ---- б) Оператор lock ----
            // Код внутри lock в каждый момент выполняет только ОДИН поток.
            // Остальные ждут у "закрытой двери", пока первый не выйдет.
            counter = 0;
            RunThreads(ThreadCount, number =>
            {
                for (int i = 0; i < Steps; i++)
                {
                    lock (locker)
                    {
                        counter++;
                    }
                }
            });
            log("б) lock:            " + counter);

            // ---- в) Monitor.Enter / Monitor.Exit ----
            // Оказывается, lock — это просто короткая запись вот такого кода.
            // finally нужен, чтобы замок открылся, даже если внутри случится
            // ошибка, иначе остальные потоки будут ждать вечно.
            counter = 0;
            RunThreads(ThreadCount, number =>
            {
                for (int i = 0; i < Steps; i++)
                {
                    Monitor.Enter(locker);      // запереть
                    try
                    {
                        counter++;
                    }
                    finally
                    {
                        Monitor.Exit(locker);   // отпереть
                    }
                }
            });
            log("в) Monitor.Enter:   " + counter);

            // ---- г) Monitor.TryEnter ----
            // TryEnter не ждёт бесконечно: он пробует войти в течение
            // указанного времени и возвращает true/false. Через lock так не сделать.
            Thread holder = new Thread(() =>
            {
                lock (locker)
                {
                    Thread.Sleep(500);          // держим замок полсекунды
                }
            });
            holder.Start();
            Thread.Sleep(50);                   // даём потоку время занять замок

            bool entered = Monitor.TryEnter(locker, 100);   // ждём не больше 100 мс
            log("г) TryEnter, пока замок занят:   " + entered + " (не дождались)");
            if (entered)
            {
                Monitor.Exit(locker);
            }

            holder.Join();                      // ждём, пока поток освободит замок

            entered = Monitor.TryEnter(locker, 100);
            log("   TryEnter, когда замок свободен: " + entered);
            if (entered)
            {
                Monitor.Exit(locker);
            }

            // ---- д) Monitor.Wait / Monitor.Pulse ----
            // Wait — "отпусти замок и спи, пока тебя не разбудят".
            // Pulse — "разбуди один поток, который ждёт на этом замке".
            // Так один поток может дождаться сигнала от другого.
            bool ready = false;

            Thread waiter = new Thread(() =>
            {
                lock (locker)
                {
                    // Проверяем условие в цикле: так советуют делать всегда,
                    // на случай если поток разбудили, а условие ещё не выполнено.
                    while (!ready)
                    {
                        Monitor.Wait(locker);
                    }
                    log("д) ждущий поток получил сигнал Pulse и проснулся");
                }
            });
            waiter.Start();

            Thread.Sleep(300);
            lock (locker)
            {
                ready = true;
                log("д) главный поток посылает сигнал Pulse");
                Monitor.Pulse(locker);
            }
            waiter.Join();

            log("");
        }

        // =====================================================================
        //  Пункт 3. Мьютекс и семафор
        // =====================================================================

        /// <summary>
        /// Мьютекс пускает к ресурсу один поток, семафор — не больше N потоков.
        /// </summary>
        public static void MutexAndSemaphore(Action<string> log)
        {
            log("=== 3. Мьютекс ===");
            log("Мьютекс похож на lock: внутри может быть только один поток.");
            log("Отличие: мьютексу можно дать имя, и тогда он работает даже между разными программами.");

            // using сам вызовет Dispose в конце блока (мьютекс — системный ресурс)
            using (Mutex mutex = new Mutex())
            {
                RunThreads(3, number =>
                {
                    mutex.WaitOne();            // ждём, пока мьютекс освободится, и занимаем
                    try
                    {
                        log("  поток " + number + " вошёл");
                        Thread.Sleep(300);      // изображаем работу
                        log("  поток " + number + " вышел");
                    }
                    finally
                    {
                        mutex.ReleaseMutex();   // обязательно освобождаем
                    }
                });
            }
            log("Видно, что потоки не перемешиваются: вошёл - вышел, вошёл - вышел.");
            log("");

            log("=== 3. Семафор ===");
            log("Семафор пускает сразу несколько потоков, но не больше заданного числа.");
            log("5 потоков хотят войти, а мест только 2 (как парковка на 2 машины).");

            // Первое число — сколько мест свободно сейчас, второе — сколько мест всего
            using (Semaphore semaphore = new Semaphore(2, 2))
            {
                int inside = 0;   // сколько потоков сейчас внутри

                RunThreads(5, number =>
                {
                    semaphore.WaitOne();        // занять место (или ждать, пока появится)
                    try
                    {
                        // Interlocked.Increment — безопасное "++" для общей
                        // переменной без lock. Возвращает новое значение.
                        int now = Interlocked.Increment(ref inside);
                        log("  поток " + number + " вошёл, внутри сейчас: " + now);
                        Thread.Sleep(400);
                        Interlocked.Decrement(ref inside);
                    }
                    finally
                    {
                        semaphore.Release();    // освободить место
                    }
                });
            }
            log("Число \"внутри сейчас\" ни разу не было больше 2.");
            log("");
        }

        // =====================================================================
        //  Пункт 4. Класс Task (библиотека TPL)
        // =====================================================================

        /// <summary>
        /// Задачи (Task) — более новый и удобный способ, чем создавать Thread вручную.
        /// </summary>
        public static void Tasks(Action<string> log)
        {
            log("=== 4. Task ===");
            log("Этот код выполняется в потоке №" + Environment.CurrentManagedThreadId);

            // ---- а) Просто выполнить код в фоне ----
            // Task.Run берёт свободный поток из пула потоков — новый поток
            // каждый раз не создаётся, поэтому это быстрее, чем new Thread.
            Task simple = Task.Run(() =>
            {
                log("а) задача выполняется в потоке №" + Environment.CurrentManagedThreadId);
            });
            simple.Wait();   // подождать окончания (похоже на Join у потока)

            // ---- б) Задача, которая возвращает результат ----
            // У Thread получить результат неудобно, а Task<int> просто
            // "обещает" вернуть int. Результат лежит в свойстве Result.
            Task<int> sumTask = Task.Run(() =>
            {
                int sum = 0;
                for (int i = 1; i <= 100; i++)
                {
                    sum += i;
                }
                return sum;
            });
            // Если задача ещё не закончилась, Result подождёт её
            log("б) Task<int> вернула сумму 1..100 = " + sumTask.Result);

            // ---- в) Несколько задач сразу ----
            Task<int>[] tasks = new Task<int>[3];
            for (int i = 0; i < tasks.Length; i++)
            {
                int number = i + 1;   // копия для лямбды (как в RunThreads)
                tasks[i] = Task.Run(() =>
                {
                    Thread.Sleep(300 * number);   // изображаем работу разной длины
                    return number * number;
                });
            }
            Task.WaitAll(tasks);      // ждём сразу все задачи
            log("в) три задачи вернули: " + tasks[0].Result + ", " + tasks[1].Result + ", " + tasks[2].Result);

            // ---- г) Продолжение ----
            // ContinueWith: "когда задача закончится — сделай ещё вот это".
            // В параметр previous попадает закончившаяся задача.
            Task<int> first = Task.Run(() => 21);
            Task continuation = first.ContinueWith(previous =>
            {
                log("г) продолжение получило " + previous.Result + " и удвоило: " + (previous.Result * 2));
            });
            continuation.Wait();

            log("Чем Task удобнее Thread (как я понял):");
            log("  - потоки берутся из пула, а не создаются заново;");
            log("  - задача может вернуть результат (Task<T>);");
            log("  - есть WaitAll / WhenAll, продолжения, отмена;");
            log("  - исключение из задачи не теряется, его можно поймать;");
            log("  - на задачах построены async / await.");
            log("");
        }

        // =====================================================================
        //  Пункт 5. Parallel.For
        // =====================================================================

        /// <summary>
        /// Обычный цикл for против параллельного Parallel.For.
        /// </summary>
        public static void ParallelFor(Action<string> log)
        {
            const int Max = 3000000;

            log("=== 5. Parallel.For ===");
            log("Считаем, сколько простых чисел от 2 до " + Max + ". Процессоров (ядер): " + Environment.ProcessorCount);

            // ---- Обычный цикл: всё делает один поток ----
            Stopwatch watch = Stopwatch.StartNew();
            int count1 = 0;
            for (int i = 2; i <= Max; i++)
            {
                if (IsPrime(i))
                {
                    count1++;
                }
            }
            watch.Stop();
            long time1 = watch.ElapsedMilliseconds;
            log("обычный for:   найдено " + count1 + ", время " + time1 + " мс");

            // ---- Parallel.For: числа делятся между несколькими потоками ----
            // Параметры: с какого числа, ДО какого (не включая!) и что делать
            // с каждым числом i (лямбда).
            watch.Restart();
            int count2 = 0;
            Parallel.For(2, Max + 1, i =>
            {
                if (IsPrime(i))
                {
                    // count2 общая для всех потоков, поэтому обычный count2++
                    // здесь терял бы прибавления (как в опыте с lock).
                    Interlocked.Increment(ref count2);
                }
            });
            watch.Stop();
            long time2 = watch.ElapsedMilliseconds;
            log("Parallel.For:  найдено " + count2 + ", время " + time2 + " мс");

            if (time2 > 0)
            {
                log("Ускорение примерно в " + ((double)time1 / time2).ToString("0.0") + " раза.");
            }

            // ---- Порядок выполнения ----
            // В Parallel.For номера идут НЕ по порядку — кто какой успел взять.
            // Значит, его нельзя применять, если шаги зависят друг от друга.
            string order = "";
            object orderLock = new object();
            Parallel.For(0, 10, i =>
            {
                Thread.Sleep(30);   // небольшая "работа", чтобы подключилось несколько потоков
                lock (orderLock)
                {
                    order += i + " ";
                }
            });
            log("Порядок обработки чисел 0..9 (обычно вперемешку): " + order);
            log("");
        }

        /// <summary>Проверяет, простое ли число (делится только на 1 и на себя).</summary>
        private static bool IsPrime(int n)
        {
            if (n < 2)
            {
                return false;
            }
            // Достаточно проверить делители до корня из n
            for (int d = 2; (long)d * d <= n; d++)
            {
                if (n % d == 0)
                {
                    return false;
                }
            }
            return true;
        }

        // =====================================================================
        //  Пункт 6. async / await
        // =====================================================================

        /// <summary>
        /// Асинхронный метод: долго считает, но не "замораживает" того, кто его вызвал.
        ///
        /// Правила, которые я для себя записал:
        ///   - перед типом результата пишется слово async;
        ///   - метод возвращает не long, а Task&lt;long&gt; ("обещание" вернуть long);
        ///   - имя по традиции заканчивается на Async;
        ///   - внутри используется await.
        /// </summary>
        public static async Task<long> SlowSumAsync(int n, Action<string> log)
        {
            log("   SlowSumAsync: начал в потоке №" + Environment.CurrentManagedThreadId);

            // Task.Delay — пауза, которая НЕ занимает поток (в отличие от Thread.Sleep).
            // await означает: "здесь метод ставится на паузу и возвращает управление
            // тому, кто его вызвал. Когда задача закончится — продолжим со следующей строки".
            log("   SlowSumAsync: жду 2 секунды (как будто отвечает сервер)...");
            await Task.Delay(2000);

            // Тяжёлый расчёт отдаём в фоновый поток и опять ждём через await
            log("   SlowSumAsync: считаю сумму в фоновом потоке...");
            long result = await Task.Run(() =>
            {
                long sum = 0;
                for (int i = 1; i <= n; i++)
                {
                    sum += i;
                }
                return sum;
            });

            // Пишем обычный return с числом — компилятор сам "завернёт" его в Task<long>
            return result;
        }
    }
}
