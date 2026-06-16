using CommandSystem.Abstractions;

namespace CommandSystem.Core.Execution
{
    //public class UndoableAsyncDelegateCommand : IAsyncCommand
    //{
    //    private readonly Func<CommandContext, Task<UndoToken>> _execute;

    //    public UndoableAsyncDelegateCommand(
    //        string name,
    //        string description,
    //        Func<CommandContext, Task<UndoToken>> execute)
    //    {
    //        _execute = execute;
    //    }

    //    public async Task ExecuteAsync(CommandContext ctx)
    //    {
    //        var token = await _execute(ctx);

    //        if (token != null)
    //        {
    //            ctx.History?.Push(token); // или как у тебя это делается
    //        }
    //    }
    //}
}
