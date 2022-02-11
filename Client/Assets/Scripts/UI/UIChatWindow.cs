using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace YeongJ.UI
{
    public class UIChatWindow : UISingleton<UIChatWindow>, IRecyclableScrollDataSource
    {
        [SerializeField] InputField _inputField;
        [SerializeField] RecyclableScrollRect _scrollRect;

        private List<string> _chatList;

        public void SendChat()
        {
            var userChat = _inputField.text;
            if (string.Empty == userChat)
                return;

            Managers.Network.Send(null);
            _inputField.text = string.Empty;
        }

        public void AddChat(string userChat)
        {
            _chatList.Add(userChat);

            _scrollRect.totalCount = _chatList.Count;
            _scrollRect.RefillCells();
        }

        public void ProvideData(Transform transform, int idx)
        {
            if (_chatList.Count <= idx)
                return;

            var chatText = transform.GetComponent<Text>();
            chatText.text = _chatList[idx];
        }
    }
}