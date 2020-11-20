namespace MultiplayerExtensions.VoiceChat.Utilities.Input
{
    public interface IInputController
    {
        bool UsePushToTalk { get; }
        bool TalkEnabled { get; }
        void TriggerFeedback();
    }
}
