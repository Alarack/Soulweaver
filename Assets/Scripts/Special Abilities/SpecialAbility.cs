using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;
using System;

using AbilityActivationTrigger = Constants.AbilityActivationTrigger;
using EffectType = Constants.EffectType;

using OwnerConstraints = Constants.OwnerConstraints;
using Keywords = Constants.Keywords;
using Subtypes = Constants.SubTypes;
using CardType = Constants.CardType;
using Attunements = Constants.Attunements;
using DeckType = Constants.DeckType;
using ConstraintType = Constants.ConstraintType;
using CardStats = Constants.CardStats;
using GameEvent = Constants.GameEvent;
using EffectDuration = Constants.EffectDuration;

[System.Serializable]
public abstract class SpecialAbility {

    [Header("Basic Info")]
    public List<AbilityActivationTrigger> trigger = new List<AbilityActivationTrigger>();
    public EffectDuration duration;
    public ConstraintList triggerConstraints = new ConstraintList();
    public ConstraintList sourceConstraints = new ConstraintList();
    public EffectType effect;
    public string abilityName;
    public int effectValue;
    public CardVisual source;
    public List<CardVisual> targets = new List<CardVisual>();
    //public List<Constraint> constraints = new List<Constraint>();
    public ConstraintList limitations = new ConstraintList();
    public int ID;
    public string abilityVFX;

    public List<StatAdjustment> statAdjustments = new List<StatAdjustment>();

    //protected abstract void Effect(CardVisual card);

    public virtual bool ProcessEffect(CardVisual card) {
        bool check = CheckConstraints(limitations, card);

        if (!check)
            return false;

        Effect(card);

        return check;

    }

    protected virtual void Unsubscribe() {

    }


    public virtual void Initialize(CardVisual owner) {
        source = owner;
        RegisterListeners();
        InitializeStatAdjusments();

        source.specialAbilities.Add(this);
        //combatManager = owner.owner.combatManager;
        //ID = IDFactory.GenerateID();

        //Debug.Log(source.gameObject.name + " has a special ability with ID " + ID);
    }


    protected virtual void Effect(CardVisual card) {

        if(!targets.Contains(card))
            targets.Add(card);


        if(abilityVFX != null && abilityVFX != "") {
            CreateVFX();
        }

        //Debug.Log("[Special Ability] Applying effect");

        switch (effect) {
            case EffectType.StatAdjustment:
                ApplyStatAdjustments(card);
                break;

            case EffectType.None:
                Debug.Log("No effect is set to happen for " + source.gameObject.name);
                break;

        }
    }

    protected virtual void RemoveEffect(List<CardVisual> cards) {

        Debug.Log("Removeing Effect");
        for (int i = 0; i < cards.Count; i++) {
            switch (effect) {

                case EffectType.StatAdjustment:
                    RemoveStatAdjustments(cards[i]);
                    break;

            }
        }

        ClearTargets();
        Unsubscribe();
    }


    protected void ApplyStatAdjustments(CardVisual card) {
        for (int i = 0; i < statAdjustments.Count; i++) {
            card.RPCApplyStatAdjustment(PhotonTargets.All, statAdjustments[i], source);
        }
    }

    protected void RemoveStatAdjustments(CardVisual card) {
        for (int i = 0; i < statAdjustments.Count; i++) {
            card.RPCRemoveStatAdjustment(PhotonTargets.All, statAdjustments[i].uniqueID, source);
        }

    }



    public virtual void RegisterListeners() {


        if (duration == EffectDuration.EndOfTurn && source.photonView.isMine) {
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEndDuration);
            Debug.Log("Registering an end of turn duration event");
        }

        if (duration == EffectDuration.WhileInZone && source.photonView.isMine) {
            Grid.EventManager.RegisterListener(GameEvent.CardLeftZone, OnLeavesZoneEndDuration);
        }






        if (trigger.Contains(AbilityActivationTrigger.EntersZone))
            Grid.EventManager.RegisterListener(GameEvent.CardEnteredZone, OnEnterZone);

        if (trigger.Contains(AbilityActivationTrigger.TakesDamage) || trigger.Contains(AbilityActivationTrigger.Healed))
            Grid.EventManager.RegisterListener(GameEvent.CreatureStatAdjusted, OnCreatureStatAdjusted);

