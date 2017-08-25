using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardFilter : MonoBehaviour {

    public Constants.Faction faction;


    private DeckBuilder deckBuilder;


    public void Initialize(DeckBuilder deckBuilder) {
        this.deckBuilder = deckBuilder;
    }




    public void OnButtonPress() {

        deckBuilder.SetFilterFaction(faction);

    }


}
