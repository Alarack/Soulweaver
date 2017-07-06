using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constants {

    public enum CardType {
        Soul,
        Spell,
        Enchantment,
        Relic,
        Domain,
        None,
        Player
    }

    public enum SubTypes {
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
        None,
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
        Cultist
    }

    public enum Attunements {
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
        None
    }

    public enum Keywords : byte {
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
        None,
        Reanimator,
        NoIntercept,
        AttackSelf,
        Corrupted,
        Terrify,
        Interceptor,
        Exhausted
    }

    public enum SpecialAbilityActivationTrigger {
        Inspire,
        Finale,
        EndofTurn,
        BeginingOfTurn,
        EveryTurn
    }

    public enum EffectType {
        StatAdjustment,
        ZoneChange,
        SpawnToken,
        UpKeepTargtedEffect,
        Sacrifice,
        Essence,
        GrantKeywordAbility,
        FetchCard,
        Exhaust,
        GrantSpecialAbility,
        Destroy,
        SearchForCard,
        LifeAdjustment,
        SpawnMultipleTokens,
        DrawCards,
        SummonCopy,
        None,
        Dispel,
        ForceIntercept,
        MindControl,
        SpawnCardsInHand,
        StealEnchants,
        CostAdjustment,
        SpellDamage,
        SpawnTokenForOpponent,
        Donate,
        AffectDomain,
        ModifyDomainStackCount,
        AffectInterceptor,
        SpawnTokenSet
    }

    public enum OwnerConstraints {
        All,
        Mine,
        Theirs,
        None
    }

    public enum DeckType {
        Grimoire,
        Domain,
        Hand,
        SoulCrypt,
        Battlefield,
        Void,
        AllCards
    }

    public enum CardStats {
        Cost,
        Attack,
        Size,
        Health
    }

    public enum GameEvent {
        None = 0,

        CardEnteredZone = 100,
        CardLeftZone = 101,

        CreatureDied = 102,

        TurnStarted = 200,
        TurnEnded = 201
    }




}
