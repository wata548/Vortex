using MapGenerator;
using Unity.VisualScripting;
using UnityEngine;

namespace Inventory {
    
    public static class InventoryData {
           
        private struct SlotData {
            public Block Block;
            public int Amount;
        }

        private const int COUNT = 10;
        private static SlotData[] _slots = new SlotData[COUNT];

        public static void GetItem(Block pBlock, int pAmount = 1) {
            var candidateIdx = -1;
            for (int i = 0; i < COUNT; i++) {
                if (candidateIdx == -1 && _slots[i].Block == Block.Air) {
                    candidateIdx = i;
                }

                if (_slots[i].Block == pBlock) {
                    _slots[i].Amount += pAmount;
                    Refresh(i);
                    return;
                }
            }

            _slots[candidateIdx].Block = pBlock;
            _slots[candidateIdx].Amount = pAmount;
            Refresh(candidateIdx);
        }

        public static bool UseItem(out Block pBlock, int pAmount = 1) =>
            UseItem(InventoryShower.Instance.Selected, out pBlock, pAmount);
        public static bool UseItem(int pIdx, out Block pBlock, int pAmount = 1) {
            pBlock = Block.Air;
            if (_slots[pIdx].Amount < pAmount)
                return false;

            _slots[pIdx].Amount -= pAmount;
            pBlock = _slots[pIdx].Block;
            
            if (_slots[pIdx].Amount == 0)
                _slots[pIdx].Block = Block.Air;
            Refresh(pIdx);
            return true;
        }
        
        private static void Refresh(int pIdx) => 
            InventoryShower.Instance.Refresh(pIdx, _slots[pIdx].Block, _slots[pIdx].Amount);
        
        public static void Start() {
            InventoryShower.Instance.Generate(COUNT);
        }
    }
}