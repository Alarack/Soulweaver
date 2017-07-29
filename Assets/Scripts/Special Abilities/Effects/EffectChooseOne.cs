using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectChooseOne : Effect {

    public List<string> choices = new List<string>();
    public Constants.CardType cardType;



    public override void Apply(CardVisual target) {



    }



    private List<CardVisual> GatherChoices() {
        List<CardVisual> cards = new List<CardVisual>();

        for (int i = 0; i < choices.Count; i++) {
            CardData tokenData = Resources.Load<CardData>("CardData/" + choices[i]) as CardData;
            cards.Add(CreateChoice(tokenData, GetCardPrefabName(cardType)));
        }

        return cards;
    }



    private CardVisual CreateChoice(CardData data, string prefabName) {

        CardVisual tokenCard = source.owner.activeGrimoire.GetComponent<Deck>().CardFactory(data, prefabName);


        return tokenCard;
    }


}
