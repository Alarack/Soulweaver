using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFilter : MonoBehaviour {

    public Button button;
    public Image image;
    public Constants.Faction faction;


    private DeckBuilder deckBuilder;


    public void Initialize(DeckBuilder deckBuilder) {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
        this.deckBuilder = deckBuilder;
    }




    public void OnButtonPress() {

        deckBuilder.SetFilterFaction(faction);

    }

    public void Lock() {
        button.interactable = false;
        image.color = Color.gray;
    }

    public void Unlock() {
        button.interactable = true;
        image.color = Color.white;
    }


}
