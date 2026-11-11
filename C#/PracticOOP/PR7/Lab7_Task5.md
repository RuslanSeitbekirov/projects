# Практическая работа №7. Принципы SOLID (второе «Задание 4» методички)

Папка проекта: `Lab7_Task5/SolidPrinciples/` — консольное приложение, шесть файлов.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки и комментарии отдельно не расписываются. WPF в этой работе нет.
>
> В методичке два задания с номером 4. Первое (паттерны) лежит в `Lab7_Task4`, второе (SOLID) — здесь, в `Lab7_Task5`.
>
> Каждый принцип — в своём файле и в своём пространстве имён (`SolidPrinciples.SingleResponsibility` и т. д.),
> поэтому в разных файлах можно использовать одинаковые имена классов (`Demo`). В начале каждого файла
> в комментарии приведён «плохой» вариант — он **не компилируется**, это только иллюстрация.

---

## 1. Немного теории

**SOLID** — пять принципов проектирования классов, сформулированных Робертом Мартином. Цель — код, который легко изменять и расширять.

| Буква | Принцип | Одной фразой |
|---|---|---|
| **S** | Single Responsibility — единственной ответственности | один класс — одно дело |
| **O** | Open/Closed — открытости/закрытости | новое добавляется новым кодом, старый не правится |
| **L** | Liskov Substitution — подстановки Лисков | наследник не ломает поведение предка |
| **I** | Interface Segregation — разделения интерфейсов | много маленьких интерфейсов лучше одного большого |
| **D** | Dependency Inversion — инверсии зависимостей | зависеть от интерфейсов, а не от конкретных классов |

Принципы связаны: **O** и **D** достигаются одним приёмом — работой через интерфейс; **I** делает эти интерфейсы удобными; **L** гарантирует, что подстановка реализаций ничего не ломает; **S** делает классы маленькими.

Рядом часто упоминают **KISS** («Keep It Simple») — не усложнять без необходимости, **DRY** («Don't Repeat Yourself») — не дублировать код, **YAGNI** («You Aren't Gonna Need It») — не писать то, что пока не нужно. SOLID не должен применяться вопреки KISS: принципы — средство, а не цель.

---

## 2. `S_SingleResponsibility.cs` — принцип единственной ответственности

**Плохо:** класс `Order`, который и хранит товары, и печатает чек, и сохраняет себя в файл. У него три причины измениться, и правка одной может сломать остальное.

**Хорошо:** три класса, у каждого одна задача.

```csharp
1   using System;
2   using System.Collections.Generic;
3   using System.IO;
5   namespace SolidPrinciples.SingleResponsibility
31      public class Order
33          private readonly List<string> names = new List<string>();
34          private readonly List<decimal> prices = new List<decimal>();
36          public void AddItem(string name, decimal price)
38              names.Add(name);
39              prices.Add(price);
42          public int Count { get { return names.Count; } }
44          public string GetName(int index) { return names[index]; }
46          public decimal GetPrice(int index) { return prices[index]; }
48          public decimal Total()
50              decimal sum = 0;
51              foreach (decimal price in prices)
53                  sum += price;
55              return sum;
```

- **стр. 2** — `List<T>`; **стр. 3** — `StreamWriter`, `Path`.
- **стр. 31** — `Order` отвечает **только за данные заказа**.
- **стр. 33–34** — два параллельных списка: названия и цены (один индекс — один товар).
- **стр. 36–39** — добавить товар.
- **стр. 42–46** — доступ к данным: количество, название и цена по номеру.
- **стр. 48–55** — сумма заказа — это тоже данные заказа, а не оформление.

```csharp
60      public class OrderPrinter
62          public void Print(Order order)
64              for (int i = 0; i < order.Count; i++)
66                  Console.WriteLine("  " + order.GetName(i) + " — " + order.GetPrice(i) + " руб.");
68              Console.WriteLine("  ИТОГО: " + order.Total() + " руб.");
```

- **стр. 60** — отвечает **только за вид заказа на экране**. Захотели другой формат чека — меняется только этот класс.

```csharp
73      public class OrderFileSaver
75          public void Save(Order order, string path)
78              using (StreamWriter writer = new StreamWriter(path))
80                  for (int i = 0; i < order.Count; i++)
82                      writer.WriteLine(order.GetName(i) + ";" + order.GetPrice(i));
```

- **стр. 73** — отвечает **только за сохранение**. Решили хранить в базе данных — пишется другой класс, `Order` не трогается.
- **стр. 78** — `using` закроет файл (практика 7, задание 1).

