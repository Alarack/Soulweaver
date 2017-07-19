using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialAttribute {

    public enum AttributeType {
        Protection,
        Regeneration,
        Volatile,
        SpellDamage
    }

    public AttributeType attributeType;
    public int attributeValue;
    public bool suspended;


    public SpecialAttribute() {

    }

    public SpecialAttribute(AttributeType type, int value) {
        this.attributeType = type;
        this.attributeValue = value;
    }


}
