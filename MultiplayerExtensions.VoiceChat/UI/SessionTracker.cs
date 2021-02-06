using MultiplayerExtensions.VoiceChat.HarmonyPatches;
using MultiplayerExtensions.VoiceChat.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace MultiplayerExtensions.VoiceChat.UI
{
    public class SessionTracker
    {
        private IMultiplayerSessionManager _sessionManager;
        private DiContainer _container;

        private static readonly Dictionary<string, GameServerPlayerTableCell> cells = new Dictionary<string, GameServerPlayerTableCell>();

        public SessionTracker(IMultiplayerSessionManager sessionManager, DiContainer container)
        {
            _sessionManager = sessionManager;
            _sessionManager.playerConnectedEvent += OnPlayerConnected;
            _sessionManager.playerDisconnectedEvent += OnPlayerDisconnected;
            _sessionManager.disconnectedEvent += OnDisconnected;
            _sessionManager.connectedEvent += OnConnected;
            _container = container;
        }

        public event EventHandler? SessionConnected;
        public event EventHandler<DisconnectedReason>? SessionDisconnected;
        public event EventHandler<IConnectedPlayer>? PlayerDisconnected;
        public event EventHandler<IConnectedPlayer>? PlayerConnected;

        public void SetTableCellData(string userId,  GameServerPlayerTableCell tableCell)
        {
            PlayerTableCellIcon? icon = tableCell.gameObject.GetComponent<PlayerTableCellIcon>();
            if (icon == null)
                icon = tableCell.gameObject.AddComponent<PlayerTableCellIcon>();
            icon.SetPlayerId(userId);
        }
        
        public void ReloadData(IEnumerable<KeyValuePair<string, GameServerPlayerTableCell>> pairs)
        {
            cells.Clear();
            foreach (var item in pairs)
            {
                SetTableCellData(item.Key, item.Value);
            }
        }

        private void OnConnected()
        {
            SessionConnected?.RaiseEventSafe(this, nameof(SessionDisconnected));
        }

        private void OnDisconnected(DisconnectedReason reason)
        {
            SessionDisconnected?.RaiseEventSafe(this, reason, nameof(SessionDisconnected));
        }

        private void OnPlayerConnected(IConnectedPlayer connectedPlayer)
        {
            PlayerConnected?.RaiseEventSafe(this, connectedPlayer, nameof(PlayerConnected));
        }
        private void OnPlayerDisconnected(IConnectedPlayer disconnectedPlayer)
        {
            PlayerDisconnected?.RaiseEventSafe(this, disconnectedPlayer, nameof(PlayerDisconnected));
        }
    }
}
