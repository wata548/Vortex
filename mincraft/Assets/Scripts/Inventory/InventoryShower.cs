using System;
using Entity;
using Extension;
using MapGenerator;
using UnityEngine;

namespace Inventory {
    public class InventoryShower: MonoSingleton<InventoryShower> {

        private int _selected;

        public int Selected {
            get => _selected;
            private set {
                _slots[_selected].SetFrame(_inactiveFrame);
                _selected = Mathf.Clamp(value, 0, _slots.Length - 1);               
                _slots[_selected].SetFrame(_activeFrame);
            }
        }

        protected override bool IsNarrowSingleton { get; set; } = true;
        
        [SerializeField] private Slot _slotPrefabs;
        [SerializeField] private Sprite _activeFrame;
        [SerializeField] private Sprite _inactiveFrame;
        private Slot[] _slots = null;
        private IPlayerInputSetting _input;
        
        //==================================================||Methods 

        public void SetUp(IPlayerInputSetting pInput) =>
            _input = pInput;
       
        public void Refresh(int pIdx, Block pBlock, int pAmount) =>
            _slots[pIdx].SetItem(pBlock, pAmount);
        public void Generate(int pAmount) {
            if (_slots != null) {
                foreach (var slot in _slots) {
                    Destroy(slot);
                }
            }

            _slots = new Slot[pAmount];
            
            var size = (_slotPrefabs.transform as RectTransform)!.rect.size;
            var origin = -size * (pAmount / 2) + (pAmount % 2 == 1 ? -size / 2 : Vector2.zero);
            
            for (int i = 0; i < pAmount; i++) {
                _slots[i] = Instantiate(_slotPrefabs, transform);
                _slots[i].SetItem(Block.Air);
                _slots[i].transform.localPosition = new(origin.x + size.x * i, -1);
            }

            Selected = 0;
        }

        public new void Update() {
            base.Update();
            if (_slots == null)
                return;
            Selected += _input.SelectItemSlot;
        }
    }
}