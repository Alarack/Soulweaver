using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeckType = Constants.DeckType;
using CardType = Constants.CardType;

[System.Serializable]
public class EffectSpawnToken : Effect {


    public enum SpawnMethod {
        Basic,
        Copy,
        CopyStats,
        Series,
    }

    public SpawnMethod spawnMethod;
    public string spawnableTokenDataName;
    public DeckType spawnTokenLocation;
    public CardType spawnCardType;
    public int numberOfSpawns;

    public bool spawnForOpponent;

    //Spawn Token Series
    public List<string> tokenSeriesNames;


    private int seriesTokenSpawnIndex;

    //public bool spawnSeriesOfTokens;
    //public bool copyTarget;
    //public bool copyTargetsStatsOnly;

    public override void Apply(CardVisual target) {

        for(int i = 0; i < numberOfSpawns; i++) {
            switch (spawnMethod) {
                case SpawnMethod.Basic:
                    SpawnToken();
                    break;

                case SpawnMethod.Copy:
                    SpawnCopy(target);
                    break;

                case SpawnMethod.CopyStats:
                    SpawnTokenWithCopiedStats(target);
                    break;

                case SpawnMethod.Series:
                    SpawnTokensInSeries();
                    break;
            }
        }

    }

    public CardVisual SpawnCopy(CardVisual target) {
        spawnCardType = target.primaryCardType;
        CardData tokenData = Resources.Load<CardData>("CardData/" + target.cardData.name) as CardData;
        CardVisual tokenCard = SpawnBaseToken(tokenData, GetCardPrefabName(spawnCardType));

        Debug.Log(tokenCard.cardData.cardName + " is being copied");

        return tokenCard;
    }

    public CardVisual SpawnTokenWithCopiedStats(CardVisual target) {
        CardData tokenData = Resources.Load<CardData>("CardData/" + spawnableTokenDataName) as CardData;
        CardVisual tokenCard = SpawnBaseToken(tokenData, GetCardPrefabName(spawnCardType));

        if (target is CreatureCardVisual) {
            CreatureCardVisual soul = target as CreatureCardVisual;

            tokenCard.RPCSetCardStats(PhotonTargets.All, soul.essenceCost, soul.attack, soul.size, soul.health);
        }

        return tokenCard;
    }

    public CardVisual SpawnToken() {
        CardData tokenData = Resources.Load<CardData>("CardData/" + spawnableTokenDataName) as CardData;
        CardVisual tokenCard = SpawnBaseToken(tokenData, GetCardPrefabName(spawnCardType));

        return tokenCard;
    }

    public CardVisual SpawnTokensInSeries() {
        CardData tokenData = Resources.Load<CardData>("CardData/" + tokenSeriesNames[seriesTokenSpawnIndex]) as CardData;
        CardVisual tokenCard = SpawnBaseToken(tokenData, GetCardPrefabName(spawnCardType));

        seriesTokenSpawnIndex++;

        if (seriesTokenSpawnIndex == tokenSeriesNames.Count) {
            seriesTokenSpawnIndex = 0;
        }

        return tokenCard;
    }

    public void SpawnTokenForOpponent() {


    }

    private CardVisual SpawnBaseToken(CardData data, string prefabName) {

        if (spawnForOpponent) {
            source.owner.opponent.activeGrimoire.GetComponent<Deck>().RPCSpawnCardRemote(PhotonTargets.Others, data.cardID, prefabName, spawnTokenLocation);
            return null;
        }

        CardVisual tokenCard = source.owner.activeGrimoire.GetComponent<Deck>().CardFactory(data, prefabName, GetDeckFromType(spawnTokenLocation, source));
        //tokenCard.isToken = true;

        return tokenCard;
    }




}
