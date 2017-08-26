using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralSelector : MonoBehaviour {

    public Constants.Faction faction;
    public List<GeneralListing> generalListings = new List<GeneralListing>();
    [Space(10)]
    public GeneralListing template;
    public RectTransform listingsContainer;

    private DeckBuilder _deckBuilder;


    public void Initialize(DeckBuilder parent, Constants.Faction faction) {
        this.faction = faction;
        _deckBuilder = parent;

        //CreateListings();
    }


    public void ResetListings(Constants.Faction faction) {
        DestroyListings();
        this.faction = faction;
        CreateListings();
    }

    private void CreateListings() {
        List<CardPlayerData> generals = new List<CardPlayerData>();

        for(int i = 0; i < _deckBuilder.allGenerals.Count; i++) {
            if(_deckBuilder.allGenerals[i].faction == faction) {
                generals.Add(_deckBuilder.allGenerals[i]);
            }
        }

        for(int i = 0; i< generals.Count; i++) {
            GameObject listing = Instantiate(template.gameObject) as GameObject;
            listing.transform.SetParent(listingsContainer, false);
            listing.SetActive(true);
            GeneralListing general = listing.GetComponent<GeneralListing>();

            general.Initialize(_deckBuilder, generals[i]);
        }

    }

    private void DestroyListings() {
        for (int i = 0; i < generalListings.Count; i++) {
            Destroy(generalListings[i].gameObject);
        }
        generalListings.Clear();
    }




}
