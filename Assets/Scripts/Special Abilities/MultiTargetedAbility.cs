using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultiTargetedAbility :  SpecialAbility {


    public int numberofTargets;


    private List<CardVisual> validTargets = new List<CardVisual>();



    public override bool ProcessEffect(CardVisual card) {

        for(int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            if(CheckConstraints(limitations, Deck._allCards.activeCards[i])) {
                validTargets.Add(Deck._allCards.activeCards[i]);
                //Debug.Log("adding " + Deck._allCards.activeCards[i].gameObject.name + " to a list of valid multi targets");
            }
                
        }

        for(int i = 0; i < validTargets.Count; i++) {
            //Debug.Log("doin' stuff to " + validTargets[i].gameObject.name);
            Effect(validTargets[i]);
        }

        validTargets.Clear();

        return true;

        //return base.ProcessEffect(card);
    }







}
