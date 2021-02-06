using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace MultiplayerExtensions.VoiceChat.Configuration
{
    [NotifyPropertyChanges]
    internal class PluginConfig : IVoiceSettings, INotifyPropertyChanged
    {
        private int voiceChatGain = 0;
        private int micGain = 0;
        private string voiceChatMicrophone = "";
        private bool micEnabled = true;
        private bool spatialAudio = false;

        [SerializedName(nameof(EnableVoiceChat))]
        [JsonProperty(nameof(EnableVoiceChat), Order = 0)]
        public virtual bool EnableVoiceChat { get; set; }
        [SerializedName(nameof(MicGain))]
        [JsonProperty(nameof(MicGain), Order = 5)]
        public virtual int MicGain
        {
            get => voiceChatGain;
            set
            {
                value = Math.Min(value, 100); // If min/max change, must adjust GetMicGainFloat in Utilities/Extensions.cs
                value = Math.Max(value, -50);
                if (micGain == value)
                    return;
                micGain = value;
                RaiseVoiceSettingChanged();
                //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("-MicGain-"));
            }
        }

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
        public virtual string VoiceChatMicrophone
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
#if DEBUG
            Plugin.Log?.Debug($"IVoiceSettings.{setting ?? "*"} changed.");
#endif
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

        public virtual event PropertyChangedEventHandler? PropertyChanged;

        public void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //public void PrintNotifyStuff()
        //{
        //    PropertyChangedEventHandler? del = PropertyChanged;
        //    BindingFlags flags = (BindingFlags)int.MaxValue;
        //    FieldInfo[]? fields = this.GetType().GetFields(flags);
        //    Plugin.Log?.Info($"Fields: {string.Join(", ", fields.Select(f => f.Name))}");
        //    if (del == null)
        //        Plugin.Log?.Error("PropertyChanged is null");
        //    else
        //    {
        //        Plugin.Log?.Warn($"PropertyChanged Delegates: {string.Join(", ", del.GetInvocationList().Select(d => d.Method.Name))}");
        //    }
        //    FieldInfo? genField = this.GetType().GetField("<event>PropertyChanged", flags);
        //    PropertyChangedEventHandler? genFieldValue = genField?.GetValue(this) as PropertyChangedEventHandler;
        //    if (genFieldValue == null)
        //        Plugin.Log?.Error("Generaged PropertyChanged is null");
        //    else
        //    {
        //        Plugin.Log?.Warn($"Generaged PropertyChanged Delegates: {string.Join(", ", genFieldValue.GetInvocationList().Select(d => d.Method.Name))}");
        //    }
        //}
        //BindingFlags flags = (BindingFlags)int.MaxValue;
        //private PropertyChangedEventHandler? _genHandler;

        //private PropertyChangedEventHandler? GenHandler
        //{
        //    get
        //    {
        //        if (_genHandler == null)
        //        {
        //            _genHandler = GenField.GetValue(this) as PropertyChangedEventHandler;
        //        }
        //        return _genHandler;
        //    }
        //    set
        //    {
        //        GenField.SetValue(this, value);
        //    }
        //}

        //private FieldInfo? _genField;

        //private FieldInfo GenField
        //{
        //    get
        //    {
        //        if (_genField == null)
        //            _genField = this.GetType().GetField("<event>PropertyChanged", flags);
        //        return _genField;
        //    }
        //}


    }
}
