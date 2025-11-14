using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Extension.Test;
using Newtonsoft.Json;
using UnityEngine;
using Object = System.Object;

namespace MapGenerator {
    
    public static class BlockData {

        [TestMethod]
        private static void Test(Block pBlock = Block.Dirty, BlockTag pTag = BlockTag.BreakTime) {
            Debug.Log(GetData(pBlock, pTag));
        }
        
       //==================================================||Constants 
        private const string PATTERN = @"(?<Tag>.+)\s*\=\s*(?<Value>.+)";
        
       //==================================================||Fields 
        private static readonly IReadOnlyDictionary<BlockTag, Type> _tagType = new Dictionary<BlockTag, Type>() {
            { BlockTag.BreakTime, typeof(float) },
            { BlockTag.Projected, typeof(bool) },
        };

        private static object _lock = new();

        private static IReadOnlyDictionary<Block, IReadOnlyDictionary<BlockTag, Object>> _tagDatas = null;

        
        public static Object GetData(this Block pBlock, BlockTag pTag) {
            
            lock(_lock)
                SetUp();
            return _tagDatas[pBlock].GetValueOrDefault(pTag, null);
        }
        
        private static void SetUp() {
            if (_tagDatas != null)
                return;

            var path = Path.Combine(Application.streamingAssetsPath, "BlockData.json");
            var context = File.ReadAllText(path);
            
            var json = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(context);
            _tagDatas = json.Select(kvp => (
                Key: Enum.Parse<Block>(kvp.Key),
                Value: kvp.Value
                    .SelectMany(tags => tags.Split(';'))
                    .Select(tagString => {
                        var match = Regex.Match(tagString, PATTERN);
                        var tag = Enum.Parse<BlockTag>(match.Groups["Tag"].Value);
                        var value = ExParse.ParseToObject(_tagType[tag], match.Groups["Value"].Value);
                        return (Tag: tag, Value: value);
                    })
                    .ToDictionary(tvp => tvp.Tag, tvp => tvp.Value)
            )).OrderBy(kvp => kvp.Key)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value as IReadOnlyDictionary<BlockTag, Object>);
        }

    }
}