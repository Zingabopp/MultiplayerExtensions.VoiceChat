using MultiplayerExtensions.VoiceChat.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Utilities
{
    public static class Extensions
    {
        public static void RaiseEventSafe(this EventHandler? e, object sender, string eventName)
        {
            if (e == null) return;
            EventHandler[] handlers = e.GetInvocationList().Select(d => (EventHandler)d).ToArray()
                ?? Array.Empty<EventHandler>();
            for (int i = 0; i < handlers.Length; i++)
            {
                try
                {
                    handlers[i].Invoke(sender, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error($"Error in '{eventName}' handlers '{handlers[i]?.Method.Name}': {ex.Message}");
                    Plugin.Log?.Debug(ex);
                }
            }
        }

        public static void RaiseEventSafe<TArgs>(this EventHandler<TArgs>? e, object sender, TArgs args, string eventName)
        {
            if (e == null) return;
            EventHandler<TArgs>[] handlers = e.GetInvocationList().Select(d => (EventHandler<TArgs>)d).ToArray()
                ?? Array.Empty<EventHandler<TArgs>>();
            for (int i = 0; i < handlers.Length; i++)
            {
                try
                {
                    handlers[i].Invoke(sender, args);
                }
                catch (Exception ex)
                {
                    Plugin.Log?.Error($"Error in '{eventName}' handlers '{handlers[i]?.Method.Name}': {ex.Message}");
                    Plugin.Log?.Debug(ex);
                }
            }
        }
        /// <summary>
        /// Converts a <see cref="Stream"/> to a byte array.
        /// From: https://stackoverflow.com/a/44929033
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ToArray(this Stream s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s));
            if (!s.CanRead)
                throw new ArgumentException("Stream cannot be read");

            if (s is MemoryStream ms)
                return ms.ToArray();

            long pos = s.CanSeek ? s.Position : 0L;
            if (pos != 0L)
                s.Seek(0, SeekOrigin.Begin);

            byte[] result = new byte[s.Length];
            s.Read(result, 0, result.Length);
            if (s.CanSeek)
                s.Seek(pos, SeekOrigin.Begin);
            return result;
        }

        public static float GetMicGainFloat(this IVoiceSettings config)
        {
            int intGain = config.MicGain + 100;
            return Math.Max(0, intGain / 100f);
        }
    }
}
