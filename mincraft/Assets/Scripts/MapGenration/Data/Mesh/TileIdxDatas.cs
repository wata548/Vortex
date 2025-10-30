using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace MapGenerator.Tile {

    public enum FaceType {
        Up,
        Side,
        Down,
    };
    
    public class BlockInfo {
        [JsonConverter(typeof(StringEnumConverter))]
        public FaceType Dir { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
    }
    
    
    public static class TileIdxData {

        private static BlockInfo[][] posInfos = null;

        public static (int X, int Y) Get(Block pBlock, FaceType pFaceType) {
            SettUp();
            var temp = posInfos[(int)pBlock].FirstOrDefault(info => info.Dir == pFaceType);
            temp ??= posInfos[(int)pBlock][0];
            return new(temp.PosX, temp.PosY);
        }
            
        private static void SettUp() {

            if (posInfos != null)
                return; 
            
            var rawData = JsonConvert.DeserializeObject<Dictionary<string, BlockInfo[]>>(
                Resources.Load<TextAsset>("TileData").text
            );
            
            var enumData = rawData
                .Select(element => (Key: (Block)Enum.Parse(typeof(Block), element.Key), Value: element.Value))
                .OrderBy(element => element.Key)
                .ToList();

            for (int i = 0; i < enumData.Count; i++) {
                if (i != (int)enumData[i].Key)
                    throw new Exception($"This json isn't correct. need: {enumData[i].Key}");
            }

            posInfos = enumData.Select(element => element.Value).ToArray();
        }
    }
}