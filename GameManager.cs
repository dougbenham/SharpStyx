using System.Diagnostics;
using SharpStyx.Structs;

namespace SharpStyx
{
    public class GameManager
    {
        private static IntPtr _winHook;
        private static int _foregroundProcessId = 0;

        private static IntPtr _lastGameHwnd = IntPtr.Zero;
        private static Process _lastGameProcess;
        private static int _lastGameProcessId = 0;
        private static ProcessContext _processContext;

        public delegate void StatusUpdateHandler(object sender, EventArgs e);

        public static event StatusUpdateHandler OnGameAccessDenied;

        private static IntPtr _UnitHashTableOffset;
        private static IntPtr _ExpansionCheckOffset;
        private static IntPtr _GameNameOffset;
        private static IntPtr _MenuPanelOpenOffset;
        private static IntPtr _MenuDataOffset;
        private static IntPtr _RosterDataOffset;
        private static IntPtr _InteractedNpcOffset;
        private static IntPtr _LastHoverDataOffset;

        private static WindowsExternal.WinEventDelegate _eventDelegate = null;
        
        public static ProcessContext GetProcessContext()
        {
            if (_processContext != null && _processContext.OpenContextCount > 0)
            {
                _processContext.OpenContextCount += 1;
                return _processContext;
            }
            else if (_lastGameProcess != null && WindowsExternal.HandleExists(_lastGameHwnd))
            {
                _processContext = new ProcessContext(_lastGameProcess); // Rarely, the VirtualMemoryRead will cause an error, in that case return a null instead of a runtime error. The next frame will try again.
                return _processContext;
            }

            return null;
        }
        
        public static UnitHashTable UnitHashTable(int offset = 0)
        {
            using (var processContext = GetProcessContext())
            {
                if (_UnitHashTableOffset == IntPtr.Zero)
                {
                    _UnitHashTableOffset = processContext.GetUnitHashtableOffset();
                }

                return processContext.Read<UnitHashTable>(IntPtr.Add(_UnitHashTableOffset, offset));
            }
        }

        public static IntPtr ExpansionCheckOffset
        {
            get
            {
                if (_ExpansionCheckOffset != IntPtr.Zero)
                {
                    return _ExpansionCheckOffset;
                }

                using (var processContext = GetProcessContext())
                {
                    _ExpansionCheckOffset = processContext.GetExpansionOffset();
                }

                return _ExpansionCheckOffset;
            }
        }

        public static IntPtr GameNameOffset
        {
            get
            {
                if (_GameNameOffset != IntPtr.Zero)
                {
                    return _GameNameOffset;
                }

                using (var processContext = GetProcessContext())
                {
                    _GameNameOffset = (IntPtr)processContext.GetGameNameOffset();
                }

                return _GameNameOffset;
            }
        }

        public static IntPtr MenuOpenOffset
        {
            get
            {
                if (_MenuPanelOpenOffset != IntPtr.Zero)
                {
                    return _MenuPanelOpenOffset;
                }

                using (var processContext = GetProcessContext())
                {
                    _MenuPanelOpenOffset = (IntPtr)processContext.GetMenuOpenOffset();
                }

                return _MenuPanelOpenOffset;
            }
        }

        public static IntPtr MenuDataOffset
        {
            get
            {
                if (_MenuDataOffset != IntPtr.Zero)
                {
                    return _MenuDataOffset;
                }

                using (var processContext = GetProcessContext())
                {
                    _MenuDataOffset = (IntPtr)processContext.GetMenuDataOffset();
                }

                return _MenuDataOffset;
            }
        }

        public static IntPtr RosterDataOffset
        {
            get
            {
                if (_RosterDataOffset != IntPtr.Zero)
                {
                    return _RosterDataOffset;
                }

                using (var processContext = GetProcessContext())
                {
                    _RosterDataOffset = processContext.GetRosterDataOffset();
                }

                return _RosterDataOffset;
            }
        }

        public static IntPtr LastHoverDataOffset
        {
            get
            {
                if (_LastHoverDataOffset != IntPtr.Zero)
                {
                    return _LastHoverDataOffset;
                }

                using (var processContext = GetProcessContext())
                {
                    _LastHoverDataOffset = processContext.GetLastHoverObjectOffset();
                }

                return _LastHoverDataOffset;
            }
        }

        public static IntPtr InteractedNpcOffset
        {
            get
            {
                if (_InteractedNpcOffset != IntPtr.Zero)
                {
                    return _InteractedNpcOffset;
                }

                using (var processContext = GetProcessContext())
                {
                    _InteractedNpcOffset = processContext.GetInteractedNpcOffset();
                }

                return _InteractedNpcOffset;
            }
        }

        public static void Dispose()
        {
            if (_lastGameProcess != null)
            {
                _lastGameProcess.Dispose();
            }
            WindowsExternal.UnhookWinEvent(_winHook);
        }
    }
}
