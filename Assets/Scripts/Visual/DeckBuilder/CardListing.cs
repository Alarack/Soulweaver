﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardListing : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {


    public Text quantityText;
    public Text costText;
    public Text cardName;

    public Image cardArt;

    public CardData card;
    public int cardQuantity;
    public GameObject quantityImage;

    public CardVisual visualTooltip;

    private DeckBuilder deckbuilder;


    public void Initialize(CardData card, DeckBuilder parent) {
        this.card = card;
        deckbuilder = parent;
        costText.text = card.cardCost.ToString();
        cardName.text = card.cardName;
        cardArt.sprite = card.cardImage;
        cardQuantity++;
        deckbuilder.AddCardCount();
        deckbuilder.deckInProgress.savedDecklist.Add((int)this.card.cardID);
    }

    public bool AddCard() {
        if (cardQuantity >= 3 || deckbuilder.cardCount >= 40)
            return false;
        else {
            cardQuantity++;
            deckbuilder.AddCardCount();
            deckbuilder.deckInProgress.savedDecklist.Add((int)card.cardID);

            if (!quantityImage.activeInHierarchy) {
                quantityImage.SetActive(true);
            }
            quantityText.text = "x" + cardQuantity.ToString();
            return true;
        }
    }

    public void RemoveCard() {
        if(card.primaryCardType == Constants.CardType.Player) {
            return;
        }

        cardQuantity--;
        deckbuilder.RemoveCardCount();
        deckbuilder.deckInProgress.savedDecklist.Remove((int)card.cardID);

        quantityText.text = "x" + cardQuantity.ToString();

        if (cardQuantity <= 0) {
            deckbuilder.currentListings.Remove(this);
            if (visualTooltip != null) {

                //Debug.Log("Killing tooltip");
                visualTooltip.UnregisterEverything();
                Destroy(visualTooltip.gameObject);

            }


            Destroy(gameObject);
        }
        else {
            if(cardQuantity == 1) {
                if (quantityImage.activeInHierarchy)
                    quantityImage.SetActive(false);
            }
        }

    }

    public void OnPointerClick(PointerEventData data) {
        RemoveCard();
    }

    public void OnPointerEnter(PointerEventData data) {
        if(visualTooltip == null) {
            visualTooltip = CardTooltip.ShowVisualTooltip(card);

            if(visualTooltip != null) {
                visualTooltip.transform.localScale *= 1.75f;
                SetVisualTooltipLocation();
            }

        }
        else {
            visualTooltip.gameObject.SetActive(true);
            SetVisualTooltipLocation();
        }

    }

    public void OnPointerExit(PointerEventData data) {
        if (visualTooltip != null && visualTooltip.gameObject.activeInHierarchy) {
            visualTooltip.gameObject.SetActive(false);
        }

    }

    private void SetVisualTooltipLocation() {
        visualTooltip.transform.position = CardTooltip.cardTooltip.staticPosition.position;

    }



}
