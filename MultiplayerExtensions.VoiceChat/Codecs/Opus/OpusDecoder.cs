using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Codecs.Opus
{
    public class OpusDecoder : IDecoder
    {
        protected readonly Concentus.Structs.OpusDecoder Decoder;
        private readonly OpusSettings _settings = new OpusSettings();

        public string CodecId => "Opus";

        public int SampleRate { get => _settings.SampleRate; protected set => _settings.SampleRate = value; }
        public int Channels { get => _settings.Channels; protected set => _settings.Channels = value; }

        public OpusDecoder(int sampleRate, int numChannels)
        {
            SampleRate = sampleRate;
            Channels = numChannels;
            Decoder = new Concentus.Structs.OpusDecoder(sampleRate, numChannels);
        }

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
