using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using MultiplayerExtensions.VoiceChat.Configuration;
using MultiplayerExtensions.VoiceChat.Zenject;
using SiraUtil.Zenject;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPALogger = IPA.Logging.Logger;

namespace MultiplayerExtensions.VoiceChat
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; } = null!;
        internal static PluginMetadata PluginMeta { get; private set; } = null!;
        internal static PluginConfig? Config { get; private set; }
        internal static IPALogger? Log { get; private set; }
        internal static Zenjector Zenjector = null!;

        [Init]
        public Plugin(IPALogger logger, Config conf, PluginMetadata pluginMetadata, Zenjector zenjector)
        {
            Instance = this;
            PluginMeta = pluginMetadata;
            Log = logger;
            Config = conf.Generated<PluginConfig>();
            TestConfig(Config);
            Zenjector = zenjector;
            zenjector.OnApp<VoiceChatInstaller>();
            Log.Info($"MultiplayerExtensions.VoiceChat v{PluginMeta.Version}: '{VersionInfo.Description}'");
        }

        private void TestConfig(PluginConfig config)
        {
            config.PropertyChanged += (s, e) => { Log?.Debug($"PluginConfig.PropertyChanged: {e.PropertyName}"); };
            if (config is INotifyPropertyChanged castConfig)
                castConfig.PropertyChanged += (s, e) => { Log?.Debug($"INotifyPropertyChanged.PropertyChanged: {e.PropertyName}"); };
            else
                Log?.Error($"'{config.GetType().FullName}' is not INotifyPropertyChanged.");
            Log?.Critical("Changing property...");
            config.MicGain = config.MicGain + 1;
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log?.Debug("OnApplicationStart");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log?.Debug("OnApplicationQuit");

        }
    }
}
