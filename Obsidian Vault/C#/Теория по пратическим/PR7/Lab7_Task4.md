# Практическая работа №7. Задание 4 — Девять паттернов проектирования

Папка проекта: `Lab7_Task4/DesignPatterns/` — консольное приложение. Тема всех примеров — интернет-магазин компьютерной техники.

> **Как читать этот файл.** Код разбит на фрагменты, под каждым — пояснение по номерам строк (`стр. N`).
> Фигурные скобки, пустые строки, комментарии и строки, которые только печатают заголовок, отдельно не расписываются.
> WPF в этой работе нет.
>
> Каждый паттерн лежит в своём файле и заканчивается статическим классом `...Demo` с методом `Run()`,
> который вызывается из `Program.cs`.

---

## 1. Немного теории

**Паттерн проектирования** — известное удачное решение типовой задачи проектирования: какие нужны классы и как они связаны. Это схема, а не готовый код. Подробнее — в реферате `Lab7_Task3/Реферат_Паттерны_проектирования.md`.

| Группа | На какой вопрос отвечает | Паттерны в проекте | Папка |
|---|---|---|---|
| Порождающие | как **создавать** объекты | Singleton, Factory Method, Builder | `Creational/` |
| Структурные | как **соединять** классы и объекты | Adapter, Decorator, Facade | `Structural/` |
| Поведенческие | как объекты **взаимодействуют** | Strategy, Observer, Command | `Behavioral/` |

Почти все паттерны опираются на одни и те же приёмы:

- **интерфейс вместо конкретного класса** — код зависит от «контракта», а реализацию можно подменить;
- **композиция** — объект хранит в поле другой объект и поручает ему работу (делегирует);
- **полиморфизм** — один и тот же вызов работает по-разному в зависимости от реального типа объекта.

---

## 2. Порождающие паттерны

### 2.1. `Creational/Singleton.cs` — Одиночка

**Задача:** у класса должен быть только один объект на всю программу (настройки магазина).

```csharp
1   using System;
3   namespace DesignPatterns.Creational
21      public sealed class ShopSettings
24          private static ShopSettings instance;
28          private static readonly object locker = new object();
30          public string ShopName { get; set; }
31          public string Currency { get; set; }
34          private ShopSettings()
36              ShopName = "КомпМаркет";
37              Currency = "руб.";
```

- **стр. 21** — `sealed` — от класса нельзя наследоваться (наследник мог бы создать второй объект).
- **стр. 24** — **статическое поле** хранит единственный объект; пока он не создан — `null`.
- **стр. 28** — «замок» на случай одновременного обращения из двух потоков.
- **стр. 30–31** — данные настроек.
- **стр. 34** — **закрытый конструктор**: написать `new ShopSettings()` снаружи невозможно. Это ключевая часть паттерна.
- **стр. 36–37** — значения по умолчанию (строка 38 печатает сообщение, чтобы было видно, сколько раз вызван конструктор).

```csharp
42          public static ShopSettings Instance
46                  lock (locker)
50                      if (instance == null)
52                          instance = new ShopSettings();
54                      return instance;
```

- **стр. 42** — статическое свойство — единственная «дверь» к объекту.
- **стр. 46** — только один поток за раз.
- **стр. 50–52** — **ленивая инициализация**: объект создаётся при первом обращении.
- **стр. 54** — всегда возвращается один и тот же объект.

```csharp
60      public static class SingletonDemo
62          public static void Run()
65              ShopSettings first = ShopSettings.Instance;
66              ShopSettings second = ShopSettings.Instance;
70              Console.WriteLine("Это один и тот же объект: " + ReferenceEquals(first, second));
73              first.ShopName = "ТехноМир";
74              Console.WriteLine("Название магазина через вторую переменную: " + second.ShopName);
```

