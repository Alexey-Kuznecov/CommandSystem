
namespace CommandSystem.Console.Autocomplete
{
    public static class AutoCompleteHelper
    {
        public static IEnumerable<string> Suggest(IEnumerable<string> options, string[] args)
        {
            if (args.Length == 0)
                return options;

            var lastArg = args.Last();
            return options.Where(opt => opt.StartsWith(lastArg, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<string> Suggest(IReadOnlyList<string[]> argumentLevels, string[] args)
        {
            int level = args.Length - 1;
            if (level < 0 || level >= argumentLevels.Count)
                return Enumerable.Empty<string>();

            string current = args.Last();
            return argumentLevels[level].Where(opt => opt.StartsWith(current, StringComparison.OrdinalIgnoreCase));
        }
    }
}
