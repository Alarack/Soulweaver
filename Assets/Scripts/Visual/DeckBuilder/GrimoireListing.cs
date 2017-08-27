using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrimoireListing : MonoBehaviour {

    public Constants.Faction faction;
    public CardPlayerData general;

    public Image grimoireImage;
    public Text deckNameText;
    public string deckName;

    [Space(10)]
    public List<CardData> deckList = new List<CardData>();


    private DeckBuilder _deckBuilder;
    private DeckBuilder.DeckData _deckData;


    public void Initialize(DeckBuilder parent, DeckBuilder.DeckData deckData) {
        _deckBuilder = parent;
        _deckData = deckData;

        if(deckData != null) {
            deckList = _deckData.GetCardData();
            general = _deckData.GetGeneral();
            faction = general.faction;
            deckName = _deckData.deckName;
            deckNameText.text = deckName;
        }

    }


    public void OnClick() {
        _deckBuilder.CurrentDeck = this;

        if (_deckData == null) {
            _deckBuilder.ShowSubPanel(DeckBuilder.DeckBuilderSubPanel.FactionSelector);
        }
        else {
            _deckBuilder.ShowSubPanel(DeckBuilder.DeckBuilderSubPanel.CardSearch, general.faction);
            _deckBuilder.PopulateLoadedDeck(_deckData);
        }
    }

}
