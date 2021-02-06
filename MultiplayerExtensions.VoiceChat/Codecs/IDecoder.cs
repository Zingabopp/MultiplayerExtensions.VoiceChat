namespace MultiplayerExtensions.VoiceChat.Codecs
{
    public interface IDecoder
    {
        /// <summary>
        /// Identifier for the codec.
        /// </summary>
        string CodecId { get; }
        /// <summary>
        /// Sample rate of the audio in hz.
        /// </summary>
        int SampleRate { get; }
        /// <summary>
        /// Number of channels to decode.
        /// </summary>
        int Channels { get; }
        /// <summary>
        /// Gain to apply to decoded audio. (-50 to 100)
        /// </summary>
        int Gain { get; set; }
        /// <summary>
        /// Returns a clone of the current settings.
        /// </summary>
        /// <returns></returns>
        ICodecSettings GetCodecSettings();
        /// <summary>
        /// Returns true if the given settings match the <see cref="IDecoder"/>'s relevant settings.
        /// Only compares values the <see cref="IDecoder"/> uses.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool SettingsMatch(ICodecSettings other);
        /// <summary>
        /// Decodes an encoded packet.
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="in_offset"></param>
        /// <param name="in_length"></param>
        /// <param name="out_data">Output size will be (NumberOfSamples * Channels).</param>
        /// <param name="out_offset"></param>
        /// <returns></returns>
        int Decode(byte[]? in_data, int in_offset, int in_length, float[] out_data, int out_offset);
        /// <summary>
        /// Decodes an encoded packet.
        /// </summary>
        /// <param name="in_data"></param>
        /// <param name="in_offset"></param>
        /// <param name="in_length"></param>
        /// <param name="out_data">Output size will be (NumberOfSamples * Channels).</param>
        /// <param name="out_offset"></param>
        /// <returns></returns>
        int Decode(byte[]? in_data, int in_offset, int in_length, short[] out_data, int out_offset);
    }
}
