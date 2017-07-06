using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CardID = CardIDs.CardID;

[CreateAssetMenu(menuName = "Card")]
public class CardData : ScriptableObject {
    [Header("Basic Card Info")]
    public CardID cardID;
    public string cardName;
    [TextArea(2, 4)]
    public string cardText;
    public int cardCost;
    public Sprite cardImage;

    [Header("Types and Attunements")]
    public Constants.CardType primaryCardType;
    public List<Constants.CardType> otherCardTypes = new List<Constants.CardType>();
    public List<Constants.Attunements> attunements = new List<Constants.Attunements>();
    public List<Constants.SubTypes> subTypes = new List<Constants.SubTypes>();
    [Header("Keywords")]
    public List<Constants.Keywords> keywords = new List<Constants.Keywords>();
    [Header("Special Abilities")]
    [SerializeField]public List<EffectOnTarget> specialAbilities = new List<EffectOnTarget>();
    //public List<EffectOnTarget>


}
