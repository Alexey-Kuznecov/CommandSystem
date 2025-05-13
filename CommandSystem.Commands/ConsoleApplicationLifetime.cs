
namespace CommandSystem.Commands
{
    public class ConsoleApplicationLifetime
    {
        private bool _isRunning = true;

        public bool IsRunning => _isRunning;

        // События
        public event Action? ApplicationStopping;
        public event Action? ApplicationStopped;

        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;

            // Сначала уведомляем, что приложение будет останавливаться
            ApplicationStopping?.Invoke();
        }

        // Можно вызвать отдельно в конце работы программы
        public void NotifyStopped()
        {
            ApplicationStopped?.Invoke();
        }
    }
}