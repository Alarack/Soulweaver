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

    //public DeckBuilder deckBuilder;
    public CardData card;
    public int cardQuantity;
    public GameObject quantityImage;

    private DeckBuilder deckbuilder;


	void Start () {
		
	}

    public void Initialize(CardData card, DeckBuilder parent) {
        this.card = card;
        deckbuilder = parent;
        costText.text = card.cardCost.ToString();
        cardName.text = card.cardName;
        cardArt.sprite = card.cardImage;
        cardQuantity++;
        deckbuilder.deckInProgress.Add((int)this.card.cardID);

        //Debug.Log(card.cardName + " is being created as a listing");
    }

    public bool AddCard() {
        if (cardQuantity >= 3)
            return false;
        else {
            cardQuantity++;
            deckbuilder.deckInProgress.Add((int)card.cardID);
            //Debug.Log(card.cardName + " is being Added to the set of listings");

            if (!quantityImage.activeInHierarchy) {
                quantityImage.SetActive(true);
            }
            quantityText.text = "x" + cardQuantity.ToString();
            return true;
        }
    }

    public void RemoveCard() {
        cardQuantity--;
        deckbuilder.deckInProgress.Remove((int)card.cardID);
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
