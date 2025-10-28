using System;

namespace MapGenerator {
    public partial class MapMeshGenerator {

        [Flags]
        private enum CheckDirection : byte {
            None = 0b000000,
            Up = 0b100000,
            Down = 0b010000,
            Left = 0b001000,
            Right = 0b000100,
            Front = 0b000010,
            Behind = 0b000001,
            All = 0b111111,
        }
    }
}