using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionSelector : MonoBehaviour {

    public List<FactionListing> factions = new List<FactionListing>();
    

    private DeckBuilder _deckBuilder;


    public void Initialize(DeckBuilder parent) {
        _deckBuilder = parent;

        for(int i = 0; i < factions.Count; i++) {
            factions[i].Initialize(parent);
        }

    }



}