- **стр. 65–66** — два обращения «из разных мест»; конструктор вызывается один раз.
- **стр. 70** — `ReferenceEquals` проверяет, что обе переменные указывают на **один** объект → `True`.
- **стр. 73–74** — изменение через одну переменную видно через другую.

### 2.2. `Creational/FactoryMethod.cs` — Фабричный метод

**Задача:** общий алгоритм оформления доставки один, а вид доставки выбирают наследники.

```csharp
24      public interface IDelivery
26          string Name { get; }
27          decimal Cost { get; }
28          int Days { get; }
31      public class CourierDelivery : IDelivery
33          public string Name { get { return "Курьер"; } }
34          public decimal Cost { get { return 500m; } }
35          public int Days { get { return 1; } }
```

- **стр. 24–28** — **продукт**: интерфейс доставки (название, цена, срок).
- **стр. 31–35** — конкретный продукт: свойства только для чтения с постоянными значениями.
- **стр. 38–42** `PostDelivery` и **стр. 45–49** `PickupDelivery` устроены так же: «Почта», 250 руб., 7 дней и «Самовывоз», 0 руб., 2 дня.

```csharp
53      public abstract class DeliveryService
58          protected abstract IDelivery CreateDelivery();
64          public void SendOrder(string product)
66              IDelivery delivery = CreateDelivery();
67–68           Console.WriteLine("«" + product + "» → " + delivery.Name + ": " + delivery.Cost + " руб., дней: " + delivery.Days);
```

- **стр. 53** — **создатель**: абстрактный класс.
- **стр. 58** — **фабричный метод**: абстрактный, тела нет. Какой объект создать — решает наследник.
- **стр. 64–68** — общая логика. В ней нет ни одного `new CourierDelivery()`: метод работает с любой доставкой через интерфейс.

```csharp
74      public class CourierService : DeliveryService
76          protected override IDelivery CreateDelivery() { return new CourierDelivery(); }
```

- **стр. 74–76** — конкретный создатель переопределяет **только** фабричный метод.
- **стр. 79–81** `PostService` и **стр. 84–86** `PickupService` — аналогично.

```csharp
91          public static void Run()
94              DeliveryService[] services = { new CourierService(), new PostService(), new PickupService() };
96              foreach (DeliveryService service in services)
99                  service.SendOrder("Ноутбук");
```

- **стр. 94–99** — вызов одинаковый, а доставка создаётся разная. Новый вид доставки — два новых класса, старый код не меняется.

### 2.3. `Creational/Builder.cs` — Строитель

**Задача:** собрать сложный объект (компьютер) по шагам вместо конструктора с десятком параметров.

```csharp
1   using System;
2   using System.Collections.Generic;
24      public class Computer
26          public string Cpu { get; set; }
27          public int RamGb { get; set; }
28          public int SsdGb { get; set; }
29          public string Gpu { get; set; }
32          public List<string> Extras { get; } = new List<string>();
34          public override string ToString()
36              string text = "процессор " + Cpu + ", память " + RamGb + " ГБ, SSD " + SsdGb + " ГБ";
37              text += ", видеокарта: " + (Gpu == null ? "встроенная" : Gpu);
38              if (Extras.Count > 0)
40                  text += ", дополнительно: " + string.Join(", ", Extras);
42              return text;
```

- **стр. 24–29** — **продукт**: части компьютера.
- **стр. 32** — список дополнений; инициализируется сразу пустым списком.
- **стр. 34–42** — `ToString` собирает описание; тернарный оператор подставляет «встроенная», если видеокарта не задана.

```csharp
47      public class ComputerBuilder
50          private Computer computer = new Computer();
55          public ComputerBuilder SetCpu(string cpu)
57              computer.Cpu = cpu;
58              return this;
```

