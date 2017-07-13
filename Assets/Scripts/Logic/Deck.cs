using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;

using DeckType = Constants.DeckType;


public class Deck : Photon.MonoBehaviour {

    public DeckType decktype;
    public List<CardData> cards = new List<CardData>();
    public List<CardVisual> activeCards = new List<CardVisual>();
    public Player owner;

    //public Deck ownersHand;

    //public static Deck _battlefield;
    public static Deck _allCards;
    public static Deck _void;
    public static List<Deck> _allDecks = new List<Deck>();


    void Awake() {



        if (decktype == DeckType.AllCards) {
            _allCards = this;
        }

        if(decktype == DeckType.Void) {
            _void = this;
        }

        RegisterDeck();
    }


    void Start() {
        //if (decktype == DeckType.Battlefield) {
        //    owner.battlefield = this;
        //    //owner = GetComponentInParent<Player>();
        //}
    }

    void Update() {

    }

    public void RegisterDeck() {
        _allDecks.Add(this);
    }

    public void AddCard(CardVisual card) {
        activeCards.Add(card);
        card.currentDeck = this;

        EventData data = new EventData();

        data.AddMonoBehaviour("Card", card);
        data.AddMonoBehaviour("Deck", this);

        Grid.EventManager.SendEvent(Constants.GameEvent.CardEnteredZone, data);

    }

    public void RemoveCard(CardVisual card) {
        if (activeCards.Contains(card)) {
            card.previousDeck = this;
            activeCards.Remove(card);
        }

        EventData data = new EventData();

        data.AddMonoBehaviour("Card", card);
        data.AddMonoBehaviour("Deck", this);

        Grid.EventManager.SendEvent(Constants.GameEvent.CardLeftZone, data);
    }


    public void DrawCard() {
        if (photonView.isMine && activeCards.Count > 0 && owner.GetComponent<Deck>().activeCards.Count < owner.handManager.cardPositions.Count) {

            int randomCard = Random.Range(0, activeCards.Count);
            //Debug.Log(activeCards[randomCard].gameObject.name);


            //if (owner.gameHasStarted) {
            activeCards[randomCard].RPCSetCardAciveState(PhotonTargets.All, true);
            //}

            switch (decktype) {
                case DeckType.Grimoire:
                    RPCTransferCard(PhotonTargets.All, activeCards[randomCard], owner.myHand);

                    break;

                case DeckType.Domain:
                    //RPCTransferCard(PhotonTargets.All, activeCards[0], _battlefield);
                    RPCTransferCard(PhotonTargets.All, activeCards[0], owner.battlefield);

                    break;
            }
        }
        else {
            if (photonView.isMine && decktype == DeckType.Grimoire)
                Debug.Log("Deck Empty or Hand Full");
        }
    }

    public void RefreshAll(List<CardVisual> cards = null) {
        if (cards == null)
            cards = activeCards;

        for (int i = 0; i < cards.Count; i++) {
            //CreatureCardVisual creature;
            //if (cards[i] is CreatureCardVisual) {
            //    creature = cards[i] as CreatureCardVisual;
            //    // Creature Only Stuff
            //}

            cards[i].RPCToggleExhaust(PhotonTargets.All, false);

        }

    }

    #region CardFactory

    public IEnumerator SpawnAllCards() {

        //Debug.Log(gameObject.name + " is spawning cards");

        int index = 0;
        foreach (CardData card in cards) {
            yield return null;
            index++;
            switch (card.primaryCardType) {
                case Constants.CardType.Soul:
                    CardFactory(card, GlobalSettings._globalSettings.creatureCard.name, null, index);
                    //Debug.Log(card.cardName + " is a soul and being spawned");
                    break;

                case Constants.CardType.Spell:
                    CardFactory(card, GlobalSettings._globalSettings.spellCard.name, null, index);
                    break;

                case Constants.CardType.Player:
                    //CardFactory(card, GlobalSettings._globalSettings.playerCard.name, _battlefield);
                    CardFactory(card, GlobalSettings._globalSettings.playerCard.name, owner.battlefield, index);

                    break;
            }
        }

        //if(_allCards.activeCards.Count >= cards.Count) {
        //    DoneSpawningCards();
        //}
    }

    public string GetCardPrefabNameByType(Constants.CardType type) {
        string result = "";

        switch (type) {
            case Constants.CardType.Soul:
                result = GlobalSettings._globalSettings.creatureCard.name;
                break;

            case Constants.CardType.Spell:

                result = GlobalSettings._globalSettings.spellCard.name;
                break;

            case Constants.CardType.Player:
                result = GlobalSettings._globalSettings.playerCard.name;

                break;
        }

        return result;
    }


    public void DoneSpawningCards() {
        Debug.Log(gameObject.name + " Is done spanwing cards");
        //for(int i = 0; i < activeCards.Count; i++) {
        //    activeCards[i].SourceAssignment();
        //}
    }

