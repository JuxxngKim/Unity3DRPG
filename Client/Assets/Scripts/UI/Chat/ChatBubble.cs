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
        [SerializeField] float _remainTime;

        int _objectId;
        BaseActor _baseActor;
        UnityAction<int> _completeCallback;

        public void SetData(BaseActor baseActor, string userName, string chatText, UnityAction<int> completeCallback)
        {
            _baseActor = baseActor;
            _nameText.text = userName;
            _chatText.text = chatText;
            _remainTime = 3.0f;
            _completeCallback = completeCallback;

            _objectId = baseActor?.Id ?? 0;

            LayoutRebuilder.ForceRebuildLayoutImmediate(_bubbleRoot);

            Update();
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
                _completeCallback?.Invoke(_objectId);
                _completeCallback = null;
            }
        }

        void LateUpdate()
        {
            if (_baseActor == null)
                return;

            var screenPoint = Camera.main.WorldToScreenPoint(_baseActor.transform.position);
            this.transform.position = screenPoint;
        }
    }
}