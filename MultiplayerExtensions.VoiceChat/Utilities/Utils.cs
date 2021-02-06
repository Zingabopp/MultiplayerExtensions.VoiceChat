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
