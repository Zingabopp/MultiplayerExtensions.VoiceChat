using System;

namespace MultiplayerExtensions.VoiceChat.Codecs
{
    public interface ICodecFactory
    {
        /// <summary>
        /// Return true if the given <paramref name="codecId"/> is registered.
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>
        bool HasCodec(string codecId);
        /// <summary>
        /// Returns a copy of the registered default settings for the given <paramref name="codecId"/> or null if it's not registered.
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>
        ICodecSettings? GetDefaultSettings(string codecId);
        /// <summary>
        /// Creates an encoder of type <paramref name="codecId"/> using the registered default settings.
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEncoder CreateEncoder(string codecId);
        /// <summary>
        /// Creates an encoder of type <paramref name="codecId"/> using the given settings.
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IEncoder CreateEncoder(string codecId, ICodecSettings codecSettings);
        /// <summary>
        /// Creates a decoder of type <paramref name="codecId"/> using the registered default settings.
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IDecoder CreateDecoder(string codecId);
        /// <summary>
        /// Creates a decoder of type <paramref name="codecId"/> using the given settings.
        /// </summary>
        /// <param name="codecId"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        IDecoder CreateDecoder(string codecId, ICodecSettings codecSettings);
        /// <summary>
        /// Registers a default settings and encoder/decoder factories for a given <paramref name="codecId"/>.
        /// </summary>
        /// <param name="codecId"></param>
        /// <param name="defaultSettings"></param>
        /// <param name="encoderFactory"></param>
        /// <param name="decoderFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        void RegisterCodec(string codecId, ICodecSettings defaultSettings, 
            Func<ICodecSettings, IEncoder> encoderFactory, 
            Func<ICodecSettings, IDecoder> decoderFactory);
    }

}
