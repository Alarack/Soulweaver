﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LogicTargetedAbility :  SpecialAbility {


    public enum LogicTargeting {
        NoTargetsNeeded,
        AllValidTargets,
        NumberOfValidTargets
    }




    public LogicTargeting logicTargetingMethod;


    public int numberofTargets;
    //public bool onlyThisCard;


    private List<CardVisual> validTargets = new List<CardVisual>();



    public override bool ProcessEffect(CardVisual card) {


        switch (logicTargetingMethod) {
            case LogicTargeting.AllValidTargets:
                validTargets = GatherValidTargets();

                for (int i = 0; i < validTargets.Count; i++) {
                    //Debug.Log("doin' stuff to " + validTargets[i].gameObject.name);
                    Effect(validTargets[i]);
                }

                validTargets.Clear();

                break;

            case LogicTargeting.NumberOfValidTargets:
                validTargets = GatherValidTargets();

                if(numberofTargets > validTargets.Count) {
                    numberofTargets = validTargets.Count;
                }

                for(int i = 0; i < numberofTargets; i++) {
                    Effect(validTargets[i]);
                }

                validTargets.Clear();

                break;


            case LogicTargeting.NoTargetsNeeded:

                Effect(source);
                break;



        }

        return true;

        //return base.ProcessEffect(card);
    }



    public List<CardVisual> GatherValidTargets() {
        List<CardVisual> results = new List<CardVisual>();

        for (int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            if (CheckConstraints(targetConstraints, Deck._allCards.activeCards[i])) {
                results.Add(Deck._allCards.activeCards[i]);
                //Debug.Log("adding " + Deck._allCards.activeCards[i].gameObject.name + " to a list of valid multi targets");
            }
        }

        return results;
    }



}
