using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandSystem.Console.Autocomplete
{
    public interface IAutoCompleteProvider
    {
        IEnumerable<string> GetSuggestions(string input);
    }
}
