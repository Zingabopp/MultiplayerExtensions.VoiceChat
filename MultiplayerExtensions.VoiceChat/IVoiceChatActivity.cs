using System;

namespace MultiplayerExtensions.VoiceChat
{
    public interface IVoiceChatActivity
    {
        IConnectedPlayer Player { get; }
        event EventHandler<bool>? TalkingStateChanged;
        event EventHandler<bool>? MutedStateChanged;
    }
}
