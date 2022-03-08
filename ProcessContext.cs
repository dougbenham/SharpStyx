using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpStyx
{
    public class ProcessContext : IDisposable
    {
        public int OpenContextCount = 1;

        public Process Process { get; set; }

        private IntPtr _handle;
        private readonly IntPtr _baseAddr;
        private readonly int _moduleSize;
        private int _disposed;

        public ProcessContext(Process process)
        {
            Process = process;
            _handle = WindowsExternal.OpenProcess((uint) WindowsExternal.ProcessAccessFlags.VirtualMemoryRead, false, Process.Id);
            _baseAddr = Process.MainModule.BaseAddress;
            _moduleSize = Process.MainModule.ModuleMemorySize;
        }

        public IntPtr Handle
        {
            get => _handle;
        }

        public IntPtr BaseAddr
        {
            get => _baseAddr;
        }

        public int ProcessId
        {
            get => Process.Id;
        }

        public IntPtr GetUnitHashtableOffset()
        {
            var pattern = "\x48\x8d\x00\x00\x00\x00\x00\x8b\xd1";
            var mask = "xx?????xx";
            var patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, 3);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0);
            var delta = patternAddress.ToInt64() - _baseAddr.ToInt64();
            return IntPtr.Add(_baseAddr, (int) (delta + 7 + offsetAddressToInt));
        }

        public IntPtr GetExpansionOffset()
        {
            var pattern = "\x48\x8B\x05\x00\x00\x00\x00\x48\x8B\xD9\xF3\x0F\x10\x50\x00";
            var mask = "xxx????xxxxxxx?";
            var patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, 3);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0);
            var delta = patternAddress.ToInt64() - _baseAddr.ToInt64();
            return IntPtr.Add(_baseAddr, (int) (delta + 7 + offsetAddressToInt));
        }

        public IntPtr GetGameNameOffset() // This is relatively more hacky than the other scans, need to test against another D2R 1.2 build. Struct changed massively with 1.2 from what I can tell.
        {
            var pattern = "\x48\x83\xC4\x28\xC3\x1A\xDF";
            var mask = "xxxxxxx";
            var patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, -0x44);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0);

            try
            {
                offsetAddressToInt = (int) Read<uint>(IntPtr.Add(_baseAddr, offsetAddressToInt + 0x145));
            }
            catch (Exception)
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            return IntPtr.Add(_baseAddr, (int) (offsetAddressToInt + 0x13));
        }

        public IntPtr GetMenuOpenOffset()
        {
            var pattern = "\x8B\x05\x00\x00\x00\x00\x89\x44\x24\x20\x74\x07";
            var mask = "xx????xxxxxx";
            var patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, 2);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0);
            var delta = patternAddress.ToInt64() - _baseAddr.ToInt64();
            return IntPtr.Add(_baseAddr, (int) (delta + 6 + offsetAddressToInt));
        }

        public IntPtr GetMenuDataOffset()
        {
            var pattern = "\x41\x0F\xB6\xAC\x3F\x00\x00\x00\x00";
            var mask = "xxxxx????";
            var patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, 5);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0);
            return IntPtr.Add(_baseAddr, offsetAddressToInt);
        }

        public IntPtr GetRosterDataOffset()
        {
            var pattern = "\x02\x45\x33\xD2\x4D\x8B";
            var mask = "xxxxxx";
            var patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, -3);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0);
            var delta = patternAddress.ToInt64() - _baseAddr.ToInt64();
            return IntPtr.Add(_baseAddr, (int) (delta + 1 + offsetAddressToInt));
        }

        public IntPtr GetInteractedNpcOffset()
        {
            var pattern = "\x43\x01\x84\x31\x00\x00\x00\x00";
            var mask = "xxxx????";
            var patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, 4);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0);
            return IntPtr.Add(_baseAddr, (int) (offsetAddressToInt + 0x1D4));
        }

        public IntPtr GetLastHoverObjectOffset()
        {
            var pattern = "\xC6\x84\xC2\x00\x00\x00\x00\x00\x48\x8B\x74\x24\x00";
            var mask = "xxx?????xxxx?";
            IntPtr patternAddress = FindPattern(pattern, mask);

            var offsetBuffer = new byte[4];
            var resultRelativeAddress = IntPtr.Add(patternAddress, 3);
            if (!WindowsExternal.ReadProcessMemory(_handle, resultRelativeAddress, offsetBuffer, sizeof(int), out _))
            {
                _log.Info($"Failed to find pattern {PatternToString(pattern)}");
                return IntPtr.Zero;
            }

            var offsetAddressToInt = BitConverter.ToInt32(offsetBuffer, 0) - 1;
            return IntPtr.Add(_baseAddr, offsetAddressToInt);
        }

        public byte[] GetProcessMemory()
        {
            var memoryBuffer = new byte[_moduleSize];
            if (WindowsExternal.ReadProcessMemory(_handle, _baseAddr, memoryBuffer, _moduleSize, out _) == false)
            {
                _log.Info("We failed to read the process memory");
                return null;
            }

            return memoryBuffer;
        }

        public T[] Read<T>(IntPtr address, int count) where T : struct
        {
            var sz = Marshal.SizeOf<T>();
            var buf = new byte[sz * count];
            var handle = GCHandle.Alloc(buf, GCHandleType.Pinned);
            try
            {
                WindowsExternal.ReadProcessMemory(_handle, address, buf, buf.Length, out _);
                var result = new T[count];
                for (var i = 0; i < count; i++)
                {
                    result[i] = (T) Marshal.PtrToStructure(handle.AddrOfPinnedObject() + (i * sz), typeof(T));
                }

                return result;
            }
            finally
            {
                handle.Free();
            }
        }

        public T Read<T>(IntPtr address) where T : struct => Read<T>(address, 1)[0];

        public IntPtr FindPattern(string pattern, string mask)
        {
            var buffer = GetProcessMemory();

            var patternLength = pattern.Length;
            for (var i = 0; i < _moduleSize - patternLength; i++)
            {
                var found = true;
                for (var j = 0; j < patternLength; j++)
                {
                    if (mask[j] != '?' && pattern[j] != buffer[i + j])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    return IntPtr.Add(_baseAddr, i);
                }
            }

            return IntPtr.Zero;
        }

        public string PatternToString(string pattern) => "\\x" + BitConverter.ToString(Encoding.Default.GetBytes(pattern)).Replace("-", "\\x");

        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (_handle != IntPtr.Zero)
                {
                    WindowsExternal.CloseHandle(_handle);
                }

                _handle = IntPtr.Zero;
            }
        }

        public void Dispose()
        {
            if (--OpenContextCount > 0)
            {
                return;
            }

            Dispose(true);
        }
    }
}