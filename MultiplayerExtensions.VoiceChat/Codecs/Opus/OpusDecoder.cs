using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Codecs.Opus
{
    public class OpusDecoder : IDecoder, IDecoderWithGain
    {
        private const int MaxGain = 7000;
        protected readonly Concentus.Structs.OpusDecoder Decoder;
        private readonly OpusSettings _settings;

        public string CodecId => OpusDefaults.CodecId;

        public int SampleRate { get => _settings.SampleRate; protected set => _settings.SampleRate = value; }
        public int Channels { get => _settings.Channels; protected set => _settings.Channels = value; }

        /// <summary>
        /// Recommended -50 to 100
        /// </summary>
        public int Gain
        {
            get => (Decoder.Gain * 100) / MaxGain;
            set => Decoder.Gain = (value * MaxGain) / 100;
        }

        protected OpusDecoder(OpusSettings settings)
        {
            if (settings == null)
                throw new InvalidOperationException("_settings is null.");
            _settings = settings;
            Decoder = new Concentus.Structs.OpusDecoder(settings.SampleRate, settings.Channels);
            Plugin.Log?.Error($"--------------Decoder default gain: {Decoder.Gain}  ------------------");
            Decoder.Gain = 50;
        }

        public OpusDecoder(int sampleRate, int numChannels)
            : this(new OpusSettings() { SampleRate = sampleRate, Channels = numChannels })
        { }
        public OpusDecoder(ICodecSettings settings)
            : this(OpusSettings.CloneSettings(settings ?? throw new ArgumentNullException(nameof(settings))))
        { }

        public ICodecSettings GetCodecSettings() => _settings.Clone();

        public bool SettingsMatch(ICodecSettings other)
        {
            if (other == null)
                return false;
            return SampleRate == other.SampleRate
                && Channels == other.Channels;
        }

        public int Decode(byte[]? in_data, int in_offset, int in_length, float[] out_data, int out_offset)
        {
            return Decoder.Decode(in_data, in_offset, in_length, out_data, out_offset, out_data.Length - out_offset);
        }

        public int Decode(byte[]? in_data, int in_offset, int in_length, short[] out_data, int out_offset)
        {
            return Decoder.Decode(in_data, in_offset, in_length, out_data, out_offset, out_data.Length - out_offset);
        }
    }
}
