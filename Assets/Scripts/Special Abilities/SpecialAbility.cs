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
using Duration = Constants.Duration;

[System.Serializable]
public abstract class SpecialAbility {

    [Header("Basic Info")]
    public List<AbilityActivationTrigger> trigger = new List<AbilityActivationTrigger>();
    public Duration effectDuration;
    public Duration triggerDuration;
    public ConstraintList triggerConstraints = new ConstraintList();
    public ConstraintList sourceConstraints = new ConstraintList();

    public EffectType effect;
    public string abilityName;
    public CardVisual source;
    public List<CardVisual> targets = new List<CardVisual>();
    //public bool recordAdjacentTargets;
    //public List<CardVisual> adjacentTargets = new List<CardVisual>();

    public List<CardVisual> triggeringCards = new List<CardVisual>();
    public ConstraintList targetConstraints = new ConstraintList();
    public string abilityVFX;
    public bool movingVFX;
    public bool clearTargetsOnEffectComplete;
    public bool clearTriggeringTargetFromOtherAbility;

    //Additional Requirements
    public List<Constants.AdditionalRequirement> additionalRequirements = new List<Constants.AdditionalRequirement>();
    public ConstraintList additionalRequirementConstraints = new ConstraintList();

    //Effects
    public EffectHolder effectHolder = new EffectHolder();



    //Editor Foldouts
    public bool togglePresets = true;
    public bool toggleTriggerOptions = true;
    public bool toggleSourceOptions;
    public bool toggleEffectOptions;
    public bool toggleTargetOptions = true;
    public bool toggleAdditonalRequirementOptions;



    [Serializable]
    public class EffectHolder {
        public List<EffectZoneChange> zoneChanges = new List<EffectZoneChange>();
        public List<EffectSpawnToken> tokenSpanws = new List<EffectSpawnToken>();
        public List<EffectStatAdjustment> statAdjustments = new List<EffectStatAdjustment>();
        public List<EffectGenerateResource> generateResources = new List<EffectGenerateResource>();
        public List<EffectAddorRemoveKeywords> addOrRemoveKeywords = new List<EffectAddorRemoveKeywords>();
        public List<EffectAddorRemoveSpecialAttribute> addOrRemoveSpecialAttribute = new List<EffectAddorRemoveSpecialAttribute>();
        public List<EffectChooseOne> chooseOne = new List<EffectChooseOne>();
        public List<EffectBestowSpecialAbility> bestowAbility = new List<EffectBestowSpecialAbility>();



        public void InitEffect<T>(List<T> effects, SpecialAbility parentAbility) where T : Effect {

            if (effects.Count < 1)
                return;

            for (int i = 0; i < effects.Count; i++) {
                effects[i].Initialize(parentAbility.source, parentAbility);
            }
        }


    }

    public enum ApplyEffectToWhom {
        TriggeringCard,
        CauseOfTrigger,
        Source
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

        //if(abilityName == "MakeDude") {
        //    Debug.Log("Init " + abilityName);
        //    Debug.Log(source.cardData.cardName + " is my source");
        //}

        RegisterListeners();
        //InitializeStatAdjusments();
        InitializeEffects();
        source.specialAbilities.Add(this);
    }

    public virtual void InitializeEffects() {

        effectHolder.InitEffect(effectHolder.statAdjustments, this);
        effectHolder.InitEffect(effectHolder.zoneChanges, this);
        effectHolder.InitEffect(effectHolder.tokenSpanws, this);
        effectHolder.InitEffect(effectHolder.generateResources, this);
        effectHolder.InitEffect(effectHolder.addOrRemoveKeywords, this);
        effectHolder.InitEffect(effectHolder.addOrRemoveSpecialAttribute, this);
        effectHolder.InitEffect(effectHolder.chooseOne, this);
        effectHolder.InitEffect(effectHolder.bestowAbility, this);

        //for (int i = 0; i < effectHolder.statAdjustments.Count; i++) {
        //    effectHolder.statAdjustments[i].Initialize(source, this);
        //}
        //for (int i = 0; i < effectHolder.zoneChanges.Count; i++) {
        //    effectHolder.zoneChanges[i].Initialize(source, this);
        //}
        //for (int i = 0; i < effectHolder.tokenSpanws.Count; i++) {
        //    effectHolder.tokenSpanws[i].Initialize(source, this);
        //}
        //for (int i = 0; i < effectHolder.generateResources.Count; i++) {
        //    effectHolder.generateResources[i].Initialize(source, this);
        //}
        //for (int i = 0; i < effectHolder.addOrRemoveKeywords.Count; i++) {
        //    effectHolder.addOrRemoveKeywords[i].Initialize(source, this);
        //}
        //for (int i = 0; i < effectHolder.addOrRemoveSpecialAttribute.Count; i++) {
        //    effectHolder.addOrRemoveSpecialAttribute[i].Initialize(source, this);
        //}
        //for (int i = 0; i < effectHolder.chooseOne.Count; i++) {
        //    effectHolder.chooseOne[i].Initialize(source, this);
        //}
        //for (int i = 0; i < effectHolder.bestowAbility.Count; i++) {
        //    effectHolder.bestowAbility[i].Initialize(source, this);
        //}
    }






