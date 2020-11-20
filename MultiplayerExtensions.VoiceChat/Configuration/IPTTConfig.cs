namespace MultiplayerExtensions.VoiceChat.Configuration
{
    public interface IPTTConfig
    {
        bool PushToTalk { get; }
        PTTOption PushToTalkButton { get; }
        bool HapticsAvailable { get; }
        bool EnableHaptics { get; }
    }
}
