using MultiplayerExtensions.VoiceChat.Networking;
using Zenject;

namespace MultiplayerExtensions.VoiceChat.Zenject
{
    public class VoiceChatInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VoipSender>().AsSingle();
            Container.Bind<VoipReceiver>().FromNewComponentOnRoot();
            Container.BindInterfacesAndSelfTo<VoiceChatPacketRouter>().AsSingle().NonLazy();
        }
    }
}
