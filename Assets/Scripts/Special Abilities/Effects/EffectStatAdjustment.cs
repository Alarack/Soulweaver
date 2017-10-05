using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardStats = Constants.CardStats;
using StatAdjustment = SpecialAbility.StatAdjustment;

[System.Serializable]
public class EffectStatAdjustment : Effect {


    public enum ValueSetMethod {
        Manual,
        DeriveValueFromTargetStat,
        DeriveValueFromCardsInZone,
        DeriveValueFromResource,
        SwapStats
    }

    public enum DeriveStatsFromWhom {
        TargetOFEffect,
        SourceOfEffect,
        TriggeringCard
    }

    public ValueSetMethod valueSetmethod;
    public List<StatAdjustment> adjustments = new List<StatAdjustment>();

    //Derive Stats From Target
    public DeriveStatsFromWhom deriveStatsFromWhom;
    public CardStats targetStat;
    public string targetAbilityName;

    //Derive Stats from Cards in Zone
    public Constants.DeckType zoneToCount;
    public SpecialAbility.ConstraintList constraints;

    //Derive Stats from Resource
    public GameResource.ResourceType targetResource;

    //Swap Stats
    public CardStats sourceStat;
    public CardStats destinationStat;


    public bool invertValue;
    public bool setStatToValue;
    public int maxValue;

    private List<int> baseAdjValues = new List<int>();

    public override void Initialize(CardVisual source, SpecialAbility parent) {
        base.Initialize(source, parent);

        InitializeAdjustments();
    }


    public override void Apply(CardVisual target) {

        switch (valueSetmethod) {
            case ValueSetMethod.Manual:
                ApplyStatAdjustment(target);
                break;

            case ValueSetMethod.DeriveValueFromTargetStat:

                DetermineSourceOfTargetStat();
                ApplyStatAdjustment(target);

                break;

            case ValueSetMethod.DeriveValueFromCardsInZone:
                SetAdjustmentValuesByCardsInZone();
                ApplyStatAdjustment(target);

                break;

            case ValueSetMethod.DeriveValueFromResource:
                SetAdjustmentValuesByResource();
                ApplyStatAdjustment(target);

                break;

            case ValueSetMethod.SwapStats:

                SwapStats(target);

                break;
        }
    }

    public override void Remove(CardVisual target) {
        RemoveStatAdjustments(target);
    }

    public void InitializeAdjustments() {
        for (int i = 0; i < adjustments.Count; i++) {

            if (source.owner != null)
                adjustments[i].uniqueID = IDFactory.GenerateAdjID(source.owner);

            adjustments[i].source = source;

            baseAdjValues.Add(adjustments[i].value);

            //source.CheckAdjID(adjustments[i].uniqueID, parentAbility.abilityName, source.cardData.cardName);

            //source.RPCCheckAdjID(PhotonTargets.All, adjustments[i].uniqueID, parentAbility.abilityName, source.cardData.cardName);
        }
    }


    //Apply All Adjustmetns to a target
    private void ApplyStatAdjustment(CardVisual target) {
        bool hasVFX = String.IsNullOrEmpty(parentAbility.abilityVFX);

        for (int i = 0; i < adjustments.Count; i++) {
            //source.RPCCheckAdjID(PhotonTargets.All, adjustments[i].uniqueID, parentAbility.abilityName);

            if (adjustments[i].spellDamage) {
                int spellDamage = Finder.FindTotalSpellDamage(Constants.OwnerConstraints.Mine);

                //Debug.Log(spellDamage + " is the amount of spelldamage reported");

                adjustments[i].value = baseAdjValues[i] -spellDamage;

                source.RPCUpdateSpecialAbilityStatAdjustment(PhotonTargets.Others, adjustments[i], source, adjustments[i].value);
            }


            target.RPCApplySpecialAbilityStatAdjustment(PhotonTargets.All, adjustments[i], source, !hasVFX, setStatToValue);
        }
    }


    private void SwapStats(CardVisual target) {

        int stat1 = target.GetStatValue(sourceStat, true);
        int stat2 = target.GetStatValue(destinationStat, true);

        StatAdjustment first = new StatAdjustment(sourceStat, stat2, false, true, source);
        StatAdjustment second = new StatAdjustment(destinationStat, stat1, false, true, source);

        //adjustments.Add(first);
        //adjustments.Add(second);
        source.RPCCreateStatAdjustment(PhotonTargets.All, first, parentAbility.abilityName);
        source.RPCCreateStatAdjustment(PhotonTargets.All, second, parentAbility.abilityName);

        ApplyStatAdjustment(target);
    }

