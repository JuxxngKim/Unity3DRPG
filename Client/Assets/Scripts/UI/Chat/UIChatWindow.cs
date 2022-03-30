using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Google.Protobuf.Protocol;
using YeongJ.Inagme;

namespace YeongJ.UI
{
    public class UIChatWindow : UISingleton<UIChatWindow>
    {
        [SerializeField] InputField _inputField;
        [SerializeField] Text _templateChatText;
        [SerializeField] RectTransform _contentRoot;
        [SerializeField] RectTransform _bubbleRoot;
        [SerializeField] ChatBubble _templateChatBubble;

        List<Text> _chatList = new List<Text>();
        Dictionary<int, ChatBubble> _chatBubbles = new Dictionary<int, ChatBubble>();

        public override void InitSingleton()
        {
            base.InitSingleton();

            _inputField.onEndEdit.AddListener(SendChat);
        }

        public void SendChat(string text)
        {
            var userChat = text;
            if (userChat == string.Empty)
                return;

            C_Chat chatPacket = new C_Chat();
            chatPacket.Chat = text;
            Managers.Network.Send(chatPacket);
            
            _inputField.text = string.Empty;
        }

        public void AddChat(int objectId, string userName, string userChat)
        {
            string chat = $"[{System.DateTime.Now.Hour:D2}:{System.DateTime.Now.Minute:D2}] {userName} : {userChat}";

            var newText =  GameObjectCache.Make<Text>(_templateChatText, _contentRoot);
            newText.text = chat;
            _chatList.Add(newText);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRoot);

            if (_chatBubbles.ContainsKey(objectId))
            {
                _chatBubbles[objectId].UpdateData(userChat);
            }
            else
            {
                var baseActor = Managers.Object.FindById(objectId)?.GetComponent<BaseActor>();
                if (baseActor == null)
                    return;

                var newBubble = GameObjectCache.Make<ChatBubble>(_templateChatBubble, _bubbleRoot);
                _chatBubbles.Add(objectId, newBubble);

                _chatBubbles[objectId].SetData(baseActor, userName, userChat, RemoveBubbleChat);
            }
        }

        private void RemoveBubbleChat(int objectId)
        {
            if (_chatBubbles.ContainsKey(objectId))
            {
                GameObjectCache.Delete(_chatBubbles[objectId].transform);
                _chatBubbles.Remove(objectId);
            }
        }
    }
}