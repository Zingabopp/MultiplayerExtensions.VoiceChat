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
}
