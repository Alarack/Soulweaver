using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LogicTargetedAbility : SpecialAbility {


    public enum LogicTargeting {
        NoTargetsNeeded,
        AllValidTargets,
        NumberOfValidTargets,
        OnlyTargetTriggeringCard,
        UseTargetsFromOtherAbility,
        TriggeringCardsOwner,
        AdjacentTagets
    }




    public LogicTargeting logicTargetingMethod;


    public int numberofTargets;
    public bool processEffectOnPrimaryEffectTargets;
    public string targetAbilityName;
    //public bool onlyThisCard;


    private List<CardVisual> validTargets = new List<CardVisual>();



    public override void Initialize(CardVisual owner) {
        base.Initialize(owner);

        //if (abilityName == "RoundBlaze")
        //    Debug.Log(numberofTargets + " during init");
    }


    public override bool ProcessEffect(CardVisual card) {

        switch (logicTargetingMethod) {
            case LogicTargeting.AllValidTargets:
                validTargets = GatherValidTargets();

                //Debug.Log(validTargets.Count + " is the number of valid targets");

                for (int i = 0; i < validTargets.Count; i++) {
                    //Debug.Log("doin' stuff to " + validTargets[i].gameObject.name);
                    Effect(validTargets[i]);
                }

                validTargets.Clear();

                break;

            case LogicTargeting.NumberOfValidTargets:
                validTargets = GatherValidTargets();

                int tempTargetNum = numberofTargets;

                if (tempTargetNum > validTargets.Count) {
                    tempTargetNum = validTargets.Count;
                }

                for (int i = 0; i < tempTargetNum; i++) {


                    Effect(validTargets[i]);
                }

                //Debug.Log("Processing Valid");

                validTargets.Clear();

                break;


            case LogicTargeting.NoTargetsNeeded:

                if(CheckConstraints(targetConstraints, source) != null)
                    Effect(source);

                break;

            case LogicTargeting.OnlyTargetTriggeringCard:
                if (CheckConstraints(targetConstraints, card) != null) {
                    Effect(card);

                }

                break;

            case LogicTargeting.UseTargetsFromOtherAbility:
                List<CardVisual> otherTargets = FindTargetsFromAnotherAbility();

                for(int i = 0; i < otherTargets.Count; i++) {
                    if (CheckConstraints(targetConstraints, otherTargets[i]) != null)
                        Effect(otherTargets[i]);

                }

                break;

            case LogicTargeting.TriggeringCardsOwner:

                CardVisual targetOwner = Finder.FindCardsOwner(card);

                if (CheckConstraints(targetConstraints, targetOwner) != null) {
                    Effect(targetOwner);

                }

                break;

            case LogicTargeting.AdjacentTagets:
                //CardVisual rightOfTarget = damageTaker.owner.battleFieldManager.GetCardToTheRight(damageTaker);
                List<CardVisual> otherTargets2 = FindTargetsFromAnotherAbility();


                CardVisual leftcard = otherTargets2[0].owner.battleFieldManager.GetCardToTheLeft(otherTargets2[0]);
                CardVisual rightCard = otherTargets2[0].owner.battleFieldManager.GetCardToTheRight(otherTargets2[0]);

                if(leftcard != null && CheckConstraints(targetConstraints, leftcard) != null) {
                    Effect(leftcard);
                }

                if (rightCard != null && CheckConstraints(targetConstraints, rightCard) != null) {
                    Effect(rightCard);
                }

                break;
        }

        return true;
    }

    public void ProcessEffect(List<CardVisual> targets) {

        Debug.Log(targets.Count);

        for(int i = 0; i < targets.Count; i++) {
            if(CheckConstraints(targetConstraints, targets[i])) {
                Effect(targets[i]);
            }
        }
    }


    private List<CardVisual> FindTargetsFromAnotherAbility() {

        SpecialAbility targetAbility = null;

        for(int i = 0; i < source.specialAbilities.Count; i++) {
            if(source.specialAbilities[i].abilityName == targetAbilityName) {
                targetAbility = source.specialAbilities[i];
                break;
            }
        }

        if(targetAbility != null) {
            return targetAbility.targets;
        }
        else {
            return null;
        }

    }



    private List<CardVisual> GatherValidTargets() {
        List<CardVisual> results = new List<CardVisual>();

        for (int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            if (CheckConstraints(targetConstraints, Deck._allCards.activeCards[i]) != null) {
                results.Add(Deck._allCards.activeCards[i]);
                //Debug.Log("adding " + Deck._allCards.activeCards[i].gameObject.name + " to a list of valid multi targets");
            }
        }

        return results;
    }



}
