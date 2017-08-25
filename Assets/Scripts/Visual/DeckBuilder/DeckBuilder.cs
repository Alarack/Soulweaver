using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using UnityEngine.UI;

public class DeckBuilder : MonoBehaviour {


    public int cardCount;
    public Text cardCountText;
    public RectTransform cardList;
    public Deck allCards;
    private int currentIndex;

    public GameObject cardListingTemplate;
    public List<CardListing> currentListings = new List<CardListing>();
    public List<int> deckInProgress = new List<int>();

    public List<CardFilter> filterButtons = new List<CardFilter>();

    public CardFilterConstraints filters;
    public InputField searchBar;
    public List<CardData> filteredCards = new List<CardData>();

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

        cardCount = 0;
        UpdateCardText();
        filteredCards = FilterCards();


        for (int i = 0; i < filterButtons.Count; i++) {
            filterButtons[i].Initialize(this);
        }


        PageRight();

        RegisterListeners();
	}

    private void RegisterListeners() {
        Grid.EventManager.RegisterListener(Constants.GameEvent.CardSelected, OnCardSelected);

    }


	//void Update () {
 //       if (Input.GetKeyDown(KeyCode.N)) {
 //           PageRight();
 //       }

 //       if (Input.GetKeyDown(KeyCode.P)) {
 //           PageLeft();
 //       }
 //   }

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
        cardCount = 0;
        UpdateCardText();
    }

    public void PageRight() {

        if (currentIndex >= filteredCards.Count)
            return;

        DestroyAllDisplays();
        SpawnCards(currentIndex, currentIndex + 7);
    }

    public void PageLeft() {

        if (currentIndex <= 8)
            return;

        DestroyAllDisplays();
        currentIndex -= 16;
        SpawnCards(currentIndex, currentIndex + 7);
    }

    private void SpawnCards(int startIndex, int numToSpawn) {

        if (startIndex < 0)
            startIndex = 0;

        if (numToSpawn >= filteredCards.Count)
            numToSpawn = filteredCards.Count -1;

        //if (numToSpawn < 7)
        //    numToSpawn = 7;
        //Debug.Log(filteredCards.Count + " is the number of cards in the filter");
        //Debug.Log(numToSpawn + " is the numToSpawn");

        for (int i = startIndex; i <= numToSpawn; i++) {
            //yield return new WaitForSeconds(0.01f);
            //Debug.Log(i + " is the numToSpawn");

            switch (filteredCards[i].primaryCardType) {
                case Constants.CardType.Soul:
                    CardFactory(filteredCards[i], GlobalSettings._globalSettings.creatureDeckbuilder);
                    break;

                case Constants.CardType.Spell:
                    CardFactory(filteredCards[i], GlobalSettings._globalSettings.spellDeckBuilder);
                    break;

                case Constants.CardType.Support:
                    CardFactory(filteredCards[i], GlobalSettings._globalSettings.supportDeckbuilder);

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


    #region EVENTS

    public void OnCardSelected(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        //Debug.Log(card.cardData.cardName + " has been recieved");
        CreateOrAddCardListing(card.cardData);
    }

    #endregion


    public void CreateOrAddCardListing(CardData card) {
        if (cardCount >= 39)
            return;

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

    public void AddCardCount() {
        cardCount++;
        UpdateCardText();
    }

    public void RemoveCardCount() {
        cardCount--;
        UpdateCardText();
    }

    private void UpdateCardText() {
        cardCountText.text = cardCount.ToString();
    }



    public List<CardData> FilterCards() {
        List<CardData> results = new List<CardData>();

        if (filters.faction != Constants.Faction.All) {
            results = GetCardsByfaction();
        }
        else {
            results = allCards.cards;
        }

        List<CardData> sortedList = results.OrderBy(o => o.cardCost).ToList();

        //Dictionary<CardData, int> orderedCards = new Dictionary<CardData, int>();

        //for(int i = 0; i < results.Count; i++) {
        //    orderedCards.Add(results[i], results[i].cardCost);
        //}



        return sortedList;
    }


    private List<CardData> GetCardsByfaction() {
        List<CardData> results = new List<CardData>();

        for(int i = 0; i < allCards.cards.Count; i++) {
            if (allCards.cards[i].faction == filters.faction) {
                results.Add(allCards.cards[i]);
            }
            
        }

        return results;
    }

    public void SetFilterFaction(Constants.Faction faction) {
        filters.faction = faction;

        filteredCards = FilterCards();

        currentIndex = 0;
        PageRight();

    }

    public void SearchCards(string value) {

        ExecuteSearch(searchBar.text);

    }


    private void ExecuteSearch(string value) {

        List<CardData> cards = FilterCards();
        List<CardData> results = new List<CardData>();

        foreach (CardData card in cards) {
            if (card.cardName.Contains(value)) {
                results.Add(card);
            }
            else if (card.cardText.Contains(value)) {
                results.Add(card);
            }
        }

        filteredCards = results;

        currentIndex = 0;
        PageRight();
    }



    #region SaveLoad

    public void SaveDeck() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/deckData.dat");

        DeckData data = new DeckData();

        List<int> listToSave = new List<int>();

        for (int i = 0; i < deckInProgress.Count; i++) {
            //Debug.Log(deckInProgress[i] + " is an id being copied to SAVE");
            listToSave.Add(deckInProgress[i]);
        }

        data.savedDeckInProgress = listToSave;

        bf.Serialize(file, data);
        file.Close();
    }

    public void LoadDeck() {
        DestroyAllListings();

        if (File.Exists(Application.persistentDataPath + "/deckData.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/deckData.dat", FileMode.Open);

            DeckData data = (DeckData)bf.Deserialize(file);
            file.Close();

            List<int> listToLoad = new List<int>();

            for(int i = 0; i < data.savedDeckInProgress.Count; i++) {
                //Debug.Log(data.savedDeckInProgress[i] + " is an id being copied to LOAD");
                listToLoad.Add(data.savedDeckInProgress[i]);
            }

            List<CardData> deckList = new List<CardData>();

            for (int i = 0; i < listToLoad.Count; i++) {

                CardIDs.CardID id = (CardIDs.CardID)listToLoad[i];
                CardData cardData = Finder.FindCardDataFromDatabase(id);

                deckList.Add(cardData);
            }

            PopulateLoadedDeck(deckList);

        }
    }

    private void PopulateLoadedDeck(List<CardData> cards) {
        for (int i = 0; i < cards.Count; i++) {
            CreateOrAddCardListing(cards[i]);
        }
    }


    #endregion


    [Serializable]
    public class DeckData {
        public List<int> savedDeckInProgress = new List<int>();
    }


    [Serializable]
    public class CardFilterConstraints {
        public Constants.Faction faction;
        public int cost;
        


    }


}
