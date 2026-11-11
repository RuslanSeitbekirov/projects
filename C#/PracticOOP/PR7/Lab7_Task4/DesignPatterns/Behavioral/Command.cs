using System;
using System.Collections.Generic;

namespace DesignPatterns.Behavioral
{
    // =========================================================================
    //  КОМАНДА (Command) — поведенческий паттерн.
    //
    //  Задача: превратить ДЕЙСТВИЕ в ОБЪЕКТ. Тогда действия можно складывать
    //  в список, выполнять позже, а главное — отменять.
    //
    //  В магазине: "добавить в корзину" и "убрать из корзины". Каждое действие —
    //  объект-команда, который умеет и выполнить себя, и отменить. История
    //  команд хранится в стеке, поэтому получается кнопка "Отменить" (Ctrl+Z).
    //
    //  Участники:
    //    ICommand        — интерфейс команды (Execute и Undo);
    //    AddItemCommand, RemoveItemCommand — конкретные команды;
    //    Cart            — "получатель": тот, кто на самом деле делает работу;
    //    CartManager     — "вызыватель": запускает команды и помнит историю.
    // =========================================================================

    public interface ICommand
    {
        void Execute();   // выполнить
        void Undo();      // отменить
    }

    /// <summary>Получатель: корзина. Про команды она ничего не знает.</summary>
    public class Cart
    {
        private readonly List<string> items = new List<string>();

        public void Add(string item)
        {
            items.Add(item);
        }

        public void Remove(string item)
        {
            items.Remove(item);
        }

        public void Print()
        {
            Console.WriteLine("  корзина: " + (items.Count == 0 ? "пусто" : string.Join(", ", items)));
        }
    }

    /// <summary>Команда "добавить товар".</summary>
    public class AddItemCommand : ICommand
    {
        // Команда помнит всё, что нужно для выполнения И для отмены:
        // с какой корзиной работать и какой товар.
        private readonly Cart cart;
        private readonly string item;

        public AddItemCommand(Cart cart, string item)
        {
            this.cart = cart;
            this.item = item;
        }

        public void Execute()
        {
            cart.Add(item);
        }

        // Отмена — действие, обратное выполнению
        public void Undo()
        {
            cart.Remove(item);
        }
    }

    /// <summary>Команда "убрать товар".</summary>
    public class RemoveItemCommand : ICommand
    {
        private readonly Cart cart;
        private readonly string item;

        public RemoveItemCommand(Cart cart, string item)
        {
            this.cart = cart;
            this.item = item;
        }

        public void Execute()
        {
            cart.Remove(item);
        }

        public void Undo()
        {
            cart.Add(item);
        }
    }

    /// <summary>Вызыватель: выполняет команды и хранит историю.</summary>
    public class CartManager
    {
        // Стек: "последним пришёл — первым ушёл". Как раз то, что нужно
        // для отмены: отменяется всегда самое последнее действие.
        private readonly Stack<ICommand> history = new Stack<ICommand>();

        public void Run(ICommand command)
        {
            command.Execute();
            history.Push(command);   // запоминаем, чтобы потом можно было отменить
        }

        public void UndoLast()
        {
            if (history.Count == 0)
            {
                Console.WriteLine("  отменять нечего");
                return;
            }

            ICommand last = history.Pop();   // достаём последнюю команду
            last.Undo();
        }
    }

    public static class CommandDemo
    {
        public static void Run()
        {
            Cart cart = new Cart();
            CartManager manager = new CartManager();

            Console.WriteLine("Добавили мышь и клавиатуру:");
            manager.Run(new AddItemCommand(cart, "мышь"));
            manager.Run(new AddItemCommand(cart, "клавиатура"));
            cart.Print();

            Console.WriteLine("Убрали мышь:");
            manager.Run(new RemoveItemCommand(cart, "мышь"));
            cart.Print();

            Console.WriteLine("Отмена (мышь вернулась):");
            manager.UndoLast();
            cart.Print();

            Console.WriteLine("Ещё отмена (клавиатура убрана):");
            manager.UndoLast();
            cart.Print();
        }
    }
}
