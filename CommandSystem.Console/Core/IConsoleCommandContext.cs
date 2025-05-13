
namespace CommandSystem.Console.Core
{
    public interface IConsoleCommandContext
    {
        string Input { get; }                 // Введённая строка целиком
        string[] Arguments { get; }            // Аргументы команды
        IConsoleOutput Output { get; }         // Вывод в консоль
    }
}
