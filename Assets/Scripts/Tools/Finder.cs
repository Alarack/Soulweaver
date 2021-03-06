﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using DeckType = Constants.DeckType;
using CardID = CardIDs.CardID;
using OwnerConstraints = Constants.OwnerConstraints;
using Keywords = Constants.Keywords;
using Subtypes = Constants.SubTypes;
using CardType = Constants.CardType;
using Attunements = Constants.Attunements;
using CardStats = Constants.CardStats;

public static class Finder {


    //public static SpecialAbility FindSpecialAbilityByID(CardVisual source, int ID) {
    //    SpecialAbility result = null;

    //    for(int i = 0; i < source.userTargtedAbilities.Count; i++) {
    //        if (source.userTargtedAbilities[i].ID == ID)
    //            result = source.userTargtedAbilities[i];
    //        break;
    //    }

    //    return result;
    //}



    public static Player FindPlayerByID(int id) {
        Player[] allPlayers = Object.FindObjectsOfType<Player>();

        for (int i = 0; i < allPlayers.Length; i++) {
            if (allPlayers[i].photonView.viewID == id) {
                return allPlayers[i];
            }
        }

        return null;
    }

    public static List<CardVisual> FindAllDamagedOrUndamagedCreatures(bool damaged) {
        List<CardVisual> results = new List<CardVisual>();

        List<CardVisual> allCards = FindAllCardsOfType(CardType.Soul);

        for (int i = 0; i < allCards.Count; i++) {
            CreatureCardVisual soul = allCards[i] as CreatureCardVisual;
            //CardCreatureData soulData = soul.cardData as CardCreatureData;

            //Debug.Log("Checking to see if " + soul.cardData.cardName + " is damaged " + damaged);

            //Debug.Log(soul.health + " is the heath of " + soul.cardData.cardName);

            if (soul.health < soul.maxHealth && damaged) {
                results.Add(soul);

                //Debug.Log(soul.cardData.cardName + " is damaged");

            }

            if (soul.health >= soul.maxHealth && !damaged) {
                results.Add(soul);
            }
        }




        return results;
    }

    public static List<CardVisual> FindCardsWithStatExtreme(CardStats stat, DeckType zone, bool high, OwnerConstraints owner) {
        List<CardVisual> results = new List<CardVisual>();

        //Debug.Log("Finding cards in" + zone.ToString());

        Dictionary<int, int> cardsByStat = StatCollector(stat, zone, owner);
        List<int> sortedStats = cardsByStat.Values.ToList();

        //Debug.Log(sortedStats.Count + " is the number of stats found");

        //for (int i = 0; i < sortedStats.Count; i++) {
        //    Debug.Log(sortedStats[i].ToString() + " is the value of a stat on a soul");
        //}




        int targetStat;
        if (high)
            targetStat = sortedStats.Max();
        else {
            targetStat = sortedStats.Min();
        }

        foreach (KeyValuePair<int, int> entry in cardsByStat) {
            if (entry.Value == targetStat) {
                results.Add(FindCardByID(entry.Key));
            }
        }

        //foreach (CardVisual card in results) {
        //    Debug.Log(card.gameObject.name + " has the least " + stat.ToString());
        //}

        return results;
    }

    private static Dictionary<int, int> StatCollector(CardStats stat, DeckType zone, OwnerConstraints owner) {
        Dictionary<int, int> results = new Dictionary<int, int>();

        List<CardVisual> cardsToSearch = FindAllCardsInZone(zone, owner); //FindAllCardsOfType(CardType.Soul, zone, owner);

        //foreach(CardVisual card in cardsToSearch) {
        //    Debug.Log(card.gameObject.name + " is " + card.cardData.cardName);
        //}

        switch (stat) {
            case CardStats.Cost:

                for (int i = 0; i < cardsToSearch.Count; i++) {
                    results.Add(cardsToSearch[i].photonView.viewID, cardsToSearch[i].essenceCost);
                }
                break;

            case CardStats.Attack:
                for (int i = 0; i < cardsToSearch.Count; i++) {
                    if (cardsToSearch[i].primaryCardType != CardType.Soul)
                        continue;


                    CreatureCardVisual soul = cardsToSearch[i] as CreatureCardVisual;

                    results.Add(soul.photonView.viewID, soul.attack);
                }
                break;

            case CardStats.Size:
                for (int i = 0; i < cardsToSearch.Count; i++) {
                    if (cardsToSearch[i].primaryCardType != CardType.Soul)
                        continue;

                    CreatureCardVisual soul = cardsToSearch[i] as CreatureCardVisual;

                    results.Add(soul.photonView.viewID, soul.size);
                }
                break;

            case CardStats.Health:
                for (int i = 0; i < cardsToSearch.Count; i++) {
                    if (cardsToSearch[i].primaryCardType != CardType.Soul)
                        continue;

                    CreatureCardVisual soul = cardsToSearch[i] as CreatureCardVisual;

                    results.Add(soul.photonView.viewID, soul.health);
                }
                break;
        }

        return results;
    }

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

        for (int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            if (Deck._allCards.activeCards[i].currentDeck.decktype == zone) {
                cards.Add(Deck._allCards.activeCards[i]);
            }
        }