    public List<StatAdjustment> GetAllStatAdjustments() {
        List<StatAdjustment> results = new List<StatAdjustment>();
        for (int i = 0; i < effectHolder.statAdjustments.Count; i++) {
            for (int j = 0; j < effectHolder.statAdjustments[i].adjustments.Count; j++) {
                results.Add(effectHolder.statAdjustments[i].adjustments[j]);
            }
        }
        return results;
    }

    public List<SpecialAttribute> GetAllSpecialAttributes() {
        List<SpecialAttribute> results = new List<SpecialAttribute>();
        for (int i = 0; i < effectHolder.addOrRemoveSpecialAttribute.Count; i++) {
            for (int j = 0; j < effectHolder.addOrRemoveSpecialAttribute[i].specialAttributes.Count; j++) {
                results.Add(effectHolder.addOrRemoveSpecialAttribute[i].specialAttributes[j]);
            }
        }

        return results;
    }

    protected virtual void Effect(CardVisual card) {
        //Debug.Log(abilityName + " is firing");

        if (effect != EffectType.RetriggerOtherEffect && effect != EffectType.RemoveOtherEffect) {
            if (!targets.Contains(card))
                targets.Add(card);
            //Debug.Log(abilityName + " is adding " + card.gameObject.name + " to its target list");
        }

        if (effect == EffectType.RetriggerOtherEffect) {
            SpecialAbility target = null;

            for (int i = 0; i < source.specialAbilities.Count; i++) {
                if (source.specialAbilities[i].abilityName == targetConstraints.abilityToRetrigger) {
                    target = source.specialAbilities[i];
                    break;
                }
            }

            if (target != null) {
                if (!target.targets.Contains(card)) {
                    target.targets.Add(card);
                    //Debug.Log(abilityName + " is adding " + card.gameObject.name + " to " + target.abilityName + "'s target list");
                }
            }
        }



        if (triggerConstraints.oncePerTurn)
            triggerConstraints.triggeredThisTurn = true;

        switch (effect) {
            case EffectType.StatAdjustment:
                for (int i = 0; i < effectHolder.statAdjustments.Count; i++) {
                    effectHolder.statAdjustments[i].Apply(card);
                }

                break;

            case EffectType.SpawnToken:
                for (int i = 0; i < effectHolder.tokenSpanws.Count; i++) {
                    effectHolder.tokenSpanws[i].Apply(card);
                }

                break;

            case EffectType.ZoneChange:
                for (int i = 0; i < effectHolder.zoneChanges.Count; i++) {
                    effectHolder.zoneChanges[i].Apply(card);
                }

                break;

            case EffectType.AddOrRemoveKeywordAbilities:

                for (int i = 0; i < effectHolder.addOrRemoveKeywords.Count; i++) {
                    effectHolder.addOrRemoveKeywords[i].Apply(card);
                }

                break;

            case EffectType.GenerateResource:
                for (int i = 0; i < effectHolder.generateResources.Count; i++) {
                    effectHolder.generateResources[i].Apply(card);
                }

                break;

            case EffectType.AddOrRemoveSpecialAttribute:
                for (int i = 0; i < effectHolder.addOrRemoveSpecialAttribute.Count; i++) {
                    effectHolder.addOrRemoveSpecialAttribute[i].Apply(card);
                }

                break;

            case EffectType.ChooseOne:
                for (int i = 0; i < effectHolder.chooseOne.Count; i++) {
                    effectHolder.chooseOne[i].Apply(card);
                }

                break;

            case EffectType.BestowAbility:
                for (int i = 0; i < effectHolder.bestowAbility.Count; i++) {
                    effectHolder.bestowAbility[i].Apply(card);
                }

                break;

            case EffectType.RemoveOtherEffect:
                RemoveOtherEffect(targetConstraints);
                break;

            case EffectType.RetriggerOtherEffect:
                RetriggerEffect(targetConstraints);
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

        triggeringCards.Clear();

        if (clearTargetsOnEffectComplete)
            source.ClearAllSpecialAbilityTargets();


        if (clearTriggeringTargetFromOtherAbility) {
            RemoveTargetFromSpecificAbility(triggerConstraints.abilityToGatherTargetsFrom, card);

            //List<CardVisual> otherTargets = FindTargetsFromAnotherAbility(triggerConstraints);
            //if (otherTargets.Contains(card)) {
            //}
        }

        if (!(String.IsNullOrEmpty(abilityVFX))) {
            CreateSingleTargetVFX(card);
        }
        //else {
        //    //Debug.Log(source.gameObject.name + " has an ability: " + abilityName + " that has no vfx. It is targeting " + card.gameObject.name);

        //    //source.RPCBroadCastNoVFXImpactEvent(PhotonTargets.All, card);

        //}



    }

    protected virtual void RemoveEffect(List<CardVisual> cards) {
        for (int i = 0; i < cards.Count; i++) {
            switch (effect) {

                case EffectType.StatAdjustment:
                    //RemoveStatAdjustments(cards[i]);

                    for (int j = 0; j < effectHolder.statAdjustments.Count; j++) {
                        effectHolder.statAdjustments[j].Remove(cards[i]);
                    }

                    break;

                case EffectType.SpawnToken:

                    break;

                case EffectType.ZoneChange:
                    for (int j = 0; j < effectHolder.zoneChanges.Count; j++) {
                        effectHolder.zoneChanges[j].Remove(cards[i]);
                    }
                    break;

                case EffectType.AddOrRemoveKeywordAbilities:
                    //RemoveKeywords(cards[i], keywordsToAddorRemove);

                    for (int j = 0; j < effectHolder.addOrRemoveKeywords.Count; j++) {
                        effectHolder.addOrRemoveKeywords[j].Remove(cards[i]);
                    }

                    break;

                case EffectType.GenerateResource:
                    //RemoveResource(targetConstraints);
                    for (int j = 0; j < effectHolder.generateResources.Count; j++) {
                        effectHolder.generateResources[j].Remove(cards[i]);
                    }

                    break;

                case EffectType.AddOrRemoveSpecialAttribute:
                    for (int j = 0; j < effectHolder.addOrRemoveSpecialAttribute.Count; j++) {
                        effectHolder.addOrRemoveSpecialAttribute[j].Remove(cards[i]);
                    }
                    break;
            }
        }

        ClearTargets();
        Unsubscribe();
    }
    //protected void ApplyStatAdjustments(CardVisual card) {
    //    for (int i = 0; i < statAdjustments.Count; i++) {

    //        if (statAdjustments[i].valueSetBytargetStat) {

    //            switch (statAdjustments[i].deriveStatsFromWhom) {
    //                case DeriveStatsFromWhom.SourceOfEffect:
    //                    statAdjustments[i].AlterValueBasedOnTarget(source as CreatureCardVisual);
    //                    break;

    //                case DeriveStatsFromWhom.TargetOFEffect:
    //                    statAdjustments[i].AlterValueBasedOnTarget(card as CreatureCardVisual);
    //                    break;
    //            }
    //        }

    //        int spelldamage = Finder.FindTotalSpellDamage();

    //        if (statAdjustments[i].spellDamage) {
    //            ApplySpellDamge(statAdjustments[i], -spelldamage);
    //        }

    //        card.RPCApplySpecialAbilityStatAdjustment(PhotonTargets.All, statAdjustments[i], source);

    //    }
    //}

    private void ApplySpellDamge(StatAdjustment adjustment, int spellDamage) {
        adjustment.ModifyValue(spellDamage);
    }

    protected void RemoveOtherEffect(ConstraintList constraint) {
        SpecialAbility target = null;

        for (int i = 0; i < source.specialAbilities.Count; i++) {
            if (source.specialAbilities[i].abilityName == constraint.abilityToRemove)
                target = source.specialAbilities[i];
        }

        if (target != null) {
            target.RemoveEffect(target.targets);
        }

    }

    protected void RetriggerEffect(ConstraintList constraint) {

        if (!source.photonView.isMine)
            return;

        SpecialAbility target = null;

        for (int i = 0; i < source.specialAbilities.Count; i++) {
            if (source.specialAbilities[i].abilityName == constraint.abilityToRetrigger)
                target = source.specialAbilities[i];
        }

        if (target != null) {

            Debug.Log(abilityName + " is retriggering " + constraint.abilityToRetrigger);

            if (target is LogicTargetedAbility) {
                LogicTargetedAbility lta = target as LogicTargetedAbility;

                if (!ManageConstraints(lta.source, lta)) {
                    return;
                }

                lta.ProcessEffect(lta.source);
            }

            target.ActivateTargeting();
        }
    }

    public void UnregisterListeners() {
        Grid.EventManager.RemoveMyListeners(this);
    }

    public virtual void RegisterListeners() {

        if (!source.photonView.isMine)
            return;


        //Trigger Duration
        if (triggerDuration == Duration.EndOfTurn) {
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEndTriggerDuration);
            Grid.EventManager.RegisterListener(GameEvent.CardLeftZone, ResetTriggerDuration);
        }
        if (triggerDuration == Duration.StartOfTurn) {
            Grid.EventManager.RegisterListener(GameEvent.TurnStarted, OnTurnEndTriggerDuration);
            Grid.EventManager.RegisterListener(GameEvent.CardLeftZone, ResetTriggerDuration);
        }





        //Effect Duration
        if (effectDuration == Duration.EndOfTurn)
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEndDuration);

