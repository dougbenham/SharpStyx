﻿using System.Runtime.InteropServices;

namespace SharpStyx.Structs
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ObjectData
    {
        public IntPtr pObjectTxt;
        public byte InteractType;
        public byte PortalFlags;
        private short unk1;
        public IntPtr pShrineTxt;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        private byte[] unk2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x10)]
        public string Owner;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ObjectTxt
    {
        public ushort Id;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
        public string ObjectName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x40)]
        public string ObjectType;
    }
}
