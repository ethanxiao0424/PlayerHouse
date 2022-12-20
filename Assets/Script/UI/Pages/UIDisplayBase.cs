#define NewLayout

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sportvida
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIDisplayBase : MonoBehaviour
    {
        public bool IsOpen;
        public bool _IsUseAnim = false;
        public float _AnimSpeed = .05f;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_IsUseAnim)
                StartCoroutine(OpenAnim());

            SetOpen(IsOpen);
        }

        protected virtual IEnumerator OpenAnim()
        {
            while (true)
            {
                yield return null;
                if (IsOpen && (!_canvasGroup.interactable || !_canvasGroup.blocksRaycasts))
                {
                    _canvasGroup.alpha += _AnimSpeed;
                }
                else if (!IsOpen && (_canvasGroup.interactable || _canvasGroup.blocksRaycasts))
                {
                    _canvasGroup.alpha -= _AnimSpeed;
                }
                else
                {
                    _canvasGroup.alpha = IsOpen ? 1 : 0;
                    continue;
                }


                if (_canvasGroup.alpha >= 1)
                {
                    SetOpen(true);
                }
                else if (_canvasGroup.alpha <= 0)
                {
                    SetOpen(false);
                }
            }
        }
        public virtual void Open()
        {
#if UNITY_EDITOR
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
#endif
            if (!_IsUseAnim)
            {
                SetOpen(true);
            }
            IsOpen = true;
            _canvasGroup.blocksRaycasts = IsOpen;
        }
        public virtual IEnumerator Open(float _time)
        {
            Open();
            yield return new WaitUntil(() => _canvasGroup.alpha == 1);
            yield return new WaitForSeconds(_time);
            Close();
            yield return new WaitUntil(() => _canvasGroup.alpha == 0);
        }
        public virtual IEnumerator Open(float _dur,float _openSpeed,float _closeSpeed)
        {
            float originSpeed = _AnimSpeed;
            _AnimSpeed = _openSpeed;
            yield return new WaitUntil(() => _canvasGroup.alpha == 1);
            yield return new WaitForSeconds(_dur);
            _AnimSpeed = _closeSpeed;
            Close();
            yield return new WaitUntil(() => _canvasGroup.alpha == 0);
            _AnimSpeed = originSpeed;
        }


        protected void SetOpen(bool isOpen)
        {
            _canvasGroup.alpha = isOpen?1:0;
            _canvasGroup.interactable = isOpen;
            _canvasGroup.blocksRaycasts = isOpen;
        }
        public void SetInteractable(bool isOpen)
        {
            _canvasGroup.interactable = isOpen;
        }
        public void SetblocksRaycasts(bool isOpen)
        {
            _canvasGroup.blocksRaycasts = isOpen;
        }

        public virtual void Close()
        {
#if UNITY_EDITOR
            if (_canvasGroup == null)
                _canvasGroup = GetComponent<CanvasGroup>();
#endif
            if (!_IsUseAnim)
            {
                SetOpen(false);
            }
            IsOpen = false;
        }

        protected CanvasGroup _canvasGroup;
    }
    [System.Serializable]
    public class BasicUI
    {
        public UIDisplayBase ShowUI;

        public virtual void UIState(bool state)
        {
            if (state)
                ShowUI.Open();
            else
                ShowUI.Close();
        }
    }
}