    public CardVisual CardFactory(CardData data, string prefabname, Deck targetDeck = null, int index = 0) {
        GameObject activeCard = PhotonNetwork.Instantiate(prefabname, new Vector3(40f, 20f, 20f), Quaternion.identity, 0) as GameObject;
        CardVisual cardVisual = activeCard.GetComponent<CardVisual>();

        cardVisual.RegisterCard(cardVisual.photonView.viewID);
        cardVisual.RPCRegisterCard(PhotonTargets.Others, cardVisual.photonView.viewID);

        cardVisual.cardData = data;
        RPCSetCardData(PhotonTargets.Others, data.cardID, cardVisual.photonView.viewID);

        activeCards.Add(cardVisual);
        activeCard.transform.SetParent(GameObject.FindGameObjectWithTag("AllCards").transform, false); //TODO: make this happen in the card itself.

        AssignCardLocationAndOwner(activeCard, owner, this);
        cardVisual.RPCSetOwner(PhotonTargets.Others);
        cardVisual.RPCSetParentDeck(PhotonTargets.Others, decktype.ToString());

        if (targetDeck != null) {

            switch (targetDeck.decktype) {
                case DeckType.Battlefield:

                    if (owner.battleFieldManager.IsCollectionFull()) {
                        if (owner.handManager.IsCollectionFull()) {
                            RPCTransferCard(PhotonTargets.All, cardVisual, owner.activeCrypt.GetComponent<Deck>());
                        }
                        else {
                            RPCTransferCard(PhotonTargets.All, cardVisual, owner.myHand);
                        }
                    }
                    else {
                        RPCTransferCard(PhotonTargets.All, cardVisual, targetDeck);
                    }

                    break;

                case DeckType.Hand:
                    if (owner.handManager.IsCollectionFull()) {
                        RPCTransferCard(PhotonTargets.All, cardVisual, owner.activeCrypt.GetComponent<Deck>());
                    }
                    else {
                        RPCTransferCard(PhotonTargets.All, cardVisual, targetDeck);
                    }

                    break;
            }

        }

        if (owner.player2) {
            activeCard.transform.localRotation = owner.p2HandRot.localRotation;
        }
        else {
            cardVisual.RPCRotateCard(PhotonTargets.Others);
        }

        cardVisual.RPCSetUpCardData(PhotonTargets.All);
        activeCard.name = cardVisual.cardData.cardName + " " + index;
        //Debug.Log(cardVisual.photonView.viewID + " has been created");
        //cardVisual.InitializeSpecialAbilities(cardVisual.photonView.viewID);

        return cardVisual;
    }

    public void AssignCardLocationAndOwner(GameObject newCard, Player owner, Deck parent) {
        //Debug.Log("Assigning " + newCard.name + " to " + owner + " in " + parent);

        CardVisual cardScript = newCard.GetComponent<CardVisual>();

        cardScript.owner = owner;
        cardScript.currentDeck = parent;
    }

    #endregion

    #region Private Methods

    private void CheckAndActivateCard(CardVisual card) {
        if (!card.active) {
            card.RPCSetCardAciveState(PhotonTargets.All, true);
        }
    }

    #endregion



    #region RPCs

    public void RPCAssignDeckInfo(PhotonTargets targets, int ID) {
        photonView.RPC("AssignDeckInfo", targets, ID);
    }

    [PunRPC]
    public void AssignDeckInfo(int id) {
        Player[] allPlayers = FindObjectsOfType<Player>();

        foreach (Player p in allPlayers) {
            if (p.gameObject.GetPhotonView().viewID == id) {
                owner = p;
                break;
            }
        }

        if (owner == null) {
            Debug.LogError("[Deck] owner not found during info assignment");
            return;
        }

        //ownersHand = owner.myHand;
        transform.SetParent(owner.gameObject.transform, false);
        switch (decktype) {
            case DeckType.Grimoire:
                owner.activeGrimoire = gameObject;
                break;

            case DeckType.Domain:
                owner.activeDomain = gameObject;
                break;

            case DeckType.SoulCrypt:
                owner.activeCrypt = gameObject;
                break;
        }
    }


    public void RPCTransferCard(PhotonTargets targets, CardVisual card, Deck targetLocation) {

        int cardID = card.photonView.viewID;
        int deckID = targetLocation.photonView.viewID;

        photonView.RPC("TransferCard", targets, cardID, deckID);
    }


    [PunRPC]
    public void TransferCard(int cardID, int deckID) {

        CardVisual card = Finder.FindCardByID(cardID);
        Deck targetLocation = Finder.FindDeckByID(deckID);

        if (owner == null) {
            owner = card.owner;
        }


        if (card == null) {
            Debug.LogError("[Deck] Could not find card with ID: " + cardID);
            return;
        }

        if (targetLocation == null) {
            Debug.LogError("[Deck] Could not find Deck with ID: " + deckID);
            return;
        }

        if (!activeCards.Contains(card)) {
            Debug.LogError("[Deck] " + card.cardData.cardName + " is not in the deck its being transfered from");
            return;
        }

        CheckAndActivateCard(card);

        switch (targetLocation.decktype) {
            case DeckType.Hand:
                SendCardToHand(card);

                break;

            case DeckType.Battlefield:
                SendCardToBattlefield(card);

                break;

            case DeckType.SoulCrypt:
                StartCoroutine(SendCardToSoulCrypt(card));

                break;

            case DeckType.Void:
                SendCardToVoid(card);
                break;

            default:

                break;

        }

        if (card.currentDeck.decktype == DeckType.Hand) {
            owner.handManager.ReleaseCardPosition(card.handPos);
        }

        if (card.currentDeck.decktype == DeckType.Battlefield) {
            owner.battleFieldManager.ReleaseCardPosition(card.battlefieldPos);
        }

        //card.previousDeck = this;

        RemoveCard(card);
        targetLocation.AddCard(card);

    }


