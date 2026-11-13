using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using ToDoListBot.Core.Services;
using ToDoListBot.Infrastructure.DataAccess;
using ToDoListBot.TelegramBot;
using ToDoListBot.TelegramBot.Scenarios;

const int maxTaskCount = 100;
const int maxTaskLength = 100;

var token = Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
if (string.IsNullOrWhiteSpace(token))
{
    Console.WriteLine("Не задана переменная окружения TELEGRAM_BOT_TOKEN");
    return;
}

// Строка подключения к БД из ДЗ "Модель базы данных".
var connectionString = Environment.GetEnvironmentVariable("TODO_DB_CONNECTION")
                       ?? "Host=localhost;Port=5432;Database=ToDoList;Username=postgres;Password=postgres";

// DataAccess (linq2db + PostgreSQL)
var dataContextFactory = new DataContextFactory(connectionString);
var userRepository = new SqlUserRepository(dataContextFactory);
var toDoRepository = new SqlToDoRepository(dataContextFactory);
var toDoListRepository = new SqlToDoListRepository(dataContextFactory);

// Services
var userService = new UserService(userRepository);
var toDoService = new ToDoService(toDoRepository, maxTaskCount, maxTaskLength);
var toDoListService = new ToDoListService(toDoListRepository);
var reportService = new ToDoReportService(toDoRepository);

// Scenarios
IScenario[] scenarios =
[
    new AddTaskScenario(userService, toDoService, toDoListService),
    new AddListScenario(userService, toDoListService),
    new DeleteListScenario(userService, toDoListService, toDoService),
    new DeleteTaskScenario(toDoService)
];
var contextRepository = new InMemoryScenarioContextRepository();

var handler = new UpdateHandler(userService, toDoService, reportService, toDoListService, scenarios, contextRepository);

using var cts = new CancellationTokenSource();
var botClient = new TelegramBotClient(token, cancellationToken: cts.Token);

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = [UpdateType.Message, UpdateType.CallbackQuery],
    DropPendingUpdates = true
};

await botClient.SetMyCommands(UpdateHandler.Commands, cancellationToken: cts.Token);
botClient.StartReceiving(handler, receiverOptions, cts.Token);

var me = await botClient.GetMe(cts.Token);
Console.WriteLine($"{me.FirstName} запущен!");

while (!cts.IsCancellationRequested)
{
    Console.WriteLine("Нажмите клавишу A для выхода");
    var key = Console.ReadKey(intercept: true);

    if (key.Key == ConsoleKey.A)
    {
        cts.Cancel();
        Console.WriteLine("Бот остановлен");
    }
    else
    {
        var info = await botClient.GetMe(cts.Token);
        Console.WriteLine($"Бот: {info.FirstName} (@{info.Username}), Id: {info.Id}");
    }
}
