using SharpStyx.Types;

namespace SharpStyx
{
    public static class GameMemory
    {
        private static readonly Dictionary<int, bool> _playerMapChanged = new();
        private static readonly Dictionary<int, uint> _playerCubeOwnerID = new();
        private static readonly Dictionary<int, Session> _sessions = new();
        private static int _currentProcessId;

        public static Dictionary<int, UnitPlayer> PlayerUnits = new();
        public static Dictionary<object, object> cache = new();
        
        public static GameData GetGameData()
        {
            var rawPlayerUnits = GetUnits<UnitPlayer>(UnitType.Player).Select(x => x.Update()).Where(x => x != null).ToArray();
            var playerUnit = rawPlayerUnits.FirstOrDefault(x => x.IsPlayer && x.IsPlayerUnit);

            if (playerUnit == null)
                throw new("Player unit not found.");

            // Players
            var playerList = rawPlayerUnits.Where(x => x.UnitType == UnitType.Player && x.IsPlayer && x.UnitId < uint.MaxValue).ToDictionary(x => x.UnitId, x => x);
            
            // Monsters
            var rawMonsterUnits = GetUnits<UnitMonster>(UnitType.Monster)
                .Select(x => x.Update()).ToArray()
                .Where(x => x != null && x.UnitId < uint.MaxValue).ToArray();
            
            var mercList = rawMonsterUnits.Where(x => x.UnitType == UnitType.Monster && x.IsMerc).ToArray();
            
            // Items
            var allItems = GetUnits<UnitItem>(UnitType.Item, true).Where(x => x.UnitId < uint.MaxValue).ToArray();
            var rawItemUnits = new List<UnitItem>();
            foreach (var item in allItems)
            {
                if (item.IsPlayerOwned && item.IsIdentified && !Items.InventoryItemUnitIdsToSkip[_currentProcessId].Contains(item.UnitId))
                {
                    item.IsCached = false;
                }

                var checkInventoryItem = Items.CheckInventoryItem(item, _currentProcessId);

                item.Update();

                cache[item.UnitId] = item;

                if (item.ItemModeMapped == ItemModeMapped.Ground)
                {
                    cache[item.HashString] = item;
                }

                if (Items.ItemUnitIdsToSkip[_currentProcessId].Contains(item.UnitId))
                    continue;

                if (_playerMapChanged[_currentProcessId] && item.IsAnyPlayerHolding && item.Item != Item.HoradricCube && !Items.ItemUnitIdsToSkip[_currentProcessId].Contains(item.UnitId))
                {
                    Items.ItemUnitIdsToSkip[_currentProcessId].Add(item.UnitId);
                    continue;
                }

                if (item.UnitId == uint.MaxValue)
                    continue;

                item.IsPlayerOwned = _playerCubeOwnerID[_currentProcessId] != uint.MaxValue && item.ItemData.dwOwnerID == _playerCubeOwnerID[_currentProcessId];

                if (item.IsInStore)
                {
                }

                var checkDroppedItem = Items.CheckDroppedItem(item, _currentProcessId);
                var checkVendorItem = Items.CheckVendorItem(item, _currentProcessId);
                if (item.IsValidItem && (checkDroppedItem || checkVendorItem || checkInventoryItem))
                {
                    Items.LogItem(item, _currentProcessId);
                }

                if (item.Item == Item.HoradricCube)
                {
                    Items.ItemUnitIdsToSkip[_currentProcessId].Add(item.UnitId);
                }

                rawItemUnits.Add(item);
            }

            var itemList = Items.ItemLog[_currentProcessId].Select(item =>
            {
                if (cache.TryGetValue(item.ItemHashString, out var cachedItem) && ((UnitItem) cachedItem).HashString == item.ItemHashString)
                {
                    item.UnitItem = (UnitItem) cachedItem;
                }

                if (item.UnitItem.DistanceTo(playerUnit) <= 40 && !rawItemUnits.Contains(item.UnitItem)) // Player is close to the item position but it was not found
                {
                    item.UnitItem.MarkInvalid();
                }

                return item.UnitItem;
            }).Where(x => x != null).ToArray();

            // Set Cube Owner
            if (_playerMapChanged[_currentProcessId])
            {
                var cube = allItems.FirstOrDefault(x => x.Item == Item.HoradricCube);
                if (cube != null)
                {
                    _playerCubeOwnerID[_currentProcessId] = cube.ItemData.dwOwnerID;
                }
            }

            // Belt items
            var belt = allItems.FirstOrDefault(x => x.ItemModeMapped == ItemModeMapped.Player && x.ItemData.BodyLoc == BodyLoc.BELT);
            var beltItems = allItems.Where(x => x.ItemModeMapped == ItemModeMapped.Belt).ToArray();

            var beltSize = belt == null ? 1 :
                new Item[] { Item.Sash, Item.LightBelt }.Contains(belt.Item) ? 2 :
                new Item[] { Item.Belt, Item.HeavyBelt }.Contains(belt.Item) ? 3 : 4;

            playerUnit.BeltItems = Enumerable.Range(0, 4).Select(i => Enumerable.Range(0, beltSize).Select(j => beltItems.FirstOrDefault(item => item.X == i + j * 4)).ToArray()).ToArray();
            
            return new()
            {
                PlayerPosition = playerUnit.Position,
                PlayerName = playerUnit.Name,
                PlayerUnit = playerUnit,
                Players = playerList,
                Mercs = mercList,
                Items = itemList,
                AllItems = allItems,
                ItemLog = Items.ItemLog[_currentProcessId].ToArray(),
                Session = _sessions[_currentProcessId],
                ProcessId = _currentProcessId
            };
        }

