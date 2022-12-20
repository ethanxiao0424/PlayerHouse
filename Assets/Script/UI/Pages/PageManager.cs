using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sportvida
{
    public class PageManager : Singleton<PageManager>
    {
        private List<PageBase> _PageList;
        private List<int> _PopupList;

        private List<int> _PageOrderList;
        private List<int> _PopupOrderList;

        private int _CurrentPage = 0;
        private bool _IsPageOpening = false;

        public int Register(PageBase page)
        {
            if (_PageList == null)
                _PageList = new List<PageBase>();

            _PageList.Add(page);

            return _PageList.Count - 1;
        }

        public void Register(int index)
        {
            if (_PopupList == null)
                _PopupList = new List<int>();

            _PopupList.Add(index);
        }

        public bool CheckOpen(int index)
        {
            if (_PopupList.Contains(index))
            {
                if (_PopupOrderList.Count > 0)
                {
                    _PopupOrderList.Add(index);
                    return false;
                }
            }
            else
            {
                _PageOrderList.Add(index);
            }

            if(_IsPageOpening)
                return false;

            _CurrentPage = index;
            _IsPageOpening = true;
            return true;
        }

        public void OnOpenDone()
        {
            _IsPageOpening = false;
            if (_PopupList.Contains(_CurrentPage))
                return;

            if (_PopupOrderList.Count > 0)
            {
                CheckNextPopup();
            }
            else if(_PageOrderList.Count > 0)
            {
                int last_ = _PageOrderList[_PageOrderList.Count - 1];
                var lastPage_ = _PageList[last_];
                if(!lastPage_.IsOpen)
                    lastPage_.Open();
            }
        }

        public void CheckNextPopup()
        {
            if (_PopupOrderList != null && _PopupOrderList.Count > 0)
            {
                int last_ = _PopupOrderList[_PopupOrderList.Count - 1];
                _PageList[last_].Open();
            }
        }
    }
}