using MultiplayerExtensions.VoiceChat.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace MultiplayerExtensions.VoiceChat
{
    public class VoiceChatInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VoipSender>().AsSingle().NonLazy();
            Container.Bind<VoipReceiver>().FromNewComponentOnRoot().AsSingle();
            Container.BindInterfacesAndSelfTo<VoiceChatPacketRouter>().AsSingle().NonLazy();
        }
    }
}
