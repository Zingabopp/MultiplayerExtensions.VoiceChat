using HarmonyLib;
using MultiplayerExtensions.VoiceChat.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace MultiplayerExtensions.VoiceChat.HarmonyPatches
{
    [HarmonyPatch(typeof(GameServerPlayerTableCell), "SetData", MethodType.Normal)]
    public class GameServerPlayerTableCell_Patch
    {
        internal static DiContainer _container;
        private static SessionTracker? _sessionTracker;

        public static SessionTracker? SessionTracker
        {
            get 
            {
                if (_sessionTracker == null && _container != null)
                    SessionTracker = _container.Resolve<SessionTracker>();
                return _sessionTracker; 
            }
            set 
            {
                if (_sessionTracker == value)
                    return;
                UnbindEvents(_sessionTracker);
                _sessionTracker = value;
                UnbindEvents(_sessionTracker); // Just in case
                BindEvents(_sessionTracker);
            }
        }
        private static readonly Dictionary<string, GameServerPlayerTableCell> cells = new Dictionary<string, GameServerPlayerTableCell>();

        static void Postfix(IConnectedPlayer connectedPlayer, ILobbyPlayerData playerDataModel, GameServerPlayerTableCell __instance)
        {
            cells[connectedPlayer.userId] = __instance;
            SessionTracker? tracker = SessionTracker;
            if(tracker != null)
            {
                tracker.SetTableCellData(connectedPlayer.userId, __instance);
            }
        }

        static void BindEvents(SessionTracker? sessionTracker)
        {
            if (sessionTracker == null)
                return;
            //sessionTracker.PlayerConnected += OnPlayerConnected;
            sessionTracker.SessionConnected += OnSessionConnected;
            sessionTracker.SessionDisconnected += OnSessionDisconnected;
            sessionTracker.PlayerDisconnected += OnPlayerDisconnected;
        }

        static void UnbindEvents(SessionTracker? sessionTracker)
        {
            if (sessionTracker == null)
                return;
            //sessionTracker.PlayerConnected -= OnPlayerConnected;
            sessionTracker.SessionConnected -= OnSessionConnected;
            sessionTracker.SessionDisconnected -= OnSessionDisconnected;
            sessionTracker.PlayerDisconnected -= OnPlayerDisconnected;
        }

        private static void OnSessionConnected(object sender, EventArgs e)
        {
            cells.Clear();
        }

        private static void OnSessionDisconnected(object sender, DisconnectedReason e)
        {
            cells.Clear();
        }

        static void OnPlayerDisconnected(object s, IConnectedPlayer disconnectedPlayer)
        {
            cells.Remove(disconnectedPlayer.userId);
        }
    }
}
