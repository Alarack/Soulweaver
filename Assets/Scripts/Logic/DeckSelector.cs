using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DeckSelector : MonoBehaviour {


    [Header("Default Deck")]
    public GameObject defaultGrimoire;
    public GameObject defaultDomain;
    public GameObject defaultSoulCrypt;

    [Header("Rillock")]
    public GameObject rillockGrimoire;
    public GameObject rillockDomain;
    //public GameObject rillockSoulcrypt;

    [Header("Wardens")]
    public GameObject wardensGrimoire;
    public GameObject wardensDomain;

    [Header("Keepers")]
    public GameObject keepersGrimoire;
    public GameObject keepersDomain;

    [Header("Custom1")]
    public GameObject custom1Grimoire;
    public GameObject custom1Domain;
    public List<CardData> customDeckData = new List<CardData>();

    [Space(10)]
    public GameObject startGameButton;

    private Player player;


    void Start () {
        player = GetComponentInParent<Player>();
	}

	void Update () {
		
	}

    public void AssignDefaultDeck() {
        DeckAssignmentHelper(defaultGrimoire, defaultDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
    }

    public void AssignRillockDeck() {
        DeckAssignmentHelper(rillockGrimoire, rillockDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
    }

    public void AssignWardensDeck() {
        DeckAssignmentHelper(wardensGrimoire, wardensDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
    }

    public void AssignKeeperDeck() {
        DeckAssignmentHelper(keepersGrimoire, keepersDomain, defaultSoulCrypt);
        player.SetUpDecks();

        ShowStartGame();
    }

    public void AssignCustom1Deck() {
        LoadDeck();

        DeckAssignmentHelper(custom1Grimoire, custom1Domain, defaultSoulCrypt);

        player.SetUpDecks();

        CardPlayerData playerData = null;

        for (int i = 0; i < customDeckData.Count; i++) {
            if(customDeckData[i].primaryCardType == Constants.CardType.Player) {
                playerData = (CardPlayerData)customDeckData[i];
            }

            player.activeGrimoire.GetComponent<Deck>().cards.Add(customDeckData[i]);
        }

        if(playerData != null) {
            for (int i = 0; i < playerData.domainPowers.Count; i++) {
                player.activeDomain.GetComponent<Deck>().cards.Add(playerData.domainPowers[i]);

            }
        }

        ShowStartGame();

    }



    private void ShowStartGame() {
        startGameButton.SetActive(true);
        gameObject.SetActive(false);
    }


    private void DeckAssignmentHelper(GameObject grimoire, GameObject domain, GameObject soulcrypt) {
        for(int i = 0; i < player.deckInfo.Count; i++) {
            switch (player.deckInfo[i].deckType) {
                case Constants.DeckType.Grimoire:
                    player.deckInfo[i].deck = grimoire;
                    break;

                case Constants.DeckType.Domain:
                    player.deckInfo[i].deck = domain;
                    break;

                case Constants.DeckType.SoulCrypt:
                    player.deckInfo[i].deck = soulcrypt;
                    break;
            }
        }
    }



    public void LoadDeck() {

        if (File.Exists(Application.persistentDataPath + "/deckData.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/deckData.dat", FileMode.Open);

            DeckBuilder.DeckData data = (DeckBuilder.DeckData)bf.Deserialize(file);
            file.Close();

            List<int> listToLoad = new List<int>();

            for (int i = 0; i < data.savedDecklist.Count; i++) {
                //Debug.Log(data.savedDeckInProgress[i] + " is an id being copied to LOAD");
                listToLoad.Add(data.savedDecklist[i]);
            }

            List<CardData> deckList = new List<CardData>();

            for (int i = 0; i < listToLoad.Count; i++) {

                CardIDs.CardID id = (CardIDs.CardID)listToLoad[i];
                CardData cardData = Finder.FindCardDataFromDatabase(id);

                deckList.Add(cardData);
            }

            customDeckData = deckList;


        }
    }


}