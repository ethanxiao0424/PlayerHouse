using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sportvida
{
    public class PopupBase : PageBase
    {
        private int _PopupIndex;
        public Action CheckNextPopup;
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            //µù¥U¨ìPageMgr¨ú±oIndex(_PageIndex)
            _PopupIndex = PageManager.instance.Register(this);
            CheckNextPopup += PageManager.instance.CheckNextPopup;
        }

        public override void Open()
        {
            //check PageMgr waitList
            base.Open();
        }

        public override void Close()
        {
            base.Close();
            CheckNextPopup?.Invoke();
        }
    }
}