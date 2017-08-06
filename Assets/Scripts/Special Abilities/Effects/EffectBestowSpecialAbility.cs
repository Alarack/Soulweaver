using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectBestowSpecialAbility : Effect {

    public string targetAbiliity;

    public override void Apply(CardVisual target) {

        target.ProcessNewSpecialAbility(GetTargetAbility());

        target.RPCProcessNewSpecialAbility(PhotonTargets.Others, source, targetAbiliity);

    }


    private SpecialAbility GetTargetAbility() {
        SpecialAbility grantedAbility = null;

        for(int i = 0; i < source.cardData.multiTargetAbilities.Count; i++) {
            if (source.cardData.multiTargetAbilities[i].abilityName == targetAbiliity) {
                grantedAbility = source.cardData.multiTargetAbilities[i];
                break;
            }
                
        }

        //Debug.Log(grantedAbility.abilityName + " has been found on " + source.gameObject.name);

        return grantedAbility;
    }


}
