using UnityEngine;
using System.Collections;
using MqttLib.Logger;

namespace MqttLib.Logger
{
    public class UnityLogger : ILog
    {
        private LogLevel _loggingLevel = LogLevel.DEV;
        private string _name;

        public UnityLogger()
        {

        }

        #region ILog Members

        public void Write(string message)
        {
            Debug.Log
            (
                "[" + LogLevel.DEBUG.ToString() + "]" +
                message
            );
        }

        public void Write(LogLevel level, string message)
        {
            if ((uint)level >= (uint)_loggingLevel)
            {
                Debug.Log
                (
                    "[" + level.ToString() + "]" +
                    message
                );
            }
        }

        public LogLevel LoggingLevel
        {
            get
            {
                return _loggingLevel;
            }
            set
            {
                _loggingLevel = value;
            }
        }

        #endregion
    }
}