        if (trigger.Contains(AbilityActivationTrigger.Attacks))
            Grid.EventManager.RegisterListener(GameEvent.CharacterAttacked, OnAttack);
    }

    #region EVENTS

    protected void OnTurnEndDuration(EventData data) {
        if (targets.Count < 1)
            return;

        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.owner == player) {
            return;
        }

        Debug.Log("End of turn effect is triggering");
        RemoveEffect(targets);
    }

    protected void OnLeavesZoneEndDuration(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (card == source && sourceConstraints.currentZone.Contains(deck.decktype)) {
            RemoveEffect(targets);
        }


    }

    protected void OnTurnStart(EventData data) {



    }

    protected void OnTurnEnd(EventData data) {


    }


    protected void OnEnterZone(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (!ManageConstraints(card)) {
            return;
        }

        //if(card == source)
        //    Debug.Log(card.gameObject.name + " has entered " + deck.decktype.ToString());


        if (this is MultiTargetedAbility) {
            if (source.photonView.isMine) {
                //Debug.Log("Processing a multi-target effect");

                ProcessEffect(source);
            }

        }


        //If a targted effect has no valid targets when it triggers, then get out of here.

        if (source.photonView.isMine && this is EffectOnTarget) {
            if (CheckValidTargets().Count < 1) {
                Debug.Log("No Valid Targets for " + source.gameObject.name + "'s targeted ability");
                return;
            }
            else {

                Debug.Log("Good to go");
                CombatManager.combatManager.isChoosingTarget = true;
                CombatManager.combatManager.ActivateSpellTargeting();
                CombatManager.combatManager.confirmedTargetCallback += ProcessEffect;

            }

        }




    }


    protected void OnAttack(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        Debug.Log(card.gameObject.name + " is attacking");
    }


    protected void OnCreatureStatAdjusted(EventData data) {
        CardStats stat = (CardStats)data.GetInt("Stat");
        int value = data.GetInt("Value");
        CardVisual target = data.GetGameObject("Target").GetComponent<CardVisual>();
        CardVisual sourceOfAdjustment = data.GetMonoBehaviour("Source") as CardVisual;

        if (limitations.currentZone.Count > 0 && !limitations.currentZone.Contains(source.currentDeck.decktype)) {
            //Debug.Log(source.gameObject.name + " is not in the proper zone to detect a stat adjustment, but is still listening for one");
            return;
        }


        if (trigger.Contains(AbilityActivationTrigger.TakesDamage)) {
            if (stat == CardStats.Health && value < 0) {
                Debug.Log(target.gameObject.name + " has taken " + Mathf.Abs(value) + " point(s) of damage");
            }



        }

    }


    #endregion



    #region CONSTRAINT CHECKING


    protected bool ManageConstraints(CardVisual target) {
        bool result = true;

        if (triggerConstraints.thisCardOnly && target != source) {
            //Debug.Log(source.gameObject.name + " can only trigger its own effect and " + target.gameObject.name + " has happened");
            return false;
        }

        if (CheckConstraints(triggerConstraints, target) == null) {
            //Debug.Log("Trigger is not in place");
            return false;
        }

        if (CheckConstraints(sourceConstraints, source) == null) {
            //Debug.Log("Source is not in place for" + source.gameObject.name + "'s ability");
            //Debug.Log("Source is not in place");
            return false;
        }

        return result;
    }

    protected List<CardVisual> CheckValidTargets() {
        List<CardVisual> results = new List<CardVisual>();

        foreach (CardVisual target in Deck._allCards.activeCards) {
            if (CheckConstraints(limitations, target) != null) {
                results.Add(target);
                //Debug.Log(target.gameObject.name + " is a valid target for " + source.gameObject.name + "'s targeted ability");
            }
        }

        return results;
    }

    public virtual CardVisual CheckConstraints(ConstraintList constraint, CardVisual target) {
        //CardVisual result = target;

        for (int i = 0; i < constraint.types.Count; i++) {
            if (ConstraintHelper(constraint, constraint.types[i], target) == null) {
                return null;
            }
        }

        return target;
    }

    protected void ClearTargets() {
        targets.Clear();
    }


    protected CardVisual ConstraintHelper(ConstraintList constraint, ConstraintType type, CardVisual target) {
        //bool result = true;

        if (!constraint.types.Contains(type))
            return target;


        switch (type) {
            case ConstraintType.PrimaryType:
                if (!constraint.primaryType.Contains(target.primaryCardType))
                    return null;

                break;

            case ConstraintType.Owner:
                if (DetermineOnwerConstraint(target, constraint.owner) == null)
                    return null;

                break;

            case ConstraintType.CurrentZone:
                if (!constraint.currentZone.Contains(target.currentDeck.decktype))
                    return null;

                break;

            case ConstraintType.PreviousZone:
                if (!constraint.previousZone.Contains(target.previousDeck.decktype))
                    return null;

                break;

            case ConstraintType.Subtype:
                if (!DoesListContainAny<Subtypes>(constraint.subtype, target.subTypes))
                    return null;

                break;

            case ConstraintType.Attunement:
                if (!DoesListContainAny<Attunements>(constraint.attunement, target.attunements))
                    return null;

                break;

            case ConstraintType.AdditionalType:
                if (!DoesListContainAny<CardType>(constraint.additionalType, target.otherCardTypes))
                    return null;

                break;

            case ConstraintType.Keyword:
                if (!DoesListContainAny<Keywords>(constraint.keyword, target.keywords))
                    return null;

                break;

            case ConstraintType.StatMaximum:
                return CreatureStatConstraint(constraint.maxStats, false, target);

            case ConstraintType.StatMinimum:
                return CreatureStatConstraint(constraint.minStats, true, target);

            case ConstraintType.CreatureStatus:

                for (int i = 0; i < constraint.creatureStatus.Count; i++) {
                    switch (constraint.creatureStatus[i]) {
                        case Constants.CreatureStatus.MostStat:
                            if (HasHighOrLowStat(constraint.mostStat, target, true) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.LeastStat:
                            if (HasHighOrLowStat(constraint.leastStat, target, false) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.Damaged:
                            if (IsDamagedOrUndamaged(target, true) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.Undamaged:
                            if (IsDamagedOrUndamaged(target, false) == null)
                                return null;
                            break;
                    }
                }

                break;
        }



        return target;
    }


    private CardVisual DetermineOnwerConstraint(CardVisual target, OwnerConstraints limit) {
        CardVisual resul = null;

        switch (limit) {
            case OwnerConstraints.All:
                return target;

            case OwnerConstraints.Mine:
                if (target.photonView.isMine)
                    return target;

                break;

            case OwnerConstraints.Theirs:
                if (!target.photonView.isMine)
                    return target;

                break;

            default:

                return null;

        }

        return resul;
    }


    private CardVisual HasHighOrLowStat(CardStats statToCompare, CardVisual target, bool highest) {
        CardVisual result = null;

        List<CardVisual> cardsToSearch = Finder.FindCardsWithStatExtreme(statToCompare, highest);
        if (cardsToSearch.Contains(target))
            return target;


        return result;
    }

    private CardVisual IsDamagedOrUndamaged(CardVisual target, bool damaged) {
        CardVisual result = null;

        List<CardVisual> cardsToSearch = Finder.FindAllDamagedOrUndamagedCreatures(damaged);
        if (cardsToSearch.Contains(target))
            return target;


        return result;
    }

    protected CardVisual HasStatExtreme(Constants.CreatureStatus status, CardStats statToCompare, CardVisual target) {

        List<CardVisual> cardsToSearch = new List<CardVisual>();

        switch (status) {
            case Constants.CreatureStatus.MostStat:
                cardsToSearch = Finder.FindCardsWithStatExtreme(statToCompare, true);
                if (cardsToSearch.Contains(target))
                    return target;

                break;

            case Constants.CreatureStatus.LeastStat:
                cardsToSearch = Finder.FindCardsWithStatExtreme(statToCompare, false);
                if (cardsToSearch.Contains(target))
                    return target;

                break;

            case Constants.CreatureStatus.Damaged:
                cardsToSearch = Finder.FindAllDamagedOrUndamagedCreatures(true);
                if (cardsToSearch.Contains(target))
                    return target;

                break;

            case Constants.CreatureStatus.Undamaged:
                cardsToSearch = Finder.FindAllDamagedOrUndamagedCreatures(false);
                if (cardsToSearch.Contains(target))
                    return target;
                break;


        }

        return null;
    }

    protected bool DoesListContainAny<T>(List<T> list1, List<T> list2) where T : struct, IFormattable, IConvertible {
        bool result = false;

        foreach (T entry in list1) {
            if (list2.Contains(entry)) {
                result = true;
                break;
            }
        }

        return result;
    }

    protected CardVisual CreatureStatConstraint(List<StatAdjustment> statLimits, bool statHigher, CardVisual target) {
        CardVisual result = target;

        CreatureCardVisual soul = target as CreatureCardVisual;

        if (statHigher) {
            foreach (StatAdjustment stat in statLimits) {
                switch (stat.stat) {
                    case CardStats.Attack:
                        if (soul.attack < stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Size:
                        if (soul.size < stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Health:
                        if (soul.health < stat.value) {
                            return null;
                        }

                        break;
                }
            }
        }

        if (!statHigher) {
            foreach (StatAdjustment stat in statLimits) {
                switch (stat.stat) {
                    case CardStats.Attack:
                        if (soul.attack > stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Size:
                        if (soul.size > stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Health:
                        if (soul.health > stat.value) {
                            return null;
                        }

                        break;
                }
            }


        }




        return result;
    }



    #endregion


    public void CreateVFX() {

        GameObject atkVFX = PhotonNetwork.Instantiate(abilityVFX, targets[0].transform.position, Quaternion.identity, 0) as GameObject;

        source.RPCDeployAttackEffect(PhotonTargets.All, atkVFX.GetPhotonView().viewID, targets[0]);
    }




    private void InitializeStatAdjusments() {
        for(int i = 0; i < statAdjustments.Count; i++) {
            statAdjustments[i].uniqueID = IDFactory.GenerateID();
            statAdjustments[i].source = source;
        }
    }

    [System.Serializable]
    public class StatAdjustment {
        public Constants.CardStats stat;
        public int value;
        public bool nonStacking;
        public bool temporary;
        public CardVisual source;
        public int uniqueID = -1;


        public StatAdjustment() {

        }

        public StatAdjustment(CardStats stat, int value, bool nonStacking, bool temp, CardVisual source) {
            this.stat = stat;
            this.value = value;
            this.nonStacking = nonStacking;
            this.source = source;
            temporary = temp;

            uniqueID = IDFactory.GenerateID();

        }

    }

    //[System.Serializable]
    //public class Constraint {

    //    public ConstraintType type;

    //    public OwnerConstraints owner;
    //    public CardType primaryType;
    //    public CardType additionalType;
    //    public Subtypes subtype;
    //    public Keywords keyword;
    //    public Attunements attunement;
    //    public DeckType currentZone;
    //    public DeckType previousZone;

    //    public List<StatAdjustment> minStats = new List<StatAdjustment>();
    //    public List<StatAdjustment> maxStats = new List<StatAdjustment>();
    //    public bool damaged;
    //    public bool undamaged;



    //    private void ClearUnviewedSelection<TEnum>(TEnum chosen) where TEnum : struct, System.IConvertible, System.IComparable, System.IFormattable {
    //        if (!typeof(TEnum).IsEnum) {
    //            throw new System.ArgumentException("TEnum must be an enum.");
    //        }

    //        if (!(chosen is OwnerConstraints)) {

    //        }

    //    }

    //}


    [System.Serializable]
    public class ConstraintList {

        public List<ConstraintType> types = new List<ConstraintType>();

        //public List<OwnerConstraints> owner = new List<OwnerConstraints>();
        public OwnerConstraints owner;
        public List<CardType> primaryType = new List<CardType>();
        public List<CardType> additionalType = new List<CardType>();
        public List<Subtypes> subtype = new List<Subtypes>();
        public List<Keywords> keyword = new List<Keywords>();
        public List<Attunements> attunement = new List<Attunements>();
        public List<DeckType> currentZone = new List<DeckType>();
        public List<DeckType> previousZone = new List<DeckType>();
        public List<Constants.CreatureStatus> creatureStatus = new List<Constants.CreatureStatus>();

        public List<StatAdjustment> minStats = new List<StatAdjustment>();
        public List<StatAdjustment> maxStats = new List<StatAdjustment>();

        public CardStats mostStat;
        public CardStats leastStat;
        public bool thisCardOnly;





    }

}