- **стр. 47** — **строитель**.
- **стр. 50** — компьютер, который сейчас собирается.
- **стр. 55–58** — один шаг сборки. `return this` — метод возвращает самого строителя, поэтому вызовы пишутся **цепочкой** через точку («текучий интерфейс»).
- `SetRam` (**стр. 61–64**), `SetSsd` (**стр. 67–70**), `SetGpu` (**стр. 73–76**), `AddExtra` (**стр. 79–82**) устроены так же; `AddExtra` добавляет строку в список.

```csharp
86          public Computer Build()
88              Computer result = computer;
92              computer = new Computer();
93              return result;
```

- **стр. 88** — запомнить готовый компьютер.
- **стр. 92** — начать новый пустой — строителем можно пользоваться повторно.
- **стр. 93** — отдать результат.

```csharp
98      public class ComputerDirector
100         public Computer BuildOfficePc(ComputerBuilder builder)
102             return builder.SetCpu("Core i3").SetRam(8).SetSsd(256).Build();
105         public Computer BuildGamingPc(ComputerBuilder builder)
107–108         return builder.SetCpu("Ryzen 7").SetRam(32).SetSsd(1000).SetGpu("RTX 4070").AddExtra("водяное охлаждение").Build();
```

- **стр. 98** — **распорядитель**: знает готовые «рецепты» — последовательности шагов.

```csharp
114         public static void Run()
116             ComputerBuilder builder = new ComputerBuilder();
119–125         Computer custom = builder.SetCpu("Ryzen 5").SetRam(16).SetSsd(512).AddExtra("Wi-Fi").AddExtra("подсветка").Build();
126             Console.WriteLine("Свой:    " + custom);
129             ComputerDirector director = new ComputerDirector();
130             Console.WriteLine("Офисный: " + director.BuildOfficePc(builder));
131             Console.WriteLine("Игровой: " + director.BuildGamingPc(builder));
```

- **стр. 119–125** — сборка вручную: по коду сразу понятно, что есть что.
- **стр. 129–131** — сборка по готовым рецептам распорядителя.

---

## 3. Структурные паттерны

### 3.1. `Structural/Adapter.cs` — Адаптер

**Задача:** магазин платит через `IPaymentSystem`, а у «чужой» банковской библиотеки другой набор методов; менять её нельзя.

```csharp
24      public interface IPaymentSystem
27          bool Pay(decimal rubles);
33      public class OldBankApi
36          public int SendMoney(string currencyCode, long kopecks)
38              Console.WriteLine("  [банк] перевод " + kopecks + " коп., валюта " + currencyCode);
39              if (kopecks <= 0)
41                  return 13;
43              return 0;
```

- **стр. 24–27** — **целевой интерфейс**: сумма в рублях, результат `true`/`false`.
- **стр. 33–43** — **адаптируемый класс**: принимает копейки и код валюты, возвращает число — 0 или код ошибки.

```csharp
48      public class BankAdapter : IPaymentSystem
51          private readonly OldBankApi bank;
53          public BankAdapter(OldBankApi bank)
55              this.bank = bank;
58          public bool Pay(decimal rubles)
61              long kopecks = (long)(rubles * 100);
62              int errorCode = bank.SendMoney("RUB", kopecks);
63              return errorCode == 0;
```

- **стр. 48** — **адаптер** реализует интерфейс магазина.
- **стр. 51–55** — внутри хранит объект банка (композиция); `this.bank` — поле, `bank` — параметр.
- **стр. 61** — перевод «нашего языка» на «язык банка»: рубли → копейки, явное приведение `decimal → long`.
- **стр. 62** — вызов чужого метода.
- **стр. 63** — код ошибки → `true`/`false`.

```csharp
72              IPaymentSystem payment = new BankAdapter(new OldBankApi());
74              Console.WriteLine("Оплата 1999,90 руб.: " + (payment.Pay(1999.90m) ? "успешно" : "ошибка"));
75              Console.WriteLine("Оплата 0 руб.: " + (payment.Pay(0m) ? "успешно" : "ошибка"));
```

