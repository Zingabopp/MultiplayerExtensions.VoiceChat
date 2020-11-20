using MultiplayerExtensions.VoiceChat.Codecs;
using MultiplayerExtensions.VoiceChat.Codecs.Opus;
using MultiplayerExtensions.VoiceChat.Configuration;
using MultiplayerExtensions.VoiceChat.Networking;
using MultiplayerExtensions.VoiceChat.Utilities.Input;
using System;
using Zenject;

namespace MultiplayerExtensions.VoiceChat.Zenject
{
    public class VoiceChatInstaller : Installer
    {
        public override void InstallBindings()
        {
            ICodecFactory codecFactory = new OpusCodecFactory();
            var encoder = codecFactory.CreateEncoder(48000, 1);
            Container.Bind<PluginConfig>().FromInstance(Plugin.Config ?? throw new InvalidOperationException("Plugin.Config is null"));
            Container.Bind<IVoiceSettings>().FromInstance(Plugin.Config);
            Container.Bind<IPTTConfig>().FromInstance(Plugin.Config.InputSettings);
            Container.Bind<IInputController>().To<InputController>().AsSingle();
            Container.Bind<ICodecFactory>().FromInstance(codecFactory);
            Container.Bind<IEncoder>().FromInstance(encoder);
            Container.BindInterfacesAndSelfTo<VoipSender>().AsSingle();
            Container.Bind<VoipReceiver>().FromNewComponentOnRoot().AsTransient();
            Container.BindInterfacesAndSelfTo<VoiceChatPacketRouter>().AsSingle();
        }
    }
}