        return cards;
    }

    public static List<CardVisual> FindAllCardsOfType(CardType type, DeckType zone = DeckType.None) {
        List<CardVisual> cardsToSearch = new List<CardVisual>();
        if (zone == DeckType.None)
            cardsToSearch = Deck._allCards.activeCards;
        else {
            cardsToSearch = FindAllCardsInZone(zone);
        }

        List<CardVisual> sortedcards = new List<CardVisual>();

        for(int i = 0; i < cardsToSearch.Count; i++) {
            if(cardsToSearch[i].primaryCardType == type) {
                sortedcards.Add(cardsToSearch[i]);
            }
        }

        return sortedcards;
    }

    public static List<CardVisual> FindAllCardsOfType(CardType type, DeckType zone, OwnerConstraints ownerConstraints) {
        List<CardVisual> cards = FindAllCardsOfType(type, zone);

        List<CardVisual> sortedcards = SortCardsByOwner(cards, ownerConstraints);

        return sortedcards;
    }


    public static List<CardVisual> FindAllCardsOfType(List<CardVisual> cardsToSearch, CardType type) {
        List<CardVisual> results = new List<CardVisual>();

        for (int i = 0; i <cardsToSearch.Count; i++) {
            if (cardsToSearch[i].primaryCardType == type)
                results.Add(cardsToSearch[i]);
        }

        return results;
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

    public static List<CardVisual> FindAllCardsInZone(DeckType zone, Keywords keyWord, OwnerConstraints ownerConstraints, CardType cardType) {
        List<CardVisual> cards = FindAllCardsInZone(zone, keyWord, ownerConstraints);

        List<CardVisual> sortedCards = FindAllCardsOfType(cards, cardType);

        return sortedCards;
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
                    cards.Add(cardsToSort[i]);

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

        for (int i = 0; i < keywords.Count; i++) {
            if (!CardHasKeyword(card, keywords[i])) {
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

    //public static bool CheckConstraints(CardVisual card,
    //    CardType primaryType = CardType.None,
    //    Subtypes subType = Subtypes.None,
    //    Attunements attunement = Attunements.None,
    //    Keywords keyword = Keywords.None,
    //    OwnerConstraints ownerConstraint = OwnerConstraints.None) {

    //    bool result = true;

    //    if (primaryType != CardType.None && result)
    //        result = CardHasPrimaryType(card, primaryType);

    //    if (subType != Subtypes.None && result)
    //        result = CardHasSubType(card, subType);

    //    if (attunement != Attunements.None && result)
    //        result = CardHasAttunement(card, attunement);

    //    if (keyword != Keywords.None && result)
    //        result = CardHasKeyword(card, keyword);

    //    if (ownerConstraint != OwnerConstraints.None && result)
    //        result = CardHasOwner(card, ownerConstraint);


    //    return result;

    //}


    public static Deck FindDeckByID(int id) {
        Deck deck = null;

        //Debug.Log(Deck._allDecks);

        try {
            for (int i = 0; i < NetworkManager._allDecks.Count; i++) {
                if (NetworkManager._allDecks[i].photonView.viewID == id) {
                    deck = NetworkManager._allDecks[i];
                    break;
                }
            }

        }
        catch (MissingReferenceException) {
            for(int i = 0; i < NetworkManager._allDecks.Count; i++) {
                Debug.Log(NetworkManager._allDecks[i].decktype.ToString() + " is a deck in all decks");
                Debug.Log(NetworkManager._allDecks[i].photonView);
            }

        }



        return deck;
    }

    public static CardData FindCardDataFromDatabase(CardID id) {
        CardData data = null;

        for (int i = 0; i < CardDB.cardDB.allCardData.Length; i++) {
            if (CardDB.cardDB.allCardData[i].cardID == id) {
                data = CardDB.cardDB.allCardData[i];
                break;
            }
        }

        return data;
    }

    public static GameObject FindEffectByID(int id) {
        GameObject[] allEffects = GameObject.FindGameObjectsWithTag("VFX");

        for(int i = 0; i < allEffects.Length; i++) {
            if(allEffects[i].GetPhotonView().viewID == id) {
                return allEffects[i];
            }
        }


        return null;

    }

    public static SpecialAbility FindSpecialAbilityOnCardByName(CardVisual source, string abilityName) {
        SpecialAbility result = null;

        for(int i = 0; i < source.specialAbilities.Count; i++) {
            if (source.specialAbilities[i].abilityName == abilityName) {
                result = source.specialAbilities[i];
                break;
            }
                
        }

        return result;
    }

    public static int FindTotalSpellDamage(OwnerConstraints owner) {
        int result = 0;

        List<CardVisual> cards = FindAllCardsInZone(DeckType.Battlefield, owner);

        for (int i = 0; i < cards.Count; i++) {
            result += cards[i].CheckSpecialAttributes(SpecialAttribute.AttributeType.SpellDamage);
        }

        return result;
    }

    public static List<CardVisual> FindAllCardsBeingChosen() {
        List<CardVisual> results = new List<CardVisual>();

        for(int i = 0; i < Deck._allCards.activeCards.Count; i++) {
            if (Deck._allCards.activeCards[i].isBeingChosen)
                results.Add(Deck._allCards.activeCards[i]);
        }


        return results;
    }

    public static CardVisual FindCardsOwner(CardVisual card) {

        List<CardVisual> cardsToSearch = FindAllCardsOfType(CardType.Player);

        for(int i = 0; i < cardsToSearch.Count; i++) {
            if (card.owner == cardsToSearch[i].owner)
                return cardsToSearch[i];
        }

        return null;

    }







}


public static class IListExtensions {
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts) {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i) {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
