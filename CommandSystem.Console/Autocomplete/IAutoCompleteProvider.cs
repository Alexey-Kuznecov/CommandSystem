
namespace CommandSystem.Console.Autocomplete
{
    public interface IAutoCompleteProvider
    {
        IEnumerable<string> GetSuggestions(string input);
    }
}