        if (effectDuration == Duration.StartOfTurn)
            Grid.EventManager.RegisterListener(GameEvent.TurnStarted, OnTurnStartEndDuration);

        if (effectDuration == Duration.WhileInZone)
            Grid.EventManager.RegisterListener(GameEvent.CardLeftZone, OnLeavesZoneEndDuration);


        //Once per turn and resent coutner at end of turn checks
        if (triggerConstraints.resetCountAtTurnEnd && triggerConstraints.requireMultipleTriggers)
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, ResetCounterOnTurnEnd);

        if (triggerConstraints.oncePerTurn)
            Grid.EventManager.RegisterListener(GameEvent.TurnStarted, RefreshOncePerTurnCounter);


        //Trigger Effects
        if (source.primaryCardType == CardType.Domain && trigger.Contains(AbilityActivationTrigger.UserActivated))
            Grid.EventManager.RegisterListener(GameEvent.UserActivatedDomainAbility, OnUserActiveDomainAbility);

        if (trigger.Contains(AbilityActivationTrigger.EntersZone))
            Grid.EventManager.RegisterListener(GameEvent.CardEnteredZone, OnEnterZone);

        if (trigger.Contains(AbilityActivationTrigger.LeavesZone))
            Grid.EventManager.RegisterListener(GameEvent.CardLeftZone, OnLeavesZone);

        if (trigger.Contains(AbilityActivationTrigger.CreatureStatChanged))
            Grid.EventManager.RegisterListener(GameEvent.CreatureStatAdjusted, OnCreatureStatAdjusted);

        if (trigger.Contains(AbilityActivationTrigger.Attacks))
            Grid.EventManager.RegisterListener(GameEvent.CharacterAttacked, OnAttack);

        if (trigger.Contains(AbilityActivationTrigger.Defends))
            Grid.EventManager.RegisterListener(GameEvent.CharacterDefends, OnCombatDefense);

        if (source.primaryCardType != CardType.Domain && trigger.Contains(AbilityActivationTrigger.UserActivated))
            Grid.EventManager.RegisterListener(GameEvent.UserActivatedAbilityInitiated, OnUserActivation);

        if (trigger.Contains(AbilityActivationTrigger.SecondaryEffect))
            Grid.EventManager.RegisterListener(GameEvent.TriggerSecondaryEffect, OnEffectComplete);

        if (trigger.Contains(AbilityActivationTrigger.TurnEnds))
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEnd);

        if (trigger.Contains(AbilityActivationTrigger.TurnStarts))
            Grid.EventManager.RegisterListener(GameEvent.TurnStarted, OnTurnStart);

        if (trigger.Contains(AbilityActivationTrigger.Slain))
            Grid.EventManager.RegisterListener(GameEvent.CreatureDied, OnCreatureSlain);

        if (trigger.Contains(AbilityActivationTrigger.ResourceChanged))
            Grid.EventManager.RegisterListener(GameEvent.ResourceChanged, OnResourceChanged);

    }

    #region EVENTS

    protected void OnTurnEndTriggerDuration(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.owner == player) {
            return;
        }

        if (!source.photonView.isMine)
            return;

        if (!triggerConstraints.suspendTrigger)
            triggerConstraints.suspendTrigger = true;
    }

    protected void ResetTriggerDuration(EventData data) {
        if (!source.photonView.isMine)
            return;

        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (card != source)
            return;

        if (deck.decktype != DeckType.SoulCrypt)
            return;


        if (triggerConstraints.suspendTrigger)
            triggerConstraints.suspendTrigger = false;
    }


    protected void OnTurnEndDuration(EventData data) {
        if (targets.Count < 1)
            return;

        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.owner == player) {
            return;
        }

        if (!source.photonView.isMine)
            return;

        if (CheckForPlayersTurn(OwnerConstraints.Mine)) {
            //Debug.Log(player.gameObject.name + " is the player who started after checks");
            Debug.Log("End of turn duration is ending");
            RemoveEffect(targets);
        }

    }

    protected void OnTurnStartEndDuration(EventData data) {
        if (targets.Count < 1)
            return;

        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.owner == player) {
            return;
        }

        if (!source.photonView.isMine)
            return;

        if (CheckForPlayersTurn(OwnerConstraints.Mine)) {
            //Debug.Log(player.gameObject.name + " is the player who started after checks");

            Debug.Log("start of turn duration  is ending");
            RemoveEffect(targets);
        }

    }

    protected void OnLeavesZoneEndDuration(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (card == source && sourceConstraints.currentZone.Contains(deck.decktype)) {
            RemoveEffect(targets);
        }


    }

    protected void OnResourceChanged(EventData data) {
        //GameResource.ResourceType type = (GameResource.ResourceType)data.GetInt("ResourceType");
        int value = data.GetInt("Value");
        Player player = data.GetMonoBehaviour("Player") as Player;

        //Debug.Log(type.ToString() + " has changed by a value of " + value);

        if (player != source.owner)
            return;

        if (!source.photonView.isMine)
            return;

        if (!CheckForResourceGainedOrLost(triggerConstraints, value))
            return;

        //Debug.Log(type.ToString() + " change has passed constraints");

        if (!ManageConstraints(source, this)) {
            return;
        }

        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {
                ProcessEffect(source);
            }
        }

        ActivateTargeting();


    }

    protected void OnCreatureSlain(EventData data) {
        CardVisual deadCard = data.GetMonoBehaviour("DeadCard") as CardVisual;
        CardVisual causeOfdeath = data.GetMonoBehaviour("CauseOfDeath") as CardVisual;

        if (!source.photonView.isMine)
            return;

        if (causeOfdeath != source)
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

        if (!ManageConstraints(effectTarget, this)) {
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

        //Debug.Log(abilityName + " is triggering");

        if (card != source)
            return;

        List<CardVisual> primaryTargets = Finder.FindSpecialAbilityOnCardByName(card, triggeringAbilityName).targets;

        if (triggerConstraints.triggerbySpecificAbility && triggerConstraints.triggerablePrimaryAbilityName != triggeringAbilityName)
            return;

        //Debug.Log("triggering a secondary effect: " + abilityName + " from " + source.cardData.cardName);

        if (!source.photonView.isMine)
            return;


        //Debug.Log(abilityName + " is triggering");


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

        //DomainTile tile = data.GetMonoBehaviour("Tile") as DomainTile;
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;


        if (card != source)
            return;

        if (!ManageConstraints(card, this)) {
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

        //if (card.primaryCardType == CardType.Domain)
        //    return;

        if (!ManageConstraints(card, this)) {
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

        if (!ManageConstraints(source, this)) {
            return;
        }

        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {
                ProcessEffect(source);
            }
        }

        ActivateTargeting();

        //if (abilityName == "RoundBlaze")
        //    Debug.Log(source.gameObject.name + " is triggering an on turn start event");
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


        if (triggerConstraints.whosTurn == OwnerConstraints.None) {
            if (!source.owner == player) {
                return;
            }
        }


        if (triggerConstraints.resetCountAtTurnEnd)
            triggerConstraints.counter = 0;

        if (!ManageConstraints(source, this)) {
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
        //Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (!ManageConstraints(card, this)) {
            return;
        }

        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {

                ProcessEffect(card);
            }
        }

        ActivateTargeting();
    }

    protected void OnLeavesZone(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        //Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (!source.photonView.isMine)
            return;

        //if (card == source)
        //Debug.Log(card.gameObject.name + " has left " + deck.decktype.ToString());

        if (!ManageConstraints(card, this)) {
            return;
        }

        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {

                ProcessEffect(card);
            }
        }

        //if (card == source)
        //Debug.Log(card.gameObject.name + " has passed constraint testing ");

        ActivateTargeting();
    }

    protected void OnAttack(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        if (!source.photonView.isMine)
            return;

        //Debug.Log(card.gameObject.name + " is attacking");

        if (!ManageConstraints(card, this)) {
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

        if (!ManageConstraints(card, this)) {
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
        CardVisual target = data.GetMonoBehaviour("Target") as CardVisual;
        CardVisual sourceOfAdjustment = data.GetMonoBehaviour("Source") as CardVisual;

        if (!source.photonView.isMine)
            return;


        if (source.currentDeck.decktype == DeckType.Battlefield) {

            //if (source.photonView.isMine) {
            //    Debug.Log("[Special Ability - MINE] " + abilityName + " " + target.gameObject.name + " :: " + target.cardData.cardName + " is the target");
            //    Debug.Log("[Special Ability - MINE] " + abilityName + " " + sourceOfAdjustment.gameObject.name + " :: " + sourceOfAdjustment.cardData.cardName + " is the source");
            //}
            //else {
            //    Debug.Log("[Special Ability - THEIRS] " + target.gameObject.name + " :: " + target.cardData.cardName + " is the target");
            //    Debug.Log("[Special Ability - THEIRS] " + sourceOfAdjustment.gameObject.name + " :: " + sourceOfAdjustment.cardData.cardName + " is the source");
            //}


            //Debug.Log(target.gameObject.name + " :: " + target.cardData.cardName + " has had " + stat.ToString() + " altered by " + sourceOfAdjustment.gameObject.name + " :: " + sourceOfAdjustment.cardData.cardName);

        }



        if (triggerConstraints.thisCardAdjusted && target != source)
            return;

        if (triggerConstraints.thisCardAdjusts && sourceOfAdjustment != source)
            return;

        //if(target == source || sourceOfAdjustment == source)
        //    Debug.Log(target.gameObject.name + " :: " + target.cardData.cardName + " has had " + stat.ToString() + " altered by " + sourceOfAdjustment.gameObject.name + " :: " + sourceOfAdjustment.cardData.cardName);


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

        if (!ManageConstraints(effectTarget, this)) {
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

    protected bool ManageConstraints(CardVisual triggeringCard, SpecialAbility ability) {
        bool result = true;

        if (ability.triggerConstraints.suspendTrigger)
            return false;

        if (ability.triggerConstraints.thisCardOnly && triggeringCard != source) {
            //Debug.Log(source.gameObject.name + " can only trigger its own effect and " + target.gameObject.name + " has happened");
            return false;
        }

        if (ability.triggerConstraints.neverTargetSelf && triggeringCard == source)
            return false;

        if (ability.triggerConstraints.oncePerTurn && ability.triggerConstraints.triggeredThisTurn)
            return false;

        //if (targetConstraints.neverTargetSelf && triggeringCard == source)
        //    return false;

        if (ability.CheckConstraints(ability.triggerConstraints, triggeringCard) == null) {
            //Debug.Log("Trigger is not in place");

            return false;
        }

        if (ability.CheckConstraints(ability.sourceConstraints, source) == null) {
            //Debug.Log("Source is not in place for" + source.gameObject.name + "'s ability");
            //Debug.Log("Source is not in place");
            return false;
        }

        for (int i = 0; i < ability.additionalRequirements.Count; i++) {
            if (!ability.CheckAdditionalRequirements(ability.additionalRequirements[i], ability.additionalRequirementConstraints))
                return false;
        }

        ability.triggeringCards.Add(triggeringCard);

        if (ability.triggerConstraints.requireMultipleTriggers) {
            result = ability.CheckForMultipleTriggers(ability.triggerConstraints);
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

    public void ClearTargets() {
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
                        return null;
                    }

                }

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
                    if (DoesListContainAny<Keywords>(constraint.keyword, target.keywords))
                        return null;
                }
                else {
                    if (!DoesListContainAny<Keywords>(constraint.keyword, target.keywords))
                        return null;
                }

                break;

            case ConstraintType.SpecialAttribute:
                if (constraint.notSpecialAttribute) {
                    if (DoesHaveSpecialAttribute(constraint, target))
                        return null;
                }
                else {
                    if (!DoesHaveSpecialAttribute(constraint, target))
                        return null;
                }

                break;

            case ConstraintType.WhosTurn:
                if (!CheckForPlayersTurn(constraint.whosTurn))
                    return null;

                break;

            case ConstraintType.CanAttack:
                if (!CreatureCanAttack(target, constraint))
                    return null;

                break;

            case ConstraintType.OtherTargets:
                if (!DoesTargetExistInOtherAbilityTargetList(target, constraint))
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
                            if (HasHighOrLowStat(constraint.mostStat, constraint.currentZone, target, true, constraint.owner) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.LeastStat:
                            if (HasHighOrLowStat(constraint.leastStat, constraint.currentZone, target, false, constraint.owner) == null)
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

    private bool DoesHaveSpecialAttribute(ConstraintList constraints, CardVisual target) {
        bool result = false;

        List<SpecialAttribute.AttributeType> targetAttributes = new List<SpecialAttribute.AttributeType>();

        for (int i = 0; i < target.specialAttributes.Count; i++) {
            targetAttributes.Add(target.specialAttributes[i].attributeType);

        }

        if (DoesListContainAny<SpecialAttribute.AttributeType>(constraints.specialAttributes, targetAttributes)) {
            return true;
        }


        return result;
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


        //if (stat == CardStats.Health && statChangeValue < 0) {
        //    Debug.Log(target.gameObject.name + " has taken " + Mathf.Abs(statChangeValue) + " point(s) of damage");
        //}

        result = CheckStatGainedOrLost(constraint.statGainedOrLost, statChangeValue);


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

    private CardVisual HasHighOrLowStat(CardStats statToCompare, List<DeckType> zones, CardVisual target, bool highest, OwnerConstraints owner) {
        CardVisual result = null;

        List<CardVisual> cardsToSearch = new List<CardVisual>();
        for (int i = 0; i < zones.Count; i++) {
            //Debug.Log("Sending " + zones[i].ToString() + " to the finder method");
            cardsToSearch.AddRange(Finder.FindCardsWithStatExtreme(statToCompare, zones[i], highest, owner));
        }

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

                switch (constraint.moreOrLessCards) {
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
        bool result = true;

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

        switch (constraint.moreOrlessResource) {
            case MoreOrLess.AtLeast:
                if (targetResource.currentValue < constraint.amountOfResourceRequried)
                    return false;

                break;

            case MoreOrLess.NoMoreThan:
                if (targetResource.currentValue > constraint.amountOfResourceRequried)
                    return false;

                break;
        }

        if (constraint.consumeResource) {
            result = targetResource.RemoveResource(constraint.amountOfResourceRequried);

            for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
                if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource == targetResource) {
                    source.owner.gameResourceDisplay.RPCUpdateResourceText(PhotonTargets.All, targetResource.resourceType);
                }
            }
        }

        return result;
    }

    protected bool CheckForResourceGainedOrLost(ConstraintList constraints, int value) {
        bool result = false;

        GameResource targetResource = null;

        for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
            if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource.resourceType == constraints.resourceThatChanged) {
                targetResource = source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource;
                break;
            }
        }

        if (targetResource == null)
            return false;


        switch (constraints.resourceGainedOrLost) {
            case GainedOrLost.Gained:
                if (value > 0) {
                    return true;
                }
                break;

            case GainedOrLost.Lost:
                if (value < 0) {
                    return true;
                }

                break;
        }




        return result;
    }

    public List<CardVisual> CheckNumberOfCardsInZone(DeckType zone, ConstraintList constraint) {
        List<CardVisual> results = new List<CardVisual>();
        List<CardVisual> allCardsInZone = Finder.FindAllCardsInZone(zone);

        for (int i = 0; i < allCardsInZone.Count; i++) {
            CardVisual applicant = CheckConstraints(constraint, allCardsInZone[i]);

            if (applicant != null)
                results.Add(applicant);
        }

        return results;
    }

    public bool CreatureCanAttack(CardVisual target, ConstraintList constraint) {

        if (!(target is CreatureCardVisual)) {
            return false;
        }

        CreatureCardVisual soul = target as CreatureCardVisual;

        return soul.CanAttack() == constraint.creatureCanAttack;

    }

    public bool DoesTargetExistInOtherAbilityTargetList(CardVisual target, ConstraintList constraint) {

        List<CardVisual> otherTargets = FindTargetsFromAnotherAbility(constraint);

        if (otherTargets == null || otherTargets.Count == 0) {
            //Debug.Log(abilityName + " tried to find the targets from " + constraint.abilityToGatherTargetsFrom + " but it was null");
            return false;
        }

        else return otherTargets.Contains(target);
    }

    #endregion


    protected virtual List<CardVisual> FindTargetsFromAnotherAbility(ConstraintList constraints) {

        //Debug.Log(abilityName + " is searching for another ability called " + constraints.abilityToGatherTargetsFrom);


        SpecialAbility targetAbility = null;

        for (int i = 0; i < source.specialAbilities.Count; i++) {
            if (source.specialAbilities[i].abilityName == constraints.abilityToGatherTargetsFrom) {
                targetAbility = source.specialAbilities[i];
                //Debug.Log(abilityName + " has found another ability called " + constraints.abilityToGatherTargetsFrom);
                break;
            }
        }

        if (targetAbility != null) {
            return targetAbility.targets;
        }
        else {
            //Debug.Log(abilityName + " could NOT find another ability called " + constraints.abilityToGatherTargetsFrom);

            return null;
        }

    }

    protected virtual void RemoveTargetFromSpecificAbility(string abilityName, CardVisual target) {
        SpecialAbility targetAbility = null;

        for (int i = 0; i < source.specialAbilities.Count; i++) {
            if (source.specialAbilities[i].abilityName == abilityName) {
                targetAbility = source.specialAbilities[i];
                break;
            }
        }

        if (targetAbility != null) {
            if (targetAbility.targets.Contains(target))
                targetAbility.targets.Remove(target);
        }

    }


    private void CreateSingleTargetVFX(CardVisual target) {
        GameObject atkVFX;

        if (movingVFX) {
            atkVFX = PhotonNetwork.Instantiate(abilityVFX, source.transform.position, Quaternion.identity, 0) as GameObject;
        }
        else {
            atkVFX = PhotonNetwork.Instantiate(abilityVFX, target.transform.position, Quaternion.identity, 0) as GameObject;
        }

        CardVFX vfx = atkVFX.GetComponent<CardVFX>();

        if (target is CreatureCardVisual) {
            CreatureCardVisual soul = target as CreatureCardVisual;

            if (vfx.photonView.isMine) {
                vfx.Initialize(soul, movingVFX);
            }
        }

        vfx.RPCSetVFXAciveState(PhotonTargets.Others, true);
    }

    public void CreateAllTargetsVFX() {

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
                    //Debug.Log(" CreateVFX " + soul.gameObject.name + " is the target of " + abilityName);
                    vfx.Initialize(soul, movingVFX);

                    //if (movingVFX) {
                    //    //atkVFX.transform.SetParent(source.transform, false);
                    //    //atkVFX.transform.localPosition = Vector3.zero;

                    //    //vfx.target = soul.battleToken.incomingEffectLocation;
                    //    //vfx.beginMovement = true;
                    //}
                    //else {
                    //    //atkVFX.transform.SetParent(soul.battleToken.incomingEffectLocation, false);
                    //    //atkVFX.transform.localPosition = Vector3.zero;
                    //}
                }
            }

            vfx.RPCSetVFXAciveState(PhotonTargets.Others, true);


            //source.RPCDeployAttackEffect(PhotonTargets.All, atkVFX.GetPhotonView().viewID, targets[i], moveingVFX);
        }
    }

    [Serializable]
    public class StatAdjustment {
        public Constants.CardStats stat;
        public int value;
        public bool nonStacking;
        public bool temporary;
        public CardVisual source;
        public int uniqueID = -1;

        public bool spellDamage;


        public StatAdjustment() {

        }

        public StatAdjustment(CardStats stat, int value, bool nonStacking, bool temp, CardVisual source) {
            this.stat = stat;
            this.value = value;
            this.nonStacking = nonStacking;
            this.source = source;
            temporary = temp;


        }

        public void ModifyValue(int modifierValue) {

            value += modifierValue;

            source.RPCUpdateSpecialAbilityStatAdjustment(PhotonTargets.Others, this, source, value);
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


    [Serializable]
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
        public List<SpecialAttribute.AttributeType> specialAttributes = new List<SpecialAttribute.AttributeType>();
        public bool notSpecialAttribute;
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

        public bool suspendTrigger;

        ////Adjacent Targets
        //public bool applyToAdjacenTagets;
        //public string abilityNameToTargetAdjacency;

        //Secondary Effect
        public bool triggerbySpecificAbility;
        public string triggerablePrimaryAbilityName;

        //Remove other effect
        public string abilityToRemove;

        //RetriggerEffect
        public string abilityToRetrigger;

        //Creature Stat Adjusted
        public GainedOrLost statGainedOrLost;
        public CardStats statChanged;
        public bool thisCardAdjusts;
        public bool thisCardAdjusted;

        //Creature Can Attack
        public bool creatureCanAttack;

        //Cards In Zone
        public DeckType zoneToCheckForNumberOfCards;
        public MoreOrLess moreOrLessCards;
        public int numberOfcardsInZone;

        //Count Toward Effect
        public bool requireMultipleTriggers;
        public bool resetCountAtTurnEnd;
        public int triggersRequired;
        public int counter;

        //Require Resource
        public GameResource.ResourceType requiredResourceType;
        public MoreOrLess moreOrlessResource;
        public int amountOfResourceRequried;
        public bool consumeResource;

        //Resource Changed
        public GameResource.ResourceType resourceThatChanged;
        public GainedOrLost resourceGainedOrLost;

        //Trigger On Targets From Another Ability
        public string abilityToGatherTargetsFrom;

    }

}