- **стр. 72** — магазин видит только `IPaymentSystem`.
- **стр. 74–75** — успешная оплата и ошибка (банк вернул код 13).

### 3.2. `Structural/Decorator.cs` — Декоратор

**Задача:** добавлять товару услуги (упаковка, гарантия) в любом сочетании, не создавая класс на каждую комбинацию.

```csharp
28      public interface IProduct
30          string Name { get; }
31          decimal Price { get; }
35      public class Laptop : IProduct
37          public string Name { get { return "Ноутбук"; } }
38          public decimal Price { get { return 60000m; } }
```

- **стр. 28–31** — общий интерфейс товара.
- **стр. 35–38** — обычный товар.

```csharp
42      public abstract class ProductDecorator : IProduct
45          protected readonly IProduct product;
47          protected ProductDecorator(IProduct product)
49              this.product = product;
53          public virtual string Name { get { return product.Name; } }
54          public virtual decimal Price { get { return product.Price; } }
```

- **стр. 42** — **главная хитрость:** декоратор сам реализует `IProduct`, поэтому завёрнутый товар можно использовать везде, где обычный, — и заворачивать ещё раз.
- **стр. 45** — «завёрнутый» товар; им может быть и другой декоратор. `protected` — доступен наследникам.
- **стр. 53–54** — по умолчанию декоратор ничего не меняет; `virtual` — наследники переопределят.

```csharp
58      public class GiftWrap : ProductDecorator
60          public GiftWrap(IProduct product) : base(product) { }
63          public override string Name { get { return product.Name + " + подарочная упаковка"; } }
64          public override decimal Price { get { return product.Price + 300m; } }
68      public class ExtendedWarranty : ProductDecorator
70          public ExtendedWarranty(IProduct product) : base(product) { }
72          public override string Name { get { return product.Name + " + гарантия 3 года"; } }
73          public override decimal Price { get { return product.Price * 1.10m; } }
```

- **стр. 60, 70** — конструктор передаёт товар базовому классу.
- **стр. 63–64** — берём то, что вернул «внутренний» товар, и добавляем своё: текст и 300 руб.
- **стр. 72–73** — гарантия: плюс 10% к цене.

```csharp
78          private static void Print(IProduct product)
80              Console.WriteLine(product.Name + " = " + product.Price + " руб.");
85              IProduct item = new Laptop();
86              Print(item);
89              item = new ExtendedWarranty(item);
90              Print(item);
93              item = new GiftWrap(item);
94              Print(item);
97              Print(new GiftWrap(new GiftWrap(new Laptop())));
```

- **стр. 85** — обычный ноутбук: 60 000.
- **стр. 89** — завернули в гарантию; переменная **того же типа** `IProduct`. Цена 66 000.
- **стр. 93** — завернули ещё раз, в упаковку: 66 300.
- **стр. 97** — декораторы вкладываются как матрёшки, в любом порядке и количестве.

### 3.3. `Structural/Facade.cs` — Фасад

**Задача:** спрятать за одним методом несколько шагов оформления заказа.

```csharp
23      public class Warehouse
25          public bool IsInStock(string product)   // стр. 27–28: печать и return true
31          public void Reserve(string product)     // стр. 33: печать
37      public class PaymentService
39          public bool Charge(decimal amount)      // стр. 41–42: печать и return true
46      public class DeliveryDepartment
48          public void Schedule(string product, string address)   // стр. 50: печать
```

- **стр. 23–50** — **подсистема**: три независимых класса — склад, оплата, доставка. В учебном примере методы только печатают сообщение; склад и оплата всегда возвращают `true`.

```csharp
57      public class OrderFacade
60          private readonly Warehouse warehouse = new Warehouse();
61          private readonly PaymentService payment = new PaymentService();
62          private readonly DeliveryDepartment delivery = new DeliveryDepartment();
65          public bool PlaceOrder(string product, decimal price, string address)
67              if (!warehouse.IsInStock(product))
69                  return false;
71              if (!payment.Charge(price))
73                  return false;
75              warehouse.Reserve(product);
76              delivery.Schedule(product, address);
77              return true;
```

