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
        Token
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
        Immunity,
        Volatile,
        Regeneration,
        Tireless,
        Vanguard,
        FirstStrike,
        Ephemeral,
        Invisible,
        Untouchable,
        Pacifist,
        Defender,
        DamageVulnerable,
        Reanimator,
        NoIntercept,
        AttackSelf,
        Corrupted,
        Terrify,
        Interceptor,
        Exhausted
    }

    public enum AbilityActivationTrigger {
        None = 0,
        TakesDamage,
        EntersZone,
        LeavesZone,
        Attacks,
        Defends,
        Dies,
        Healed,
        TurnStarts,
        TurnEnds,
        Targted

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
        CreatureStatus

    }

    public enum EffectType {
        None = 0,
        StatAdjustment,
        ZoneChange,
        SpawnToken,
        //UpKeepTargtedEffect,
        Sacrifice,
        GenerateResource,
        GrantKeywordAbilities,
        RemoveKeywordAbilities,
        //FetchCard,
        //Exhaust,
        GrantSpecialAbility,

        //Destroy,
        //SearchForCard,
        //LifeAdjustment,
        //SpawnMultipleTokens,
        DrawCards,
        SummonCopy,
        Dispel,
        //ForceIntercept,
        //MindControl,
        //SpawnCardsInHand,
        StealEnchants,
        //CostAdjustment,
        SpellDamage,
        SpawnTokenForOpponent,
        //Donate,
        AffectDomain,
        //ModifyDomainStackCount,
        //AffectInterceptor,
        //SpawnTokenSet
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

        CardClicked = 400
    }




}
