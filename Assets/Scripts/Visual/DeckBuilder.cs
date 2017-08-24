using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DeckBuilder : MonoBehaviour {

    public RectTransform cardList;
    public Deck allCards;
    private int currentIndex;
    public GameObject cardListingTemplate;
    public List<CardListing> currentListings = new List<CardListing>();
    public List<int> deckInProgress = new List<int>();

	void Start () {
        allCards = GetComponent<Deck>();

        for (int i = 0; i < CardDB.cardDB.allCardData.Length; i++) {
            if (CardDB.cardDB.allCardData[i].primaryCardType == Constants.CardType.Domain)
                continue;

            if (CardDB.cardDB.allCardData[i].primaryCardType == Constants.CardType.Player)
                continue;

            if (CardDB.cardDB.allCardData[i].keywords.Contains(Constants.Keywords.Token))
                continue;

            allCards.cards.Add(CardDB.cardDB.allCardData[i]);
        }

        PageRight();

        RegisterListeners();
	}

    private void RegisterListeners() {
        Grid.EventManager.RegisterListener(Constants.GameEvent.CardSelected, OnCardSelected);

    }


	void Update () {
        if (Input.GetKeyDown(KeyCode.N)) {
            PageRight();
        }

        if (Input.GetKeyDown(KeyCode.P)) {
            PageLeft();
        }
    }

    private void DestroyAllDisplays() {

        for(int i = 0; i < allCards.activeCards.Count; i++) {
            Destroy(allCards.activeCards[i].gameObject);
        }

        allCards.activeCards.Clear();

    }

    private void DestroyAllListings() {
        for(int i = 0; i < currentListings.Count; i++) {
            Destroy(currentListings[i].gameObject);
        }

        currentListings.Clear();
        deckInProgress.Clear();
    }

    private void PageRight() {

        if (currentIndex >= allCards.cards.Count)
            return;

        DestroyAllDisplays();
        SpawnAllCards(currentIndex, currentIndex + 7);
    }

    private void PageLeft() {

        if (currentIndex <= 8)
            return;

        DestroyAllDisplays();
        currentIndex -= 16;
        SpawnAllCards(currentIndex, currentIndex + 7);
    }

    private void SpawnAllCards(int startIndex, int numToSpawn) {

        if (startIndex < 0)
            startIndex = 0;

        if (numToSpawn > allCards.cards.Count)
            numToSpawn = allCards.cards.Count -1;

        if (numToSpawn < 7)
            numToSpawn = 7;

        for (int i = startIndex; i <= numToSpawn; i++) {
            //yield return new WaitForSeconds(0.01f);

            switch (allCards.cards[i].primaryCardType) {
                case Constants.CardType.Soul:
                    CardFactory(allCards.cards[i], GlobalSettings._globalSettings.creatureDeckbuilder);
                    break;

                case Constants.CardType.Spell:
                    CardFactory(allCards.cards[i], GlobalSettings._globalSettings.spellDeckBuilder);
                    break;

                case Constants.CardType.Support:
                    CardFactory(allCards.cards[i], GlobalSettings._globalSettings.supportDeckbuilder);

                    break;
            }
        }

        currentIndex += 8;
    }

    private CardVisual CardFactory(CardData data, GameObject cardPrefab) {
        GameObject activeCard = Instantiate(cardPrefab) as GameObject;

        CardVisual cardVisual = activeCard.GetComponent<CardVisual>();

        cardVisual.cardData = data;

        allCards.activeCards.Add(cardVisual);
        activeCard.transform.SetParent(GameObject.FindGameObjectWithTag("AllCards").transform, false);

        cardVisual.SetupCardData();

        cardVisual.currentDeck = allCards;

        activeCard.gameObject.name = cardVisual.cardData.cardName;


        return cardVisual;
    }


    public void OnCardSelected(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        //Debug.Log(card.cardData.cardName + " has been recieved");
        CreateOrAddCardListing(card.cardData);
    }

    public void CreateOrAddCardListing(CardData card) {
        //Debug.Log("Creating " + card.cardName);

        CardListing existingListing = null;
        for (int i = 0; i < currentListings.Count; i++) {
            if (currentListings[i].card == card) {
                existingListing = currentListings[i];
                break;
            }
        }

        if (existingListing != null) {
            existingListing.AddCard();
        }
        else {
            GameObject newListing = Instantiate(cardListingTemplate) as GameObject;
            newListing.transform.SetParent(cardList, false);
            newListing.SetActive(true);
            CardListing listing = newListing.GetComponent<CardListing>();
            listing.Initialize(card, this);
            currentListings.Add(listing);
        }
    }




    public void SaveDeck() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/deckData.dat");

        DeckData data = new DeckData();

        List<int> listToSave = new List<int>();

        for (int i = 0; i < deckInProgress.Count; i++) {
            Debug.Log(deckInProgress[i] + " is an id being copied to SAVE");
            listToSave.Add(deckInProgress[i]);
        }


        data.savedDeckInProgress = listToSave;

        //Debug.Log(deckInProgress.Count + " SAVING: is the number of items in hte deck");
        for (int i = 0; i < data.savedDeckInProgress.Count; i++) {
            Debug.Log(data.savedDeckInProgress[i].ToString() + " is the id of a card while SAVING");
        }



        bf.Serialize(file, data);
        file.Close();

        //SceneManager.LoadScene("main");
    }

    public void LoadDeck() {
        //Debug.Log("Loading Clicked");
        DestroyAllListings();

        for (int i = 0; i < deckInProgress.Count; i++) {
            Debug.Log(deckInProgress[i].ToString() + " is the id of a card in the deckinprogress list before loading");
        }

        if (File.Exists(Application.persistentDataPath + "/deckData.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/deckData.dat", FileMode.Open);

            DeckData data = (DeckData)bf.Deserialize(file);
            file.Close();


            //for(int i = 0; i < data.deckInProgress.Count; i++) {
            //    Debug.Log(data.deckInProgress[i].ToString() + " is the id of a card from the saved data");
            //}

            List<int> listToLoad = new List<int>();

            for(int i = 0; i < data.savedDeckInProgress.Count; i++) {
                Debug.Log(data.savedDeckInProgress[i] + " is an id being copied to LOAD");
                listToLoad.Add(data.savedDeckInProgress[i]);
            }

           // deckInProgress = listToLoad;


            List<CardData> deckList = new List<CardData>();

            for (int i = 0; i < listToLoad.Count; i++) {
                //Debug.Log(deckInProgress[i].ToString() + " is the id of a card while LOADING");

                CardIDs.CardID id = (CardIDs.CardID)listToLoad[i];
                CardData cardData = Finder.FindCardDataFromDatabase(id);

                deckList.Add(cardData);
            }

            PopulateLoadedDeck(deckList);

        }
    }

    private void PopulateLoadedDeck(List<CardData> cards) {

        //Debug.Log(deckInProgress.Count + " is the deck count");

        for (int i = 0; i < cards.Count; i++) {
            CreateOrAddCardListing(cards[i]);
        }

    }





    [Serializable]
    private class DeckData {

        public List<int> savedDeckInProgress = new List<int>();

    }

}