- **стр. 60–62** — **фасад** сам создаёт и хранит части подсистемы.
- **стр. 65** — один метод вместо четырёх вызовов.
- **стр. 67–73** — нет товара или не прошла оплата — заказ не оформлен.
- **стр. 75–77** — резерв, доставка, успех. Фасад знает **правильный порядок** шагов.

```csharp
85              OrderFacade shop = new OrderFacade();
90              bool ok = shop.PlaceOrder("Монитор", 15000m, "Симферополь, ул. Ленина, 1");
91              Console.WriteLine("Заказ оформлен: " + ok);
```

- **стр. 85–91** — вызывающему коду не нужно знать про склад, оплату и доставку.

---

## 4. Поведенческие паттерны

### 4.1. `Behavioral/Strategy.cs` — Стратегия

**Задача:** скидка считается по-разному, и способ расчёта нужно легко менять — без длинного `if / else if`.

```csharp
23      public interface IDiscountStrategy
25          string Title { get; }
28          decimal Apply(decimal price);
31      public class NoDiscount : IDiscountStrategy
33          public string Title { get { return "без скидки"; } }
35          public decimal Apply(decimal price)
37              return price;
```

- **стр. 23–28** — **интерфейс стратегии**: название и метод «цена → цена со скидкой».
- **стр. 31–37** — стратегия «без скидки».

```csharp
41      public class PercentDiscount : IDiscountStrategy
43          private readonly int percent;
45          public PercentDiscount(int percent)
47              this.percent = percent;
50          public string Title { get { return "скидка " + percent + "%"; } }
52          public decimal Apply(decimal price)
54              return price - price * percent / 100;
58      public class PromoCodeDiscount : IDiscountStrategy
60          public string Title { get { return "промокод на 500 руб."; } }
62          public decimal Apply(decimal price)
65              return Math.Max(0, price - 500m);
```

- **стр. 43–47** — у стратегии могут быть свои настройки (процент).
- **стр. 54** — цена минус процент.
- **стр. 65** — `Math.Max(0, ...)` — цена не может стать отрицательной.

```csharp
70      public class PriceCalculator
73          public IDiscountStrategy Discount { get; set; }
75          public PriceCalculator()
77              Discount = new NoDiscount();
80          public void PrintTotal(decimal price)
84              Console.WriteLine(price + " руб., " + Discount.Title + " → " + Discount.Apply(price) + " руб.");
```

- **стр. 70** — **контекст**.
- **стр. 73** — текущая стратегия; `set` — можно менять в любой момент.
- **стр. 77** — стратегия по умолчанию.
- **стр. 84** — ни одного `if`: какая бы стратегия ни стояла, вызов один и тот же.

```csharp
92              PriceCalculator calculator = new PriceCalculator();
93              calculator.PrintTotal(3000m);
95              calculator.Discount = new PercentDiscount(15);
96              calculator.PrintTotal(3000m);
98              calculator.Discount = new PromoCodeDiscount();
99              calculator.PrintTotal(3000m);
```

- **стр. 93–99** — та же цена и тот же калькулятор, три разных результата: 3000, 2550, 2500.

### 4.2. `Behavioral/Observer.cs` — Наблюдатель

**Задача:** покупатели подписываются на товар и автоматически узнают о снижении цены.

```csharp
27      public interface ISubscriber
29          void Update(string productName, decimal newPrice);
33      public class WatchedProduct
35          public string Name { get; }
37          private decimal price;
41          private readonly List<ISubscriber> subscribers = new List<ISubscriber>();
43          public WatchedProduct(string name, decimal price)
45              Name = name;
46              this.price = price;
49          public void Subscribe(ISubscriber subscriber)
51              subscribers.Add(subscriber);
54          public void Unsubscribe(ISubscriber subscriber)
56              subscribers.Remove(subscriber);
```

