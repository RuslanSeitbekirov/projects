using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MyTelegramBot;

class Program
{
    // Вставьте сюда токен, который дал BotFather
    private static readonly string Token = "8825595358:AAEaDfFAjo7yZNw2AGGQ-o-Ykp2VZNKrC1c";

    static async Task Main(string[] args)
    {
        // 1. Создаем клиента бота
        var botClient = new TelegramBotClient(Token);

        // 2. Проверяем связь с Telegram
        var me = await botClient.GetMe();
        Console.WriteLine($"Бот {me.FirstName} (@{me.Username}) запущен");
        Console.WriteLine("Нажмите Ctrl+C для остановки.\n");

        // 3. Настраиваем отмену по Ctrl+C
        using CancellationTokenSource cts = new();
        Console.CancelKeyPress += (sender, eventArgs) =>
        {
            cts.Cancel();
            eventArgs.Cancel = true;
        };

        // 4. Настройки получения обновлений (Long Polling)
        ReceiverOptions receiverOptions = new()
        {
            // Указываем, какие типы обновлений мы хотим получать. 
            // Пока ограничимся только текстовыми сообщениями.
            AllowedUpdates = new[] { UpdateType.Message } 
        };

        // 5. Запускаем бесконечный цикл получения сообщений
        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        // 6. Держим приложение запущенным, пока не нажмут Ctrl+C
        await Task.Delay(Timeout.Infinite, cts.Token);
    }

    // --- МЕТОДЫ ОБРАБОТКИ ---

    // Сюда приходят все обновления от Telegram
    static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        // Проверяем, что это именно текстовое сообщение
        if (update.Type == UpdateType.Message && update.Message is { } message)
        {
            long chatId = message.Chat.Id;
            string? text = message.Text;

            Console.WriteLine($"[Получено] От пользователя {message.From?.Username} (ID: {chatId}): {text}");

            // Простая логика: если написали /start, отвечаем приветствием
            if (text == "/start")
            {
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Привет, {message.From?.FirstName}! Я твой домашний бот на C#.",
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                // Во всех остальных случаях просто возвращаем текст обратно (Эхо)
                await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: $"Вы написали: {text}",
                    cancellationToken: cancellationToken
                );
            }
        }
    }

    // Сюда попадают ошибки (например, нет интернета)
    static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[ОШИБКА] {exception.Message}");
        
        // Если это ошибка сети, можно добавить задержку перед следующим запросом
        if (exception is Telegram.Bot.Exceptions.ApiRequestException apiEx)
        {
            Console.WriteLine($"Код ошибки Telegram: {apiEx.ErrorCode}");
        }

        return Task.CompletedTask;
    }
}