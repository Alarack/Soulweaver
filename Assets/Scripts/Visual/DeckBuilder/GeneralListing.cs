using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralListing : MonoBehaviour {

    public Text generalNameText;
    public Image generalArt;

    public List<Image> domainImages = new List<Image>(); 

    public CardPlayerData generalData;


    private DeckBuilder _deckBuilder;


    public void Initialize(DeckBuilder parent, CardPlayerData general) {
        _deckBuilder = parent;
        generalData = general;

        SetUpVisuals();
    }

    private void SetUpVisuals() {
        generalArt.sprite = generalData.cardImage;
        generalNameText.text = generalData.cardName;

        for (int i = 0; i < generalData.domainPowers.Count; i++) {
            domainImages[i].sprite = generalData.domainPowers[i].domainIcon;
        }
    }


    public void OnClick() {

        if(_deckBuilder.CurrentDeck != null)
            _deckBuilder.CurrentDeck.general = generalData;

        _deckBuilder.CreateOrAddCardListing(generalData);
        _deckBuilder.ShowSubPanel(DeckBuilder.DeckBuilderSubPanel.CardSearch);
    }

}
