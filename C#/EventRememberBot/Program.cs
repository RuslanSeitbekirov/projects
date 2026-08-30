using System;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using DotNetEnv;
using MyTelegramBot;
using Microsoft.Extensions.WebEncoders.Testing;
using Superpower;
// 1. Загружаем переменные из .env
Env.Load();

// 2. Читаем токен
string token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN") 
               ?? throw new Exception("Токен бота не найден в файле .env!");

Console.WriteLine($"Бот с токеном {token[..5]}... инициализирован.");

// 3. Создаем клиента бота
var botClient = new TelegramBotClient(token);

// 4. Проверяем связь с Telegram
var me = await botClient.GetMe();
Console.WriteLine($"✅ Бот {me.FirstName} (@{me.Username}) успешно запущен!");
Console.WriteLine("Нажмите Ctrl+C для остановки.\n");

// 5. Настраиваем отмену по Ctrl+C
using CancellationTokenSource cts = new();
Console.CancelKeyPress += (sender, eventArgs) =>
{
    cts.Cancel();
    eventArgs.Cancel = true;
};

// 6. Настройки получения обновлений (Long Polling)
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = new[] { UpdateType.Message, UpdateType.CallbackQuery } // Добавил CallbackQuery на будущее для кнопок
};

// 7. Запускаем бесконечный цикл получения сообщений
// В будущем updateHandler можно заменить на метод вашего отдельного класса-обработчика
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    errorHandler: HandleErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

// 8. Держим приложение запущенным
await Task.Delay(Timeout.Infinite, cts.Token);


// ==========================================
// МЕТОДЫ ОБРАБОТКИ 
// (В будущем их можно вынести в отдельный класс, например, BotUpdateHandler)
// ==========================================

ParsText.testclassconnect();

static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update.Message is { } message)
    {
        long chatId = message.Chat.Id;
        string? text = message.Text;

        Console.WriteLine($"[Получено] От {message.From?.Username} (ID: {chatId}): {text}");

        // TODO: Здесь будет вызов вашего класса парсинга!
        // Пример:
        // var parser = new TextPars(text);
        // if (parser.IsValidReminder) { ... }

        if (text == "/start")
        {
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Привет, {message.From?.FirstName}! Я твой бот-напоминалка на C#.\nОтправь мне текст и время, и я напомню!",
                cancellationToken: cancellationToken
            );
        }
        else
        {
            // Временный ответ, пока вы не реализуете парсинг
            await botClient.SendMessage(
                chatId: chatId,
                text: $"Вы написали: {text}\n(Здесь скоро будет логика обработки напоминаний)",
                cancellationToken: cancellationToken
            );
        }
    }
}

static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[ОШИБКА СЕТИ/API] {exception.Message}");
    Console.ResetColor();
    
    
    return Task.CompletedTask;
}
