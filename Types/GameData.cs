using SharpStyx.Structs;

namespace SharpStyx.Types
{
    public class GameData
    {
        public Point PlayerPosition;
        public uint MapSeed;
        public Difficulty Difficulty;
        public Area Area;
        public IntPtr MainWindowHandle;
        public string PlayerName;
        public UnitPlayer PlayerUnit;
        public Dictionary<uint, UnitPlayer> Players;
        public UnitPlayer[] Corpses;
        public UnitMonster[] Monsters;
        public UnitMonster[] Mercs;
        public UnitObject[] Objects;
        public UnitItem[] Items;
        public UnitItem[] AllItems;
        public ItemLogEntry[] ItemLog;
        public Session Session;
        public Roster Roster;
        public byte MenuPanelOpen;
        public MenuData MenuOpen;
        public Npc LastNpcInteracted;
        public int ProcessId;

        public override string ToString()
        {
            return $"{nameof(PlayerPosition)}: {PlayerPosition}, {nameof(MapSeed)}: {MapSeed}, {nameof(Difficulty)}: {Difficulty}, {nameof(Area)}: {Area}, {nameof(MenuOpen.Map)}: {MenuOpen.Map}";
        }
    }
}
