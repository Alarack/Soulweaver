using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using DeckData = DeckBuilder.DeckData;

public class GrimoireSelector : MonoBehaviour {

    public RectTransform deckListingContainer;
    public List<GrimoireListing> grimoireListings = new List<GrimoireListing>();
    public GrimoireListing grimoireTemplate;


    private DeckBuilder _deckBuilder;


    public void Initialize(DeckBuilder parent) {
        _deckBuilder = parent;


        RefreshLibrary();

        grimoireTemplate.Initialize(parent, null);
    }


    public void RefreshLibrary() {
        DestroyLibraryListings();

        for (int i = 0; i < _deckBuilder.savedDecks.Count; i++) {

            DeckData loadedDeck = TryLoadDeck(_deckBuilder.savedDecks[i]);

            if (loadedDeck == null) {

                Debug.LogError("Could not find a saved deck with name: " + _deckBuilder.savedDecks[i]);
                _deckBuilder.savedDecks.RemoveAt(i);
                continue;
            }

            GameObject deck = Instantiate(grimoireTemplate.gameObject) as GameObject;
            deck.transform.SetParent(deckListingContainer, false);
            GrimoireListing grimoire = deck.GetComponent<GrimoireListing>();

            grimoireListings.Add(grimoire);
            grimoire.Initialize(_deckBuilder, loadedDeck);

        }



    }



    private DeckData TryLoadDeck(string deckName) {
        DeckData deck = null;

        if (File.Exists(Application.persistentDataPath + "/" + deckName + ".dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + deckName + ".dat", FileMode.Open);

            DeckData data = (DeckData)bf.Deserialize(file);
            file.Close();

            deck = data;
        }

        return deck;
    }




    private void DestroyLibraryListings() {
        for (int i = 0; i < grimoireListings.Count; i++) {
            Destroy(grimoireListings[i].gameObject);
        }

        grimoireListings.Clear();
    }


}
