
namespace CommandSystem.Abstractions
{
    public class UndoToken
    {
        public static UndoToken Empty { get; set; }

        public virtual Task UndoAsync() => Task.CompletedTask;
        public virtual Task RedoAsync() => Task.CompletedTask;
    }
}
