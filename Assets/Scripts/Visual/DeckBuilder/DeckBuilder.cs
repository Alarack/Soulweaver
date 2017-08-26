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

    public enum DeckBuilderSubPanel {
        CardSearch,
        GeneralSelector,
        GrimoireSelector,
        FactionSelector
    }


    public int cardCount;
    public Text cardCountText;
    public Text deckNameText;
    public RectTransform cardList;
    public Deck allCards;
    public List<CardPlayerData> allGenerals = new List<CardPlayerData>();
    private int currentIndex;

    public GameObject cardListingTemplate;
    public List<CardListing> currentListings = new List<CardListing>();


    public DeckData deckInProgress = new DeckData();

    //public List<int> deckInProgress = new List<int>();
    public List<string> savedDecks = new List<string>();

    public List<CardFilter> filterButtons = new List<CardFilter>();

    public CardFilterConstraints filters;
    public InputField searchBar;
    public InputField deckNameInput;
    public List<CardData> filteredCards = new List<CardData>();


    public GrimoireListing CurrentDeck { get; set; }

    [Header("Sub Panels")]
    public List<SubPanel> subPanels = new List<SubPanel>();
    public GeneralSelector generalSelector;
    public GrimoireSelector grimoireSelector;
    public FactionSelector factionSelector;

    void Start() {
        allCards = GetComponent<Deck>();

        for (int i = 0; i < CardDB.cardDB.allCardData.Length; i++) {
            if (CardDB.cardDB.allCardData[i].primaryCardType == Constants.CardType.Domain)
                continue;

            if (CardDB.cardDB.allCardData[i].primaryCardType == Constants.CardType.Player) {
                if(CardDB.cardDB.allCardData[i] is CardPlayerData) {
                    Debug.Log(CardDB.cardDB.allCardData[i].cardName + " is a general");
                    allGenerals.Add((CardPlayerData)CardDB.cardDB.allCardData[i]);
                }

                continue;
            }


            if (CardDB.cardDB.allCardData[i].keywords.Contains(Constants.Keywords.Token))
                continue;

            allCards.cards.Add(CardDB.cardDB.allCardData[i]);
        }

        LoadLibraryList();

        RegisterListeners();

        cardCount = 0;
        UpdateCardText();
        filteredCards = FilterCards();


        for (int i = 0; i < filterButtons.Count; i++) {
            filterButtons[i].Initialize(this);
        }


        //PageRight();

        grimoireSelector.Initialize(this);
        factionSelector.Initialize(this);
        generalSelector.Initialize(this, Constants.Faction.All);
        ShowSubPanel(DeckBuilderSubPanel.GrimoireSelector);

    }

    private void RegisterListeners() {
        Grid.EventManager.RegisterListener(Constants.GameEvent.CardSelected, OnCardSelected);

    }


    public void ShowSubPanel(DeckBuilderSubPanel panel, Constants.Faction faction = Constants.Faction.All) {

        SubPanel selectedPanel = null;

        for (int i = 0; i < subPanels.Count; i++) {
            if (subPanels[i].panel == panel) {
                subPanels[i].container.gameObject.SetActive(true);
                selectedPanel = subPanels[i];
            }
            else {
                subPanels[i].container.gameObject.SetActive(false);
            }
        }


        switch (panel) {
            case DeckBuilderSubPanel.CardSearch:
                if (CurrentDeck != null) {
                    filters.faction = CurrentDeck.faction;
                }
                else {
                    filters.faction = Constants.Faction.All;
                }

                filteredCards = FilterCards();
                PageRight();

                break;

            case DeckBuilderSubPanel.GeneralSelector:
                if (faction != Constants.Faction.All) {
                    generalSelector.ResetListings(faction);
                }


                break;

            case DeckBuilderSubPanel.GrimoireSelector:
                LoadLibraryList();
                grimoireSelector.RefreshLibrary();

                break;

            case DeckBuilderSubPanel.FactionSelector:

                break;
        }

    }



    private void DestroyAllDisplays() {

        for (int i = 0; i < allCards.activeCards.Count; i++) {
            Destroy(allCards.activeCards[i].gameObject);
        }

        allCards.activeCards.Clear();

    }

    private void DestroyAllListings() {
        for (int i = 0; i < currentListings.Count; i++) {
            Destroy(currentListings[i].gameObject);
        }

        currentListings.Clear();
        deckInProgress.savedDecklist.Clear();
        deckInProgress.deckName = "";
        deckNameText.text = "";
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
            numToSpawn = filteredCards.Count - 1;

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

        for (int i = 0; i < allCards.cards.Count; i++) {
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

    public void SetDeckName() {

        deckNameText.text = deckNameInput.text;
        deckInProgress.deckName = deckNameInput.text;


    }

    #region SaveLoad

    public void SaveDeck() {

        if (String.IsNullOrEmpty(deckInProgress.deckName)) {

            Debug.LogError("You must name your Grimoire");
            return;
        }


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + deckInProgress.deckName + ".dat");

        DeckData data = new DeckData();

        List<int> listToSave = new List<int>();

        for (int i = 0; i < deckInProgress.savedDecklist.Count; i++) {
            //Debug.Log(deckInProgress[i] + " is an id being copied to SAVE");
            listToSave.Add(deckInProgress.savedDecklist[i]);
        }

        data.savedDecklist = listToSave;
        data.deckName = deckNameText.text;

        bf.Serialize(file, data);
        file.Close();

        savedDecks.Add(deckNameText.text);
        SaveLibraryList();
    }

    private void SaveLibraryList() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/library.dat");

        LibraryData data = new LibraryData();

        data.savedDecks = savedDecks;

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

            //PopulateLoadedDeck(data.deckName, deckList);

        }
    }

    private void LoadLibraryList() {
        if (File.Exists(Application.persistentDataPath + "/library.dat")) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/library.dat", FileMode.Open);

            LibraryData data = (LibraryData)bf.Deserialize(file);
            file.Close();

            savedDecks = data.savedDecks;

        }

    }




    public void PopulateLoadedDeck(DeckData data) {
        DestroyAllListings();


        deckInProgress = data;
        deckNameText.text = deckInProgress.deckName;

        List<CardData> decklist = data.GetCardData();

        for (int i = 0; i < decklist.Count; i++) {
            CreateOrAddCardListing(decklist[i]);
        }
    }







    #endregion


    [Serializable]
    public class DeckData {
        public string deckName;
        public List<int> savedDecklist = new List<int>();


        public List<CardData> GetCardData() {
            List<CardData> results = new List<CardData>();

            for (int i = 0; i < savedDecklist.Count; i++) {

                CardIDs.CardID id = (CardIDs.CardID)savedDecklist[i];
                CardData cardData = Finder.FindCardDataFromDatabase(id);

                results.Add(cardData);
            }
            return results;
        }

        public CardPlayerData GetGeneral() {
            CardPlayerData general = null;

            List<CardData> deckList = GetCardData();

            for (int i = 0; i < deckList.Count; i++) {
                if(deckList[i] is CardPlayerData) {
                    general = (CardPlayerData)deckList[i];
                    break;
                }
            }
            return general;
        }

    }


    [Serializable]
    public class LibraryData {
        public List<string> savedDecks = new List<string>();
    }



    [Serializable]
    public class CardFilterConstraints {
        public Constants.Faction faction;
        public int cost;
    }

    [Serializable]
    public class SubPanel {
        public DeckBuilderSubPanel panel;
        public RectTransform container;
    }

}
