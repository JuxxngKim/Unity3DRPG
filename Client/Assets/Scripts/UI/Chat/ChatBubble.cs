using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YeongJ.Inagme;

namespace YeongJ.UI
{
    public class ChatBubble : MonoBehaviour
    {
        [SerializeField] RectTransform _bubbleRoot;
        [SerializeField] Text _nameText;
        [SerializeField] Text _chatText;

        float _remainTime;
        BaseActor _baseActor;
        UnityAction<ChatBubble> _completeCallback;

        public void SetData(BaseActor baseActor, string userName, string chatText, UnityAction<ChatBubble> completeCallback)
        {
            _baseActor = baseActor;
            _nameText.text = userName;
            _chatText.text = chatText;
            _remainTime = 3.0f;
            _completeCallback = completeCallback;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_bubbleRoot);
        }

        public void UpdateData(string chatText)
        {
            _remainTime = 3.0f;
            _chatText.text = chatText;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_bubbleRoot);
        }

        void Update()
        {
            _remainTime -= Time.deltaTime;
            if(_remainTime <= 0f)
            {
                _completeCallback?.Invoke(this);
                _completeCallback = null;
            }
        }

        void LateUpdate()
        {
            if (_baseActor == null)
                return;

            var screenPoint = Camera.main.WorldToScreenPoint(_baseActor.transform.position);
        }
    }
}