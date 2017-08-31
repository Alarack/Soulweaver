using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {

    public enum CardType {
        None = 0,
        Soul,
        Spell,
        Support,
        Relic,
        Domain,
        Player
    }

    public enum SubTypes {
        None = 0,
        Zombie,
        Skeleton,
        Phoenix,
        Avatar,
        Elemental,
        Human,
        Wizard,
        Wall,
        Treant,
        Elf,
        //Gift,
        Curse,
        Emanation,
        Spider,
        Artificial,
        Soldier,
        Axim,
        Elohim,
        Demon,
        Angel,
        Dragon,
        Wolf,
        Manipulator,
        Devourer,
        Nightmare,
        Cultist,
        Token,
        Guardian,
        Ferryman,
        LivingFortress,
        Mage,
        Archer,
        Rifleman,
        Structure,
        Arbiter,
        Dullahan,
        Canine,
        Abomination,
        Cavalry,
        Golem,
        Knight,
        Shaper,
        Unique,
        Illusion,
        Tower,
        Seige,
        Keeper,
        Strider,
        Other,
        Fated,
        Scheme,
        Bureau,
        Giant,
        Artifact,
        Ritual,
        Enchantment,
        Blessing,
        Rat,
        Spirit,
        Imp,
        Gunner,
        Dwarf,
        Ooze,
        Animal,
        Ghost,
        Thief,
        Assassin,
        Priest,
        Innkeeper,
        Shaman,
        Gravekeeper,
        Dealer,
        Mobster,
        Alchemist,
        Hydra,
        Engineer,
        Starlifter,
        Scientist



    }

    public enum Attunements {
        None = 0,
        Air,
        Earth,
        Fire,
        Water,
        Light,
        Darkness,
        Life,
        Death,
        Time,
        Space,
        Void,
        Force,
        Holy,
        Unholy,
        Order,
        Chaos,

    }

    public enum Keywords {
        None = 0,
        Flight,
        Momentum,
        Guardian,
        Protection,
        Deathtouch,
        //Immunity,
        Volatile,
        Regeneration,
        Tireless,
        Vanguard,
        FirstStrike,
        Ephemeral,
        Invisible,
        //Untouchable,
        Pacifist,
        Defender,
        DamageVulnerable,
        Reanimator,
        NoIntercept,
        AttackSelf,
        Corrupted,
        Terrify,
        Interceptor,
        Exhausted,
        Fusion,
        Fission,
        Inspire,
        Finale,
        Threshold,
        Phalanx,
        Deathwatch,
        Berserk,
        Bloodthirst,
        Tortured,
        Rush,
        Reach,
        Cleave,
        NoAttack,
        Stun,
        Ranged,
        Token,
        Lightforged,
        Enrage,

    }



    public enum AbilityActivationTrigger {
        None = 0,
        //TakesDamage,
        EntersZone,
        LeavesZone,
        Attacks,
        Defends,
        //Dies,
        //Healed,
        TurnStarts,
        TurnEnds,
        Targted,
        //DealsDamage,
        CreatureStatChanged,
        UserActivated,
        //Combat
        SecondaryEffect,
        Slain,
        ResourceChanged

    }

    public enum ConstraintType {
        None = 0,
        Owner,
        PrimaryType,
        AdditionalType,
        CurrentZone,
        PreviousZone,
        Subtype,
        Attunement,
        Keyword,
        StatMinimum,
        StatMaximum,
        CreatureStatus,
        WhosTurn,
        SpecialAttribute,
        CanAttack,
        OtherTargets,
        CardAdjacentToSource



    }

    public enum AdditionalRequirement {
        None = 0,
        NumberofCardsInZone,
        RequireResource

    }

    public enum EffectType {
        None = 0,
        StatAdjustment,
        ZoneChange,
        SpawnToken,
        //Sacrifice,
        GenerateResource,
        AddOrRemoveKeywordAbilities,
        //RemoveKeywordAbilities,
        //FetchCard,
        AddOrRemoveSpecialAttribute,
        //SearchForCard,
        //Dispel,
        //ForceIntercept,
        //MindControl,
        //StealEnchants,
        //SpawnTokenForOpponent,
        //Donate,
        //AffectDomain,
        //ModifyDomainStackCount,
        //AffectInterceptor,
        RemoveOtherEffect,
        RetriggerOtherEffect,
        ChooseOne,
        BestowAbility,
    }


    public enum SpecialAbilityTypes {
        None = 0,
        UserSingleTargeted,
        LogicMultiTarget,

    }

    public enum Duration {
        None = 0,
        Permanent,
        EndOfTurn,
        StartOfTurn,
        WhileInZone,
    }

    public enum OwnerConstraints {
        None = 0,
        All,
        Mine,
        Theirs,

    }

    public enum DeckType {
        None = 0,
        Grimoire,
        Domain,
        Hand,
        SoulCrypt,
        Battlefield,
        Void,
        AllCards,
        NotInGame

    }

    public enum Faction {
        Neutral,
        CultOfRillock,
        ChildrenOfIllia,
        BloodOfNok,
        RealmKeepers,
        Wardens,
        Others,
        Severed,
        All
    }

    public enum CardStats {
        None = 0,
        Cost,
        Attack,
        Size,
        Health,
        MaxHealth,
        SupportValue,
        MaxSupport

    }

    public enum CreatureStatus {
        None = 0,
        Damaged,
        Undamaged,
        MostStat,
        LeastStat
    }

    public enum GameEvent {
        None = 0,

        CardEnteredZone = 100,
        CardLeftZone = 101,

        CreatureDied = 102,

        TurnStarted = 200,
        TurnEnded = 201,

        CreatureStatAdjusted = 300,
        CharacterAttacked = 301,
        CharacterDefends = 302,
        Combat = 303,
        ResourceChanged = 304,

        CardClicked = 400,
        UserActivatedAbilityInitiated = 401,
        UserActivatedDomainAbility = 402,

        TriggerSecondaryEffect = 500,

        VFXLanded = 600,


        //DeckBuilder
        CardSelected = 1000,
        CardDeselected = 1001,
    }




}
