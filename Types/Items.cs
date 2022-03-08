namespace SharpStyx.Types
{
    public static class Items
    {
        public static readonly Dictionary<int, HashSet<string>> ItemUnitHashesSeen = new();
        public static readonly Dictionary<int, HashSet<uint>> ItemUnitIdsSeen = new();
        public static readonly Dictionary<int, HashSet<uint>> ItemUnitIdsToSkip = new();
        public static readonly Dictionary<int, HashSet<uint>> InventoryItemUnitIdsToSkip = new();
        public static readonly Dictionary<int, Dictionary<uint, Npc>> ItemVendors = new();

        public static void LogItem(UnitItem item, int processId)
        {
            if (item.IsInStore)
            {
                InventoryItemUnitIdsToSkip[processId].Add(item.UnitId);
            }
            else
            {
                ItemUnitHashesSeen[processId].Add(item.HashString);
            }

            if (item.IsPlayerOwned && item.IsIdentified)
            {
                InventoryItemUnitIdsToSkip[processId].Add(item.UnitId);
                ItemUnitIdsToSkip[processId].Add(item.UnitId);
            }

            ItemUnitIdsSeen[processId].Add(item.UnitId);
        }

        public static bool CheckInventoryItem(UnitItem item, int processId) =>
            item.IsIdentified && item.IsPlayerOwned && !item.IsInSocket &&
            !InventoryItemUnitIdsToSkip[processId].Contains(item.UnitId);

        public static bool CheckDroppedItem(UnitItem item, int processId) =>
            !item.IsIdentified && item.IsDropped &&
            !ItemUnitHashesSeen[processId].Contains(item.HashString) &&
            !ItemUnitIdsSeen[processId].Contains(item.UnitId) &&
            !ItemUnitIdsToSkip[processId].Contains(item.UnitId);

        public static bool CheckVendorItem(UnitItem item, int processId) =>
            item.IsIdentified && item.IsInStore &&
            !ItemUnitIdsSeen[processId].Contains(item.UnitId) &&
            !ItemUnitIdsToSkip[processId].Contains(item.UnitId);
        
        public static int GetItemStat(UnitItem item, Stats.Stat stat)
        {
            return item.Stats.TryGetValue(stat, out var statValue) ? statValue : 0;
        }

        public static int GetItemStatShifted(UnitItem item, Stats.Stat stat)
        {
            return item.Stats.TryGetValue(stat, out var statValue) && Stats.StatShifts.TryGetValue(stat, out var shift) ? statValue >> shift : 0;
        }

        public static double GetItemStatDecimal(UnitItem item, Stats.Stat stat)
        {
            return item.Stats.TryGetValue(stat, out var statValue) && Stats.StatDivisors.TryGetValue(stat, out var divisor) ? statValue / divisor : 0;
        }
    }
    
    [Flags]
    public enum ItemFlags : uint
    {
        IFLAG_NEWITEM = 0x00000001,
        IFLAG_TARGET = 0x00000002,
        IFLAG_TARGETING = 0x00000004,
        IFLAG_DELETED = 0x00000008,
        IFLAG_IDENTIFIED = 0x00000010,
        IFLAG_QUANTITY = 0x00000020,
        IFLAG_SWITCHIN = 0x00000040,
        IFLAG_SWITCHOUT = 0x00000080,
        IFLAG_BROKEN = 0x00000100,
        IFLAG_REPAIRED = 0x00000200,
        IFLAG_UNK1 = 0x00000400,
        IFLAG_SOCKETED = 0x00000800,
        IFLAG_NOSELL = 0x00001000,
        IFLAG_INSTORE = 0x00002000,
        IFLAG_NOEQUIP = 0x00004000,
        IFLAG_NAMED = 0x00008000,
        IFLAG_ISEAR = 0x00010000,
        IFLAG_STARTITEM = 0x00020000,
        IFLAG_UNK2 = 0x00040000,
        IFLAG_INIT = 0x00080000,
        IFLAG_UNK3 = 0x00100000,
        IFLAG_COMPACTSAVE = 0x00200000,
        IFLAG_ETHEREAL = 0x00400000,
        IFLAG_JUSTSAVED = 0x00800000,
        IFLAG_PERSONALIZED = 0x01000000,
        IFLAG_LOWQUALITY = 0x02000000,
        IFLAG_RUNEWORD = 0x04000000,
        IFLAG_ITEM = 0x08000000
    }

    public enum ItemQuality : uint
    {
        INFERIOR = 0x01, //0x01 Inferior
        NORMAL = 0x02, //0x02 Normal
        SUPERIOR = 0x03, //0x03 Superior
        MAGIC = 0x04, //0x04 Magic
        SET = 0x05, //0x05 Set
        RARE = 0x06, //0x06 Rare
        UNIQUE = 0x07, //0x07 Unique
        CRAFT = 0x08, //0x08 Crafted
        TEMPERED = 0x09 //0x09 Tempered
    }

    public enum InvPage : byte
    {
        INVENTORY = 0,
        EQUIP = 1,
        TRADE = 2,
        CUBE = 3,
        STASH = 4,
        BELT = 5,
        NULL = 255,
    }

    public enum StashType : byte
    {
        Body = 0,
        Personal = 1,
        Shared1 = 2,
        Shared2 = 3,
        Shared3 = 4,
        Belt = 5
    }

    public enum BodyLoc : byte
    {
        NONE, //Not Equipped
        HEAD, //Helm
        NECK, //Amulet
        TORSO, //Body Armor
        RARM, //Right-Hand
        LARM, //Left-Hand
        RRIN, //Right Ring
        LRIN, //Left Ring
        BELT, //Belt
        FEET, //Boots
        GLOVES, //Gloves
        SWRARM, //Right-Hand on Switch
        SWLARM //Left-Hand on Switch
    };

    public enum ItemMode : uint
    {
        STORED, //Item is in Storage (inventory, cube, Stash?)
        EQUIP, //Item is Equippped
        INBELT, //Item is in Belt Rows
        ONGROUND, //Item is on Ground
        ONCURSOR, //Item is on Cursor
        DROPPING, //Item is Being Dropped
        SOCKETED //Item is Socketed in another Item
    };

    public enum ItemModeMapped // Provides more detail over ItemMode
    {
        Player,
        Inventory,
        Belt,
        Cube,
        Stash,
        Vendor,
        Trade,
        Mercenary,
        Socket,
        Ground,
        Selected,
        Unknown
    };

    public enum Item : uint
    {
        HandAxe,
        Axe,
        DoubleAxe,
        MilitaryPick,
        WarAxe,
        LargeAxe,
        BroadAxe,
        BattleAxe,
        GreatAxe,
        GiantAxe,
        Wand,
        YewWand,
        BoneWand,
        GrimWand,
        Club,
        Scepter,
        GrandScepter,
        WarScepter,
        SpikedClub,
        Mace,
        MorningStar,
        Flail,
        WarHammer,
        Maul,
        GreatMaul,
        ShortSword,
        Scimitar,
        Sabre,
        Falchion,
        CrystalSword,
        BroadSword,
        LongSword,
        WarSword,
        TwoHandedSword,
        Claymore,
        GiantSword,
        BastardSword,
        Flamberge,
        GreatSword,
        Dagger,
        Dirk,
        Kris,
        Blade,
        ThrowingKnife,
        ThrowingAxe,
        BalancedKnife,
        BalancedAxe,
        Javelin,
        Pilum,
        ShortSpear,
        Glaive,
        ThrowingSpear,
        Spear,
        Trident,
        Brandistock,
        Spetum,
        Pike,
        Bardiche,
        Voulge,
        Scythe,
        Poleaxe,
        Halberd,
        WarScythe,
        ShortStaff,
        LongStaff,
        GnarledStaff,
        BattleStaff,
        WarStaff,
        ShortBow,
        HuntersBow,
        LongBow,
        CompositeBow,
        ShortBattleBow,
        LongBattleBow,
        ShortWarBow,
        LongWarBow,
        LightCrossbow,
        Crossbow,
        HeavyCrossbow,
        RepeatingCrossbow,
        RancidGasPotion,
        OilPotion,
        ChokingGasPotion,
        ExplodingPotion,
        StranglingGasPotion,
        FulminatingPotion,
        DecoyGidbinn,
        TheGidbinn,
        WirtsLeg,
        HoradricMalus,
        HellforgeHammer,
        HoradricStaff,
        StaffOfKings,
        Hatchet,
        Cleaver,
        TwinAxe,
        Crowbill,
        Naga,
        MilitaryAxe,
        BeardedAxe,
        Tabar,
        GothicAxe,
        AncientAxe,
        BurntWand,
        PetrifiedWand,
        TombWand,
        GraveWand,
        Cudgel,
        RuneScepter,
        HolyWaterSprinkler,
        DivineScepter,
        BarbedClub,
        FlangedMace,
        JaggedStar,
        Knout,
        BattleHammer,
        WarClub,
        MartelDeFer,
        Gladius,
        Cutlass,
        Shamshir,
        Tulwar,
        DimensionalBlade,
        BattleSword,
        RuneSword,
        AncientSword,
        Espandon,
        DacianFalx,
        TuskSword,
        GothicSword,
        Zweihander,
        ExecutionerSword,
        Poignard,
        Rondel,
        Cinquedeas,
        Stiletto,
        BattleDart,
        Francisca,
        WarDart,
        Hurlbat,
        WarJavelin,
        GreatPilum,
        Simbilan,
        Spiculum,
        Harpoon,
        WarSpear,
        Fuscina,
        WarFork,
        Yari,
        Lance,
        LochaberAxe,
        Bill,
        BattleScythe,
        Partizan,
        BecDeCorbin,
        GrimScythe,
        JoStaff,
        QuarterStaff,
        CedarStaff,
        GothicStaff,
        RuneStaff,
        EdgeBow,
        RazorBow,
        CedarBow,
        DoubleBow,
        ShortSiegeBow,
        LargeSiegeBow,
        RuneBow,
        GothicBow,
        Arbalest,
        SiegeCrossbow,
        Ballista,
        ChuKoNu,
        KhalimsFlail,
        KhalimsWill,
        Katar,
        WristBlade,
        HatchetHands,
        Cestus,
        Claws,
        BladeTalons,
        ScissorsKatar,
        Quhab,
        WristSpike,
        Fascia,
        HandScythe,
        GreaterClaws,
        GreaterTalons,
        ScissorsQuhab,
        Suwayyah,
        WristSword,
        WarFist,
        BattleCestus,
        FeralClaws,
        RunicTalons,
        ScissorsSuwayyah,
        Tomahawk,
        SmallCrescent,
        EttinAxe,
        WarSpike,
        BerserkerAxe,
        FeralAxe,
        SilverEdgedAxe,
        Decapitator,
        ChampionAxe,
        GloriousAxe,
        PolishedWand,
        GhostWand,
        LichWand,
        UnearthedWand,
        Truncheon,
        MightyScepter,
        SeraphRod,
        Caduceus,
        TyrantClub,
        ReinforcedMace,
        DevilStar,
        Scourge,
        LegendaryMallet,
        OgreMaul,
        ThunderMaul,
        Falcata,
        Ataghan,
        ElegantBlade,
        HydraEdge,
        PhaseBlade,
        ConquestSword,
        CrypticSword,
        MythicalSword,
        LegendSword,
        HighlandBlade,
        BalrogBlade,
        ChampionSword,
        ColossusSword,
        ColossusBlade,
        BoneKnife,
        MithrilPoint,
        FangedKnife,
        LegendSpike,
        FlyingKnife,
        FlyingAxe,
        WingedKnife,
        WingedAxe,
        HyperionJavelin,
        StygianPilum,
        BalrogSpear,
        GhostGlaive,
        WingedHarpoon,
        HyperionSpear,
        StygianPike,
        Mancatcher,
        GhostSpear,
        WarPike,
        OgreAxe,
        ColossusVoulge,
        Thresher,
        CrypticAxe,
        GreatPoleaxe,
        GiantThresher,
        WalkingStick,
        Stalagmite,
        ElderStaff,
        Shillelagh,
        ArchonStaff,
        SpiderBow,
        BladeBow,
        ShadowBow,
        GreatBow,
        DiamondBow,
        CrusaderBow,
        WardBow,
        HydraBow,
        PelletBow,
        GorgonCrossbow,
        ColossusCrossbow,
        DemonCrossBow,
        EagleOrb,
        SacredGlobe,
        SmokedSphere,
        ClaspedOrb,
        JaredsStone,
        StagBow,
        ReflexBow,
        MaidenSpear,
        MaidenPike,
        MaidenJavelin,
        GlowingOrb,
        CrystallineGlobe,
        CloudySphere,
        SparklingBall,
        SwirlingCrystal,
        AshwoodBow,
        CeremonialBow,
        CeremonialSpear,
        CeremonialPike,
        CeremonialJavelin,
        HeavenlyStone,
        EldritchOrb,
        DemonHeart,
        VortexOrb,
        DimensionalShard,
        MatriarchalBow,
        GrandMatronBow,
        MatriarchalSpear,
        MatriarchalPike,
        MatriarchalJavelin,
        Cap,
        SkullCap,
        Helm,
        FullHelm,
        GreatHelm,
        Crown,
        Mask,
        QuiltedArmor,
        LeatherArmor,
        HardLeatherArmor,
        StuddedLeather,
        RingMail,
        ScaleMail,
        ChainMail,
        BreastPlate,
        SplintMail,
        PlateMail,
        FieldPlate,
        GothicPlate,
        FullPlateMail,
        AncientArmor,
        LightPlate,
        Buckler,
        SmallShield,
        LargeShield,
        KiteShield,
        TowerShield,
        GothicShield,
        LeatherGloves,
        HeavyGloves,
        ChainGloves,
        LightGauntlets,
        Gauntlets,
        Boots,
        HeavyBoots,
        ChainBoots,
        LightPlatedBoots,
        Greaves,
        Sash,
        LightBelt,
        Belt,
        HeavyBelt,
        PlatedBelt,
        BoneHelm,
        BoneShield,
        SpikedShield,
        WarHat,
        Sallet,
        Casque,
        Basinet,
        WingedHelm,
        GrandCrown,
        DeathMask,
        GhostArmor,
        SerpentskinArmor,
        DemonhideArmor,
        TrellisedArmor,
        LinkedMail,
        TigulatedMail,
        MeshArmor,
        Cuirass,
        RussetArmor,
        TemplarCoat,
        SharktoothArmor,
        EmbossedPlate,
        ChaosArmor,
        OrnatePlate,
        MagePlate,
        Defender,
        RoundShield,
        Scutum,
        DragonShield,
        Pavise,
        AncientShield,
        DemonhideGloves,
        SharkskinGloves,
        HeavyBracers,
        BattleGauntlets,
        WarGauntlets,
        DemonhideBoots,
        SharkskinBoots,
        MeshBoots,
        BattleBoots,
        WarBoots,
        DemonhideSash,
        SharkskinBelt,
        MeshBelt,
        BattleBelt,
        WarBelt,
        GrimHelm,
        GrimShield,
        BarbedShield,
        WolfHead,
        HawkHelm,
        Antlers,
        FalconMask,
        SpiritMask,
        JawboneCap,
        FangedHelm,
        HornedHelm,
        AssaultHelmet,
        AvengerGuard,
        Targe,
        Rondache,
        HeraldicShield,
        AerinShield,
        CrownShield,
        PreservedHead,
        ZombieHead,
        UnravellerHead,
        GargoyleHead,
        DemonHeadShield,
        Circlet,
        Coronet,
        Tiara,
        Diadem,
        Shako,
        Hydraskull,
        Armet,
        GiantConch,
        SpiredHelm,
        Corona,
        DemonHead,
        DuskShroud,
        Wyrmhide,
        ScarabHusk,
        WireFleece,
        DiamondMail,
        LoricatedMail,
        Boneweave,
        GreatHauberk,
        BalrogSkin,
        HellforgePlate,
        KrakenShell,
        LacqueredPlate,
        ShadowPlate,
        SacredArmor,
        ArchonPlate,
        Heater,
        Luna,
        Hyperion,
        Monarch,
        Aegis,
        Ward,
        BrambleMitts,
        VampireboneGloves,
        Vambraces,
        CrusaderGauntlets,
        OgreGauntlets,
        WyrmhideBoots,
        ScarabshellBoots,
        BoneweaveBoots,
        MirroredBoots,
        MyrmidonGreaves,
        SpiderwebSash,
        VampirefangBelt,
        MithrilCoil,
        TrollBelt,
        ColossusGirdle,
        BoneVisage,
        TrollNest,
        BladeBarrier,
        AlphaHelm,
        GriffonHeaddress,
        HuntersGuise,
        SacredFeathers,
        TotemicMask,
        JawboneVisor,
        LionHelm,
        RageMask,
        SavageHelmet,
        SlayerGuard,
        AkaranTarge,
        AkaranRondache,
        ProtectorShield,
        GildedShield,
        RoyalShield,
        MummifiedTrophy,
        FetishTrophy,
        SextonTrophy,
        CantorTrophy,
        HierophantTrophy,
        BloodSpirit,
        SunSpirit,
        EarthSpirit,
        SkySpirit,
        DreamSpirit,
        CarnageHelm,
        FuryVisor,
        DestroyerHelm,
        ConquerorCrown,
        GuardianCrown,
        SacredTarge,
        SacredRondache,
        KurastShield,
        ZakarumShield,
        VortexShield,
        MinionSkull,
        HellspawnSkull,
        OverseerSkull,
        SuccubusSkull,
        BloodlordSkull,
        Elixir,
        INVALID509,
        INVALID510,
        INVALID511,
        INVALID512,
        StaminaPotion,
        AntidotePotion,
        RejuvenationPotion,
        FullRejuvenationPotion,
        ThawingPotion,
        TomeOfTownPortal,
        TomeOfIdentify,
        Amulet,
        AmuletOfTheViper,
        Ring,
        Gold,
        ScrollOfInifuss,
        KeyToTheCairnStones,
        Arrows,
        Torch,
        Bolts,
        ScrollOfTownPortal,
        ScrollOfIdentify,
        Heart,
        Brain,
        Jawbone,
        Eye,
        Horn,
        Tail,
        Flag,
        Fang,
        Quill,
        Soul,
        Scalp,
        Spleen,
        Key,
        TheBlackTowerKey,
        PotionOfLife,
        AJadeFigurine,
        TheGoldenBird,
        LamEsensTome,
        HoradricCube,
        HoradricScroll,
        MephistosSoulstone,
        BookOfSkill,
        KhalimsEye,
        KhalimsHeart,
        KhalimsBrain,
        Ear,
        ChippedAmethyst,
        FlawedAmethyst,
        Amethyst,
        FlawlessAmethyst,
        PerfectAmethyst,
        ChippedTopaz,
        FlawedTopaz,
        Topaz,
        FlawlessTopaz,
        PerfectTopaz,
        ChippedSapphire,
        FlawedSapphire,
        Sapphire,
        FlawlessSapphire,
        PerfectSapphire,
        ChippedEmerald,
        FlawedEmerald,
        Emerald,
        FlawlessEmerald,
        PerfectEmerald,
        ChippedRuby,
        FlawedRuby,
        Ruby,
        FlawlessRuby,
        PerfectRuby,
        ChippedDiamond,
        FlawedDiamond,
        Diamond,
        FlawlessDiamond,
        PerfectDiamond,
        MinorHealingPotion,
        LightHealingPotion,
        HealingPotion,
        GreaterHealingPotion,
        SuperHealingPotion,
        MinorManaPotion,
        LightManaPotion,
        ManaPotion,
        GreaterManaPotion,
        SuperManaPotion,
        ChippedSkull,
        FlawedSkull,
        Skull,
        FlawlessSkull,
        PerfectSkull,
        Herb,
        SmallCharm,
        LargeCharm,
        GrandCharm,
        INVALID606,
        INVALID607,
        INVALID608,
        INVALID609,
        ElRune,
        EldRune,
        TirRune,
        NefRune,
        EthRune,
        IthRune,
        TalRune,
        RalRune,
        OrtRune,
        ThulRune,
        AmnRune,
        SolRune,
        ShaelRune,
        DolRune,
        HelRune,
        IoRune,
        LumRune,
        KoRune,
        FalRune,
        LemRune,
        PulRune,
        UmRune,
        MalRune,
        IstRune,
        GulRune,
        VexRune,
        OhmRune,
        LoRune,
        SurRune,
        BerRune,
        JahRune,
        ChamRune,
        ZodRune,
        Jewel,
        MalahsPotion,
        ScrollOfKnowledge,
        ScrollOfResistance,
        KeyOfTerror,
        KeyOfHate,
        KeyOfDestruction,
        DiablosHorn,
        BaalsEye,
        MephistosBrain,
        TokenofAbsolution,
        TwistedEssenceOfSuffering,
        ChargedEssenceOfHatred,
        BurningEssenceOfTerror,
        FesteringEssenceOfDestruction,
        StandardOfHeroes,

        // Used only for item filter
        ClassAxes = 0xFFD0,

        ClassWands,
        ClassClubs,
        ClassScepters,
        ClassMaces,
        ClassHammers,
        ClassSwords,
        ClassDaggers,
        ClassThrowingKnifes,
        ClassThrowingAxes,
        ClassJavelins,
        ClassSpears,
        ClassPolearms,
        ClassStaves,
        ClassBows,
        ClassCrossbows,

        ClassHelms,
        ClassArmors,
        ClassShields,
        ClassGloves,
        ClassBoots,
        ClassBelts,
        ClassCirclets,

        ClassAssassinKatars,
        ClassSorceressOrbs,
        ClassAmazonBows,
        ClassAmazonSpears,
        ClassAmazonJavelins,
        ClassDruidHelms,
        ClassBarbarianHelms,
        ClassPaladinShields,
        ClassNecromancerShields,

        xBases = 0xFFFF - 0x1,
        Any = 0xFFFF
    };

    public enum ItemTier
    {
        Normal,
        Exceptional,
        Elite,
        NotApplicable
    }
}
