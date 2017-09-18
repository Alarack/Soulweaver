using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

using Keywords = Constants.Keywords;

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
    public int maxHealth;
    [Header("VFX")]
    public string damageVFX;
    public CardVFX damageToken;

    public CardCreatureData _creatureData;

    public override void SetupCardData() {
        base.SetupCardData();

        _creatureData = cardData as CardCreatureData;

        cardAttackText.text = _creatureData.attack.ToString();
        cardSizeText.text = _creatureData.size.ToString();
        cardHealthText.text = _creatureData.health.ToString();

        if (battleToken != null) {
            battleToken.Initialize(_creatureData, this);
        }

        //Initialzing Current Data
        int tempAtk = _creatureData.attack;
        attack = tempAtk;

        int tempSize = _creatureData.size;
        size = tempSize;

        int tempHealth = _creatureData.health;
        health = tempHealth;

        maxHealth = health;

        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Attack, attack);
        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Size, size);
        battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Health, health);

        if (owner != null && owner.player2) {
            interceptPos = new Vector3(interceptPos.x, -interceptPos.y, interceptPos.z);
        }

        Grid.EventManager.RegisterListener(Constants.GameEvent.CardEnteredZone, OnEnterBattlefield);

    }

    public override void ResetCardData() {
        base.ResetCardData();





        StartCoroutine(ResetCardVisualData());
    }

    private IEnumerator ResetCardVisualData() {
        yield return new WaitForSeconds(2.5f);


        int tempAtk = _creatureData.attack;
        attack = tempAtk;

        int tempSize = _creatureData.size;
        size = tempSize;

        int cost = _creatureData.cardCost;
        essenceCost = cost;

        int tempHealth = _creatureData.health;
        health = tempHealth;

        maxHealth = health;

        //Debug.Log("Reseting card data visual");

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

    public override void AlterCardStats(Constants.CardStats stat, int value, CardVisual source, bool waitForVFX = true, bool sendEvent = true, bool setStats = false) {
        base.AlterCardStats(stat, value, source, waitForVFX, sendEvent, setStats);

        //Debug.Log("creature card alter stat");
        switch (stat) {
            case Constants.CardStats.Attack:

                if (setStats)
                    attack = value;
                else
                    attack += value;

                if (!waitForVFX) {

                    if(attack < 0) {
                        cardAttackText.text = 0.ToString();
                    }
                    else {
                        cardAttackText.text = attack.ToString();
                    }

                    battleToken.UpdateBattleTokenTokenText(stat, attack);
                    TextTools.AlterTextColor(attack, _creatureData.attack, cardAttackText);
                }

                break;

            case Constants.CardStats.Size:

                if (setStats)
                    size = value;
                else
                    size += value;

                if (size <= 0)
                    size = 0;

                if (!waitForVFX) {
                    cardSizeText.text = size.ToString();
                    battleToken.UpdateBattleTokenTokenText(stat, size);
                    TextTools.AlterTextColor(size, _creatureData.size, cardSizeText);
                }

                break;

            case Constants.CardStats.Health:
                value = CalcProtection(value);

                if (value < 0 && keywords.Contains(Constants.Keywords.ImmuneToGenerals) && source.primaryCardType == Constants.CardType.Player) {
                    value = 0;
                }

                if (setStats)
                    health = value;
                else
                    health += value;

                if (value > 0 && health > maxHealth) {
                    health = maxHealth;
                }

                if (value < 1) {
                    Debug.Log(gameObject.name + " :: " + cardData.cardName + " has taken " + Mathf.Abs(value) + " point(s) of damage");
                }

                if (value < 0) {
                    CheckDeath(source.photonView.viewID, false, waitForVFX);

                    if (source.keywords.Contains(Keywords.Deathtouch) && health > 0) {
                        health = 0;
                        CheckDeath(source.photonView.viewID, true, waitForVFX);
                    }
                }

                if (!waitForVFX) {
                    cardHealthText.text = health.ToString();
                    battleToken.UpdateBattleTokenTokenText(stat, health);
                    TextTools.AlterTextColor(health, _creatureData.health, cardHealthText);
                    ShowDamage(value);
                }

                break;

            case Constants.CardStats.MaxHealth:

                if (setStats) {
                    maxHealth = value;
                    health = value;
                }
                else {
                    maxHealth += value;
                    health += value;
                }

                if (value < 0) {
                    CheckDeath(source.photonView.viewID, false, waitForVFX);
                }

                if (!waitForVFX) {
                    cardHealthText.text = health.ToString();
                    battleToken.UpdateBattleTokenTokenText(stat, health);
                    TextTools.AlterTextColor(health, _creatureData.health, cardHealthText);
                }

                break;
        }

        if (waitForVFX) {
            SpecialAbility.StatAdjustment latest = new SpecialAbility.StatAdjustment(stat, value, false, false, null);
            lastStatAdjustment = latest;

            Grid.EventManager.RegisterListener(Constants.GameEvent.VFXLanded, OnVFXLanded);
        }
    }

    public override void ActivateGlow(Color32 color) {
        if (currentDeck.decktype == Constants.DeckType.Hand)
            base.ActivateGlow(color);

        if (currentDeck.decktype == Constants.DeckType.Battlefield) {
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

    public bool CanAttack() {
        bool result = true;


        if (keywords.Contains(Keywords.Defender))
            return false;

        if (keywords.Contains(Keywords.NoAttack))
            return false;

        if (keywords.Contains(Keywords.Pacifist))
            return false;

        if (keywords.Contains(Keywords.Stun))
            return false;

        if (keywords.Contains(Keywords.Exhausted))
            return false;

        if (hasAttacked)
            return false;

        if (attack < 1)
            return false;


        return result;
    }

    #region Private Methods

    private bool CheckForSummonSickness() {
        bool result = true;

        if (keywords.Contains(Constants.Keywords.Vanguard))
            return false;

        if (keywords.Contains(Constants.Keywords.Rush))
            return false;

        return result;
    }

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

                    if (hasAttacked)
                        hasAttacked = false;

                }
                break;

            case Constants.Keywords.Interceptor:
                if (add) {
                    cardImage.color = interceptingColor;
                    battleFrame.color = interceptingColor;
                    if (photonView.isMine)
                        battlefieldPos.position += interceptPos;
                }
                else {
                    cardImage.color = Color.white;
                    battleFrame.color = Color.white;
                    if (photonView.isMine)
                        battlefieldPos.position -= interceptPos;
                }
                break;

            case Constants.Keywords.NoIntercept:
                if (add) {

                    if (keywords.Contains(Constants.Keywords.Interceptor)) {
                        ToggleKeyword(false, (int)Constants.Keywords.Interceptor);
                    }

                }

                break;

            case Keywords.Pacifist:
                if (add) {
                    if (keywords.Contains(Constants.Keywords.Interceptor)) {
                        ToggleKeyword(false, (int)Constants.Keywords.Interceptor);
                    }

                }

                break;


        }
    }
    #endregion


    #region EVENTS

    protected override void OnVFXLanded(EventData data) {
        base.OnVFXLanded(data);

        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        //CardVFX vfx = data.GetMonoBehaviour("VFX") as CardVFX;

        if (card != this)
            return;

        switch (lastStatAdjustment.stat) {

            case Constants.CardStats.Attack:

                if (attack < 0) {
                    cardAttackText.text = 0.ToString();
                }
                else {
                    cardAttackText.text = attack.ToString();
                }

                battleToken.UpdateBattleTokenTokenText(lastStatAdjustment.stat, attack);
                TextTools.AlterTextColor(attack, _creatureData.attack, cardAttackText);
                break;

            case Constants.CardStats.Size:
                cardSizeText.text = size.ToString();
                battleToken.UpdateBattleTokenTokenText(lastStatAdjustment.stat, size);
                TextTools.AlterTextColor(size, _creatureData.size, cardSizeText);
                break;

            case Constants.CardStats.Health:
                ShowDamage(lastStatAdjustment.value);
                cardHealthText.text = health.ToString();
                battleToken.UpdateBattleTokenTokenText(lastStatAdjustment.stat, health);
                TextTools.AlterTextColor(health, maxHealth, cardHealthText);
                break;

            case Constants.CardStats.MaxHealth:
                ShowDamage(lastStatAdjustment.value);
                cardHealthText.text = health.ToString();
                battleToken.UpdateBattleTokenTokenText(lastStatAdjustment.stat, health);
                TextTools.AlterTextColor(health, _creatureData.health, cardHealthText);
                break;
        }

        Grid.EventManager.RemoveListener(Constants.GameEvent.VFXLanded, OnVFXLanded);
    }

    private void OnEnterBattlefield(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (card != this)
            return;

        if (deck.decktype != Constants.DeckType.Battlefield)
            return;

        if (CheckForSummonSickness())
            hasAttacked = true;

    }

    #endregion



    #region RPCs



    public override void RPCCheckDeath(PhotonTargets targets, CardVisual source, bool forceDeath, bool waitForVFX) {
        int cardID = source.photonView.viewID;

        if (health < 0 && deathEffect != "") {
            Debug.Log(cardData.cardName + " is showing a death effect");
        }

        photonView.RPC("CheckDeath", targets, cardID, forceDeath, waitForVFX);
    }

    [PunRPC]
    public void CheckDeath(int source, bool forceDeath, bool waitForVFX) {

        if (currentDeck.decktype == Constants.DeckType.SoulCrypt) {
            Debug.LogError(cardData.cardName + " is already dead, and was told to go to the soulcypt");
            return;
        }

        CardVisual causeOfDeath = Finder.FindCardByID(source);

        if (health <= 0 || forceDeath) {

            if (photonView.isMine) {
                //StartCoroutine(DisplayDeathEffect());

                if (!waitForVFX) {
                    StartCoroutine(DisplayDeathEffect());
                    StartCoroutine(RemoveCardVisualFromField(this));
                }
                else {
                    Grid.EventManager.RegisterListener(Constants.GameEvent.VFXLanded, OnDeathVisual);
                }

                if (keywords.Contains(Keywords.Interceptor)) {
                    battlefieldPos.position -= interceptPos;
                }

            }

            Debug.Log(causeOfDeath.cardData.cardName + " has killed " + cardData.cardName);

            if (CheckSpecialAttributes(SpecialAttribute.AttributeType.Volatile) > 0) {
                HandleVolatile(causeOfDeath);
            }

            currentDeck.TransferCard(photonView.viewID, owner.activeCrypt.GetComponent<Deck>().photonView.viewID);

            EventData data = new EventData();

            data.AddMonoBehaviour("DeadCard", this);
            data.AddMonoBehaviour("CauseOfDeath", causeOfDeath);

            Grid.EventManager.SendEvent(Constants.GameEvent.CreatureDied, data);
        }

    }

    private void HandleVolatile(CardVisual sourceOfDeath) {
        int volatileValue = CheckSpecialAttributes(SpecialAttribute.AttributeType.Volatile);

        if (sourceOfDeath.primaryCardType == Constants.CardType.Soul) {
            sourceOfDeath.ApplyCombatDamage((int)Constants.CardStats.Health, -volatileValue, photonView.viewID, false);
        }
        else {
            CardVisual murderer = Finder.FindCardsOwner(sourceOfDeath);
            murderer.ApplyCombatDamage((int)Constants.CardStats.Health, -volatileValue, photonView.viewID, false);
        }
    }



    protected override IEnumerator DisplayDeathEffect() {
        if (currentDeck.decktype != Constants.DeckType.Battlefield)
            yield return null;


        yield return new WaitForSeconds(0.7f);
        GameObject deathVFX;

        bool hasDeathEffect = string.IsNullOrEmpty(deathEffect);

        if (!hasDeathEffect)
            deathVFX = PhotonNetwork.Instantiate(deathEffect, battleToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
        else {
            deathVFX = PhotonNetwork.Instantiate("VFX_NecroticFlash", battleToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
        }

        if (deathVFX != null) {
            CardVFX cardVFX = deathVFX.GetComponent<CardVFX>();
            cardVFX.Initialize(this, false, false);
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
        StartCoroutine(ShowDamageEffect(value));
    }

    private IEnumerator ShowDamageEffect(int value) {
        yield return new WaitForSeconds(0.1f);

        if (value > 0) {
            damageToken.SetText("+" + value.ToString());
        }
        else {
            damageToken.SetText(value.ToString());
        }

        //Debug.Log(value + " was given to SHOW DAMAGE");

        damageToken.PlayAnim();
        damageToken.PlayParticles();

    }


    #endregion






}