    public void RPCSetCardData(PhotonTargets targets, CardIDs.CardID id, int cardVisualPhotonViewID) {
        int cardDataID = (int)id;

        photonView.RPC("SetCardData", targets, cardDataID, cardVisualPhotonViewID);
    }


    [PunRPC]
    public void SetCardData(int cardDataID, int cardVisualPhotonViewID) {

        CardIDs.CardID dataID = (CardIDs.CardID)cardDataID;
        CardVisual cardVisual = Finder.FindCardByID(cardVisualPhotonViewID);

        if (cardVisual == null) {
            Debug.LogError("[Deck] could not find card with View ID " + cardVisualPhotonViewID);
            return;
        }

        CardData cardData = Finder.FindCardDataFromDatabase(dataID);

        if (cardData == null) {
            Debug.LogError("[Deck] could not find CardData with id " + dataID.ToString());
            return;
        }

        cardVisual.cardData = cardData;
    }

    #endregion

    #region Transfer Card Helpers

    private void SendCardToHand(CardVisual card) {

        //Debug.Log(card.gameObject.name + " is being sent to the hand");

        if (owner.handManager.IsCollectionFull()) {
            Debug.LogWarning("Hand is Full");
            StartCoroutine(SendCardToSoulCrypt(card));
            return;
        }

        if (card.photonView.isMine) {
            card.RPCChangeCardVisualState(PhotonTargets.Others, CardVisual.CardVisualState.ShowBack);
            card.ChangeCardVisualState((int)CardVisual.CardVisualState.ShowFront);
            //Debug.Log(card.cardData.cardName + " is mine and is showing the card back to others");

        }

        card.handPos = owner.handManager.GetFirstEmptyCardPosition();

    }

    private void SendCardToBattlefield(CardVisual card) {
        if (owner == null) {
            owner = card.owner;
        }


        if (owner.battleFieldManager.IsCollectionFull()) {
            Debug.LogWarning("board is Full");
            //SendCardToHand(card);
            if (card.photonView.isMine) {
                RPCTransferCard(PhotonTargets.All, card, owner.myHand);
            }
            return;
        }

        //Debug.Log(card.gameObject.name + " is going to the battlefield");

        switch (card.cardData.primaryCardType) {
            case Constants.CardType.Player:
                card.RPCChangeCardVisualState(PhotonTargets.All, CardVisual.CardVisualState.ShowBattleToken);

                if (card.photonView.isMine)
                    card.battlefieldPos = owner.battleFieldManager.AssignSpecificPosition(owner.battleFieldManager.transform.FindChild("PlayerBattlePos"));

                break;

            case Constants.CardType.Soul:
                card.RPCChangeCardVisualState(PhotonTargets.All, CardVisual.CardVisualState.ShowBattleToken);

                if (card.photonView.isMine) {
                    card.battlefieldPos = owner.battleFieldManager.GetFirstEmptyCardPosition();
                }


                break;

            case Constants.CardType.Domain:
                //TODO: Domain Manager

                break;

            default:

                break;
        }
    }

    private IEnumerator SendCardToSoulCrypt(CardVisual card) {
        yield return new WaitForSeconds(0.4f);

        if (card.photonView.isMine) {
            card.ChangeCardVisualState((int)CardVisual.CardVisualState.ShowFront);
            card.RPCChangeCardVisualState(PhotonTargets.Others, CardVisual.CardVisualState.ShowBack);
        }

        if (card is CreatureCardVisual) {
            CreatureCardVisual creature = card as CreatureCardVisual;
            creature.RPCToggleExhaust(PhotonTargets.All, false);
        }

        card.RestCardData();
        //card.RPCSetUpCardData(PhotonTargets.All);
        //card.SetupCardData();
        if (card.photonView.isMine)
            card.transform.localPosition = new Vector3(-40f, 20f, 20f);


    }

    private void SendCardToVoid(CardVisual card) {
        if (card.photonView.isMine) {
            card.ChangeCardVisualState((int)CardVisual.CardVisualState.ShowFront);
            card.RPCChangeCardVisualState(PhotonTargets.Others, CardVisual.CardVisualState.ShowBack);
        }

        if (card is CreatureCardVisual) {
            CreatureCardVisual creature = card as CreatureCardVisual;
            creature.RPCToggleExhaust(PhotonTargets.All, false);
        }

        card.RestCardData();
        //card.RPCSetUpCardData(PhotonTargets.All);
        //card.SetupCardData();
        if (card.photonView.isMine)
            card.transform.localPosition = new Vector3(-40f, 20f, -40f);


    }

    #endregion

}
