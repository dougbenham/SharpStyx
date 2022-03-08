using System.Runtime.InteropServices;
using SharpStyx.Types;

namespace SharpStyx.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct Level
    {
        [FieldOffset(0x1F8)] public Area LevelId;
    }
}
