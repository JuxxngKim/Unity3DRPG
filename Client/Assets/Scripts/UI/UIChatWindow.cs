using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace YeongJ.UI
{
    public class UIChatWindow : UISingleton<UIChatWindow>
    {
        [SerializeField] InputField _inputField;
        [SerializeField] Text _templateChatText;
        [SerializeField] RectTransform _contentRoot;

        List<Text> _chatList = new List<Text>();

        public override void InitSingleton()
        {
            _inputField.onEndEdit.AddListener(SendChat);
        }

        public void SendChat(string text)
        {
            var userChat = text;
            if (userChat == string.Empty)
                return;

            //Managers.Network.Send(null);
            AddChat(userChat);
            _inputField.text = string.Empty;
        }

        public void AddChat(string userChat)
        {
            var newText =  GameObjectCache.Make<Text>(_templateChatText, _contentRoot);
            newText.text = userChat;
            _chatList.Add(newText);

            LayoutRebuilder.ForceRebuildLayoutImmediate(_contentRoot);
        }
    }
}