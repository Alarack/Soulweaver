using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeckType = Constants.DeckType;

[System.Serializable]
public class EffectZoneChange : Effect {


    public DeckType targetLocation;



    public override void Apply(CardVisual target) {
        Debug.Log(targetLocation.ToString());

        target.currentDeck.RPCTransferCard(PhotonTargets.All, target, GetDeckFromType(targetLocation, target));
    }

    public override void Remove(CardVisual target) {
        target.currentDeck.RPCTransferCard(PhotonTargets.All, target, target.previousDeck);
    }





}