- **стр. 27–29** — **наблюдатель**: тот, кого надо оповещать.
- **стр. 33** — **издатель**: товар.
- **стр. 41** — список подписчиков. Товар знает о них только то, что у них есть `Update`.
- **стр. 49–56** — подписка и отписка.

```csharp
59          public decimal Price
61              get { return price; }
64                  bool cheaper = value < price;
65                  price = value;
68                  if (cheaper)
70                      Notify();
76          private void Notify()
78              foreach (ISubscriber subscriber in subscribers)
80                  subscriber.Update(Name, price);
```

- **стр. 64** — в блоке `set`: `value` — новая цена; сравниваем со старой **до** присваивания.
- **стр. 68–70** — оповещаем, только если цена снизилась.
- **стр. 76–80** — обход всех подписчиков.

```csharp
86      public class Customer : ISubscriber
88          private readonly string name;
96          public void Update(string productName, decimal newPrice)
98              Console.WriteLine("  " + name + ", «" + productName + "» подешевел: теперь " + newPrice + " руб.");
```

- **стр. 96** — этот метод вызывает **не покупатель, а товар**.

```csharp
106             WatchedProduct phone = new WatchedProduct("Смартфон", 30000m);
107             Customer anna = new Customer("Анна");
108             Customer oleg = new Customer("Олег");
110             phone.Subscribe(anna);
111             phone.Subscribe(oleg);
114             phone.Price = 27000m;
117             phone.Price = 29000m;
120             phone.Unsubscribe(oleg);
123             phone.Price = 25000m;
```

- **стр. 114** — цена упала: сообщение получают оба.
- **стр. 117** — цена выросла: никто не оповещается.
- **стр. 120–123** — Олег отписался; следующее снижение видит только Анна.

События C# (`event`) из практики 6 — это встроенный в язык Наблюдатель; здесь он сделан вручную, чтобы было видно устройство.

### 4.3. `Behavioral/Command.cs` — Команда

**Задача:** превратить действие в объект, чтобы его можно было запомнить и отменить.

```csharp
23      public interface ICommand
25          void Execute();
26          void Undo();
30      public class Cart
32          private readonly List<string> items = new List<string>();
34          public void Add(string item)      // стр. 36: items.Add(item);
39          public void Remove(string item)   // стр. 41: items.Remove(item);
44          public void Print()
46              Console.WriteLine("  корзина: " + (items.Count == 0 ? "пусто" : string.Join(", ", items)));
```

- **стр. 23–26** — **интерфейс команды**: выполнить и отменить.
- **стр. 30–46** — **получатель**: корзина. Про команды ничего не знает.

```csharp
51      public class AddItemCommand : ICommand
55          private readonly Cart cart;
56          private readonly string item;
58          public AddItemCommand(Cart cart, string item)
60              this.cart = cart;
61              this.item = item;
64          public void Execute()
66              cart.Add(item);
70          public void Undo()
72              cart.Remove(item);
```

- **стр. 55–61** — команда **помнит всё нужное** для выполнения и отмены: с какой корзиной работать и какой товар.
- **стр. 64–66** — выполнение.
- **стр. 70–72** — отмена — обратное действие.
- `RemoveItemCommand` (**стр. 77–96**) зеркальна: `Execute` убирает (**стр. 90**), `Undo` возвращает (**стр. 95**).

```csharp
100     public class CartManager
104         private readonly Stack<ICommand> history = new Stack<ICommand>();
106         public void Run(ICommand command)
108             command.Execute();
109             history.Push(command);
112         public void UndoLast()
114             if (history.Count == 0)
117                 return;
120             ICommand last = history.Pop();
121             last.Undo();
```

