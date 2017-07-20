﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LogicTargetedAbility : SpecialAbility {


    public enum LogicTargeting {
        NoTargetsNeeded,
        AllValidTargets,
        NumberOfValidTargets,
        OnlyTargetTriggeringCard
    }




    public LogicTargeting logicTargetingMethod;


    public int numberofTargets;
    public bool processEffectOnPrimaryEffectTargets;
    //public bool onlyThisCard;


    private List<CardVisual> validTargets = new List<CardVisual>();



    public override void Initialize(CardVisual owner) {
        base.Initialize(owner);

        if (abilityName == "RoundBlaze")
            Debug.Log(numberofTargets + " during init");
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

                Effect(source);
                break;

            case LogicTargeting.OnlyTargetTriggeringCard:
                if (CheckConstraints(targetConstraints, card) != null) {
                    Effect(card);

                }
                    

                break;



        }

        return true;

        //return base.ProcessEffect(card);
    }

    public void ProcessEffect(List<CardVisual> targets) {

        for(int i = 0; i < targets.Count; i++) {
            if(CheckConstraints(targetConstraints, targets[i])) {
                Effect(targets[i]);
            }


        }

    }



    public List<CardVisual> GatherValidTargets() {
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
