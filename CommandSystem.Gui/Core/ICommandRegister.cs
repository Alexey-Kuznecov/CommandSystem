
using CommandSystem.Abstractions;

namespace CommandSystem.Gui.Core
{
    public interface ICommandRegister
    {
        // 1. Простая команда без параметров
        void Register(CommandMetadata metadata, Func<CommandContext, Task> executeAsync);

        // 2. Команда с параметром
        void Register<TParam>(CommandMetadata metadata, Func<TParam, CommandContext, Task> executeAsync);

        // 3. Команда с ViewModel и параметром
        void Register<TViewModel, TParam>(CommandMetadata metadata, Func<TViewModel, TParam, CommandContext, Task> executeAsync);
        //void Register<TViewModel>(CommandMetadata metadata, Func<TViewModel, CommandContext, Task> executeAsync);
    }
}