        public static UnitPlayer PlayerUnit => PlayerUnits.TryGetValue(_currentProcessId, out var player) ? player : null;

        private static T[] GetUnits<T>(UnitType unitType, bool saveToCache = false) where T : UnitAny
        {
            var allUnits = new Dictionary<uint, T>();
            Func<IntPtr, T> CreateUnit = (ptr) => (T) Activator.CreateInstance(typeof(T), new object[] { ptr });

            var unitHashTable = GameManager.UnitHashTable(128 * 8 * (int) unitType);

            foreach (var ptrUnit in unitHashTable.UnitTable)
            {
                if (ptrUnit == IntPtr.Zero)
                    continue;

                var unit = CreateUnit(ptrUnit);

                Action<object> UseCachedUnit = (seenUnit) =>
                {
                    var castedSeenUnit = (T) seenUnit;
                    castedSeenUnit.CopyFrom(unit);

                    allUnits[castedSeenUnit.UnitId] = castedSeenUnit;
                };

                do
                {
                    if (saveToCache && cache.TryGetValue(unit.UnitId, out var seenUnit1) && seenUnit1 is T && !allUnits.ContainsKey(((T) seenUnit1).UnitId))
                    {
                        UseCachedUnit(seenUnit1);
                    }
                    else if (unit.IsValidUnit && !allUnits.ContainsKey(unit.UnitId))
                    {
                        allUnits[unit.UnitId] = unit;

                        if (saveToCache)
                        {
                            cache[unit.UnitId] = unit;
                        }
                    }
                } while (unit.Struct.pListNext != IntPtr.Zero && (unit = CreateUnit(unit.Struct.pListNext)).IsValidUnit);
            }

            return allUnits.Values.ToArray();
        }

        private static void UpdateMemoryData()
        {
            if (!Items.ItemUnitHashesSeen.ContainsKey(_currentProcessId))
            {
                Items.ItemUnitHashesSeen.Add(_currentProcessId, new());
                Items.ItemUnitIdsSeen.Add(_currentProcessId, new());
                Items.ItemUnitIdsToSkip.Add(_currentProcessId, new());
                Items.InventoryItemUnitIdsToSkip.Add(_currentProcessId, new());
                Items.ItemVendors.Add(_currentProcessId, new());
                Items.ItemLog.Add(_currentProcessId, new List<ItemLogEntry>());
            }
            else
            {
                Items.ItemUnitHashesSeen[_currentProcessId].Clear();
                Items.ItemUnitIdsSeen[_currentProcessId].Clear();
                Items.ItemUnitIdsToSkip[_currentProcessId].Clear();
                Items.InventoryItemUnitIdsToSkip[_currentProcessId].Clear();
                Items.ItemVendors[_currentProcessId].Clear();
                Items.ItemLog[_currentProcessId].Clear();
            }

            if (!Corpses.ContainsKey(_currentProcessId))
            {
                Corpses.Add(_currentProcessId, new());
            }
            else
            {
                Corpses[_currentProcessId].Clear();
            }
        }
    }
}