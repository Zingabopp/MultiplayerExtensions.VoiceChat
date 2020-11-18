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
            //Container.BindInterfacesAndSelfTo<VoipReceiver>().AsSingle();
            Container.InstantiateComponent<VoipReceiver>(new UnityEngine.GameObject("VoipReceiver"));
            //Container.Resolve<VoipSender>().OnAudioGenerated += VoiceChatInstaller_OnAudioGenerated;
        }

        private void VoiceChatInstaller_OnAudioGenerated(object sender, VoipPacket e)
        {
            Plugin.Log?.Debug($"AudioGenerated: {e.Index.ToString().PadLeft(5)} | {e.DataLength}");
            if (e.Data == null || e.Data.Length == 0)
                return;
            int total = 0;
            for(int i = 0; i < e.DataLength; i++)
            {
                total += e.Data[i];
            }
            Plugin.Log?.Debug($"   Average: {total / e.DataLength}");
        }
    }
}
