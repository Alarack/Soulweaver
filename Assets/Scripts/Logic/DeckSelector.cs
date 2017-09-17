using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

using DeckData = DeckBuilder.DeckData;
using LibraryData = DeckBuilder.LibraryData;

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
    //public List<CardData> customDeckData = new List<CardData>();

    [Space(10)]
    public GameObject startGameButton;

    [Header("Templates")]
    public UserDeckButton userDeckTemplate;
    public RectTransform userDeckContainer;

    List<UserDeckButton> userDecks = new List<UserDeckButton>();

    private Player player;

    private List<string> savedDecks = new List<string>();


    void Start () {
        player = GetComponentInParent<Player>();


        savedDecks = LoadLibraryList();

        LoadAllCustomDecks();
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

    public void AssignCustom1Deck(DeckData data) {
        DeckAssignmentHelper(custom1Grimoire, custom1Domain, defaultSoulCrypt);

        player.SetUpDecks();

        CardPlayerData playerData = data.GetGeneral();

        List<CardData> decklist = data.GetCardData();

        StartCoroutine(FillCustomDeckLists(playerData, decklist));

    }

    private IEnumerator FillCustomDeckLists(CardPlayerData playerData, List<CardData> decklist) {
        yield return new WaitForSeconds(1f);

        if (playerData != null) {
            player.activeDomain.GetComponent<Deck>().cards = playerData.domainPowers.Cast<CardData>().ToList();

            //for (int i = 0; i < playerData.domainPowers.Count; i++) {
            //    player.activeDomain.GetComponent<Deck>().cards.Add(playerData.domainPowers[i]);
            //}
        }


        player.activeGrimoire.GetComponent<Deck>().cards = decklist;

        //for (int i = 0; i < decklist.Count; i++) {
        //    player.activeGrimoire.GetComponent<Deck>().cards.Add(decklist[i]);
        //}

        //yield return new WaitForSeconds(1f);

        ShowStartGame();
    }

    private void ShowStartGame() {
        startGameButton.SetActive(true);
        gameObject.SetActive(false);
    }


    private void DeckAssignmentHelper(GameObject grimoire, GameObject domain, GameObject soulcrypt) {
        for(int i = 0; i < player.deckInfo.Count; i++) {
            //Debug.Log(player.deckInfo[i].deckType.ToString() + " is being assigned");
            switch (player.deckInfo[i].deckType) {
                case Constants.DeckType.Grimoire:
                    //Debug.Log("Assigning " + grimoire.gameObject.name + " as active grimoire");

                    player.deckInfo[i].deck = grimoire;

                    //Debug.Log(player.activeGrimoire.gameObject.name);
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



    private void LoadAllCustomDecks() {

        for(int i = 0; i < savedDecks.Count; i++) {
            DeckData loadedDeck = TryLoadDeck(savedDecks[i]);

            if (loadedDeck == null) {

                Debug.LogError("Could not find a saved deck with name: " + savedDecks[i]);
                savedDecks.RemoveAt(i);
                continue;
            }

            GameObject deck = Instantiate(userDeckTemplate.gameObject) as GameObject;
            deck.transform.SetParent(userDeckContainer, false);
            deck.SetActive(true);
            UserDeckButton grimoire = deck.GetComponent<UserDeckButton>();

            userDecks.Add(grimoire);
            grimoire.Initialize(this, loadedDeck);
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



    private List<string> LoadLibraryList() {
        List<string> library = new List<string>();

        if (File.Exists(Application.persistentDataPath + "/library.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/library.dat", FileMode.Open);

            LibraryData data = (LibraryData)bf.Deserialize(file);
            file.Close();

            library = data.savedDecks;
        }

        return library;
    }

    //public void LoadDeck() {

    //    if (File.Exists(Application.persistentDataPath + "/deckData.dat")) {
    //        BinaryFormatter bf = new BinaryFormatter();
    //        FileStream file = File.Open(Application.persistentDataPath + "/deckData.dat", FileMode.Open);

    //        DeckBuilder.DeckData data = (DeckBuilder.DeckData)bf.Deserialize(file);
    //        file.Close();

    //        List<int> listToLoad = new List<int>();

    //        for (int i = 0; i < data.savedDecklist.Count; i++) {
    //            //Debug.Log(data.savedDeckInProgress[i] + " is an id being copied to LOAD");
    //            listToLoad.Add(data.savedDecklist[i]);
    //        }

    //        List<CardData> deckList = new List<CardData>();

    //        for (int i = 0; i < listToLoad.Count; i++) {

    //            CardIDs.CardID id = (CardIDs.CardID)listToLoad[i];
    //            CardData cardData = Finder.FindCardDataFromDatabase(id);

    //            deckList.Add(cardData);
    //        }

    //        customDeckData = deckList;


    //    }
    //}



}