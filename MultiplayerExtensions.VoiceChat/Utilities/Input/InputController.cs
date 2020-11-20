using MultiplayerExtensions.VoiceChat.Configuration;
using MultiplayerExtensions.VoiceChat.Utilities;
using System;
using UnityEngine.XR;

namespace MultiplayerExtensions.VoiceChat.Utilities.Input
{
    public class InputController : IInputController
    {
        private IPTTConfig _config;
        public bool UsePushToTalk => _config.PushToTalk;
        public bool TalkEnabled
        {
            get
            {
                PTTOption option = _config.PushToTalkButton;
                PTTOption currentState = PTTOption.None;
                if (option.HasFlag(PTTOption.LeftGrip) || option.HasFlag(PTTOption.LeftTrigger))
                {
                    if (option.HasFlag(PTTOption.LeftTrigger) && ControllersHelper.LeftTriggerActive)
                    {
                        currentState |= PTTOption.LeftTrigger;
                    }
                    if (option.HasFlag(PTTOption.LeftGrip) && ControllersHelper.LeftGripActive)
                    {
                        currentState |= PTTOption.LeftGrip;
                    }
                }
                if (option.HasFlag(PTTOption.RightGrip) || option.HasFlag(PTTOption.RightTrigger))
                {
                    if (option.HasFlag(PTTOption.RightGrip) && ControllersHelper.RightGripActive)
                    {
                        currentState |= PTTOption.RightGrip;
                    }
                    if (option.HasFlag(PTTOption.RightTrigger) && ControllersHelper.RightTriggerActive)
                    {
                        currentState |= PTTOption.RightTrigger;
                    }
                }
                bool satisfied = currentState.Satisfies(option);
                return satisfied;
            }
        }

        public InputController(IPTTConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void TriggerFeedback()
        {
            PTTOption option = _config.PushToTalkButton;
            if (option.HasFlag(PTTOption.LeftGrip) || option.HasFlag(PTTOption.LeftTrigger))
                ControllersHelper.TriggerShortRumble(XRNode.LeftHand);
            if (option.HasFlag(PTTOption.RightGrip) || option.HasFlag(PTTOption.RightTrigger))
                ControllersHelper.TriggerShortRumble(XRNode.RightHand);
        }
    }
}
