using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeckType = Constants.DeckType;

[System.Serializable]
public class EffectZoneChange : Effect {


    public DeckType targetLocation;



    public override void Apply(CardVisual target) {
        //Debug.Log(targetLocation.ToString());
        bool hasVFX = String.IsNullOrEmpty(parentAbility.abilityVFX);


        switch (targetLocation) {
            case DeckType.SoulCrypt:
                if(target is CreatureCardVisual || target is SupportCardVisual)
                    target.RPCCheckDeath(PhotonTargets.All, source, true, !hasVFX);
                else
                    target.currentDeck.RPCTransferCard(PhotonTargets.All, target, GetDeckFromType(targetLocation, target));

                break;

            case DeckType.Void:
                target.StartCoroutine(target.RemoveCardVisualFromField(target));

                break;

            default:
                target.currentDeck.RPCTransferCard(PhotonTargets.All, target, GetDeckFromType(targetLocation, target));
                break;
        }



    }

    public override void Remove(CardVisual target) {
        target.currentDeck.RPCTransferCard(PhotonTargets.All, target, target.previousDeck);
    }





}
