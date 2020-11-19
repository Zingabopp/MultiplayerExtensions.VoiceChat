using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace MultiplayerExtensions.VoiceChat.Configuration
{
    internal class PluginConfig
    {
        private float voiceChatVolume = 0.8f;

        [SerializedName(nameof(EnableVoiceChat))]
        [JsonProperty(nameof(EnableVoiceChat), Order = 0)]
        public virtual bool EnableVoiceChat { get; set; }

        [SerializedName(nameof(VoiceChatVolume))]
        [JsonProperty(nameof(VoiceChatVolume), Order = 10)]
        public virtual float VoiceChatVolume
        {
            get => voiceChatVolume;
            set
            {
                if (voiceChatVolume == value) return;
                voiceChatVolume = value;
                var handler = VoiceChatVolumeChanged;
                handler?.Invoke(this, value);
            }
        }
        public event EventHandler<float>? VoiceChatVolumeChanged;

        [SerializedName(nameof(SpatialAudio))]
        [JsonProperty(nameof(SpatialAudio), Order = 20)]
        public virtual bool SpatialAudio { get; set; } = false;

        [SerializedName(nameof(MicEnabled))]
        [JsonProperty(nameof(MicEnabled), Order = 30)]
        public virtual bool MicEnabled { get; set; } = true;

        [SerializedName(nameof(PushToTalk))]
        [JsonProperty(nameof(PushToTalk), Order = 40)]
        public virtual bool PushToTalk { get; set; } = true;

        [SerializedName(nameof(PushToTalkButton))]
        [UseConverter(typeof(EnumConverter<PTTOption>))]
        [JsonProperty(nameof(PushToTalkButton), Order = 50)]
        public virtual PTTOption PushToTalkButton { get; set; } = PTTOption.LeftAndRightTrigger;

        [SerializedName(nameof(VoiceChatMicrophone))]
        [JsonProperty(nameof(VoiceChatMicrophone), Order = 60)]
        public string VoiceChatMicrophone { get; set; } = "";

        [NonNullable]
        [JsonProperty(nameof(InputSettings), Order = 70)]
        public virtual InputConfig InputSettings { get; set; } = new InputConfig();
        
        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }
    }
    
}
