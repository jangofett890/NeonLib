using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace NeonLib.Debugging {

    public static class NeonDebug {

        static bool init = false;

        public static bool enableDebugging = true;

        public static List<LogChannel> channels { get { if (!init) Init(); return m_channels; } set { if (!init) Init(); m_channels = value; } }

        private static List<LogChannel> m_channels = new List<LogChannel>();

        public static UnityEvent<string> OnConsoleLog { get { if (!init) Init(); return m_OnConsoleLog; } set { if (!init) Init(); m_OnConsoleLog = value; } }

        private static UnityEvent<string> m_OnConsoleLog;

        public static void Log(string ChannelName, string msg) {
            if (!enableDebugging)
                return;
            if (!init)
                Init();
            LogChannel channel;
            channel = channels.Find(c => c.name == ChannelName);
            if (channel == null) {
                UnityEngine.Debug.LogWarning("Could not find any Log Channel with the name: " + ChannelName);
                return;
            }
            if (!channel.Enabled)
                return;
            string colorStart = "<color=#" + ColorUtility.ToHtmlStringRGB(channel.PrintColor) + ">";
            string toOut = "[" + ChannelName + "]" + "  " + msg;
            string colorEnd = "</color>";
            channel.Log(colorStart + toOut + colorEnd);
            Debug.Log(colorStart + toOut + colorEnd);
        }

        static void Init() {
            if (m_OnConsoleLog == null) {
                m_OnConsoleLog = new UnityEvent<string>();
            }
            init = true;
            var channell = Resources.LoadAll<LogChannel>("LogChannels").ToList();
            for (int i = 0; i < channell.Count; i++) {
                AddLogChannel(channell[i]);
            }
            m_channels = channell;
        }

        public static void OnDisable() {
            for (int i = channels.Count; i --> 0;) {
                RemoveLogChannel(channels[i]);
            } 
        }

        private static void AddLogChannel(LogChannel channel) {
            channel.OnLog.AddListener(ConsoleLog);
            channels.Add(channel);
        }

        private static void RemoveLogChannel(LogChannel channel) {
            channel.OnLog.RemoveListener(ConsoleLog);
            channels.Remove(channel);
        }

        private static void ConsoleLog(string Message) {
            OnConsoleLog.Invoke(Message);
        }

    }
}
