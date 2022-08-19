﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Bindito.Core;
using TimberApi.Core.ConsoleSystem;
using UnityEngine;

namespace TimberApi.Internal.LoggingSystem
{
    public class ConsoleLogger : MonoBehaviour, IConsoleWriter
    {
        private ImmutableArray<ILogListener> _listeners;

        [Inject]
        public void InjectDependencies(IEnumerable<ILogListener> listeners)
        {
            _listeners = listeners.ToImmutableArray();
        }

        private void Awake()
        {
            Application.logMessageReceivedThreaded += OnLogMessageReceived;
        }

        private void OnLogMessageReceived(string message, string stacktrace, LogType type)
        {
            SendLogToListeners(message, stacktrace, type);
        }

        public void Log(string message)
        {
            SendLogToListeners(message, "", LogType.Log);
        }

        private void SendLogToListeners(string message, string stacktrace, LogType type)
        {
            foreach (ILogListener logListener in _listeners)
            {
                logListener.OnLogMessageReceived(message, stacktrace, type);
            }
        }
    }
}