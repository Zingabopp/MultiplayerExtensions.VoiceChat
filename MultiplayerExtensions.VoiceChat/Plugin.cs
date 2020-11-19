using IPA;
using IPA.Config;
using IPA.Config.Stores;
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
        internal static PluginConfig? Config { get; private set; }
        internal static IPALogger? Log { get; private set; }
        internal static Zenjector Zenjector = null!;

        [Init]
        public Plugin(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            // Config = conf.Generated<Configuration.PluginConfig>();
            Zenjector = zenjector;
            zenjector.OnApp<VoiceChatInstaller>();
            Log.Info($"MultiplayerExtensions: '{VersionInfo.Description}'");
        }

        [OnStart]
        public void OnApplicationStart()
        {
            Log?.Debug("OnApplicationStart");
            //TestAssembly();
        }

        void TestAssembly()
        {
            CSCore.CoreAudioAPI.MMDeviceEnumerator deviceEnumerator = new CSCore.CoreAudioAPI.MMDeviceEnumerator();
            foreach (var d in deviceEnumerator.EnumAudioEndpoints(CSCore.CoreAudioAPI.DataFlow.All, CSCore.CoreAudioAPI.DeviceState.Active))
            {
                Log?.Info($"{d.FriendlyName} | {d.DataFlow} | {d.DeviceState}");
            }
            Concentus.Structs.OpusEncoder encoder = new Concentus.Structs.OpusEncoder(48000, 2, Concentus.Enums.OpusApplication.OPUS_APPLICATION_VOIP);
            ArrayPool<short> arrayPool = ArrayPool<short>.Shared;
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Log?.Debug("OnApplicationQuit");

        }
    }
}
