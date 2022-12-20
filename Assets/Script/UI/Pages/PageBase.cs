using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sportvida
{
    public class PageBase : UIDisplayBase
    {
        private int _PageIndex;
        public Func<int,bool> CheckOpen;
        public Action OpenDone;
        // Start is called before the first frame update
        protected virtual void Start()
        {
            //µù¥U¨ìPageMgr¨ú±oIndex(_PageIndex)
            _PageIndex = PageManager.instance.Register(this);
            CheckOpen += PageManager.instance.CheckOpen;
            OpenDone += PageManager.instance.OnOpenDone;
        }

        public override void Open()
        {
            //var isOK_ = CheckOpen?.Invoke(_PageIndex);
            //if ((bool)!isOK_)
            //    return;
            //check PageMgr waitList
            base.Open();

            //OpenDone?.Invoke();
        }

        public override void Close()
        {
            base.Close();
        }
    }
}