using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Sportvida {
    [RequireComponent(typeof(Button))]
    public class ButtonUI :UIDisplayBase
    {
        private Button btn;
        protected override void Awake()
        {
            base.Awake();
            btn = GetComponent<Button>();
        }
        public Button GetButton()
        {
            return btn;
        }

    }
}
