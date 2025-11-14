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
    public enum Block:Byte {
        Air,
        Base,
        Grass,
        Dirty,
        Stone,
        TreeBlock,
        Leaf,
    }
}