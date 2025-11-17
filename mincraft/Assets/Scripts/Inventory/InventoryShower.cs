using System;
using MapGenerator;
using UnityEngine;

namespace Inventory {
    public class InventoryShower: MonoBehaviour {
        [SerializeField] private Slot _slotPrefabs;
        private const int SLOT_CNT = 10;
        private Slot[] _slots = new Slot[SLOT_CNT];

        private void Generate() {
            var size = (_slotPrefabs.transform as RectTransform)!.rect.size;
            var origin = -size * (SLOT_CNT / 2) + (SLOT_CNT % 2 == 1 ? -size / 2 : Vector2.zero);
            for (int i = 0; i < SLOT_CNT; i++) {
                _slots[i] = Instantiate(_slotPrefabs, transform);
                _slots[i].SetItem(Block.Air);
                _slots[i].transform.localPosition = new(origin.x + size.x * i, 0);
            }
        }
        
        private void Awake() {
            Generate();
        }
    }
}