using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Utilities
{
    public static class Utils
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
    }
}