```csharp
88      public static class Demo
90          public static void Run()
92              Order order = new Order();
93              order.AddItem("Мышь", 900m);
94              order.AddItem("Коврик", 300m);
97              new OrderPrinter().Print(order);
99              string path = Path.Combine(AppContext.BaseDirectory, "order.txt");
100             new OrderFileSaver().Save(order, path);
```

- **стр. 92–94** — заказ из двух товаров.
- **стр. 97** — печать: объект создаётся и сразу используется, переменная не нужна.
- **стр. 99–100** — сохранение в `order.txt` рядом с exe-файлом (строка 101 сообщает об этом).

---

## 3. `O_OpenClosed.cs` — принцип открытости/закрытости

**Плохо:** метод с цепочкой `if (customerType == "vip") ...`. Каждый новый вид покупателя заставляет править этот метод.

**Хорошо:** правило расчёта — отдельный класс с общим интерфейсом.

```csharp
1   using System;
3   namespace SolidPrinciples.OpenClosed
30      public interface ICustomerDiscount
32          string Title { get; }
33          decimal Apply(decimal price);
36      public class RegularCustomer : ICustomerDiscount
38          public string Title { get { return "обычный покупатель"; } }
39          public decimal Apply(decimal price) { return price; }
42      public class VipCustomer : ICustomerDiscount
44          public string Title { get { return "VIP (−20%)"; } }
45          public decimal Apply(decimal price) { return price * 0.80m; }
52      public class StudentCustomer : ICustomerDiscount
54          public string Title { get { return "студент (−10%)"; } }
55          public decimal Apply(decimal price) { return price * 0.90m; }
```

- **стр. 30–33** — интерфейс правила: название и расчёт цены.
- **стр. 36–45** — два исходных правила.
- **стр. 52–55** — правило, добавленное **позже**: новый класс, ни одной правки в остальных. Это и есть «открыт для расширения».

```csharp
62      public class PriceCalculator
64          public decimal GetPrice(decimal price, ICustomerDiscount discount)
66              return discount.Apply(price);
```

- **стр. 62–66** — «закрыт для изменения»: в калькуляторе нет ни одного `if` по видам покупателей, поэтому ради новой скидки его править не нужно.

```csharp
72          public static void Run()
74              PriceCalculator calculator = new PriceCalculator();
75              ICustomerDiscount[] customers = { new RegularCustomer(), new VipCustomer(), new StudentCustomer() };
77              foreach (ICustomerDiscount customer in customers)
79                  Console.WriteLine("  " + customer.Title + ": " + calculator.GetPrice(1000m, customer) + " руб.");
```

- **стр. 75–79** — один калькулятор, три правила: 1000, 800 и 900 руб.

По устройству это паттерн Стратегия из задания 4 — паттерны и есть способ соблюдать принципы.

---

## 4. `L_LiskovSubstitution.cs` — принцип подстановки Лисков

**Формулировка:** вместо объекта базового класса можно подставить объект наследника, и программа должна работать правильно.

### 4.1. Плохой вариант (он компилируется и запускается — чтобы показать ошибку)

```csharp
22      public class BadRectangle
24          public virtual int Width { get; set; }
25          public virtual int Height { get; set; }
27          public int Area()
29              return Width * Height;
33      public class BadSquare : BadRectangle
37          public override int Width
39              get { return base.Width; }
40              set { base.Width = value; base.Height = value; }
43          public override int Height
45              get { return base.Height; }
46              set { base.Width = value; base.Height = value; }
```

- **стр. 22–29** — прямоугольник: ширину и высоту можно менять **независимо** — это его «обещание».
- **стр. 24–25** — `virtual` у свойств: наследник может их переопределить.
- **стр. 33** — «квадрат — это прямоугольник», кажется логичным.
- **стр. 40, 46** — чтобы квадрат оставался квадратом, при записи одной стороны тайком меняется и другая. `base.Width` — обращение к свойству предка. Здесь обещание предка нарушено.

### 4.2. Хороший вариант

```csharp
56      public abstract class Shape
58          public abstract int Area();
61      public class Rectangle : Shape
63          public int Width { get; set; }
64          public int Height { get; set; }
66          public override int Area() { return Width * Height; }
69      public class Square : Shape
71          public int Side { get; set; }
73          public override int Area() { return Side * Side; }
```

- **стр. 56–58** — общий предок обещает только одно: у фигуры есть площадь. Это обещание честно выполняют оба наследника.
- **стр. 61–73** — прямоугольник и квадрат — независимые классы, никто никого не «портит».

### 4.3. Проверка

