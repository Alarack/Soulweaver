using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilder : MonoBehaviour {

    public RectTransform cards;
    public Deck allCards;
    public int currentIndex;

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

        //StartCoroutine(SpawnAllCards());
        //SpawnAllCards(0, 7);
        PageRight();
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

    private void PageRight() {

        if (currentIndex >= allCards.cards.Count)
            return;

        DestroyAllDisplays();

        //int previousIndex = currentIndex;

        SpawnAllCards(currentIndex, currentIndex + 7);



    }

    private void PageLeft() {

        if (currentIndex <= 8)
            return;

        DestroyAllDisplays();

        currentIndex -= 16;

        //if (currentIndex < 7)
        //    currentIndex = 7;

        //int previousIndex = currentIndex;

        SpawnAllCards(currentIndex, currentIndex + 7);


    }


    private void SpawnAllCards(int startIndex, int numToSpawn) {

        //Debug.Log(startIndex + " is the start index");
        //Debug.Log(numToSpawn + " is the number to spawn");

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

            //currentIndex++;
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

}
