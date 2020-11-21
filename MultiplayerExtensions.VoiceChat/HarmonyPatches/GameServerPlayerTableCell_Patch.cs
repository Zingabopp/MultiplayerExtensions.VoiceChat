using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerExtensions.VoiceChat.HarmonyPatches
{
    [HarmonyPatch(typeof(GameServerPlayerTableCell), "SetData", MethodType.Normal)]
    public class GameServerPlayerTableColor
    {
        private static Dictionary<string, GameServerPlayerTableCell> cells = new Dictionary<string, GameServerPlayerTableCell>();
        private static Dictionary<string, ILobbyPlayerDataModel> models = new Dictionary<string, ILobbyPlayerDataModel>();

        static void Postfix(IConnectedPlayer connectedPlayer, ILobbyPlayerDataModel playerDataModel, GameServerPlayerTableCell __instance)
        {
            cells[connectedPlayer.userId] = __instance;
            models[connectedPlayer.userId] = playerDataModel;
        }
    }
}
