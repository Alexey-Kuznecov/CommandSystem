
namespace CommandSystem.Console.Core
{
    public interface IConsoleCommand
    {
        IEnumerable<string> Aliases => Enumerable.Empty<string>();
        IEnumerable<string> GetSuggestions(string[] args) => Enumerable.Empty<string>(); // <- поддержка автодополнения по аргументам
        string Name { get; }
        string Description { get; }
        Task ExecuteAsync(IConsoleCommandContext context, CancellationToken cancellationToken);
        Task FinalizeAsync();
    }
}
