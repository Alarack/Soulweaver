﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardIDs {

    public enum CardID {
        None = 0,
        TestPlayer = 1,


        //Test Cards
        Shock_100 = 100,
        Awe_101 = 101,
        FlameSprite_102 = 102,
        PainLover_103 = 103,
        Blargen_104 = 104,
        FireLover_105 = 105,
        ATKBuffTotem_106 = 106,
        SparkSpawner_107 =107,
        SparkSpawn_108 = 108,
        AngrySkeletons_109 = 109,
        Domain_TestDomain = 110,
        TestSubject1 = 111,
        TestSubject2 = 112,
        AngryFist = 113,
        TestSpell = 114,

        //Tokens
        FusedSoul_1000 = 1000,
        

        //Cult of Rillock 10000
        COR_DesolateWaste = 10000,
        COR_DOM_EssenceBurn = 10001,
        COR_DOM_Remnants = 10002,
        COR_DOM_GraveKindling = 10003,
        COR_TNK_Ashling = 10004,
        COR_TNK_Ember = 10005,
        COR_DOM_RapidNecromancy = 10006,
        COR_DOM_Boneyard = 10007,
        COR_DOM_Reanimate = 10008,
        COR_TKN_Bonegolem = 10009,
        COR_DOM_BlazeGoRound = 10010,
        COR_DOM_RecklessBlaze = 10011,
        COR_DOM_Churn = 10012,
        COR_Kindler = 10013,
        COR_CorpseYard = 10014,
        COR_TKN_Zonbie = 10015,
        COR_RillocksBargain = 10016,
        COR_RillocksAvatar = 10017,
        COR_SoulSear = 10018,
        COR_Effigy = 10019,
        COR_BlackPhoenix = 10020,
        COR_SearFlesh = 10021,
        COR_Pyromaster = 10022,
        COR_FuelForTheFire = 10023,
        COR_TKN_Flameling = 10024,
        COR_CharredLord = 10025,
        COR_Conflagration = 10026,
        COR_Incinerate = 10027,
        COR_SpitefulGravestone = 10028,
        COR_AshesToFlames = 10029,
        COR_BurntOffering = 10030,
        COR_RestlessDead = 10031,
        COR_TKN_VengefulRevenant = 10032,
        COR_BlazingDead = 10033,
        COR_CacklingFireSpinner = 10034,
        COR_TKN_FirePoi = 10035,
        COR_RecklessStrike = 10036,
        COR_PyroBiologist = 10037,
        COR_AutoImmolation = 10038,
        COR_IgnitionBracers = 10039,
        COR_UnstablePyromancer = 10040,
        COR_BoomFly = 10041,
        COR_EndlessHorde = 10042,
        COR_BurningRemnants = 10043,
        COR_Flamecaller = 10044,
        COR_WallOfCorpses = 10045,
        COR_ZombieHorde = 10046,




        //The Severed 20000
        SEV_SoulTorturer = 20000,
        SEV_SubverterOfIntentions = 20001,
        SEV_GrishamTwistedReveler = 20002,
        SEV_VicariousDevourer = 20003,
        SEV_NashkaThirstyAbom = 20004,
        SEV_SpitefulSkulker = 20005,
        SEV_TwistVisage = 20006,
        SEV_TKN_CorruptedSoul = 20007,
        SEV_AnguishedGiant = 20008,
        SEV_TormentedButcher = 20009,
        SEV_CagedSpecimen = 20010,
        SEV_PainChanneler = 20011,
        SEV_DOM_SadisticGlee = 20012,
        SEV_DOM_CollectSpecimen = 20013,
        SEV_DOM_Agonize = 20014,
        SEV_TNK_CapturedVictim = 20015,
        SEV_DOM_GluttonousFeasting = 20016,
        SEV_DOM_Wasting = 20017,

        //The Realm Keepers 30000
        RLM_DrillmanGorman = 30000,
        RLM_TKN_Firebolt = 30001,
        RLM_DOM_Efficiency = 30002,
        RLM_DOM_Spellbook = 30003,
        RLM_TKN_Renew = 30004,
        RLM_TKN_StoneGolem = 30005,
        RLM_TKN_SecondWind = 30006,
        RLM_TKN_HardlightGolem = 30007,
        RLM_TKN_BigGirl = 30008,
        RLM_TKN_Trainee = 30009,
        RLM_TKN_Hoplite = 30010,
        RLM_TKN_WarpedLightBaracade = 30011,
        RLM_DrillmanGormen = 30012,
        RLM_LightLanceCavalry = 30013,
        RLM_GateGuardianCarth = 30014,
        RLM_GateGuardian = 30015,
        RLM_HardlightLongBowman = 30016,
        RLM_HardlightRifleman = 30017,
        RLM_HopliteGolem = 30018,
        RLM_Defender = 30019,
        RLM_HardlightCavalry = 30020,
        RLM_TemporalKnight = 30021,
        RLM_Undying = 30022,
        RLM_ShaperSavantYorvthan = 30023,
        RLM_WarShaperKairna = 30024,
        RLM_MendingShaper = 30025,
        RLM_Shaper = 30026,
        RLM_InaneShaper = 30027,
        RLM_ShaperSapper = 30028,
        RLM_HelpfulShaper = 30029,
        RLM_RoktharTheGuardiansBastion = 30030,
        RLM_OrthaneTheLivingFortress = 30031,
        RLM_HardlightWall = 30032,
        RLM_HardLightAshlarWall = 30033,
        RLM_TowerSheildofLight = 30034,
        RLM_CombatGlove = 30035,
        RLM_SuppliesRunner = 30036,
        RLM_LuminouseWings = 30037,
        RLM_ArchersOnTheWall = 30038,
        RLM_HardlightParapet = 30039,
        RLM_SpikedFortifications = 30040,
        RLM_TKN_PhantasmalSoldier = 30041,
        RLM_TKN_IllusionaryWall = 30042,
        RLM_TKN_LightSprite = 30043,
        RLM_TKN_LesserLight = 30044,
        RLM_MorthonTheLoreKeeper = 30045,
        RLM_TowerOfTheArcgmage = 30046,
        RLM_Obscuro = 30047,
        RLM_SignalTower = 30048,
        RLM_LightLinnedCrankgun = 30049,
        RLM_FragileLightBender = 30050,
        RLM_DazzlingDestroyer = 30051,
        RLM_PrismaticBoltThrower = 30052,
        RLM_TheKeeper = 30053,
        RLM_LoreSeekerKeen = 30054,
        RLM_Seeker = 30055,
        RLM_LightBearer = 30056,
        RLM_LightRider = 30057,
        RLM_PhantasmalWeaver = 30058,
        RLM_ShiningKnight = 30059,
        RLM_OverchargedElemental = 30060,
        RLM_UndyingLight = 30061,
        RLM_ShiningValkryie = 30062,
        RLM_RadiantSoldier = 30063,
        RLM_BrightDregs = 30064,
        RLM_LuminouseElemental = 30065,
        RLM_SpacialFractureCannon = 30066,
        RLM_TheForgeOfLight = 30067,
        RLM_EndlessLibrary = 30068,
        RLM_Mirage = 30069,
        RLM_Volley = 30070,
        RLM_UnstopableLight = 30071,
        RLM_TomeOfKnowledge = 30072,
        RLM_Dazzle = 30073,
        RLM_TrickOfLight = 30074,
        RLM_HailOfBolts = 30075,
        RLM_RathixTheVoidTamer = 30076,
        RLM_VoidSnares = 30077,
        RLM_FarseerRanger = 30078,
        RLM_VoidHated = 30079,
        RLM_VoidStriders = 30080,
        RLM_MadScout = 30081,
        RLM_EnsnaredAboration = 30082,
        RLM_EnragedTitan = 30083,
        RLM_BlackBladeOfTheAbyss = 30084,
        RLM_AnimateFortification = 30085,
        RLM_Banish = 30086,
        RLM_ChainsofLight = 30087,
        RLM_SpacialProjection = 30088,
        RLM_IntoTheVoid = 30089,
        RLM_ConsumeTheStars = 30090,
        RLM_ALightInTheVoid = 30091,
        RLM_CloseTheGates = 30092,
        RLM_RealityVortex = 30093,
        RLM_HornOfTheLegions = 30094,
        RLM_FlashOfLight = 30095,
        RLM_Alarm = 30096,



        //The Others 40000
        OTH_CorpseFeeder = 40000,
        OTH_TKN_DistendedRipper = 40001,
        OTH_DOM_PainSpawn = 40002,
        OTH_DOM_UncheckedGrowth = 40003,
        OTH_DOM_Evolve = 40004,
        OTH_DOM_Abiogenisis = 40005,
        OTH_DOM_AlterForm = 40006,
        OTH_DOM_AlterMind= 40007,
        OTH_DOM_Mindshed = 40008,
        OTH_DOM_WarpSoul = 40009,
        OTH_DOM_Mindfang = 40010,
        OTH_TKN_NithEvolve1 = 40011,
        OTH_TKN_NithEvolve2 = 40012,
        OTH_TKN_NithEvolve3 = 40013,
        OTH_TKN_NithEvolve4 = 40014,
        OTH_TKN_TentacleOfNith = 40015,
        OTH_TKN_AmorphousVestige = 40016,
        OTH_TKN_AmorphousVestige2 = 40017,
        OTH_TKN_ShatteredMemory = 40018,
        OTH_RothranTheEndless = 40019,
        OTH_WrithingVoidSpawn = 40020,
        OTH_DissolvingHorror = 40021,
        OTH_AbhorrentGestator = 40022,
        OTH_MioticBeast = 40023,
        OTH_Thoughtshaper = 40024,
        OTH_ColossalMindWorm = 40025,
        OTH_BubblingBiomass = 40026,
        OTH_SpontaneousGenesis = 40027,
        OTH_ViolentSeparation = 40028,
        OTH_MindShred = 40029,
        OTH_Brainworms = 40030,
        OTH_ChaoticSpawn = 40034,
        OTH_ParasiticFlyer = 40035,
        OTH_PrimalMutator = 40036,
        OTH_TKN_BloatedHorror = 40037,
        OTH_BloatedButcher = 40038,
        OTH_RedEyedVoidling = 40039,
        OTH_PrimalHorror = 40040,


        //Children of Illia 50000
        COI_DOM_Sentinel = 50000,
        COI_AvengeTheHost = 50001,



        //Blood of Nok 60000


        //The Wardens 70000
        //Arbiter Sub Faction 70000 - 73000
		WAR_BureauofBureaucracy = 70000,
		WAR_BenifsBrooding = 70001,
		WAR_BureaucraticImpediment = 70002,
		WAR_Paperpushing = 70003,
		WAR_SoulCrush = 70004,
		WAR_DisarmingOrder = 70005,
		WAR_Redirect = 70006,
		WAR_RedTape = 70007,
		WAR_BureaucraticInsanity = 70008,
		WAR_SoulSecretary = 70009,
		WAR_BusyVeteran = 70010,
		WAR_SoulstoneGolem = 70011,
		WAR_SoulstoneTitan = 70012,
		WAR_LibrarianRoki = 70013,

		//Guardian Sub Faction 73001 - 76000
		WAR_ArgensStrikes = 73001,
		WAR_StormAssault = 73002,
		WAR_Thunderslam = 73003,
		WAR_DeathDefied = 73004,
		WAR_ConfidentProtector = 73005,
		WAR_VengefulWarden = 73006,
		WAR_UrsolusAndTorusin = 73007,
		WAR_TagTeamTacklers = 73008,
		WAR_SpiritGuard = 73009,
		WAR_SwornProtector = 73010,
		WAR_SwornAvenger = 73011,
		WAR_ShieldWarden = 73012,
		WAR_LurikTheShadow = 73013,
		WAR_TKN_ShadowOfLurik = 73014,

		//Ferryman Sub Faction 76001 - 79000
		WAR_TavernOfSouls = 76001,
		WAR_FerrymansPlight = 76002,
		WAR_UrteilsWarding = 76003,
		WAR_WatchdogWard = 76004,
		WAR_FerrymansSoulskiff = 76005,
		WAR_SoulLightLantern = 76006,
		WAR_FreshFerryman = 76007,
		WAR_BeastHandler = 76008,
		WAR_SoulMender = 76009,
		WAR_LoyalHound = 76010,
		WAR_SoulchainSlinger = 76011,
		WAR_HeadlessChanneler = 76012,
		WAR_MasterOfTheMeek = 76013,

		//Warden Other 79001 - 79999
		WAR_DOM_SpiritGuide = 79001,
		WAR_DOM_Bolster = 79001,
		WAR_DOM_Fearsome = 79001,
		WAR_DOM_DeadlySalvation = 79001,
		WAR_DOM_OneManPhalanx = 79001,
		WAR_DOM_Flex = 79001,
		WAR_DOM_GraveyardShift = 79001,
		WAR_DOM_SoulExchange = 79001,
		WAR_DOM_Micromanage = 79001,

        //Neutral 80000
        NEU_SkeletalMinions = 80000,
        NEU_FireElemental = 80001,
        NEU_Emberstone = 80002,

    }



}
