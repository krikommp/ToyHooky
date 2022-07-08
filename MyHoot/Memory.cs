#define WIN
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MyHoot
{
    public class Memory
    {
#if WIN
        [DllImport("Kernel32.dll")]
        public static extern bool VirtualProtect(
            IntPtr address,
            uint size,
            uint flNewProtect,
            out uint flOldProtect);
#endif
        public static void WriteBytes(IntPtr address, byte[] bytes)
        {
            for (int index = 0; index < bytes.Length; ++index)
                Memory.WriteByte(address + index, bytes[index]);
        }

        public static byte[] ReadBytes(IntPtr address, int size)
        {
            List<byte> byteList = new List<byte>();
            for (int index = 0; index < size; ++index)
                byteList.Add(Memory.ReadByte(address + index));
            return byteList.ToArray();
        }

        public static unsafe void WriteInt(IntPtr address, int intToWrite)
        {
#if WIN
            uint flOldProtect;
            Memory.VirtualProtect(address, 4U, 64U, out flOldProtect);
#endif
            *(int*)(void*)address = intToWrite;
#if WIN
            Memory.VirtualProtect(address, 4U, flOldProtect, out flOldProtect);
#endif
        }

        public static unsafe void WriteLong(IntPtr address, long longToWrite)
        {
#if WIN
            uint flOldProtect;
            Memory.VirtualProtect(address, 8U, 64U, out flOldProtect);
#endif
            *(long*)(void*)address = longToWrite;
#if WIN
            Memory.VirtualProtect(address, 8U, flOldProtect, out flOldProtect);
#endif
        }

        public static unsafe void WriteByte(IntPtr address, byte byteToWrite)
        {
#if WIN
            uint flOldProtect;
            Memory.VirtualProtect(address, 1U, 64U, out flOldProtect);
#endif
            *(sbyte*)(void*)address = (sbyte)byteToWrite;
#if WIN
            Memory.VirtualProtect(address, 1U, flOldProtect, out flOldProtect);
#endif
        }

        public static unsafe byte ReadByte(IntPtr address)
        {
#if WIN
            uint flOldProtect;
            Memory.VirtualProtect(address, 1U, 64U, out flOldProtect);
#endif
            int num = (int)*(byte*)(void*)address;
#if WIN
            Memory.VirtualProtect(address, 1U, flOldProtect, out flOldProtect);
#endif
            return (byte)num;
        }
    }
}