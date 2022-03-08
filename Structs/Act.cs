﻿using System.Runtime.InteropServices;

namespace MapAssist.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Act
    {
        [FieldOffset(0x14)] public uint MapSeed;
        [FieldOffset(0x20)] public uint ActId;
        [FieldOffset(0x70)] public IntPtr pActMisc;
    }
}