    //When a stat adjustment is based on the value of another target's stat. Determine which target to derive that stat from.
    private void DetermineSourceOfTargetStat() {
        switch (deriveStatsFromWhom) {
            case DeriveStatsFromWhom.SourceOfEffect:

                SetAdjustmentValuesByTargetStat(source);
                break;

            case DeriveStatsFromWhom.TargetOFEffect:
                SetAdjustmentValuesByTargetStat(Finder.FindSpecialAbilityOnCardByName(source, targetAbilityName).targets[0]);
                break;

            case DeriveStatsFromWhom.TriggeringCard:
                SetAdjustmentValuesByTargetStat(parentAbility.triggeringCards[0]);

                break;
        }
    }


    //Update all my stat adjustments with the selecte tagret's chosen stat.
    private void SetAdjustmentValuesByTargetStat(CardVisual target) {
        if (!(target is CreatureCardVisual))
            return;

        CreatureCardVisual soul = target as CreatureCardVisual;

        for (int i = 0; i < adjustments.Count; i++) {
            adjustments[i].value = SetValueBasedOnTarget(soul, invertValue);
            source.RPCUpdateSpecialAbilityStatAdjustment(PhotonTargets.Others, adjustments[i], source, adjustments[i].value);
        }
    }


    //Determine which stat to derive a value from
    private int SetValueBasedOnTarget(CreatureCardVisual target, bool inverse) {
        int value = 0;

        if (target != null) {
            switch (targetStat) {
                case CardStats.Cost:
                    value = target.essenceCost;
                    break;

                case CardStats.Attack:
                    value = target.attack;
                    break;

                case CardStats.Size:
                    value = target.size;
                    break;

                case CardStats.Health:
                    value = target.health;
                    break;
            }
        }

        if (maxValue > 0 && value > maxValue) {
            value = maxValue;
        }

        if (inverse) {
            value = -value;
        }



        return value;

    }


    //Set all my stat adjustment values to be based on the number ofcards in a zone.
    private void SetAdjustmentValuesByCardsInZone() {
        for (int i = 0; i < adjustments.Count; i++) {
            adjustments[i].value = SetValueBasedOnCardsInZone(invertValue);
            source.RPCUpdateSpecialAbilityStatAdjustment(PhotonTargets.Others, adjustments[i], source, adjustments[i].value);
        }
    }


    //Get the number of cards in a zone with constraints applied.
    private int SetValueBasedOnCardsInZone(bool inverse) {
        int results = 0;

        results = parentAbility.CheckNumberOfCardsInZone(zoneToCount, constraints).Count;

        if (maxValue > 0 && results > maxValue) {
            results = maxValue;
        }

        if (inverse)
            results = -results;

        return results;
    }


    private void SetAdjustmentValuesByResource() {
        for (int i = 0; i < adjustments.Count; i++) {
            adjustments[i].value = SetValueByAmountOfResource(invertValue);
            baseAdjValues[i] = adjustments[i].value;

            source.RPCUpdateSpecialAbilityStatAdjustment(PhotonTargets.Others, adjustments[i], source, adjustments[i].value);
        }

    }

    private int SetValueByAmountOfResource(bool inverse) {

        int result = source.owner.gameResourceDisplay.GetCurrentResourceValueByType(targetResource);

        if (maxValue > 0 && result > maxValue) {
            result = maxValue;
        }

        if (inverse) {
            result = -result;
        }

        return result;
    }

    //Remove all my adjustments from a target
    private void RemoveStatAdjustments(CardVisual card) {
        //bool hasVFX = String.IsNullOrEmpty(parentAbility.abilityVFX);

        List<StatAdjustment> targetAdjustments = new List<StatAdjustment>();

        for (int i = 0; i < card.statAdjustments.Count; i++) {
            for (int j = 0; j < adjustments.Count; j++) {
                if (card.statAdjustments[i].uniqueID == adjustments[j].uniqueID) {
                    //Debug.Log("Match Found");
                    targetAdjustments.Add(card.statAdjustments[i]);
                }
                //else {
                //    Debug.Log(card.statAdjustments[i].uniqueID + " is the id I'm looking at and " + adjustments[j].uniqueID + " is my id");
                //}
            }
        }

        for (int i = 0; i < targetAdjustments.Count; i++) {
            card.RPCRemoveSpecialAbilityStatAdjustment(PhotonTargets.All, targetAdjustments[i].uniqueID, source, false, setStatToValue);
        }


    }


}
