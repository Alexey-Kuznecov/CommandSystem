using CommandSystem.Abstractions;
using CommandSystem.Core.Execution;
using CommandSystem.Core.UndoRedo;
using CommandSystem.Infrastructure.Lifecycle;

namespace CommandSystem.Infrastructure
{
    public static class CommandSystemBootstrap
    {
        public static void RegisterSystemCommands(
            ICommandRegistry register,
            IHistoryManager history)
        {
            IAsyncCommand cmd = new AsyncDelegateCommand("history.undo", "Undo",
                 async ctx =>
                 {
                     {
                         if (history.CanUndo)
                         {
                             ctx.SuppressHistory = true;
                             await history.UndoAsync();
                         }
                     }
                 }
                );
            register.Register(new RegisteredCommand(new CommandMetadata("history.undo", "Undo"), cmd));
            register.Register(new RegisteredCommand(
                 new CommandMetadata("history.redo", "Redo"),
                 new AsyncDelegateCommand("history.redo", "Redo",
             async ctx =>
             {
                 await history.RedoAsync();
             })));
        }
    }
}
