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
        DeriveValueFromResource
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

    public bool invertValue;
    public int maxValue;


    public override void Initialize(CardVisual source, SpecialAbility parent) {
        base.Initialize(source, parent);

        //if(source.photonView.isMine)
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

        }
    }

    public override void Remove(CardVisual target) {
        RemoveStatAdjustments(target);
    }

    public void InitializeAdjustments() {
        for (int i = 0; i < adjustments.Count; i++) {
            adjustments[i].uniqueID = IDFactory.GenerateAdjID() + 1000;
            adjustments[i].source = source;

            //source.RPCCheckAdjID(PhotonTargets.All, adjustments[i].uniqueID, parentAbility.abilityName, source.cardData.cardName);
        }
    }


    //Apply All Adjustmetns to a target
    private void ApplyStatAdjustment(CardVisual target) {
        bool hasVFX = String.IsNullOrEmpty(parentAbility.abilityVFX);

        for (int i = 0; i < adjustments.Count; i++) {

            //Debug.Log("Applying " + adjustments[i].stat.ToString() + " adjustment of value " + adjustments[i].value.ToString() + " to " + target.cardData.name);
            //source.RPCCheckAdjID(PhotonTargets.All, adjustments[i].uniqueID, parentAbility.abilityName);
            target.RPCApplySpecialAbilityStatAdjustment(PhotonTargets.All, adjustments[i], source, !hasVFX);
        }
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
        for(int i = 0; i < adjustments.Count; i++) {
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
            }
        }

        for (int i = 0; i < targetAdjustments.Count; i++) {
            card.RPCRemoveSpecialAbilityStatAdjustment(PhotonTargets.All, targetAdjustments[i].uniqueID, source, false);
        }


    }


}
