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
    public ConstraintList targetConstraints = new ConstraintList();
    public int ID;
    public string abilityVFX;
    public bool movingVFX;
    //Cards In Zone
    public ConstraintList additionalRequirementConstraints = new ConstraintList();

    //Keyword Fields
    public List<Keywords> keywordsToAddorRemove = new List<Keywords>();


    //Additional Requirements
    public List<Constants.AdditionalRequirement> additionalRequirements = new List<Constants.AdditionalRequirement>();

    public List<StatAdjustment> statAdjustments = new List<StatAdjustment>();

    [SerializeField]
    private List<StatAdjustment> activeStatAdjustments = new List<StatAdjustment>();

    //Secondary Effect
    //public List<EffectOnTarget> secondaryEffectOnTarget = new List<EffectOnTarget>();
    //public List<LogicTargetedAbility> secondaryLogicTargtedAbility = new List<LogicTargetedAbility>();



    public enum ApplyEffectToWhom {
        TriggeringCard,
        CauseOfTrigger,
        Source
    }

    public enum DeriveStatsFromWhom {
        TargetOFEffect,
        SourceOfEffect
    }

    public ApplyEffectToWhom processTriggerOnWhom;

    public enum GainedOrLost {
        Gained,
        Lost
    }

    public enum MoreOrLess {
        AtLeast,
        NoMoreThan
    }


    public virtual bool ProcessEffect(CardVisual card) {
        bool check = CheckConstraints(targetConstraints, card);

        if (!check)
            return false;

        Effect(card);

        return check;

    }

    protected virtual void Unsubscribe() {

    }

    public virtual void Initialize(CardVisual owner) {
        source = owner;

        if (source == null) {
            Debug.LogError("[Special Ability] This Ability has a null source.");
            return;
        }

        RegisterListeners();
        InitializeStatAdjusments();

        source.specialAbilities.Add(this);
        //combatManager = owner.owner.combatManager;
        //ID = IDFactory.GenerateID();

        //Debug.Log(source.gameObject.name + " has a special ability with ID " + ID);


    }

    protected virtual void Effect(CardVisual card) {

        //Debug.Log("Effect");

        if (!targets.Contains(card))
            targets.Add(card);


        if (abilityVFX != null && abilityVFX != "") {
            CreateVFX();
        }

        if (triggerConstraints.oncePerTurn)
            triggerConstraints.triggeredThisTurn = true;

        //Debug.Log("[Special Ability] Applying effect");

        switch (effect) {
            case EffectType.StatAdjustment:
                ApplyStatAdjustments(card);
                break;

            case EffectType.SpawnToken:
                SpawnToken(targetConstraints);
                break;

            case EffectType.ZoneChange:
                ForcedZoneChange(card, targetConstraints);
                break;

            case EffectType.GrantKeywordAbilities:
                GrantKeywords(card, keywordsToAddorRemove);
                break;

            case EffectType.GenerateResource:
                GenerateResource(targetConstraints);
                break;

            case EffectType.GrantSpecialAttribute:
                AddSpecialAttribute(card, targetConstraints);
                break;

            case EffectType.None:
                Debug.Log("No effect is set to happen for " + source.gameObject.name);
                break;

        }


        if (!trigger.Contains(AbilityActivationTrigger.SecondaryEffect)) {
            EventData data = new EventData();

            data.AddMonoBehaviour("Source", source);
            data.AddString("AbilityName", abilityName);

            Grid.EventManager.SendEvent(GameEvent.TriggerSecondaryEffect, data);
        }


    }

    protected virtual void RemoveEffect(List<CardVisual> cards) {

        Debug.Log("Removeing Effect");
        for (int i = 0; i < cards.Count; i++) {
            switch (effect) {

                case EffectType.StatAdjustment:
                    RemoveStatAdjustments(cards[i]);
                    break;

                case EffectType.SpawnToken:

                    break;

                case EffectType.ZoneChange:

                    break;

                case EffectType.GrantKeywordAbilities:
                    RemoveKeywords(cards[i], keywordsToAddorRemove);
                    break;

                case EffectType.GenerateResource:

                    RemoveResource(targetConstraints);
                    break;

                case EffectType.GrantSpecialAttribute:
                    Debug.Log("Find a good way to remove granted attributes");
                    //ToggleSpecialAttributeSuspension(cards[i], targetConstraints, true);
                    break;

            }
        }

        ClearTargets();
        Unsubscribe();
    }


    protected void ApplyStatAdjustments(CardVisual card) {
        for (int i = 0; i < statAdjustments.Count; i++) {

            if (statAdjustments[i].valueSetBytargetStat) {

                switch (statAdjustments[i].deriveStatsFromWhom) {
                    case DeriveStatsFromWhom.SourceOfEffect:
                        statAdjustments[i].AlterValueBasedOnTarget(source as CreatureCardVisual);
                        break;

                    case DeriveStatsFromWhom.TargetOFEffect:
                        statAdjustments[i].AlterValueBasedOnTarget(card as CreatureCardVisual);
                        break;
                }
            }

            //Debug.Log("Applying stats");
            int spelldamage = Finder.FindTotalSpellDamage();

            if (statAdjustments[i].spellDamage) {
                ApplySpellDamge(statAdjustments[i], -spelldamage);
            }


            card.RPCApplySpecialAbilityStatAdjustment(PhotonTargets.All, statAdjustments[i], source);

            activeStatAdjustments.Add(statAdjustments[i]);
        }
    }

    private void ApplySpellDamge(StatAdjustment adjustment, int spellDamage) {

        adjustment.ModifyValue(spellDamage);

    }

    protected void RemoveStatAdjustments(CardVisual card) {
        for (int i = 0; i < activeStatAdjustments.Count; i++) {
            card.RPCRemoveStatAdjustment(PhotonTargets.All, activeStatAdjustments[i].uniqueID, source);
            //Debug.Log("Removing stat adjustment with ID " + activeStatAdjustments[i].uniqueID);

        }
        activeStatAdjustments.Clear();

    }



    public virtual void RegisterListeners() {

        if (!source.photonView.isMine)
            return;


        if (source.primaryCardType == CardType.Domain && trigger.Contains(AbilityActivationTrigger.UserActivated)) {
            Grid.EventManager.RegisterListener(GameEvent.UserActivatedDomainAbility, OnUserActiveDomainAbility);
        }



        if (duration == EffectDuration.EndOfTurn && source.photonView.isMine) {
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEndDuration);
            Debug.Log("Registering an end of turn duration event");
        }

        if (duration == EffectDuration.WhileInZone && source.photonView.isMine) {
            Grid.EventManager.RegisterListener(GameEvent.CardLeftZone, OnLeavesZoneEndDuration);
        }

        if (triggerConstraints.resetCountAtTurnEnd && triggerConstraints.requireMultipleTriggers)
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, ResetCounterOnTurnEnd);

        if (triggerConstraints.oncePerTurn)
            Grid.EventManager.RegisterListener(GameEvent.TurnStarted, RefreshOncePerTurnCounter);



        if (trigger.Contains(AbilityActivationTrigger.EntersZone))
            Grid.EventManager.RegisterListener(GameEvent.CardEnteredZone, OnEnterZone);

        if (trigger.Contains(AbilityActivationTrigger.CreatureStatChanged))
            Grid.EventManager.RegisterListener(GameEvent.CreatureStatAdjusted, OnCreatureStatAdjusted);

        if (trigger.Contains(AbilityActivationTrigger.Attacks))
            Grid.EventManager.RegisterListener(GameEvent.CharacterAttacked, OnAttack);

        if (trigger.Contains(AbilityActivationTrigger.Defends))
            Grid.EventManager.RegisterListener(GameEvent.CharacterDefends, OnCombatDefense);

        if (trigger.Contains(AbilityActivationTrigger.UserActivated))
            Grid.EventManager.RegisterListener(GameEvent.UserActivatedAbilityInitiated, OnUserActivation);

        if (trigger.Contains(AbilityActivationTrigger.SecondaryEffect))
            Grid.EventManager.RegisterListener(GameEvent.TriggerSecondaryEffect, OnEffectComplete);

        if (trigger.Contains(AbilityActivationTrigger.TurnEnds))
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEnd);

        if (trigger.Contains(AbilityActivationTrigger.TurnStarts))
            Grid.EventManager.RegisterListener(GameEvent.TurnStarted, OnTurnStart);

        if (trigger.Contains(AbilityActivationTrigger.Slain))
            Grid.EventManager.RegisterListener(GameEvent.CreatureDied, OnCreatureSlain);

    }

    #region EVENTS

    protected void OnTurnEndDuration(EventData data) {
        if (targets.Count < 1)
            return;

        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.owner == player) {
            return;
        }

        if (!source.photonView.isMine)
            return;

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

    protected void OnCreatureSlain(EventData data) {
        CardVisual deadCard = data.GetMonoBehaviour("DeadCard") as CardVisual;
        CardVisual causeOfdeath = data.GetMonoBehaviour("CauseOfDeath") as CardVisual;


        if (!source.photonView.isMine)
            return;


        CardVisual effectTarget = null;

        switch (processTriggerOnWhom) {
            case ApplyEffectToWhom.TriggeringCard:
                effectTarget = deadCard;
                break;

            case ApplyEffectToWhom.CauseOfTrigger:
                effectTarget = causeOfdeath;
                break;

            case ApplyEffectToWhom.Source:
                effectTarget = source;
                break;
        }


        if (!ManageConstraints(effectTarget)) {
            return;
        }


        if (this is LogicTargetedAbility && source.photonView.isMine) {
            ProcessEffect(effectTarget);
        }

        ActivateTargeting();


    }



    protected void OnEffectComplete(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Source") as CardVisual;
        string triggeringAbilityName = data.GetString("AbilityName");

        if (card != source)
            return;




        List<CardVisual> primaryTargets = Finder.FindSpecialAbilityOnCardByName(card, triggeringAbilityName).targets;


        if (triggerConstraints.triggerbySpecificAbility && triggerConstraints.triggerablePrimaryAbilityName != triggeringAbilityName)
            return;


        //Debug.Log("triggering a secondary effect from " + source.cardData.cardName);

        if (!source.photonView.isMine)
            return;


        if (this is LogicTargetedAbility) {
            LogicTargetedAbility lta = this as LogicTargetedAbility;

            if (lta.processEffectOnPrimaryEffectTargets) {
                lta.ProcessEffect(primaryTargets);
            }
            else {
                ProcessEffect(source);
            }
        }

        ActivateTargeting();

    }


    protected void OnUserActiveDomainAbility(EventData data) {
        if (!source.photonView.isMine)
            return;

        DomainTile tile = data.GetMonoBehaviour("Tile") as DomainTile;
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;


        if (card != source)
            return;

        if (!ManageConstraints(card)) {
            return;
        }

        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {

                ProcessEffect(card);
            }
        }

        ActivateTargeting();


    }



    protected void OnUserActivation(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        if (!source.photonView.isMine)
            return;

        if (!ManageConstraints(card)) {
            return;
        }

        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {

                ProcessEffect(card);
            }
        }

        ActivateTargeting();
    }

    protected void RefreshOncePerTurnCounter(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.photonView.isMine)
            return;


        if (!source.owner == player) {
            return;
        }

        if (triggerConstraints.oncePerTurn) {
            triggerConstraints.triggeredThisTurn = false;
        }

    }


    protected void OnTurnStart(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.photonView.isMine)
            return;


        if (!source.owner == player) {
            return;
        }

        if (triggerConstraints.oncePerTurn) {
            triggerConstraints.triggeredThisTurn = false;
        }

        if (!ManageConstraints(source)) {
            return;
        }

        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {
                ProcessEffect(source);
            }
        }


    }

    protected void ResetCounterOnTurnEnd(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.photonView.isMine)
            return;


        if (!source.owner == player) {
            return;
        }

        if (triggerConstraints.resetCountAtTurnEnd)
            triggerConstraints.counter = 0;

    }

    protected void OnTurnEnd(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.photonView.isMine)
            return;


        if (!source.owner == player) {
            return;
        }

        if (triggerConstraints.resetCountAtTurnEnd)
            triggerConstraints.counter = 0;


        if (!ManageConstraints(source)) {
            return;
        }


        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {
                ProcessEffect(source);
            }
        }


    }


    protected void OnEnterZone(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (!ManageConstraints(card)) {
            return;
        }


        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {

                ProcessEffect(card);
            }

        }


        ActivateTargeting();
    }


    protected void OnAttack(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        //Debug.Log(card.gameObject.name + " is attacking");

        if (!ManageConstraints(card)) {
            return;
        }

        if (this is LogicTargetedAbility && source.photonView.isMine) {
            ProcessEffect(card);
        }

        ActivateTargeting();


    }

    protected void OnCombatDefense(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        //Debug.Log(card.cardData.cardName + " is defending");


        if (!ManageConstraints(card)) {
            return;
        }

        //Debug.Log("A defence trigger has happened");

        if (this is LogicTargetedAbility && source.photonView.isMine) {
            ProcessEffect(card);
        }

        ActivateTargeting();


    }




    protected void OnCreatureStatAdjusted(EventData data) {
        CardStats stat = (CardStats)data.GetInt("Stat");
        int value = data.GetInt("Value");
        CardVisual target = data.GetGameObject("Target").GetComponent<CardVisual>();
        CardVisual sourceOfAdjustment = data.GetMonoBehaviour("Source") as CardVisual;


        if (!source.photonView.isMine)
            return;


        if (!CheckCreatureStatAltered(triggerConstraints, stat, value, target))
            return;



        CardVisual effectTarget = null;

        switch (processTriggerOnWhom) {
            case ApplyEffectToWhom.TriggeringCard:
                effectTarget = target;
                break;

            case ApplyEffectToWhom.CauseOfTrigger:
                effectTarget = sourceOfAdjustment;
                break;

            case ApplyEffectToWhom.Source:
                effectTarget = source;
                break;
        }


        if (!ManageConstraints(effectTarget)) {
            return;
        }



        if (this is LogicTargetedAbility && source.photonView.isMine) {
            ProcessEffect(effectTarget);
        }

        ActivateTargeting();

    }


    #endregion


    private void ActivateTargeting() {
        if (source.photonView.isMine && this is EffectOnTarget && source.owner.myTurn) {
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


    #region CONSTRAINT CHECKING


    protected bool ManageConstraints(CardVisual triggeringCard, CardVisual causeOfTrigger = null) {
        bool result = true;

        if (triggerConstraints.thisCardOnly && triggeringCard != source) {
            //Debug.Log(source.gameObject.name + " can only trigger its own effect and " + target.gameObject.name + " has happened");
            return false;
        }

        if (triggerConstraints.oncePerTurn && triggerConstraints.triggeredThisTurn)
            return false;


        //if (targetConstraints.neverTargetSelf && triggeringCard == source)
        //    return false;

        if (CheckConstraints(triggerConstraints, triggeringCard) == null) {
            //Debug.Log("Trigger is not in place");

            return false;
        }

        if (CheckConstraints(sourceConstraints, source) == null) {
            //Debug.Log("Source is not in place for" + source.gameObject.name + "'s ability");
            //Debug.Log("Source is not in place");
            return false;
        }


        for (int i = 0; i < additionalRequirements.Count; i++) {
            if (!CheckAdditionalRequirements(additionalRequirements[i], additionalRequirementConstraints))
                return false;
        }

        if (triggerConstraints.requireMultipleTriggers) {
            result = CheckForMultipleTriggers(triggerConstraints);
        }


        return result;
    }

    protected List<CardVisual> CheckValidTargets() {
        List<CardVisual> results = new List<CardVisual>();

        foreach (CardVisual target in Deck._allCards.activeCards) {
            if (CheckConstraints(targetConstraints, target) != null) {
                results.Add(target);
                //Debug.Log(target.gameObject.name + " is a valid target for " + source.gameObject.name + "'s targeted ability");
            }
        }

        return results;
    }

    public virtual CardVisual CheckConstraints(ConstraintList constraint, CardVisual target) {
        //CardVisual result = target;

        if (constraint.thisCardOnly && target != source)
            return null;

        if (constraint.neverTargetSelf && target == source)
            return null;


        for (int i = 0; i < constraint.types.Count; i++) {
            if (ConstraintHelper(constraint, constraint.types[i], target) == null) {
                //Debug.Log(target.gameObject.name + " has failed to pass");
                return null;
            }
            else {
                //Debug.Log(target.gameObject.name + " has passed contraints");
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

                if (constraint.notPrimaryType) {
                    if (constraint.primaryType.Contains(target.primaryCardType))
                        return null;
                }
                else {
                    if (!constraint.primaryType.Contains(target.primaryCardType))
                        return null;
                }

                break;

            case ConstraintType.Owner:
                if (DetermineOnwerConstraint(target, constraint.owner) == null)
                    return null;

                break;

            case ConstraintType.CurrentZone:

                if (constraint.notCurrentZone) {
                    if (constraint.currentZone.Contains(target.currentDeck.decktype))
                        return null;
                }
                else {
                    if (!constraint.currentZone.Contains(target.currentDeck.decktype)) {
                        //Debug.Log("Invalid Zone");
                        return null;
                    }

                }

                //if(source == target)
                //    Debug.Log(source.gameObject.name +  " is in a Valid Zone");
                break;

            case ConstraintType.PreviousZone:

                if (constraint.notPreviousZone) {
                    if (constraint.previousZone.Contains(target.previousDeck.decktype))
                        return null;
                }
                else {
                    if (!constraint.previousZone.Contains(target.previousDeck.decktype))
                        return null;
                }

                break;

            case ConstraintType.Subtype:
                if (constraint.notSubtype) {
                    if (DoesListContainAny<Subtypes>(constraint.subtype, target.subTypes))
                        return null;
                }
                else {
                    if (!DoesListContainAny<Subtypes>(constraint.subtype, target.subTypes))
                        return null;
                }

                break;

            case ConstraintType.Attunement:
                if (constraint.notAttunement) {
                    if (DoesListContainAny<Attunements>(constraint.attunement, target.attunements))
                        return null;
                }
                else {
                    if (!DoesListContainAny<Attunements>(constraint.attunement, target.attunements))
                        return null;
                }

                break;

            case ConstraintType.AdditionalType:
                if (constraint.notAdditionalType) {
                    if (DoesListContainAny<CardType>(constraint.additionalType, target.otherCardTypes))
                        return null;
                }
                else {
                    if (!DoesListContainAny<CardType>(constraint.additionalType, target.otherCardTypes))
                        return null;
                }

                break;

            case ConstraintType.Keyword:
                if (constraint.notKeyword) {
                    if (DoesListContainAny<Keywords>(constraint.keyword, target.keywords)) {
                        return null;
                    }

                        
                }
                else {
                    if (!DoesListContainAny<Keywords>(constraint.keyword, target.keywords))
                        return null;
                }

                break;

            case ConstraintType.WhosTurn:
                if (!CheckForPlayersTurn(constraint.whosTurn))
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
                            if (HasHighOrLowStat(constraint.mostStat, constraint.currentZone, target, true) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.LeastStat:
                            if (HasHighOrLowStat(constraint.leastStat, constraint.currentZone, target, false) == null)
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

    private bool CheckForMultipleTriggers(ConstraintList constraints) {

        constraints.counter++;

        if (constraints.counter >= constraints.triggersRequired) {
            constraints.counter = 0;
            return true;
        }
        else {
            return false;
        }

    }

    private bool CheckForPlayersTurn(OwnerConstraints playersTurn) {

        switch (playersTurn) {
            case OwnerConstraints.Mine:
                if (source.owner.myTurn)
                    return true;
                else
                    return false;


            case OwnerConstraints.Theirs:
                if (!source.owner.myTurn)
                    return true;
                else
                    return false;

            default:

                return false;
        }


    }

    private bool CheckCreatureStatAltered(ConstraintList constraint, CardStats stat, int statChangeValue, CardVisual target = null) {
        bool result = true;

        if (stat != constraint.statChanged)
            return false;


        if (stat == CardStats.Health && statChangeValue < 0) {
            Debug.Log(target.gameObject.name + " has taken " + Mathf.Abs(statChangeValue) + " point(s) of damage");
        }

        result = CheckStatGainedOrLost(constraint.gainedOrLost, statChangeValue);


        return result;
    }

    private bool CheckStatGainedOrLost(GainedOrLost gainedorLost, int value) {

        switch (gainedorLost) {
            case GainedOrLost.Gained:
                return value > 0;

            case GainedOrLost.Lost:
                return value < 0;

            default:

                return false;
        }
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


    private CardVisual HasHighOrLowStat(CardStats statToCompare, List<DeckType> zones, CardVisual target, bool highest) {
        CardVisual result = null;

        List<CardVisual> cardsToSearch = new List<CardVisual>();
        for (int i = 0; i < zones.Count; i++) {
            cardsToSearch.AddRange(Finder.FindCardsWithStatExtreme(statToCompare, zones[i], highest));
        }

        if (cardsToSearch.Contains(target))
            return target;


        //if(result != null)
        //    Debug.Log(result.gameObject.name + " is being sent back");

        return result;
    }

    private CardVisual IsDamagedOrUndamaged(CardVisual target, bool damaged) {
        CardVisual result = null;

        List<CardVisual> cardsToSearch = Finder.FindAllDamagedOrUndamagedCreatures(damaged);
        if (cardsToSearch.Contains(target))
            return target;


        return result;
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

                    case CardStats.Cost:
                        if (soul.essenceCost < stat.value) {
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

                    case CardStats.Cost:
                        if (soul.essenceCost > stat.value) {
                            return null;
                        }

                        break;
                }
            }


        }

        return result;
    }




    protected bool CheckAdditionalRequirements(Constants.AdditionalRequirement typeOfRequirement, ConstraintList constraint) {
        bool result = true;

        switch (typeOfRequirement) {
            case Constants.AdditionalRequirement.NumberofCardsInZone:

                switch (constraint.moreOrLess) {
                    case MoreOrLess.AtLeast:
                        if (CheckNumberOfCardsInZone(constraint.zoneToCheckForNumberOfCards, additionalRequirementConstraints).Count < constraint.numberOfcardsInZone) {
                            //Debug.Log("Not enough " + cardsInZoneConstraints.subtype[0].ToString() + " in " + zoneToCheckForNumberOfCards.ToString());
                            return false;
                        }

                        break;

                    case MoreOrLess.NoMoreThan:
                        if (CheckNumberOfCardsInZone(constraint.zoneToCheckForNumberOfCards, additionalRequirementConstraints).Count > constraint.numberOfcardsInZone) {
                            //Debug.Log("Too many " + cardsInZoneConstraints.subtype[0].ToString() + " in " + zoneToCheckForNumberOfCards.ToString());
                            return false;
                        }

                        break;
                }

                break;

            case Constants.AdditionalRequirement.RequireResource:
                if (!CheckForRequiredResource(additionalRequirementConstraints)) {
                    return false;
                }

                break;
        }

        return result;
    }

    protected bool CheckForRequiredResource(ConstraintList constraint) {
        bool result = false;

        GameResource targetResource = null;

        for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
            if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource.resourceType == constraint.requiredResourceType) {
                targetResource = source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource;
                //Debug.Log(targetResource.resourceType.ToString() + " exists in the list");
                break;
            }
        }

        if (targetResource == null)
            return false;

        if (constraint.consumeResource) {
            result = targetResource.RemoveResource(constraint.amountOfResourceRequried);

            //Debug.Log(result + " is the result of consuming resources");

            for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
                if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource == targetResource) {
                    source.owner.gameResourceDisplay.RPCUpdateResourceText(PhotonTargets.All, targetResource.resourceType);
                }
            }
        }

        else {
            if (targetResource.currentValue >= constraint.amountOfResourceRequried) {
                //Debug.Log("I had enough");
                result = true;
            }
            else {
                //Debug.Log("Not enough");
            }
        }

        return result;
    }



    protected List<CardVisual> CheckNumberOfCardsInZone(DeckType zone, ConstraintList constraint) {
        List<CardVisual> results = new List<CardVisual>();

        List<CardVisual> allCardsInZone = Finder.FindAllCardsInZone(zone);

        for (int i = 0; i < allCardsInZone.Count; i++) {
            CardVisual applicant = CheckConstraints(constraint, allCardsInZone[i]);

            if (applicant != null)
                results.Add(applicant);
        }




        return results;
    }



    #endregion




    #region Effect Methods


    public List<CardVisual> SpawnToken(ConstraintList spawnConstraints) {

        List<CardVisual> tokens = new List<CardVisual>();
        int spawnIndex = targetConstraints.numberOfSpawns;

        if (spawnIndex > targets.Count)
            spawnIndex = targets.Count;


        for (int i = 0; i < targetConstraints.numberOfSpawns; i++) {

            CardData tokenData = Resources.Load<CardData>("CardData/" + spawnConstraints.spawnableTokenDataName) as CardData;


            if (targetConstraints.copyTargets) {
                spawnConstraints.spawnCardType = targets[spawnIndex - 1].primaryCardType;
                tokenData = Resources.Load<CardData>("CardData/" + targets[spawnIndex - 1].cardData.name) as CardData;
            }

            string prefabName = Deck._allCards.GetCardPrefabNameByType(spawnConstraints.spawnCardType);

            //Debug.Log(prefabName);

            CardVisual tokenCard = source.owner.activeGrimoire.GetComponent<Deck>().CardFactory(tokenData, prefabName, GetDeckFromType(spawnConstraints.spawnTokenLocation, source));
            tokenCard.isToken = true;

            tokens.Add(tokenCard);

            if (targetConstraints.copyTargetsStatsOnly) {
                if (targets[spawnIndex - 1] is CreatureCardVisual) {
                    CreatureCardVisual soul = targets[spawnIndex - 1] as CreatureCardVisual;

                    tokenCard.RPCSetCardStats(PhotonTargets.All, soul.essenceCost, soul.attack, soul.size, soul.health);
                }
            }


            spawnIndex++;
        }

        return tokens;
    }


    private Deck GetDeckFromType(DeckType type, CardVisual card) {
        switch (type) {
            case DeckType.Battlefield:
                return card.owner.battlefield;

            case DeckType.Hand:
                return card.owner.myHand;

            case DeckType.Grimoire:
                return card.owner.activeGrimoire.GetComponent<Deck>();

            case DeckType.SoulCrypt:
                return card.owner.activeCrypt.GetComponent<Deck>();

            case DeckType.Void:
                return Deck._void;

            case DeckType.None:
                return null;

            default:
                return null;
        }
    }


    public void ForcedZoneChange(CardVisual target, ConstraintList zoneChangeConstraints) {

        target.currentDeck.RPCTransferCard(PhotonTargets.All, target, GetDeckFromType(zoneChangeConstraints.targetZone, target));
    }


    public void GrantKeywords(CardVisual target, List<Keywords> keywords) {

        for (int i = 0; i < keywords.Count; i++) {

            target.RPCAddKeyword(PhotonTargets.All, keywords[i], true);
        }

        //grantedKeywords.Add(target.photonView.viewID, keywords);

    }

    public void RemoveKeywords(CardVisual target, List<Keywords> keywords) {

        for (int i = 0; i < keywords.Count; i++) {
            target.RPCAddKeyword(PhotonTargets.All, keywords[i], false);
        }

        //removedKeywords.Add(target.photonView.viewID, keywords);

    }

    public void AddSpecialAttribute(CardVisual target, ConstraintList constraints) {
        target.RPCAddSpecialAttribute(PhotonTargets.All, constraints.grantedSpecialAttributeType, constraints.grantedSpecialAttributeValue);
    }

    public void ToggleSpecialAttributeSuspension(CardVisual target, ConstraintList constraints, bool suspend) {
        target.RPCToggleSpecialAttributeSuspension(PhotonTargets.All, constraints.grantedSpecialAttributeType, suspend);
    }



    public void GenerateResource(ConstraintList constraints) {

        for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
            if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource.resourceType == constraints.generatedResourceType) {
                source.owner.gameResourceDisplay.RPCAddResource(PhotonTargets.All, constraints.generatedResourceType, constraints.amountOfResourceToGenerate);
                return;
            }
        }

        source.owner.RPCSetupResources(PhotonTargets.All, false,
            constraints.generatedResourceType,
            constraints.amountOfResourceToGenerate,
            0,
            constraints.generatedResourceName,
            constraints.generatedResouceCap);
    }

    public void RemoveResource(ConstraintList constraints) {

        for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
            if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource.resourceType == constraints.generatedResourceType) {
                source.owner.gameResourceDisplay.RPCRemoveResource(PhotonTargets.All, constraints.generatedResourceType, constraints.amountOfResourceToGenerate);
                return;
            }
        }

    }


    #endregion



    public void CreateVFX() {

        for (int i = 0; i < targets.Count; i++) {
            GameObject atkVFX;

            if (movingVFX) {
                atkVFX = PhotonNetwork.Instantiate(abilityVFX, source.transform.position, Quaternion.identity, 0) as GameObject;
            }
            else {
                atkVFX = PhotonNetwork.Instantiate(abilityVFX, targets[i].transform.position, Quaternion.identity, 0) as GameObject;
            }


            CardVFX vfx = atkVFX.GetComponent<CardVFX>();

            if (targets[i] is CreatureCardVisual) {
                CreatureCardVisual soul = targets[i] as CreatureCardVisual;

                if (vfx.photonView.isMine) {
                    if (movingVFX) {
                        atkVFX.transform.SetParent(source.transform, false);
                        atkVFX.transform.localPosition = Vector3.zero;
                        vfx.target = soul.battleToken.incomingEffectLocation;
                        vfx.beginMovement = true;
                    }
                    else {
                        atkVFX.transform.SetParent(soul.battleToken.incomingEffectLocation, false);
                        atkVFX.transform.localPosition = Vector3.zero;
                    }
                }
            }

            vfx.RPCSetVFXAciveState(PhotonTargets.Others, true);


            //source.RPCDeployAttackEffect(PhotonTargets.All, atkVFX.GetPhotonView().viewID, targets[i], moveingVFX);
        }
    }




    private void InitializeStatAdjusments() {
        for (int i = 0; i < statAdjustments.Count; i++) {
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

        public bool valueSetBytargetStat;
        public DeriveStatsFromWhom deriveStatsFromWhom;
        public CardStats targetStat;
        public bool invertValue;
        public bool spellDamage;

        public StatAdjustment() {

        }

        public StatAdjustment(CardStats stat, int value, bool nonStacking, bool temp, CardVisual source) {
            this.stat = stat;
            this.value = value;
            this.nonStacking = nonStacking;
            this.source = source;
            //this.invertValue = inverse;
            temporary = temp;

            uniqueID = IDFactory.GenerateID();
        }

        public void AlterValueBasedOnTarget(CreatureCardVisual targetToBasevalueFrom) {
            if (!valueSetBytargetStat)
                return;

            if (targetToBasevalueFrom != null) {
                switch (targetStat) {
                    case CardStats.Cost:
                        value = targetToBasevalueFrom.essenceCost;
                        break;

                    case CardStats.Attack:
                        value = targetToBasevalueFrom.attack;
                        break;

                    case CardStats.Size:
                        value = targetToBasevalueFrom.size;
                        break;

                    case CardStats.Health:
                        value = targetToBasevalueFrom.health;
                        break;
                }
            }

            if (invertValue) {
                value = -value;
            }


            source.RPCUpdateStatAdjustment(PhotonTargets.Others, this, source, value);

        }

        public void ModifyValue(int modifierValue) {

            value += modifierValue;

            source.RPCUpdateStatAdjustment(PhotonTargets.Others, this, source, value);
        }

        public static List<StatAdjustment> CopyStats(CreatureCardVisual target) {
            List<StatAdjustment> results = new List<StatAdjustment>();

            StatAdjustment atk = new StatAdjustment(CardStats.Attack, target.attack, false, false, null);
            StatAdjustment siz = new StatAdjustment(CardStats.Size, target.size, false, false, null);
            StatAdjustment hth = new StatAdjustment(CardStats.Health, target.health, false, false, null);

            results.Add(atk);
            results.Add(siz);
            results.Add(hth);


            return results;
        }

        public static List<StatAdjustment> CreateStatSet(int atk, int siz, int hth) {
            List<StatAdjustment> results = new List<StatAdjustment>();

            StatAdjustment atka = new StatAdjustment(CardStats.Attack, atk, false, false, null);
            StatAdjustment siza = new StatAdjustment(CardStats.Size, siz, false, false, null);
            StatAdjustment htha = new StatAdjustment(CardStats.Health, hth, false, false, null);

            results.Add(atka);
            results.Add(siza);
            results.Add(htha);

            return results;
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
        public OwnerConstraints whosTurn;
        public List<CardType> primaryType = new List<CardType>();
        public bool notPrimaryType;
        public List<CardType> additionalType = new List<CardType>();
        public bool notAdditionalType;
        public List<Subtypes> subtype = new List<Subtypes>();
        public bool notSubtype;
        public List<Keywords> keyword = new List<Keywords>();
        public bool notKeyword;
        public List<Attunements> attunement = new List<Attunements>();
        public bool notAttunement;
        public List<DeckType> currentZone = new List<DeckType>();
        public bool notCurrentZone;
        public List<DeckType> previousZone = new List<DeckType>();
        public bool notPreviousZone;
        public List<Constants.CreatureStatus> creatureStatus = new List<Constants.CreatureStatus>();
        public List<GameResource.ResourceType> resources = new List<GameResource.ResourceType>();

        public List<StatAdjustment> minStats = new List<StatAdjustment>();
        public List<StatAdjustment> maxStats = new List<StatAdjustment>();

        public CardStats mostStat;
        public CardStats leastStat;
        public bool thisCardOnly;
        public bool oncePerTurn;
        public bool triggeredThisTurn;
        public bool neverTargetSelf;

        ////OnCombat
        //public AttackerOrDefender targetAttackerOrDefender;

        //Secondary Effect
        public bool triggerbySpecificAbility;
        public string triggerablePrimaryAbilityName;



        //Creature Stat Adjusted
        public GainedOrLost gainedOrLost;
        public CardStats statChanged;


        //Cards In Zone
        public DeckType zoneToCheckForNumberOfCards;
        public MoreOrLess moreOrLess;
        public int numberOfcardsInZone;


        //Spawn Token
        public bool copyTargets;
        public bool copyTargetsStatsOnly;
        public string spawnableTokenDataName;
        public DeckType spawnTokenLocation;
        public CardType spawnCardType;
        public int numberOfSpawns;

        //Zone Change Fields
        public DeckType targetZone;

        //Count Toward Effect
        public bool requireMultipleTriggers;
        public bool resetCountAtTurnEnd;
        public int triggersRequired;
        public int counter;

        //GenerateResource;
        public GameResource.ResourceType generatedResourceType;
        public int amountOfResourceToGenerate;
        public int generatedResouceCap;
        public string generatedResourceName;
        //Require Resource
        public GameResource.ResourceType requiredResourceType;
        public int amountOfResourceRequried;
        public bool consumeResource;

        //Special Attribute
        public SpecialAttribute.AttributeType grantedSpecialAttributeType;
        public int grantedSpecialAttributeValue;


    }

}
