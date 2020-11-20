using System.Collections.Generic;

namespace MultiplayerExtensions.VoiceChat.Codecs
{
    public interface IEncoder
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
        /// Number of channels to encode.
        /// </summary>
        int Channels { get; }
        /// <summary>
        /// Duration of frames in milliseconds.
        /// </summary>
        int FrameDuration { get; }

        /// <summary>
        /// Size of the input frame.
        /// </summary>
        int FrameSize { get; }
        /// <summary>
        /// Returns a collection of valid frame durations (in milliseconds) for the <see cref="IEncoder"/>.
        /// </summary>
        IReadOnlyCollection<int> ValidFrameDurations { get; }

        /// <summary>
        /// Returns true if the given settings match the <see cref="IEncoder"/>'s relevant settings.
        /// Only compares values the <see cref="IEncoder"/> uses.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool SettingsMatch(ICodecSettings other);

        int Encode(float[] in_data, int in_offset, int in_length, byte[] out_data, int out_offset, int out_max);
        int Encode(short[] in_data, int in_offset, int in_length, byte[] out_data, int out_offset, int out_max);

    }
}
