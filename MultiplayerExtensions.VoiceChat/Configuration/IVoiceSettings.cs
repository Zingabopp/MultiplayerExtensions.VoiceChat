using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Configuration
{
    public interface IVoiceSettings
    {
        bool EnableVoiceChat { get; }
        bool SpatialAudio { get; }
        bool MicEnabled { get; }
        string VoiceChatMicrophone { get; }
        int VoiceChatGain { get; }

        event EventHandler<int>? VoiceChatGainChanged;
        event EventHandler<string>? VoiceSettingChanged;
    }
}
