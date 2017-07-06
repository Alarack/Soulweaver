using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;

using SpecialAbilityActivationTrigger = Constants.SpecialAbilityActivationTrigger;
using EffectType = Constants.EffectType;

[System.Serializable]
public abstract class SpecialAbility  {

    [Header("Basic Info")]
    public SpecialAbilityActivationTrigger type;
    public EffectType effect;
    public string abilityName;
    public int effectValue;
    public CardVisual source;


    public abstract void Effect(CardVisual card);
    public abstract void RegisterEventListeners();


    public virtual void Initialize(CardVisual card) {
        source = card;
    }

    [System.Serializable]
    public class StatAdjustment {
        public Constants.CardStats stat;
        public int value;

        //public List<KeyValuePair<Constants.CardStats, int>> statMods = new List<KeyValuePair<Constants.CardStats, int>>();
    }

}
