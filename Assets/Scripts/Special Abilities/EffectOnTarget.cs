using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;

using EffectType = Constants.EffectType;

[System.Serializable]
public class EffectOnTarget : SpecialAbility {

    public List<StatAdjustment> statAdjustments = new List<StatAdjustment>();



    public override void RegisterEventListeners() {

    }

    private void Unsubscribe() {
        if (source == null) {
            Debug.LogError("[Effect on Target] has no source cardvisual");
            return;
        }

        if (!source.photonView.isMine) {
            Debug.LogError("[Effect on Target] " + source.cardData.cardName + " was not mine");
            return;
        }


        Debug.Log("[Effect on Target is Unsubscribing]");
        //source.StartCoroutine(CombatManager.SetTargetingMode(CombatManager.TargetingMode.CombatTargeting));


        //CombatManager.StaticReset();
        //source.owner.combatManager.ResetTargeting();
        //CombatManager.combatManager.targetCallback -= Effect;
        source.owner.combatManager.targetCallback -= Effect;

    }

    public override void Effect(CardVisual card) {

        Debug.Log("[Effect on Target] Applying effect");

        switch (effect) {
            case EffectType.StatAdjustment:
                ApplyStatAdjustments(card);
                Unsubscribe();
                break;

        }
    }






    private void ApplyStatAdjustments(CardVisual card) {
        for(int i = 0; i < statAdjustments.Count; i++) {
            card.RPCAlterStat(PhotonTargets.All, statAdjustments[i]);
        }
    }

}
