using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MultiplayerExtensions.VoiceChat.Utilities;
using Zenject;

namespace MultiplayerExtensions.VoiceChat.UI
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public class PlayerTableCellIcon : MonoBehaviour
    {
        public static Sprite? TalkingSpritePrefab;
        public static Sprite? MutedSpritePrefab;
        public static Sprite? SelfMutedSpritePrefab;
        private SpriteRenderer? _talkingSprite;
        private SpriteRenderer? _mutedSprite;
        private SpriteRenderer? _selfMutedSprite;
        public string? PlayerId { get; protected set; }
        private IVoiceChatActivity? _chatActivity;
        public IVoiceChatActivity? ChatActivity
        {
            get => _chatActivity;
            private set
            {
                if (_chatActivity == value)
                    return;
                UnbindEvents(_chatActivity);
                _chatActivity = value;
                UnbindEvents(_chatActivity);
                BindEvents(_chatActivity);
            }
        }
        protected SpriteRenderer TalkingSprite
        {
            get
            {
                if (_talkingSprite == null)
                {
                    _talkingSprite = gameObject.AddComponent<SpriteRenderer>();
                    _talkingSprite.sprite = TalkingSpritePrefab;
                }
                return _talkingSprite;
            }
            set => _talkingSprite = value;
        }
        protected SpriteRenderer MutedSprite
        {
            get
            {
                if (_mutedSprite == null)
                {
                    _mutedSprite = gameObject.AddComponent<SpriteRenderer>();
                    _mutedSprite.sprite = MutedSpritePrefab;
                }
                return _mutedSprite;
            }
            set => _mutedSprite = value;
        }
        protected SpriteRenderer SelfMutedSprite
        {
            get
            {
                if (_selfMutedSprite == null)
                {
                    _selfMutedSprite = gameObject.AddComponent<SpriteRenderer>();
                    _selfMutedSprite.sprite = SelfMutedSpritePrefab;
                }
                return _selfMutedSprite;
            }
            set => _selfMutedSprite = value;
        }

        private bool _isMe;

        public bool IsMe
        {
            get { return _isMe && _chatActivity != null; }
            set
            {
                _isMe = value;
                UpdateIconState();
            }
        }


        private bool _playerTalking;

        public bool PlayerTalking
        {
            get { return _playerTalking && _chatActivity != null; }
            set
            {
                if (_playerMuted == value)
                    return;
                _playerTalking = value;
                UpdateIconState();
            }
        }

        private bool _playerMuted;

        public bool PlayerMuted
        {
            get { return _playerMuted && _chatActivity != null; }
            set
            {
                if (_playerMuted == value)
                    return;
                _playerMuted = value;
                UpdateIconState();
            }
        }

        private void UpdateIconState()
        {
            TalkingSprite.enabled = PlayerTalking;
            SelfMutedSprite.enabled = IsMe && PlayerMuted;
            MutedSprite.enabled = !IsMe && PlayerMuted;
        }
        public void Initialize(IVoiceChatActivity chatActivity)
        {
            ChatActivity = chatActivity;
            if(chatActivity != null)
            {
                IsMe = chatActivity.Player?.isMe ?? false;
                PlayerTalking = chatActivity.Talking;
                PlayerMuted = chatActivity.Muted;
            }
            else
            {
                IsMe = false;
                PlayerTalking = false;
                PlayerMuted = false;
            }
        }

        private void BindEvents(IVoiceChatActivity? chatActivity)
        {
            if (chatActivity == null)
                return;
            chatActivity.TalkingStateChanged += OnTalkingStateChanged;
            chatActivity.MutedStateChanged += OnMutedStateChanged;
        }

        private void UnbindEvents(IVoiceChatActivity? chatActivity)
        {
            if (chatActivity == null)
                return;
            chatActivity.TalkingStateChanged -= OnTalkingStateChanged;
            chatActivity.MutedStateChanged -= OnMutedStateChanged;
        }

        private void OnMutedStateChanged(object sender, bool isMuted)
        {
            PlayerMuted = isMuted;
        }

        private void OnTalkingStateChanged(object sender, bool isTalking)
        {
            PlayerTalking = isTalking;
        }

        public void SetPlayerId(string playerId) => PlayerId = playerId;
        public event EventHandler<string?>? Destroyed;
        #region Monobehaviour Messages
        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Destroyed.RaiseEventSafe(this, PlayerId, nameof(Destroyed));
        }

        #endregion
    }
}
