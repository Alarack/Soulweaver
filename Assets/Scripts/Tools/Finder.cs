using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DeckType = Constants.DeckType;
using CardID = CardIDs.CardID;
using OwnerConstraints = Constants.OwnerConstraints;
using Keywords = Constants.Keywords;
using Subtypes = Constants.SubTypes;
using CardType = Constants.CardType;
using Attunements = Constants.Attunements;

public static class Finder {





    public static CardVisual FindCardByID(int id) {
        CardVisual card = null;

        for (int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            if (Deck._allCards.activeCards[i].photonView.viewID == id) {
                card = Deck._allCards.activeCards[i];
                break;
            }
        }

        return card;
    }

    public static List<CardVisual> FindAllCardsInZone(DeckType zone) {
        List<CardVisual> cards = new List<CardVisual>();

        for(int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            if(Deck._allCards.activeCards[i].currentDeck.decktype == zone) {
                cards.Add(Deck._allCards.activeCards[i]);
            }
        }

        return cards;
    }

    public static List<CardVisual> FindAllCardsInZone(DeckType zone, OwnerConstraints ownerConstraints) {
        List<CardVisual> cards = FindAllCardsInZone(zone);

        List<CardVisual> sortedCards = SortCardsByOwner(cards, ownerConstraints);

        return sortedCards;
    }

    public static List<CardVisual> FindAllCardsInZone(DeckType zone, Keywords keyWord) {
        List<CardVisual> cards = FindAllCardsInZone(zone);

        List<CardVisual> sortedCards = FindAllCardsWithKeyword(keyWord, cards);

        return sortedCards;
    }

    public static List<CardVisual> FindAllCardsInZone(DeckType zone, Keywords keyWord, OwnerConstraints ownerConstraints) {
        List<CardVisual> cards = FindAllCardsInZone(zone);

        List<CardVisual> sortedCards1 = FindAllCardsWithKeyword(keyWord, cards);
        List<CardVisual> sortedCards2 = SortCardsByOwner(sortedCards1, ownerConstraints);

        return sortedCards2;
    }

    public static List<CardVisual> FindAllCardsWithKeyword(Keywords keyword, List<CardVisual> cardsToSort = null) {
        if (cardsToSort == null)
            cardsToSort = Deck._allCards.activeCards;

        List<CardVisual> cards = new List<CardVisual>();

        for (int i = 0; i < cardsToSort.Count; i++) {
            if (CardHasKeyword(cardsToSort[i], keyword))
                cards.Add(cardsToSort[i]);
        }

        return cards;
    }

    public static List<CardVisual> SortCardsByOwner(List<CardVisual> cardsToSort, OwnerConstraints ownerConstraints) {
        List<CardVisual> cards = new List<CardVisual>();

        for (int i = 0; i < cardsToSort.Count; i++) {
            switch (ownerConstraints) {
                case OwnerConstraints.Mine:
                    if (cardsToSort[i].photonView.isMine)
                        cards.Add(cardsToSort[i]);
                    break;

                case OwnerConstraints.Theirs:
                    if (!cardsToSort[i].photonView.isMine)
                        cards.Add(cardsToSort[i]);
                    break;

                default:

                    break;
            }
        }

        return cards;
    }

    public static bool CardHasKeyword(CardVisual card, Keywords keyword) {
        return card.keywords.Contains(keyword);
    }

    public static bool CardHasMultipleKeywords(CardVisual card, List<Keywords> keywords) {
        bool result = true;

        for(int i = 0; i < keywords.Count; i++) {
            if(!CardHasKeyword(card, keywords[i])) {
                result = false;
                break;
            }
        }

        return result; //TODO: Retun a list of keywords the card has?
    }

    public static bool CardHasPrimaryType(CardVisual card, CardType type) {
        return card.primaryCardType == type;
    }

    public static bool CardHasSubType(CardVisual card, Subtypes subType) {
        return card.subTypes.Contains(subType);
    }
    
    public static bool CardHasAttunement(CardVisual card, Attunements attunement) {
        return card.attunements.Contains(attunement);
    }

    public static bool CardHasOwner(CardVisual card, OwnerConstraints ownerConstraints) {
        switch (ownerConstraints) {
            case OwnerConstraints.Mine:
                return card.photonView.isMine;

            case OwnerConstraints.Theirs:
                return !card.photonView.isMine;

            default:
                return true;
        }
    }

    public static bool CheckConstraints(CardVisual card, 
        CardType primaryType = CardType.None, 
        Subtypes subType = Subtypes.None,
        Attunements attunement = Attunements.None,
        Keywords keyword = Keywords.None,
        OwnerConstraints ownerConstraint = OwnerConstraints.None) {

        bool result = true;

        if (primaryType != CardType.None && result)
            result = CardHasPrimaryType(card, primaryType);

        if (subType != Subtypes.None && result)
            result = CardHasSubType(card, subType);

        if (attunement != Attunements.None && result)
            result = CardHasAttunement(card, attunement);

        if (keyword != Keywords.None && result)
            result = CardHasKeyword(card, keyword);

        if (ownerConstraint != OwnerConstraints.None && result)
            result = CardHasOwner(card, ownerConstraint);


        return result;

    }


    public static Deck FindDeckByID(int id) {
        Deck deck = null;

        for (int i = 0; i < Deck._allDecks.Count; i++) {
            if (Deck._allDecks[i].photonView.viewID == id) {
                deck = Deck._allDecks[i];
                break;
            }
        }

        return deck;
    }

    public static CardData FindCardDataFromDatabase(CardID id) {
        CardData data = null;

        for (int i = 0; i < CardDB.cardDB.allCardData.Length; i++) {
            if(CardDB.cardDB.allCardData[i].cardID == id) {
                data = CardDB.cardDB.allCardData[i];
                break;
            }
        }

        return data;
    }


}
