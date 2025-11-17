using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;
using Object = System.Object;

namespace MapGenerator.Tile {
    
    public static class TileIdxData {
        public enum FaceType {
            Up,
            Side,
            Down,
        };
    
        private class BlockInfo {
            [JsonConverter(typeof(StringEnumConverter))]
            public FaceType Dir { get; set; }
            public int PosX { get; set; }
            public int PosY { get; set; }
        }
        
        private static BlockInfo[][] posInfos = null;
        private static bool isInited = false;
        private static readonly object _lock = new();

        public static (int X, int Y) GetFace(this Block pBlock, FaceType pFaceType) {

            lock (_lock) {
                SettUp();
            }

            var temp = posInfos[(int)pBlock].FirstOrDefault(info => info.Dir == pFaceType);
            temp ??= posInfos[(int)pBlock][0];
            return new(temp.PosX, temp.PosY);
        }
            
        private static void SettUp() {

            if (posInfos != null || isInited)
                return;    
            isInited = true;

            var rawData = JsonConvert.DeserializeObject<Dictionary<string, BlockInfo[]>>(
                File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "TileIdxData.json"))
            );
            
            var enumData = rawData
                .Select(element => (Key: Enum.Parse<Block>(element.Key), Value: element.Value))
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