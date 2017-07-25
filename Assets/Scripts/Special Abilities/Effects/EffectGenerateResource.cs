using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EffectGenerateResource : Effect {

    public GameResource.ResourceType resourceType;
    public int amount;
    public int cap;
    public string resourceName;


    public override void Apply(CardVisual target) {
        GenerateResource();
    }

    public override void Remove(CardVisual target) {
        RemoveResource();
    }


    public void GenerateResource() {

        for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
            if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource.resourceType == resourceType) {
                source.owner.gameResourceDisplay.RPCAddResource(PhotonTargets.All, resourceType, amount);
                return;
            }
        }

        source.owner.RPCSetupResources(PhotonTargets.All, false, resourceType, amount, 0, resourceName, cap);
    }

    public void RemoveResource() {

        for (int i = 0; i < source.owner.gameResourceDisplay.resourceDisplayInfo.Count; i++) {
            if (source.owner.gameResourceDisplay.resourceDisplayInfo[i].resource.resourceType == resourceType) {
                source.owner.gameResourceDisplay.RPCRemoveResource(PhotonTargets.All, resourceType, amount);
                return;
            }
        }
    }


}