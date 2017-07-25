using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {

    public enum CardType {
        None = 0,
        Soul,
        Spell,
        Enchantment,
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
        Gift,
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
        Giant


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
        Reach
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
        Slain

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
        WhosTurn



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
        RemoveKeywordAbilities,
        //FetchCard,
        GrantSpecialAttribute,
        //SearchForCard,
        //Dispel,
        //ForceIntercept,
        //MindControl,
        //StealEnchants,
        SpawnTokenForOpponent,
        //Donate,
        //AffectDomain,
        //ModifyDomainStackCount,
        //AffectInterceptor,
        RemoveOtherEffect,
        RetriggerOtherEffect,
    }


    public enum SpecialAbilityTypes {
        None = 0,
        UserSingleTargeted,
        LogicMultiTarget,

    }

    public enum EffectDuration {
        None = 0,
        Permanent,
        EndOfTurn,
        StartOfTurn,
        WhileInZone
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

    }

    public enum CardStats {
        None = 0,
        Cost,
        Attack,
        Size,
        Health,
        MaxHealth

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

        CardClicked = 400,
        UserActivatedAbilityInitiated = 401,
        UserActivatedDomainAbility = 402,

        TriggerSecondaryEffect = 500,
    }




}
