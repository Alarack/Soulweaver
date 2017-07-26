using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectAddorRemoveSpecialAttribute : Effect {

    public enum AttributeAction {
        Add,
        Remove,
        Modify
    }

    public AttributeAction attributeAction;
    public SpecialAttribute.AttributeType attributeType;
    public int value;
    public int modificationValue;


    public override void Apply(CardVisual target) {
        switch (attributeAction) {
            case AttributeAction.Add:
                AddSpecialAttribute(target);
                break;

            case AttributeAction.Remove:
                RemoveSpecialAttribute(target);
                break;

            case AttributeAction.Modify:
                ModifySpecialAttribute(target, modificationValue);
                break;

        }
    }

    public override void Remove(CardVisual target) {
        switch (attributeAction) {
            case AttributeAction.Add:
                RemoveSpecialAttribute(target);
                break;

            case AttributeAction.Remove:
                AddSpecialAttribute(target);
                break;

            case AttributeAction.Modify:
                ModifySpecialAttribute(target, -modificationValue);
                break;

        }
    }

    public void AddSpecialAttribute(CardVisual target) {
        target.RPCAddSpecialAttribute(PhotonTargets.All, attributeType, value);
    }

    public void RemoveSpecialAttribute(CardVisual target) {
        target.RPCRemoveSpecialAttributeSuspension(PhotonTargets.All, attributeType);
    }

    public void ModifySpecialAttribute(CardVisual target, int value) {
        target.RPCModifySpecialAttribute(PhotonTargets.All, attributeType, value);
    }

}
