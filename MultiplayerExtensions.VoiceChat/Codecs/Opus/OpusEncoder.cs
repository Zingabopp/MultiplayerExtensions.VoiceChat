using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Codecs.Opus
{
    public class OpusEncoder : IEncoder
    {
        protected readonly Concentus.Structs.OpusEncoder Encoder;
        private static readonly int[] _validFrameDurations = new int[] { 5, 10, 20, 40, 60 };
        private readonly OpusSettings _settings = new OpusSettings();
        public string CodecId => "Opus";
        public int SampleRate { get => _settings.SampleRate; protected set => _settings.SampleRate = value; }
        public int Channels { get => _settings.Channels; protected set => _settings.Channels = value; }
        public int Bitrate { get => _settings.Bitrate; protected set => _settings.Bitrate = value; }
        public int FrameDuration { get => _settings.FrameDuration; protected set => _settings.FrameDuration = value; }
        public int FrameSize => (SampleRate * Channels * FrameDuration) / 1000;
        public IReadOnlyCollection<int> ValidFrameDurations { get; } = Array.AsReadOnly(_validFrameDurations);

        public OpusEncoder(int sampleRate, int numChannels, int frameDuration, int bitrate)
        {
            SampleRate = sampleRate;
            Channels = numChannels;
            Bitrate = bitrate;
            if (!_validFrameDurations.Contains(frameDuration))
                throw new ArgumentException($"{nameof(frameDuration)} must be a value in {{ {string.Join(", ", _validFrameDurations)} }}", nameof(frameDuration));
            FrameDuration = frameDuration;
            Encoder = new Concentus.Structs.OpusEncoder(sampleRate, numChannels, Concentus.Enums.OpusApplication.OPUS_APPLICATION_VOIP)
            {
                Bitrate = bitrate
            };
        }

        bool IEncoder.SettingsMatch(ICodecSettings other)
        {
            return _settings.Equals(other);
        }

        public int Encode(float[] in_data, int in_offset, int in_length, byte[] out_data, int out_offset, int out_max)
        {
            return Encoder.Encode(in_data, in_offset, in_length / Channels, out_data, out_offset, out_max);
        }
        public int Encode(short[] in_data, int in_offset, int in_length, byte[] out_data, int out_offset, int out_max)
        {
            return Encoder.Encode(in_data, in_offset, in_length / Channels, out_data, out_offset, out_max);
        }
    }
}
