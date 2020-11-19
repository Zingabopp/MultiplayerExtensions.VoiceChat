using IPA.Config.Stores.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.Configuration
{
    internal class InputConfig
    {
        private float triggerInputThreshold = 0.85f;
        [SerializedName(nameof(TriggerInputThreshold))]
        [JsonProperty(nameof(TriggerInputThreshold), Order = 0)]
        public virtual float TriggerInputThreshold
        {
            get => triggerInputThreshold;
            set
            {
                if (triggerInputThreshold == value) return;
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;
                triggerInputThreshold = value;
            }
        }

        private float gripInputThreshold = 0.85f;
        [SerializedName(nameof(GripInputThreshold))]
        [JsonProperty(nameof(GripInputThreshold), Order = 10)]
        public virtual float GripInputThreshold
        {
            get => gripInputThreshold;
            set
            {
                if (gripInputThreshold == value) return;
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;
                gripInputThreshold = value;
            }
        }

        [SerializedName(nameof(EnableHaptics))]
        [JsonProperty(nameof(EnableHaptics), Order = 15)]
        public virtual bool EnableHaptics { get; set; } = true;

        private float hapticAmplitude = 0.5f;
        [SerializedName(nameof(HapticAmplitude))]
        [JsonProperty(nameof(HapticAmplitude), Order = 20)]
        public virtual float HapticAmplitude
        {
            get => hapticAmplitude;
            set
            {
                if (hapticAmplitude == value) return;
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;
                hapticAmplitude = value;
            }
        }

        private float hapticDuration = 0.1f;
        [SerializedName(nameof(HapticDuration))]
        [JsonProperty(nameof(HapticDuration), Order = 30)]
        public virtual float HapticDuration
        {
            get => hapticDuration;
            set
            {
                if (hapticDuration == value) return;
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;
                hapticDuration = value;
            }
        }
    }

    [Flags]
    public enum PTTOption
    {
        None = 0,                                          // 0000 0000
        LeftTrigger = 1 << 0,                              // 0000 0001                              
        RightTrigger = 1 << 1,                             // 0000 0010
        LeftAndRightTrigger = LeftTrigger | RightTrigger,  // 0000 0011
        AnyTrigger = 1 << 3 | LeftAndRightTrigger,         // 0000 0111
        LeftGrip = 1 << 5,                                 // 0001 0000
        RightGrip = 1 << 6,                                // 0010 0000
        LeftAndRightGrip = LeftGrip | RightGrip,           // 0011 0000
        AnyGrip = 1 << 7 | LeftAndRightGrip                // 0111 0000
    }
    public static class PPTOptionExtensions
    {
        public static bool Satisfies(this PTTOption actualState, PTTOption checkState)
        {
            if (checkState == PTTOption.AnyTrigger)
                return (actualState & PTTOption.AnyTrigger) != 0;
            if (checkState == PTTOption.AnyGrip)
                return (actualState & PTTOption.AnyGrip) != 0;
            return actualState.HasFlag(checkState);
        }

        public static int OptionIndex(this PTTOption option)
        {
            switch (option)
            {
                case PTTOption.None:
                    return 0;
                case PTTOption.LeftTrigger:
                    return 1;
                case PTTOption.RightTrigger:
                    return 2;
                case PTTOption.LeftAndRightTrigger:
                    return 3;
                case PTTOption.AnyTrigger:
                    return 4;
                case PTTOption.LeftGrip:
                    return 5;
                case PTTOption.RightGrip:
                    return 6;
                case PTTOption.LeftAndRightGrip:
                    return 7;
                case PTTOption.AnyGrip:
                    return 8;
                default:
                    return 0;
            }
        }
    }
}
