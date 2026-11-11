# Книжный магазин — UML-диаграмма классов

```mermaid
classDiagram
    direction TB

    class IProduct {
        <<interface>>
        +Title : string
        +Price : decimal
    }
    class IBook {
        <<interface>>
        +Author : string
    }
    class IDownloadable {
        <<interface>>
        +Download() string
    }
    class ICatalog {
        <<interface>>
        +Add(IBook)
        +GetAll() IEnumerable~IBook~
        +FindByAuthor(string) IEnumerable~IBook~
    }
    class ICart {
        <<interface>>
        +Items : IReadOnlyList~IProduct~
        +Add(IProduct)
        +Total() decimal
    }
    class IPaymentMethod {
        <<interface>>
        +Name : string
        +Pay(decimal) bool
    }

    class Product {
        <<abstract>>
        +Title : string
        +Price : decimal
        +Describe()* string
    }
    class Book {
        +Author : string
        +Describe() string
    }
    class EBook {
        +Describe() string
        IDownloadable.Download() string
    }
    class Catalog
    class Cart
    class CardPayment
    class CashPayment
    class Store {
        -catalog : ICatalog
        -payment : IPaymentMethod
        +Checkout(ICart)
    }

    IProduct <|-- IBook
    IProduct <|.. Product
    Product <|-- Book
    IBook <|.. Book
    Book <|-- EBook
    IDownloadable <|.. EBook
    ICatalog <|.. Catalog
    ICart <|.. Cart
    IPaymentMethod <|.. CardPayment
    IPaymentMethod <|.. CashPayment
    Store o-- ICatalog
    Store o-- IPaymentMethod
    Store ..> ICart : использует
    Catalog o-- IBook
    Cart o-- IProduct
```

## Обозначения

- `<|--` — наследование (класс от класса, интерфейс от интерфейса)
- `<|..` — реализация интерфейса
- `o--` — агрегация (объект хранит ссылку на другой)
- `..>` — зависимость (использует в методе)
