using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

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
    public Vector3 interceptPos;
    public Image battleFrame;
    //public bool isExhausted;
    //public bool isIntercepting;
    public bool hasAttacked;
    [Header("Current Creature Stats")]
    public int attack;
    public int size;
    public int health;
    [Header("VFX")]
    public string damageVFX;
    public CardVFX damageToken;

    private CardCreatureData _creatureData;

    public override void SetupCardData() {
        base.SetupCardData();

        _creatureData = cardData as CardCreatureData;

        cardAttackText.text = _creatureData.attack.ToString();
        cardSizeText.text = _creatureData.size.ToString();
        cardHealthText.text = _creatureData.health.ToString();

        if(battleToken != null) {
            battleToken.Initialize(_creatureData, this);
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

        if (owner.player2) {
            interceptPos = new Vector3(interceptPos.x, -interceptPos.y, interceptPos.z);
        }

    }

    public override void RestCardData() {
        base.RestCardData();


        int tempAtk = _creatureData.attack;
        attack = tempAtk;

        int tempSize = _creatureData.size;
        size = tempSize;

        int tempHealth = _creatureData.health;
        health = tempHealth;


        StartCoroutine(RestCardVisualData());

    }

    private IEnumerator RestCardVisualData() {
        yield return new WaitForSeconds(0.5f);

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

    public override void AlterCardStats(Constants.CardStats stat, int value, CardVisual source, bool sendEvent = true) {
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

                if(health > _creatureData.health) {
                    health = _creatureData.health;
                }


                cardHealthText.text = health.ToString();
                battleToken.UpdateBattleTokenTokenText(stat, health);
                TextTools.AlterTextColor(health, _creatureData.health, cardHealthText);


                if(value < 1) {
                    //RPCShowDamage(PhotonTargets.Others, value);
                    ShowDamage(value);
                }

                if(value < 0) {
                    CheckDeath(source.photonView.viewID, false);
                }

                break;

            case Constants.CardStats.MaxHealth:

                health += value;

                if (value < 0) {
                    CheckDeath(source.photonView.viewID, false);
                }

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

    //public int CheckSpecialAttributes(SpecialAttribute.AttributeType attribute) {

    //    for (int i = 0; i < specialAttributes.Count; i++) {
    //        if (specialAttributes[i].attributeType == attribute && !specialAttributes[i].suspended) {
    //            return specialAttributes[i].attributeValue;
    //        }
    //    }

    //    return 0;
    //}

    public int CalcProtection(int value) {

        int prot = CheckSpecialAttributes(SpecialAttribute.AttributeType.Protection);

        if (prot > 0) {

            if (value < 0) {
                value += prot;

                if (value > 0)
                    value = 0;
            }
        }

        return value;
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

                    if (keywords.Contains(Constants.Keywords.Interceptor)) {
                        ToggleKeyword(false, (int)Constants.Keywords.Interceptor);
                    }

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
                    if(photonView.isMine)
                        battlefieldPos.position += interceptPos;
                }
                else {
                    cardImage.color = Color.white;
                    battleFrame.color = Color.white;
                    if(photonView.isMine)
                        battlefieldPos.position -= interceptPos;
                }
                break;
        }
    }
    #endregion




    #region RPCs



    public void RPCCheckDeath(PhotonTargets targets, CardVisual source, bool forceDeath = false) {
        int cardID = source.photonView.viewID;

        
        if(health < 0 && deathEffect != "") {
            Debug.Log(cardData.cardName + " is showing a death effect");
            
        }
            


        photonView.RPC("CheckDeath", targets, cardID, forceDeath);
    }

    [PunRPC]
    public void CheckDeath(int source, bool forceDeath) {
        

        if (currentDeck.decktype == Constants.DeckType.SoulCrypt) {
            Debug.LogError(cardData.cardName + " is already dead, and was told to go to the soulcypt");
            return;
        }

        CardVisual causeOfDeath = Finder.FindCardByID(source);
        
        if (health <=0 || forceDeath) {

            if (photonView.isMine) {
                StartCoroutine(DisplayDeathEffect());
            }

            Debug.Log(causeOfDeath.cardData.cardName + " has killed " + cardData.cardName);
            //currentDeck.RPCTransferCard(PhotonTargets.All, this, owner.activeCrypt.GetComponent<Deck>());
            currentDeck.TransferCard(photonView.viewID, owner.activeCrypt.GetComponent<Deck>().photonView.viewID);

            EventData data = new EventData();

            data.AddMonoBehaviour("DeadCard", this);
            data.AddMonoBehaviour("CauseOfDeath", causeOfDeath);

            Grid.EventManager.SendEvent(Constants.GameEvent.CreatureDied, data);
        }

    }

    private IEnumerator DisplayDeathEffect() {
        yield return new WaitForSeconds(0.2f);
        GameObject deathVFX;

        if (deathEffect != "")
            deathVFX = PhotonNetwork.Instantiate(deathEffect, battleToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
        else {
            deathVFX = PhotonNetwork.Instantiate("VFX_NecroticFlash", battleToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
        }

    }

    public void RPCShowDamage(PhotonTargets targets, int damage) {
        //GameObject dmgVFX = PhotonNetwork.Instantiate(damageVFX, transform.position, Quaternion.identity, 0) as GameObject;
        //CardVFX vfx = dmgVFX.GetComponent<CardVFX>();
        //int id = dmgVFX.GetPhotonView().viewID;

        photonView.RPC("ShowDamage", targets, damage);
    }


    [PunRPC]
    public void ShowDamage(int value) {
        //CardVFX vfx = Finder.FindEffectByID(vfxID).GetComponent<CardVFX>();

        //vfx.transform.SetParent(battleToken.incomingEffectLocation, true);
        //vfx.transform.localPosition = Vector3.zero;
        //vfx.transform.SetParent(vfx.transform);

        //vfx.SetText(value.ToString());
        StartCoroutine(ShowDamageEffect(value));

    }

    private IEnumerator ShowDamageEffect(int value) {
        yield return new WaitForSeconds(0.1f);

        damageToken.SetText(value.ToString());
        damageToken.PlayAnim();
        damageToken.PlayParticles();

    }


    #endregion






}
