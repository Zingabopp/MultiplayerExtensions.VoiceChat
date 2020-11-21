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
            if (codecSettings is OpusSettings opusSettings)
                return opusSettings.Clone();
            else
            {
                return new OpusSettings()
                {
                    SampleRate = codecSettings?.SampleRate ?? OpusDefaults.SampleRate,
                    Channels = codecSettings?.Channels ?? OpusDefaults.Channels,
                    FrameDuration = OpusDefaults.FrameDuration,
                    Bitrate = OpusDefaults.Bitrate
                };
            }
        }

        public OpusSettings()
        {
            SampleRate = OpusDefaults.SampleRate;
            Channels = OpusDefaults.Channels;
            Bitrate = OpusDefaults.Bitrate;
            FrameDuration = OpusDefaults.FrameDuration;
        }

        public string CodecId => OpusDefaults.CodecId;

        public int SampleRate { get; set; }
        public int Channels { get; set; }
        public int FrameDuration { get; set; }
        public int Bitrate { get; set; }

        public int FrameSize => (SampleRate * Channels * FrameDuration) / 1000;

        public OpusSettings Clone() => new OpusSettings()
        {
            SampleRate = SampleRate,
            Channels = Channels,
            Bitrate = Bitrate,
            FrameDuration = FrameDuration
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
                   Bitrate == other.Bitrate;
        }

        public bool Equals(ICodecSettings? other)
        {
            if (other is OpusSettings opusSettings)
                return Equals(opusSettings);
            return other != null &&
                   SampleRate == other.SampleRate &&
                   Channels == other.Channels &&
                   FrameDuration == OpusDefaults.FrameDuration &&
                   Bitrate == OpusDefaults.Bitrate;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = -2083580346;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CodecId);
                hashCode = hashCode * -1521134295 + SampleRate.GetHashCode();
                hashCode = hashCode * -1521134295 + Channels.GetHashCode();
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
