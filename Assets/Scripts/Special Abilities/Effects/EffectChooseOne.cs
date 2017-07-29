using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EffectChooseOne : Effect {

    public List<string> choices = new List<string>();
    public Constants.CardType cardType;



    public override void Apply(CardVisual target) {

        AssignPositions(GatherChoices());

    }


    private void AssignPositions(List<CardVisual> cards) {
        for(int i = 0; i < cards.Count; i++) {
            Vector3 pos = source.owner.mulliganManager.cardPositions[i].cardPosition.position;

            cards[i].transform.position = pos;
            cards[i].isBeingChosen = true;
            //cards[i].RPCChangeCardVisualState(PhotonTargets.All, CardVisual.CardVisualState.ShowFront);

            cards[i].RPCChangeCardVisualState(PhotonTargets.Others, CardVisual.CardVisualState.ShowBack);
            cards[i].ChangeCardVisualState((int)CardVisual.CardVisualState.ShowFront);
        }

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
