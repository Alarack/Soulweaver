using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeckType = Constants.DeckType;

public abstract class Effect {

    public Constants.EffectType effectType;
    public CardVisual source;

    public abstract void Apply(CardVisual target);



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
