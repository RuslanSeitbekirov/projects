using System;

namespace MyTelegramBot
{
    // ⚠️ Переименовал Type -> ReminderType, т.к. System.Type уже существует в .NET
    enum Command { create, edit, delete, all }
    enum ReminderType { repeate, once, DaysOfTheWeek, AtASpecificTime }

    // ✅ Добавлено public — теперь класс виден из Program.cs
    public class ParsText
    {
        private string message = "";
        private int data;
        private int timer;

        // ✅ Добавлено public static — теперь метод виден и его можно вызвать без new
        public static void testclassconnect()
        {
            Console.WriteLine("✅ Связь между файлами работает!");
        }
    }
}
    

