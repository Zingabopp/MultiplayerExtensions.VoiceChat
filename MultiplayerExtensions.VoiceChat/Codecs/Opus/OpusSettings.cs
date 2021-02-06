using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Codecs.Opus
{
    public class OpusSettings : ICodecSettings, IEquatable<OpusSettings?>, IEquatable<ICodecSettings?>
    {
        public static OpusSettings CloneSettings(ICodecSettings codecSettings)
        {
            if (codecSettings == null)
                throw new ArgumentNullException(nameof(codecSettings));
            if (codecSettings is OpusSettings opusSettings)
                return opusSettings.Clone();
            else
            {
                return new OpusSettings()
                {
                    SampleRate = codecSettings.SampleRate,
                    Channels = codecSettings.Channels,
                    DecoderGain = codecSettings.DecoderGain,
                    FrameDuration = OpusDefaults.FrameDuration,
                    Bitrate = OpusDefaults.Bitrate,
                };
            }
        }

        public OpusSettings()
        {
            SampleRate = OpusDefaults.SampleRate;
            Channels = OpusDefaults.Channels;
            Bitrate = OpusDefaults.Bitrate;
            FrameDuration = OpusDefaults.FrameDuration;
            DecoderGain = OpusDefaults.Gain;
        }

        public string CodecId => OpusDefaults.CodecId;

        public int SampleRate { get; set; }
        public int Channels { get; set; }
        public int FrameDuration { get; set; }
        public int Bitrate { get; set; }
        public int DecoderGain { get; set; }

        public int FrameSize => (SampleRate * Channels * FrameDuration) / 1000;

        public OpusSettings Clone() => new OpusSettings()
        {
            SampleRate = SampleRate,
            Channels = Channels,
            Bitrate = Bitrate,
            FrameDuration = FrameDuration,
            DecoderGain = DecoderGain
        };
        ICodecSettings ICodecSettings.Clone() => Clone();

        public override bool Equals(object? obj)
        {
            return Equals(obj as OpusSettings);
        }

        public bool Equals(OpusSettings? other)
        {
            return other != null &&
                   SampleRate == other.SampleRate &&
                   Channels == other.Channels &&
                   FrameDuration == other.FrameDuration &&
                   Bitrate == other.Bitrate &&
                   DecoderGain == other.DecoderGain;
        }

        public bool Equals(ICodecSettings? other)
        {
            if (other is OpusSettings opusSettings)
                return Equals(opusSettings);
            return other != null &&
                   SampleRate == other.SampleRate &&
                   Channels == other.Channels &&
                   DecoderGain == other.DecoderGain &&
                   FrameDuration == OpusDefaults.FrameDuration &&
                   Bitrate == OpusDefaults.Bitrate;
        }


        public void UpdateFrom(ICodecSettings? other)
        {
            if (other == null)
                return;
            SampleRate = other.SampleRate;
            Channels = other.Channels;
            DecoderGain = other.DecoderGain;
            if (other is OpusSettings opusSettings)
            {
                FrameDuration = opusSettings.FrameDuration;
                Bitrate = opusSettings.Bitrate;
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = -2083580346;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodecId);
                hashCode = hashCode * -1521134295 + SampleRate.GetHashCode();
                hashCode = hashCode * -1521134295 + Channels.GetHashCode();
                hashCode = hashCode * -1521134295 + DecoderGain.GetHashCode();
                hashCode = hashCode * -1521134295 + FrameDuration.GetHashCode();
                hashCode = hashCode * -1521134295 + Bitrate.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(OpusSettings? left, OpusSettings? right)
        {
            if (ReferenceEquals(left, right))
                return true;
            if (left == null || right == null)
                return false;
            return EqualityComparer<OpusSettings>.Default.Equals(left, right);
        }

        public static bool operator !=(OpusSettings? left, OpusSettings? right)
        {
            return !(left == right);
        }
    }
}