- **стр. 100** — **вызыватель**.
- **стр. 104** — **стек** («последним пришёл — первым ушёл») — отменяется всегда самое последнее действие.
- **стр. 108–109** — выполнить команду и положить в историю.
- **стр. 114–117** — отменять нечего.
- **стр. 120–121** — достать последнюю команду и вызвать её `Undo`.

```csharp
129             Cart cart = new Cart();
130             CartManager manager = new CartManager();
133             manager.Run(new AddItemCommand(cart, "мышь"));
134             manager.Run(new AddItemCommand(cart, "клавиатура"));
138             manager.Run(new RemoveItemCommand(cart, "мышь"));
142             manager.UndoLast();
146             manager.UndoLast();
```

- **стр. 133–134** — в корзине мышь и клавиатура.
- **стр. 138** — мышь убрана.
- **стр. 142** — отмена последнего действия: мышь вернулась.
- **стр. 146** — ещё отмена: отменено добавление клавиатуры.
- После каждого шага вызывается `cart.Print()` (**стр. 135, 139, 143, 147**).

---

## 5. `Program.cs`

```csharp
1   using System;
2   using System.Text;
3   using DesignPatterns.Behavioral;
4   using DesignPatterns.Creational;
5   using DesignPatterns.Structural;
7   namespace DesignPatterns
26      internal class Program
28          private static void Main()
30              Console.OutputEncoding = Encoding.UTF8;
34              SingletonDemo.Run();
36              FactoryMethodDemo.Run();
38              BuilderDemo.Run();
43              AdapterDemo.Run();
45              DecoratorDemo.Run();
47              FacadeDemo.Run();
52              StrategyDemo.Run();
54              ObserverDemo.Run();
56              CommandDemo.Run();
```

- **стр. 3–5** — подключение трёх пространств имён; они соответствуют папкам проекта.
- **стр. 30** — UTF-8 для вывода.
- **стр. 34–56** — девять демонстраций по очереди. Перед каждой вызывается `Title(...)` с названием паттерна, перед каждой группой печатается её заголовок.
- **стр. 58–63** — ожидание клавиши, если программа запущена в обычной консоли.
- **стр. 66–70** — метод `Title`: пустая строка и заголовок.

---

## 6. Важные моменты (вопросы на защите)

1. **Как Singleton не даёт создать второй объект?** — Закрытый конструктор и статическое поле с единственным экземпляром.
2. **Зачем `lock` в Singleton?** — Без него два потока могли бы одновременно увидеть `null` и создать два объекта.
3. **Главный недостаток Singleton?** — По сути глобальная переменная: скрытые зависимости, трудно тестировать.
4. **В чём суть Фабричного метода?** — Базовый класс описывает алгоритм, а создание объекта отдаёт наследникам через абстрактный метод.
5. **Зачем `return this` в Строителе?** — Чтобы вызовы шагов писались цепочкой.
6. **Адаптер и Фасад — в чём разница?** — Адаптер меняет интерфейс **одного** класса под ожидаемый; Фасад даёт новый простой интерфейс к **группе** классов.
7. **Почему Декоратор реализует тот же интерфейс, что и товар?** — Чтобы обёрнутый объект можно было использовать как обычный и оборачивать повторно.
8. **Декоратор или наследование?** — Наследование потребовало бы класс на каждую комбинацию услуг; декораторы комбинируются во время работы.
9. **Стратегия: где исчез `if`?** — Выбор алгоритма заменён выбором объекта; контекст вызывает его через интерфейс.
10. **Чем Наблюдатель связан с событиями C#?** — `event` — готовая реализация этого паттерна: `+=` — `Subscribe`, `-=` — `Unsubscribe`, вызов события — `Notify`.
11. **Почему в Команде история — стек?** — Отменять нужно в порядке, обратном выполнению.
12. **Какие принципы стоят за всеми паттернами?** — Программировать на уровне интерфейсов и предпочитать композицию наследованию.
