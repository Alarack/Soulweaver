using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeckType = Constants.DeckType;

[System.Serializable]
public abstract class Effect {

    public Constants.EffectType effectType;
    public CardVisual source;
    public SpecialAbility parentAbility;


    public virtual void Initialize(CardVisual source, SpecialAbility parent) {
        this.source = source;
        parentAbility = parent;
    }

    public abstract void Apply(CardVisual target);

    public virtual void Remove(CardVisual target) {

    }


    protected Deck GetDeckFromType(DeckType type, CardVisual card) {
        switch (type) {
            case DeckType.Battlefield:
                return card.owner.battlefield;

            case DeckType.Hand:
                return card.owner.myHand;

            case DeckType.Grimoire:
                return card.owner.activeGrimoire.GetComponent<Deck>();

            case DeckType.SoulCrypt:
                return card.owner.activeCrypt.GetComponent<Deck>();

            case DeckType.Void:
                return Deck._void;

            case DeckType.None:
                return null;

            default:
                return null;
        }
    }

}
