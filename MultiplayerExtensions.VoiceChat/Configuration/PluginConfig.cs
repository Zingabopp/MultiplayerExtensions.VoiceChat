using System;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using Newtonsoft.Json;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace MultiplayerExtensions.VoiceChat.Configuration
{
    internal class PluginConfig : IVoiceSettings
    {
        private int voiceChatGain = 0;
        private string voiceChatMicrophone = "";
        private bool micEnabled = true;
        private bool spatialAudio = false;

        [SerializedName(nameof(EnableVoiceChat))]
        [JsonProperty(nameof(EnableVoiceChat), Order = 0)]
        public virtual bool EnableVoiceChat { get; set; }

        [SerializedName(nameof(VoiceChatGain))]
        [JsonProperty(nameof(VoiceChatGain), Order = 10)]
        public virtual int VoiceChatGain
        {
            get => voiceChatGain;
            set
            {
                value = Math.Min(value, 100);
                value = Math.Max(value, -50);
                if (voiceChatGain == value) 
                    return;
                voiceChatGain = value;
                VoiceChatGainChanged?.Invoke(this, value);
            }
        }

        [SerializedName(nameof(SpatialAudio))]
        [JsonProperty(nameof(SpatialAudio), Order = 20)]
        public virtual bool SpatialAudio
        {
            get => spatialAudio;
            set
            {
                if (spatialAudio == value)
                    return;
                spatialAudio = value;
                RaiseVoiceSettingChanged();
            }
        }
        [SerializedName(nameof(MicEnabled))]
        [JsonProperty(nameof(MicEnabled), Order = 30)]
        public virtual bool MicEnabled
        {
            get => micEnabled;
            set
            {
                if (micEnabled == value)
                    return;
                micEnabled = value;
                RaiseVoiceSettingChanged();
            }
        }
        [SerializedName(nameof(VoiceChatMicrophone))]
        [JsonProperty(nameof(VoiceChatMicrophone), Order = 60)]
        public string VoiceChatMicrophone
        {
            get => voiceChatMicrophone;
            set
            {
                if (voiceChatMicrophone == value)
                    return;
                voiceChatMicrophone = value;
                RaiseVoiceSettingChanged();
            }
        }
        [NonNullable]
        [JsonProperty(nameof(InputSettings), Order = 70)]
        public virtual InputConfig InputSettings { get; set; } = new InputConfig();

        public event EventHandler<int>? VoiceChatGainChanged;
        public event EventHandler<string?>? VoiceSettingChanged;

        private void RaiseVoiceSettingChanged([CallerMemberName] string? setting = null)
        {
            VoiceSettingChanged?.Invoke(this, setting);
        }

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
