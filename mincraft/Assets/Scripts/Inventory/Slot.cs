using System;
using System.Linq;
using Extension.Test;
using MapGenerator;
using MapGenerator.Tile;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Inventory {
    public class Slot:MonoBehaviour {
        [SerializeField] private Material _matPref;
        [SerializeField] private Image _shower;
        private Block _block;
        private Material _mat = null;

        [TestMethod]
        public void SetItem(Block pBlock = Block.Dirty) {
            _block = pBlock;
            if (_mat == null) {
                _mat = new Material(_matPref);
                _shower.material = _mat;
            }
            
            var idx = _block.GetFace(TileIdxData.FaceType.Up);
            _mat.SetVector("_Pos", new(idx.X, idx.Y));
        }
    }
}