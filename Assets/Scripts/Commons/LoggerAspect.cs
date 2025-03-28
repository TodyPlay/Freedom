using System;
using UnityEngine;

namespace Commons
{
    public static class LoggerAspect
    {
        public static void Log(object message) => Debug.Log(message);
        public static void LogWarning(object message) => Debug.LogWarning(message);
        public static void LogError(object message) => Debug.LogError(message);
        public static void LogException(Exception message) => Debug.LogException(message);
    }
}