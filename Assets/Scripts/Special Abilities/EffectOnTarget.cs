using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;

using EffectType = Constants.EffectType;

[System.Serializable]
public class EffectOnTarget : SpecialAbility {




    protected override void Unsubscribe() {
        if (source == null) {
            Debug.LogError("[Effect on Target] has no source cardvisual");
            return;
        }

        if (!source.photonView.isMine) {
            Debug.LogError("[Effect on Target] " + source.cardData.cardName + " was not mine");
            return;
        }

        CombatManager.combatManager.confirmedTargetCallback -= ProcessEffect;

    }


    protected override void Effect(CardVisual card) {

        base.Effect(card);

        Unsubscribe();

        //targets.Add(card);

        //Debug.Log("[Effect on Target] Applying effect");

        //switch (effect) {
        //    case EffectType.StatAdjustment:
        //        ApplyStatAdjustments(card);
        //        Unsubscribe();
        //        break;

        //}
    }





}
