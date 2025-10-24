/*using System;
using DG.Tweening;
using Extension;
using Extension.Scene;
using UnityEngine;

namespace Extension.UI {
    public class TestButton: InteractableButtonBase {
        private void Start() {
            RealNumber a = 0.78f;
            Debug.Log(a.ToSingle());
        }

        public override void OnClick() {
            SceneManager.LoadScene(Scene.Scene.Main);
        }
        
        protected override Tween OnEnter() => 
            transform.DOScale(1.2f, 0.2f)
                .SetEase(Ease.OutCirc);

        protected override Tween OnExit() =>
            transform.DOScale(1.0f, 0.05f)
                .SetEase(Ease.OutCirc);
    }
}*/