namespace SharpStyx.Types
{
    public class AdjacentLevel
    {
        public Area Area;
        public Point[] Exits;
        public bool IsPortal;
    }

    public class AreaData
    {
        public Area Area;
        public Point Origin;
        public int MapPadding = 0;
        public Dictionary<Area, AdjacentLevel> AdjacentLevels;
        public Dictionary<Area, AreaData> AdjacentAreas = new Dictionary<Area, AreaData>();
        public int[][] CollisionGrid;
        public Rectangle ViewInputRect;
        public Rectangle ViewOutputRect;
        public Dictionary<Npc, Point[]> NPCs;
        public Dictionary<GameObject, Point[]> Objects;
    }

    class AreaLabel
    {
        public string Text;
        public int[] Level;
    }
}
