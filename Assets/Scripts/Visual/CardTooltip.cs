using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardTooltip : MonoBehaviour {

    public static CardTooltip cardTooltip;
    public static Canvas canvas;
    public RectTransform cardContainer;
    public RectTransform staticPosition;

    public GameObject tooltipContainer;
    public Text toolTipText;

	void Awake () {
        cardTooltip = this;
        canvas = cardTooltip.GetComponentInParent<Canvas>();
	}


    public static void ShowTooltip(string value) {
        if (!cardTooltip.tooltipContainer.activeInHierarchy)
            cardTooltip.tooltipContainer.SetActive(true);

        cardTooltip.toolTipText.text = value;
    }

    public static void HideTooltip() {
        if (cardTooltip.tooltipContainer.activeInHierarchy)
            cardTooltip.tooltipContainer.SetActive(false);

    }



    public static CardVisual ShowVisualTooltip(CardData cardData) {
        CardVisual visual = null;

        switch (cardData.primaryCardType) {
            case Constants.CardType.Soul:
                visual = cardTooltip.CardFactory(cardData, GlobalSettings._globalSettings.creatureDeckbuilder);
                break;

            case Constants.CardType.Spell:
                visual = cardTooltip.CardFactory(cardData, GlobalSettings._globalSettings.spellDeckBuilder);
                break;

            case Constants.CardType.Support:
                visual = cardTooltip.CardFactory(cardData, GlobalSettings._globalSettings.supportDeckbuilder);
                break;
        }



        return visual;
    }


    private CardVisual CardFactory(CardData data, GameObject cardPrefab) {
        GameObject activeCard = Instantiate(cardPrefab) as GameObject;

        CardVisual cardVisual = activeCard.GetComponent<CardVisual>();

        cardVisual.cardData = data;

        //allCards.activeCards.Add(cardVisual);
        activeCard.transform.SetParent(cardContainer, false);
        activeCard.transform.localPosition = new Vector3(-50f,-50f, 0f);

        cardVisual.SetupCardData();

        //cardVisual.currentDeck = allCards;

        activeCard.gameObject.name = cardVisual.cardData.cardName + " [VISUAL TOKEN]";

        return cardVisual;
    }


}