```csharp
82          private static void SetSizeAndCheck(BadRectangle rectangle, string name)
84              rectangle.Width = 5;
85              rectangle.Height = 4;
87              int area = rectangle.Area();
88–89           Console.WriteLine("  " + name + ": ждали 5 * 4 = 20, получили " + area + (area == 20 ? " — верно" : " — ОШИБКА!"));
92          public static void Run()
95              SetSizeAndCheck(new BadRectangle(), "прямоугольник");
99              SetSizeAndCheck(new BadSquare(), "квадрат      ");
102             Shape[] shapes = { new Rectangle { Width = 5, Height = 4 }, new Square { Side = 4 } };
103             foreach (Shape shape in shapes)
106                 Console.WriteLine("  " + shape.GetType().Name + ": площадь " + shape.Area());
```

- **стр. 82** — метод написан для прямоугольника и полагается на его поведение.
- **стр. 84–87** — ставим 5 и 4, ждём площадь 20.
- **стр. 95** — с прямоугольником — 20, верно.
- **стр. 99** — **подставили наследника** — получили 16: запись высоты 4 изменила и ширину. Метод перестал работать правильно — принцип нарушен.
- **стр. 102** — **инициализатор объекта**: `new Rectangle { Width = 5, Height = 4 }` создаёт объект и сразу задаёт свойства.
- **стр. 103–106** — с правильной иерархией любую фигуру можно подставить без сюрпризов; `GetType().Name` — имя реального класса.

---

## 5. `I_InterfaceSegregation.cs` — принцип разделения интерфейсов

**Плохо:** один «толстый» интерфейс `IShopItem` с методами `ShipByCourier()` и `Download()`. Бумажная книга вынуждена реализовать `Download()` и бросать в нём `NotSupportedException` — ошибка всплывёт только во время работы.

**Хорошо:** маленькие интерфейсы-«роли».

```csharp
1   using System;
3   namespace SolidPrinciples.InterfaceSegregation
38      public interface IShippable
40          void ShipByCourier();
44      public interface IDownloadable
46          void Download();
50      public class PaperBook : IShippable
52          public void ShipByCourier()      // стр. 54: печать сообщения
59      public class EBook : IDownloadable
61          public void Download()           // стр. 63: печать сообщения
71      public class VideoGame : IShippable, IDownloadable
73          public void ShipByCourier()      // стр. 75: печать сообщения
78          public void Download()           // стр. 80: печать сообщения
```

- **стр. 38–46** — два интерфейса, в каждом по одному методу: «можно отправить» и «можно скачать».
- **стр. 50** — бумажная книга берёт только доставку: лишних методов нет.
- **стр. 59** — электронная — только скачивание.
- **стр. 71** — игра продаётся и на диске, и в цифровом виде — класс реализует **оба** интерфейса (их у класса может быть сколько угодно).

```csharp
86          public static void Run()
89              IShippable[] forCourier = { new PaperBook(), new VideoGame() };
90              foreach (IShippable item in forCourier)
92                  item.ShipByCourier();
96              IDownloadable[] forDownload = { new EBook(), new VideoGame() };
97              foreach (IDownloadable item in forDownload)
99                  item.Download();
```

- **стр. 89–92** — курьерской службе нужны только товары, которые можно везти.
- **стр. 96–99** — серверу загрузок — только те, что можно скачать.
- Положить бумажную книгу в массив `forDownload` не даст **компилятор** — ошибка видна сразу, а не во время работы.

---

## 6. `D_DependencyInversion.cs` — принцип инверсии зависимостей

**Плохо:** `OrderService` сам создаёт `new EmailSender()` в своём поле. Он намертво привязан к почте: чтобы слать SMS, придётся его переписывать; проверить без настоящей отправки нельзя.

**Хорошо:** сервис зависит от интерфейса, а конкретный отправитель передаётся снаружи.

```csharp
1   using System;
2   using System.Collections.Generic;
4   namespace SolidPrinciples.DependencyInversion
39      public interface IMessageSender
41          void Send(string text);
46      public class EmailSender : IMessageSender
48          public void Send(string text)
50              Console.WriteLine("  [e-mail] " + text);
54      public class SmsSender : IMessageSender
56          public void Send(string text)
58              Console.WriteLine("  [SMS] " + text);
```

- **стр. 39–41** — **абстракция**: «что-то, что умеет отправлять сообщения».
- **стр. 46–58** — **модули нижнего уровня** (детали): две реализации.

```csharp
67      public class FakeSender : IMessageSender
69          public List<string> Messages { get; } = new List<string>();
71          public void Send(string text)
73              Messages.Add(text);
```

- **стр. 67–73** — «ненастоящий» отправитель для проверки: ничего не отправляет, только запоминает сообщения. Возможен именно потому, что сервис зависит от интерфейса.

