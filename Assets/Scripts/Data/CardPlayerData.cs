using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Player Card")]
[System.Serializable]
public class CardPlayerData : CardCreatureData {

    [Header("Player Stats")]
    public List<CardDomainData> domainPowers = new List<CardDomainData>();




}
