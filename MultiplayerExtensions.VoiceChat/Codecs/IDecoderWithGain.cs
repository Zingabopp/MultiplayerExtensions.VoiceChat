using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Codecs
{
    public interface IDecoderWithGain : IDecoder
    {
        /// <summary>
        /// Recommended -50 to 100
        /// </summary>
        public int Gain { get; set; }
    }
}
