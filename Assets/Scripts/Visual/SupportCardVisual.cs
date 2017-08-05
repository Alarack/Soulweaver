using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

public class SupportCardVisual : CardVisual {


    [Header("Support Info")]
    public Text cardSupportValueText;
    [Header("Current Stats")]
    public int supportValue;
    [Header("Board Token Info")]
    public SupportToken supportToken;

    private CardSupportData _supportData;

    public override void SetupCardData() {
        base.SetupCardData();

        _supportData = cardData as CardSupportData;

        cardSupportValueText.text = _supportData.supportValue.ToString();

        if(supportToken != null) {
            supportToken.Initialize(_supportData, this);
        }

        int tempSupportValue = _supportData.supportValue;
        supportValue = tempSupportValue;

        supportToken.SetUpSupportText();

        supportToken.UpdateSupportText(Constants.CardStats.SupportValue, supportValue);

    }

    public override void ResetCardData() {
        base.ResetCardData();

        int tempSupportValue = _supportData.supportValue;
        supportValue = tempSupportValue;

        cardSupportValueText.text = _supportData.supportValue.ToString();

        TextTools.SetTextColor(cardSupportValueText, Color.white);

        supportToken.UpdateSupportText(Constants.CardStats.SupportValue, supportValue);
    }

    public override void AlterCardStats(Constants.CardStats stat, int value, CardVisual source, bool waitForVFX = true, bool sendEvent = true) {
        base.AlterCardStats(stat, value, source, waitForVFX, sendEvent);


        switch (stat) {
            case Constants.CardStats.SupportValue:

                supportValue += value;

                if(supportValue > _supportData.supportValue) {
                    supportValue = _supportData.supportValue;
                }

                if (value < 0)
                    CheckDeath(source.photonView.viewID, false, waitForVFX);

                if (!waitForVFX) {
                    cardSupportValueText.text = supportValue.ToString();
                    supportToken.UpdateSupportText(stat, supportValue);
                    TextTools.AlterTextColor(supportValue, _supportData.supportValue, cardSupportValueText);
                }

                break;

            case Constants.CardStats.MaxSupport:

                supportValue += value;

                if (value < 0)
                    CheckDeath(source.photonView.viewID, false, waitForVFX);

                if (!waitForVFX) {
                    cardSupportValueText.text = supportValue.ToString();
                    supportToken.UpdateSupportText(stat, supportValue);
                    TextTools.AlterTextColor(supportValue, _supportData.supportValue, cardSupportValueText);
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
            if (!supportToken.supportTokenGlow.activeInHierarchy) {
                supportToken.supportTokenGlow.SetActive(true);
            }

            if (supportToken.supportTokenGlow.GetComponent<Image>().color != color)
                supportToken.supportTokenGlow.GetComponent<Image>().color = color;

        }
    }

    public override void DeactivateGlow() {
        if (currentDeck.decktype == Constants.DeckType.Hand)
            base.DeactivateGlow();

        if (currentDeck.decktype == Constants.DeckType.Battlefield) {
            supportToken.supportTokenGlow.SetActive(false);
        }
    }


    #region EVENTS

    protected override void OnVFXLanded(EventData data) {
        base.OnVFXLanded(data);

        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        CardVFX vfx = data.GetMonoBehaviour("VFX") as CardVFX;


        if (card != this)
            return;



        switch (lastStatAdjustment.stat) {

            case Constants.CardStats.SupportValue:

                cardSupportValueText.text = supportValue.ToString();
                supportToken.UpdateSupportText(lastStatAdjustment.stat, supportValue);
                TextTools.AlterTextColor(supportValue, _supportData.supportValue, cardSupportValueText);

                break;

      
        }

        Debug.Log(card.gameObject.name + " has been hit with a VFX: " + vfx.gameObject.name);



        Grid.EventManager.RemoveListener(Constants.GameEvent.VFXLanded, OnVFXLanded);
    }

    protected override IEnumerator DisplayDeathEffect() {
        yield return new WaitForSeconds(0.7f);
        GameObject deathVFX;

        if (deathEffect != "")
            deathVFX = PhotonNetwork.Instantiate(deathEffect, supportToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
        else {
            deathVFX = PhotonNetwork.Instantiate("VFX_NecroticFlash", supportToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
        }

        if (deathVFX != null) {
            CardVFX cardVFX = deathVFX.GetComponent<CardVFX>();
            cardVFX.Initialize(this, false, false);
        }

    }

    #endregion



    #region RPCs


    public override void RPCCheckDeath(PhotonTargets targets, CardVisual source, bool forceDeath, bool waitForVFX) {
        int cardID = source.photonView.viewID;

        if (supportValue < 0 && deathEffect != "") {
            Debug.Log(cardData.cardName + " is showing a death effect");

        }

        photonView.RPC("CheckDeath", targets, cardID, forceDeath, waitForVFX);
    }

    [PunRPC]
    public void CheckDeath(int source, bool forceDeath, bool waitForVFX) {
        //GameObject deathVFX;

        if (currentDeck.decktype == Constants.DeckType.SoulCrypt) {
            Debug.LogError(cardData.cardName + " is already dead, and was told to go to the soulcypt");
            return;
        }

        CardVisual causeOfDeath = Finder.FindCardByID(source);

        if (supportValue <= 0 || forceDeath) {

            if (photonView.isMine) {


                if (!waitForVFX) {
                    StartCoroutine(DisplayDeathEffect());
                    StartCoroutine(RemoveCardVisualFromField(this));
                }
                else {
                    Grid.EventManager.RegisterListener(Constants.GameEvent.VFXLanded, OnDeathVisual);
                }

                //if (deathEffect != "")
                //    deathVFX = PhotonNetwork.Instantiate(deathEffect, supportToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
                //else {
                //    deathVFX = PhotonNetwork.Instantiate("VFX_NecroticFlash", supportToken.incomingEffectLocation.position, Quaternion.identity, 0) as GameObject;
                //}
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

    #endregion
}
