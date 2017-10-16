using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeckType = Constants.DeckType;

[System.Serializable]
public abstract class Effect {

    //public Constants.EffectType effectType;
    public CardVisual source;
    public SpecialAbility parentAbility;


    public virtual void Initialize(CardVisual source, SpecialAbility parent) {
        this.source = source;
        parentAbility = parent;


        //Debug.Log(this + " is being initialized for " + parentAbility.source.cardData.cardName);
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
                return card.owner.theVoid;

            case DeckType.NotInGame:
                return card.owner.notInGame;

            case DeckType.None:
                return null;

            default:
                return null;
        }
    }

    protected string GetCardPrefabName(Constants.CardType cardType) {
        return Deck._allCards.GetCardPrefabNameByType(cardType);
    }

}
