using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class CreatureCardVisual : CardVisual {


    [Header("Creature Info")]
    public Text cardAttackText;
    public Text cardSizeText;
    public Text cardHealthText;
    [Header("Battle Token Info")]
    public BattleToken battleToken;
    [Header("Status Info")]
    public Color32 exhaustedColor;
    public Color32 interceptingColor;
    public Image battleFrame;
    //public bool isExhausted;
    //public bool isIntercepting;
    public bool hasAttacked;
    [Header("Current Creature Stats")]
    public int attack;
    public int size;
    public int health;

    private CardCreatureData _creatureData;

    public override void SetupCardData() {
        base.SetupCardData();

        _creatureData = cardData as CardCreatureData;

        cardAttackText.text = _creatureData.attack.ToString();
        cardSizeText.text = _creatureData.size.ToString();
        cardHealthText.text = _creatureData.health.ToString();

        if(battleToken != null) {
            battleToken.Initalize(_creatureData, this);
        }


        //Initialzing Current Data
        int tempAtk = _creatureData.attack;
        attack = tempAtk;

        int tempSize = _creatureData.size;
        size = tempSize;

        int tempHealth = _creatureData.health;
        health = tempHealth;

        //attack = _creatureData.attack;
        //size = _creatureData.size;
        //health = _creatureData.health;

        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Attack, attack);
        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Size, size);
        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Health, health);
    }

    public override void RestCardData() {
        base.RestCardData();


        int tempAtk = _creatureData.attack;
        attack = tempAtk;

        int tempSize = _creatureData.size;
        size = tempSize;

        int tempHealth = _creatureData.health;
        health = tempHealth;


        cardAttackText.text = _creatureData.attack.ToString();
        cardSizeText.text = _creatureData.size.ToString();
        cardHealthText.text = _creatureData.health.ToString();

        TextTools.SetTextColor(cardAttackText, Color.white);
        TextTools.SetTextColor(cardSizeText, Color.white);
        TextTools.SetTextColor(cardHealthText, Color.white);

        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Attack, attack);
        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Size, size);
        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Health, health);

    }

    public override void AlterCardStats(Constants.CardStats stat, int value, CardVisual source) {
        base.AlterCardStats(stat, value, source);

        //Debug.Log("creature card alter stat");
        switch (stat) {
            case Constants.CardStats.Attack:
                
                attack += value;

                if (attack <= 0)
                    attack = 0;

                cardAttackText.text = attack.ToString();
                battleToken.UpdateBattleTokenTokenText(stat, attack);
                TextTools.AlterTextColor(attack, _creatureData.attack, cardAttackText);
                break;

            case Constants.CardStats.Size:
                
                size += value;

                if (size <= 0)
                    size = 0;

                cardSizeText.text = size.ToString();
                battleToken.UpdateBattleTokenTokenText(stat, size);
                TextTools.AlterTextColor(size, _creatureData.size, cardSizeText);
                break;

            case Constants.CardStats.Health:

                
                health += value;
                cardHealthText.text = health.ToString();
                battleToken.UpdateBattleTokenTokenText(stat, health);
                TextTools.AlterTextColor(health, _creatureData.health, cardHealthText);
                break;
        }
    }


    public override void ActivateGlow(Color32 color) {
        if(currentDeck.decktype == Constants.DeckType.Hand)
            base.ActivateGlow(color);

        if(currentDeck.decktype == Constants.DeckType.Battlefield) {
            if (!battleToken.battleTokenGlow.activeInHierarchy) {
                battleToken.battleTokenGlow.SetActive(true);
            }

            if (battleToken.battleTokenGlow.GetComponent<Image>().color != color)
                battleToken.battleTokenGlow.GetComponent<Image>().color = color;

        }
    }

    public override void DeactivateGlow() {
        if (currentDeck.decktype == Constants.DeckType.Hand)
            base.DeactivateGlow();

        if (currentDeck.decktype == Constants.DeckType.Battlefield) {
            battleToken.battleTokenGlow.SetActive(false);
        }
    }





    #region Private Methods
    protected override void KeywordHelper(Constants.Keywords keyword, bool add) {
        base.KeywordHelper(keyword, add);
        //Debug.Log("creature visual keyword helper");
        switch (keyword) {
            case Constants.Keywords.Exhausted:
                if (add) {
                    cardImage.color = exhaustedColor;
                    battleFrame.color = exhaustedColor;
                }
                else {
                    cardImage.color = Color.white;
                    battleFrame.color = Color.white;
                }
                break;

            case Constants.Keywords.Interceptor:
                if (add) {
                    cardImage.color = interceptingColor;
                    battleFrame.color = interceptingColor;
                }
                else {
                    cardImage.color = Color.white;
                    battleFrame.color = Color.white;
                }
                break;
        }
    }
    #endregion




    #region RPCs



    public void RPCCheckDeath(PhotonTargets targets, CardVisual source, bool forceDeath = false) {
        int cardID = source.photonView.viewID;

        photonView.RPC("CheckDeath", targets, cardID, forceDeath);
    }

    [PunRPC]
    public void CheckDeath(int source, bool forceDeath) {

        if(currentDeck.decktype == Constants.DeckType.SoulCrypt) {
            Debug.LogError(cardData.cardName + " is already dead, and was told to go to the soulcypt");
            return;
        }

        CardVisual causeOfDeath = Finder.FindCardByID(source);

        if(health <=0 || forceDeath) {
            Debug.Log(causeOfDeath.cardData.cardName + " has killed " + cardData.cardName);
            //currentDeck.RPCTransferCard(PhotonTargets.All, this, owner.activeCrypt.GetComponent<Deck>());
            currentDeck.TransferCard(photonView.viewID, owner.activeCrypt.GetComponent<Deck>().photonView.viewID);
        }

    }




    #endregion






}