```csharp
79      public class OrderService
82          private readonly IMessageSender sender;
85          public OrderService(IMessageSender sender)
87              this.sender = sender;
90          public void Confirm(string order)
92              sender.Send("Ваш заказ «" + order + "» принят");
```

- **стр. 79** — **модуль верхнего уровня** (логика магазина).
- **стр. 82** — поле типа **интерфейса**; слова `new` в классе нет вообще.
- **стр. 85–87** — отправитель приходит через конструктор — это **внедрение зависимости** (Dependency Injection).
- **стр. 92** — сервис не знает, уйдёт сообщение почтой, SMS или никуда.

```csharp
98          public static void Run()
102             new OrderService(new EmailSender()).Confirm("Клавиатура");
103             new OrderService(new SmsSender()).Confirm("Клавиатура");
106             FakeSender fake = new FakeSender();
107             new OrderService(fake).Confirm("Клавиатура");
108–109         Console.WriteLine("  [проверка] отправлено сообщений: " + fake.Messages.Count + ", текст: " + fake.Messages[0]);
```

- **стр. 102–103** — один и тот же `OrderService` с разными отправителями, без изменения его кода.
- **стр. 106–109** — проверка логики без настоящей отправки: по списку `Messages` видно, что сервис отправил одно сообщение и какое.

**Почему «инверсия»:** обычно важный класс зависит от «рабочих» (`OrderService → EmailSender`). Теперь оба зависят от абстракции (`OrderService → IMessageSender ← EmailSender`) — стрелка зависимости от детали развернулась.

---

## 7. `Program.cs`

```csharp
1   using System;
2   using System.Text;
4   namespace SolidPrinciples
25      internal class Program
27          private static void Main()
29              Console.OutputEncoding = Encoding.UTF8;
31              Console.WriteLine("===== S — принцип единственной ответственности =====");
32              SingleResponsibility.Demo.Run();
35–36           (заголовок) OpenClosed.Demo.Run();
39–40           (заголовок) Liskov.Demo.Run();
43–44           (заголовок) InterfaceSegregation.Demo.Run();
47–48           (заголовок) DependencyInversion.Demo.Run();
50              if (!Console.IsInputRedirected)
52–54               (пустая строка, приглашение, Console.ReadKey())
```

- **стр. 29** — UTF-8 для вывода.
- **стр. 32–48** — пять демонстраций. В каждом файле класс называется `Demo`, поэтому перед именем указывается пространство имён: `SingleResponsibility.Demo`, `OpenClosed.Demo` и т. д. (общую часть `SolidPrinciples.` писать не нужно — мы уже внутри неё). Строки 34, 38, 42, 46 — пустые `Console.WriteLine()` между разделами.
- **стр. 50–54** — ожидание клавиши в обычной консоли.

---

## 8. Важные моменты (вопросы на защите)

1. **Что значит «одна ответственность»?** — Одна причина для изменения. `Order` меняется, только если меняются данные заказа; вид чека и способ хранения — дело других классов.
2. **Как добавить новую скидку, не нарушая принцип O?** — Написать новый класс, реализующий `ICustomerDiscount`. `PriceCalculator` не меняется.
3. **Почему квадрат нельзя наследовать от прямоугольника?** — Прямоугольник обещает независимые ширину и высоту; квадрат это обещание нарушает, и код, написанный для прямоугольника, даёт неверный результат.
4. **Как понять, что наследование нарушает принцип Лисков?** — Наследник бросает исключения там, где предок работал, меняет смысл методов или требует проверки типа (`if (x is Square)`).
5. **Чем плох «толстый» интерфейс?** — Классы вынуждены реализовывать ненужные методы (часто — заглушками с исключениями), а клиенты зависят от того, чем не пользуются.
6. **Чем принцип I отличается от S?** — S — про классы (одна ответственность), I — про интерфейсы (не заставлять зависеть от лишнего).
7. **Что такое внедрение зависимости?** — Объект не создаёт свои зависимости сам, а получает их готовыми — обычно через конструктор.
8. **Dependency Inversion и Dependency Injection — одно и то же?** — Нет. Inversion — принцип (зависеть от абстракций), Injection — приём, которым он реализуется.
9. **Что даёт `FakeSender`?** — Возможность проверять логику без внешних систем (почты, сети). Это главная практическая выгода принципа D.
10. **Всегда ли нужно следовать SOLID?** — Нет: в маленькой программе лишние интерфейсы только усложняют код (противоречие с KISS). Принципы окупаются там, где код живёт долго и часто меняется.
