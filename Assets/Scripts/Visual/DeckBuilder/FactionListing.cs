using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionListing : MonoBehaviour {

    public Constants.Faction faction;

    private DeckBuilder _deckBuilder;

    public void Initialize(DeckBuilder parent) {
        _deckBuilder = parent;
    }


    public void OnClick() {
        _deckBuilder.generalSelector.faction = faction;

        _deckBuilder.ShowSubPanel(DeckBuilder.DeckBuilderSubPanel.GeneralSelector, faction);

    }

}
