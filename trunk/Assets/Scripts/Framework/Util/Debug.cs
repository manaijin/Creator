using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Framework.Util
{
    public static class Debug
    {
        public static void Log<T>(IEnumerable<T> array)
        {
            Log(ArrayUtil.ToString(array));
        }

        public static void LogError<T>(IEnumerable<T> array)
        {
            LogError(ArrayUtil.ToString(array));
        }

        public static void Log(object obj)
        {
            Log(obj.ToString());
        }

        public static void Log(StringBuilder format)
        {
            Log(format.ToString());
        }

        public static void Log(string format, object param1)
        {
            UnityEngine.Debug.Log(string.Format(format, param1));
        }

        public static void Log(string format, object param1, object param2)
        {
            UnityEngine.Debug.Log(string.Format(format, param1, param2));
        }

        public static void Log(string format, object param1, object param2, object param3)
        {
            UnityEngine.Debug.Log(string.Format(format, param1, param2, param3));
        }

        public static void Log(String format, params object[] args)
        {
            UnityEngine.Debug.Log(string.Format(format, args));
        }

        public static void LogError(StringBuilder format)
        {
            UnityEngine.Debug.LogError(string.Format(format.ToString()));
        }

        public static void LogError(string format, object param1)
        {
            UnityEngine.Debug.LogError(string.Format(format, param1));
        }

        public static void LogError(string format, object param1, object param2)
        {
            UnityEngine.Debug.LogError(string.Format(format, param1, param2));
        }

        public static void LogError(string format, object param1, object param2, object param3)
        {
            UnityEngine.Debug.LogError(string.Format(format, param1, param2, param3));
        }

        public static void LogError(String format, params object[] args)
        {
            UnityEngine.Debug.LogError(string.Format(format, args));
        }
    }
}
