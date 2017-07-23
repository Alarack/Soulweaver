using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeckType = Constants.DeckType;

public class EffectZoneChange : Effect {


    public DeckType targetLocation;



    public override void Apply(CardVisual target) {
        target.currentDeck.RPCTransferCard(PhotonTargets.All, target, GetDeckFromType(targetLocation, target));
    }





}
