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
            ICodecFactory codecFactory = BuildFactory();
            var encoder = codecFactory.CreateEncoder(OpusDefaults.CodecId);
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

        private ICodecFactory BuildFactory()
        {
            ICodecFactory codecFactory = new CodecFactory();

            codecFactory.RegisterCodec(OpusDefaults.CodecId, new OpusSettings(), s => new OpusEncoder(s), s => new OpusDecoder(s));

            return codecFactory;
        }
    }
}
