using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        /// <summary>
        /// Gets a resource and returns it as a byte array.
        /// From https://github.com/brian91292/BeatSaber-CustomUI/blob/master/Utilities/Utilities.cs
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="ResourceName"></param>
        /// <returns></returns>
        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            try
            {
                using Stream stream = asm.GetManifestResourceStream(ResourceName);
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, (int)stream.Length);
                return data;
            }
            catch (NullReferenceException)
            {
                throw;
                //Logger.log?.Debug($"Resource {ResourceName} was not found.");
            }
        }


        /// <summary>
        /// Creates a <see cref="Sprite"/> from an image <see cref="Stream"/>.
        /// </summary>
        /// <param name="imageStream"></param>
        /// <param name="pixelsPerUnit"></param>
        /// <param name="returnDefaultOnFail"></param>
        /// <returns></returns>
        public static Sprite? GetSpriteFromStream(Stream imageStream, float pixelsPerUnit = 100.0f)
        {
            if (imageStream == null || (imageStream.CanSeek && imageStream.Length == 0))
            {
                throw new ArgumentNullException(nameof(imageStream));
            }
            byte[]? data;
            if (imageStream is MemoryStream memStream)
            {
                data = memStream.ToArray();
            }
            else
            {
                data = imageStream.ToArray();
            }
            if (data == null || data.Length == 0)
            {
                throw new ArgumentException("Image stream's array is empty.", nameof(imageStream));
            }
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(data);
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0), pixelsPerUnit);

        }
    }
}
