using System;

namespace MultiplayerExtensions.VoiceChat
{
    public interface IVoiceChatActivity
    {
        IConnectedPlayer Player { get; }
        bool Talking { get; }
        bool Muted { get; }
        event EventHandler<bool>? TalkingStateChanged;
        event EventHandler<bool>? MutedStateChanged;
    }

    public enum VoiceChatState
    {
        None = 0,
        Talking = 1 << 0,
        Muted = 1 << 1
    }
}
