using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardListing : MonoBehaviour, IPointerClickHandler {


    public Text quantityText;
    public Text costText;
    public Text cardName;

    public Image cardArt;

    public CardData card;
    public int cardQuantity;
    public GameObject quantityImage;

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
        if (cardQuantity >= 3 || deckbuilder.cardCount >= 39)
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
        cardQuantity--;
        deckbuilder.RemoveCardCount();
        deckbuilder.deckInProgress.savedDecklist.Remove((int)card.cardID);
        deckbuilder.currentListings.Remove(this);
        quantityText.text = "x" + cardQuantity.ToString();

        if (cardQuantity <= 0) {
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



}
