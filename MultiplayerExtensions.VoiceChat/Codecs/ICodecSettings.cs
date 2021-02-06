using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Codecs
{
    public interface ICodecSettings
    {
        string CodecId { get; }

        int SampleRate { get; set; }
        int Channels { get; set; }
        /// <summary>
        /// Gain to apply to decoded audio. (-50 to 100)
        /// </summary>
        int DecoderGain { get; set; }

        ICodecSettings Clone();
        void UpdateFrom(ICodecSettings codecSettings);
    }
}
