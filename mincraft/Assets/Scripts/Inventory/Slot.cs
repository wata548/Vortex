using Extension.Test;
using MapGenerator;
using MapGenerator.Tile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory {
    public class Slot:MonoBehaviour {
        [SerializeField] private Material _matPref;
        [SerializeField] private Image _frame;
        [SerializeField] private Image _shower;
        [SerializeField] private TMP_Text _cnt;
        private Block _block;
        private Material _mat = null;

        [TestMethod]
        public void SetItem(Block pBlock = Block.Dirty, int pCnt = 1) {
            _block = pBlock;
            if (_mat == null) {
                _mat = new Material(_matPref);
                _shower.material = _mat;
            }
            
            var idx = _block.GetFace(TileIdxData.FaceType.Up);
            _mat.SetVector("_Pos", new(idx.X, idx.Y));
            _cnt.text = pBlock == Block.Air ? "" : pCnt.ToString();
        }

        public void SetFrame(Sprite pSprite) {
            _frame.sprite = pSprite;
        }
    }
}