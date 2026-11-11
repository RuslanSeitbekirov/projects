using System;
using System.Collections.Generic;

namespace ClassRelations
{
    // =========================================================================
    //  АГРЕГАЦИЯ — связь "целое — часть" (говорят "has-a", "имеет"),
    //  но часть может жить БЕЗ целого.
    //
    //  Как узнать в коде: части создаются СНАРУЖИ и передаются в "целое"
    //  готовыми (через конструктор или метод Add...). Целое только хранит
    //  ссылки на них. Если целое исчезнет — части останутся.
    //
    //  Пример: команда и игроки. Команду можно распустить, а игроки
    //  никуда не денутся и могут перейти в другую команду.
    // =========================================================================

    /// <summary>Игрок — "часть".</summary>
    public class Player
    {
        public string Name { get; }

        public Player(string name)
        {
            Name = name;
        }
    }

    /// <summary>Команда — "целое".</summary>
    public class Team
    {
        public string Title { get; }

        // Команда хранит список ссылок на игроков, но самих игроков не создаёт
        private readonly List<Player> players = new List<Player>();

        public Team(string title)
        {
            Title = title;
        }

        /// <summary>
        /// Игрок приходит ГОТОВЫМ снаружи — это и есть признак агрегации.
        /// (Сравните с композицией: там внутри метода стояло бы new.)
        /// </summary>
        public void AddPlayer(Player player)
        {
            players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            players.Remove(player);
        }

        public void PrintPlayers()
        {
            Console.Write("Команда «" + Title + "»: ");
            if (players.Count == 0)
            {
                Console.WriteLine("игроков нет");
                return;
            }

            List<string> names = new List<string>();
            foreach (Player p in players)
            {
                names.Add(p.Name);
            }
            Console.WriteLine(string.Join(", ", names));
        }
    }

    public static class AggregationDemo
    {
        public static void Run()
        {
            // Игроки создаются сами по себе, ещё до всякой команды
            Player anna = new Player("Анна");
            Player boris = new Player("Борис");
            Player vera = new Player("Вера");

            Team tigers = new Team("Тигры");
            tigers.AddPlayer(anna);
            tigers.AddPlayer(boris);
            tigers.PrintPlayers();

            // Один и тот же игрок может входить сразу в две команды —
            // при композиции такое невозможно.
            Team lions = new Team("Львы");
            lions.AddPlayer(boris);
            lions.AddPlayer(vera);
            lions.PrintPlayers();

            // "Распускаем" команду: убираем на неё ссылку. Объект команды
            // потом удалит сборщик мусора, а игроки останутся, потому что
            // на них есть ссылки в других местах.
            tigers = null;
            Console.WriteLine("Команда «Тигры» распущена, а игроки остались: " + anna.Name + ", " + boris.Name);
            lions.PrintPlayers();
        }
    }
}
