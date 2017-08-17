using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialAttribute {

    public enum AttributeType {
        None,
        Protection,
        Regeneration,
        Volatile,
        SpellDamage,
        Reanimator,
    }

    public AttributeType attributeType;
    public int attributeValue;
    public int uniqueID = -1;


    public SpecialAttribute() {

    }

    public SpecialAttribute(AttributeType type, int value) {
        this.attributeType = type;
        this.attributeValue = value;
    }


}
