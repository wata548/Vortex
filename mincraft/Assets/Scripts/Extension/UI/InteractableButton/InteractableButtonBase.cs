/*using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Wata.Extension.Serialize;
using UnityEngine.EventSystems;

namespace Wata.Extension.UI {
    public abstract partial class InteractableButtonBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
        
        //==================================================||Fields        
        private Tween _animation;
        [SerializeField] private InteractableButtonTag _tag;

        //==================================================||Abstract methods 
        public abstract void OnClick();
        protected abstract Tween OnEnter();
        protected abstract Tween OnExit();
        
        //==================================================||Methods
        private void Enter() {

            if (_tag != activeTag)
                return;
            selectedButton?.Exit();
            selectedButton = this;
            PrevAnimationKill();
            _animation = OnEnter();
        }
        
        private void Exit() {
            
            PrevAnimationKill();
            _animation = OnExit();
            if (selectedButton == this)
                selectedButton = null;
        }

        private void PrevAnimationKill() {
            
            _animation?.Kill();
        }
        
        //==================================================||Unity Functions
        
        public void OnPointerEnter(PointerEventData eventData) {
            Enter();
        }

        public void OnPointerExit(PointerEventData eventData) {
            Exit();
        }
        private void Awake() =>
            buttons.Add(this);
    }
}*/