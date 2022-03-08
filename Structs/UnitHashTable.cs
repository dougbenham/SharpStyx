using System.Runtime.InteropServices;

namespace SharpStyx.Structs
{
    [StructLayout(LayoutKind.Explicit)]
    public struct UnitHashTable
    {
        [FieldOffset(0x00)] [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public IntPtr[] UnitTable;
    }
}
