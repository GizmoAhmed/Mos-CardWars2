namespace Networking
{
    public static class DisconnectNotice
    {
        private static string _message;

        // keeps the first reason: the root cause beats the generic "disconnected" that follows it
        public static void Set(string message)
        {
            if (string.IsNullOrEmpty(_message))
                _message = message;
        }

        // read once, then clear so it doesn't show again on a normal menu visit
        public static string Take()
        {
            string message = _message;
            _message = null;
            return message;
        }

        public static void Clear() => _message = null;
    }
}