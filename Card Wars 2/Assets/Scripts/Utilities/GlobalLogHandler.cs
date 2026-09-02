using UnityEngine;

namespace Utilities
{
    public class GlobalLogHandler : MonoBehaviour
    {
        private class BuildLogHandler : ILogHandler
        {
            private readonly ILogHandler _defaultHandler;

            public BuildLogHandler(ILogHandler defaultHandler)
            {
                _defaultHandler = defaultHandler;
            }

            public void LogFormat(LogType logType, Object context, string format, params object[] args)
            {
#if UNITY_EDITOR
                // Editor: show everything
                _defaultHandler.LogFormat(logType, context, format, args);
#endif

                // Build: show nothing!
            }

            public void LogException(System.Exception exception, Object context)
            {
#if UNITY_EDITOR
                _defaultHandler.LogException(exception, context);
#endif
            }
        }

        void Awake()
        {
            Debug.unityLogger.logHandler = new BuildLogHandler(Debug.unityLogger.logHandler);
            DontDestroyOnLoad(gameObject);
        }
    }
}