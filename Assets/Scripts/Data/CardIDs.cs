using System.Collections;
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



        //Children of Illia 50000
        COI_DOM_Sentinel = 50000,



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

    }



}
