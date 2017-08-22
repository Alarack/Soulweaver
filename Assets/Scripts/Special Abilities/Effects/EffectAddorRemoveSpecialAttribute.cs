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
    //public SpecialAttribute.AttributeType attributeType;
    //public int value;


    public List<SpecialAttribute> specialAttributes = new List<SpecialAttribute>();

    public int modificationValue;

    public override void Initialize(CardVisual source, SpecialAbility parent) {
        base.Initialize(source, parent);
        InitializeSpecialAttributes();
    }

    private void InitializeSpecialAttributes() {
        for(int i = 0; i < specialAttributes.Count; i++) {
            if(source.owner != null)
                specialAttributes[i].uniqueID = IDFactory.GenerateAttID(source.owner);
        }
    }


    public override void Apply(CardVisual target) {
        switch (attributeAction) {
            case AttributeAction.Add:
                AddSpecialAttribute(target);
                break;

            case AttributeAction.Remove:
                RemoveSpecialAttribute(target);
                break;

            //case AttributeAction.Modify:
            //    ModifySpecialAttribute(target, modificationValue);
            //    break;

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

            //case AttributeAction.Modify:
            //    ModifySpecialAttribute(target, -modificationValue);
            //    break;

        }
    }

    public void AddSpecialAttribute(CardVisual target) {
        //target.RPCAddSpecialAttribute(PhotonTargets.All, attributeType, value);

        for(int i = 0; i < specialAttributes.Count; i++) {
            target.RPCApplySpecialAttribute(PhotonTargets.All, specialAttributes[i], source);
        }

    }

    public void RemoveSpecialAttribute(CardVisual target) {
        //target.RPCRemoveSpecialAttributeSuspension(PhotonTargets.All, attributeType);

        for (int i = 0; i < specialAttributes.Count; i++) {
            target.RPCRemoveSpecialAttribute(PhotonTargets.All, specialAttributes[i], source);
        }

    }

    public void ModifySpecialAttribute(CardVisual target, int value) {
        //target.RPCModifySpecialAttribute(PhotonTargets.All, attributeType, value);
    }

}
