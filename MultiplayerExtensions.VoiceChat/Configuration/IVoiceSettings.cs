using System;

namespace MultiplayerExtensions.VoiceChat.Configuration
{
    public interface IVoiceSettings
    {
        bool EnableVoiceChat { get; }
        bool SpatialAudio { get; }
        bool MicEnabled { get; }
        string VoiceChatMicrophone { get; }
        int MicGain { get; }
        int VoiceChatGain { get; }

        event EventHandler<int>? VoiceChatGainChanged;
        event EventHandler<string?>? VoiceSettingChanged;
    }
}
