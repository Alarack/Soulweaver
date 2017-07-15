using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SoulWeaver;

public class Player : Photon.MonoBehaviour {

    public enum GameStates {
        Refresh,
        Upkeep,
        Draw,
        Main,
        End,
        Idle
    }

    public GameStates gameState;
    //public int life = 20;
    //public int maxEssence;
    //public int curEssence;
    public int startingHandSize = 4;
    public Deck myHand;
    public Transform p2HandRot;
    //public GameObject mulliganLocations;
    //public GameObject mulliganButton;
    [Header("Card Position Managers")]
    public CardPositionManager handManager;
    public CardPositionManager battleFieldManager;
    [Header("Decks to Load")]
    //public GameObject grimoire;
    //public GameObject domain;
    //public GameObject soulCrypt;
    public List<DeckInfo> deckInfo = new List<DeckInfo>();
    [Header("HUD stuff")]
    public GameObject hud;
    public GameObject myCamera;
    public Button endTurnButton;
    //public Transform camRot;
    //public Transform grimLoc;
    //public Transform domLoc;
    //public Transform cryptLoc;
    //public Transform visualTokenLoc;
    //public Text myEssenceText;
    //public Text enemyEssenceText;
    [Space(10)]
    public float lerpSmoothing = 10f;
    public bool player2;
    public bool myTurn;
    public bool firstTurn = true;
    [Header("Player Card")]
    public GameObject playerCard;
    public Transform playerLoc;

    [Header("Active Decks")]
    public GameObject activeGrimoire;
    public GameObject activeDomain;
    public GameObject activeCrypt;
    public Deck battlefield;


    [Header("Resources")]
    public List<GameResource> gameResources = new List<GameResource>();
    public GameResourceDisplay gameResourceDisplay;

    //[Header("UI Stuff")]
    //public GameObject returnToMenuButton;

    [Space(10)]
    public Player opponent;
    //public Deck myHand;

    [HideInInspector]
    public bool gameHasStarted = false;

    public ClickMoveDrop dragger;

    [Space(10)]
    //public Button endTurnButton;
    private Vector3 position;
    private Quaternion rotation;
    //private bool isAlive = true;


    public CombatManager combatManager;

    void Start() {
        combatManager = GetComponentInChildren<CombatManager>();

        RPCConfirmPos(PhotonTargets.AllBufferedViaServer);

        if (photonView.isMine) {
            gameObject.name = "Me";
            myCamera.SetActive(true);
            hud.SetActive(true);
            myHand = GetComponent<Deck>();

            //GameObject.Find("Battlefield").GetComponent<Deck>().owner = this;
            //Timeline.timeline.owner = this;
        }
        else {
            gameObject.name = "Network Player";
            //myCamera.SetActive(false);
            //hud.SetActive(false);
            Destroy(hud);
            Destroy(myCamera);
        }
    }

    void Update() {
        if (myTurn && photonView.isMine && gameHasStarted) {
            switch (gameState) {
                case GameStates.Refresh:
                    endTurnButton.interactable = true;
                    //DrawFromDomain();
                    //RefreshAll();
                    if (battlefield != null)
                        RefreshMySouls();

                    StartCoroutine(RPCBroadcastTurnStart(PhotonTargets.All, this));

                    break;

                case GameStates.Upkeep:
                    PlayerUpkeep();
                    break;

                case GameStates.Draw:
                    DrawFromGrimoire();
                    break;

                case GameStates.Main:

                    break;

                case GameStates.End:
                    CheckEndOfTurnEffects();

                    myTurn = false;

                    if (opponent != null)
                        opponent.RPCResetState(PhotonTargets.All);
                    else {
                        ResetState();
                    }

                    break;

                case GameStates.Idle:

                    break;
            }
        }
        else {
            gameState = GameStates.Idle;
        }

        if (Input.GetKeyDown(KeyCode.D)) {
            DrawFromGrimoire();
        }

    }//End of Update



    public void EndTurn() {
        if (myTurn) {
            endTurnButton.interactable = false;
            gameState = GameStates.End;
        }
    }

    void CheckEndOfTurnEffects() {
        //EndOfTurnCleanUp();
        //KillGlow();
        //EventManager.EffectOnEndTurn();

        RPCBroadcastTurnEnd(PhotonTargets.All, this);
    }

    //void KillGlow() {
    //    foreach (GameObject card in GameObject.FindGameObjectWithTag("Battlefield").GetComponent<Deck>().activeCards) {
    //        if (card.GetPhotonView().isMine && card.GetComponent<CardModel>().glowEffect != null && card.GetComponent<CardModel>().glowEffect.activeSelf) {
    //            card.GetComponent<CardModel>().glowEffect.SetActive(false);
    //        }
    //    }

    //    foreach (GameObject card in myHand.activeCards) {
    //        if (card.GetPhotonView().isMine && card.GetComponent<CardModel>().glowEffect != null && card.GetComponent<CardModel>().glowEffect.activeSelf) {
    //            card.GetComponent<CardModel>().glowEffect.SetActive(false);
    //        }
    //    }


    //}

    public IEnumerator StartGame() {
        //domainManager = activeDomain.GetComponent<DomainManager>();

        RPCSetupResources(PhotonTargets.AllBufferedViaServer, true);
        yield return new WaitForSeconds(1f);
        //yield return null;
        //domainManager.gameObject.GetPhotonView().RPC("InitDomain", PhotonTargets.All);

        RPCCheckOpponents(PhotonTargets.All);

        if (myTurn) {
            gameState = GameStates.Refresh;
        }


        StartCoroutine(DrawStartingHand());
        //mulliganButton.SetActive(true);


        gameHasStarted = true;

    }

    public void EndGame() {
        NetworkManager._networkManager.firstPlayer = false;
        //Mulligan.choosingMulligan = true;
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene("Menu");

    }

    public void RefreshMySouls() {
        List<CardVisual> cardsOnField = Finder.FindAllCardsInZone(Constants.DeckType.Battlefield, Constants.OwnerConstraints.Mine);

        //Deck._battlefield.RefreshAll(cardsOnField);
        battlefield.RefreshAll(cardsOnField);

        for (int i = 0; i < cardsOnField.Count; i++) {
            if (cardsOnField[i] is CreatureCardVisual) {
                CreatureCardVisual creature = cardsOnField[i] as CreatureCardVisual;
                creature.hasAttacked = false;
            }
        }

        gameState = GameStates.Upkeep;
    }

    //public void RefreshAll() {
    //    foreach (CardModel c in TargetUtils.FindAllCardModelsOnBattlefield()) {
    //        if (!c.visualOnly && c.gameObject.GetPhotonView().isMine && c.parentDeck.decktype == Deck.DeckType.Battlefield) {
    //            if (c.isExhausted)
    //                c.gameObject.GetPhotonView().RPC("ToggleExhaust", PhotonTargets.All, false);

    //            if (c.GetComponent<CardCreature>() != null && c.GetComponent<CardCreature>().hasAttacked) {
    //                c.GetComponent<CardCreature>().hasAttacked = false;
    //            }

    //            if (c.glowEffect == null && !c.cardTypes.Contains(CardModel.CardType.Player)) {
    //                c.CreateGlowEffect();
    //            }

    //            if (c.glowEffect != null && !c.keywords.Contains(CardModel.Keywords.Pacifist) && !c.keywords.Contains(CardModel.Keywords.Defender)) {
    //                c.glowEffect.SetActive(true);
    //            }

    //            if ((c.keywords.Contains(CardModel.Keywords.Pacifist) || c.keywords.Contains(CardModel.Keywords.Defender) && c.glowEffect.activeSelf)) {
    //                c.glowEffect.SetActive(false);
    //            }

    //            if (c.GetComponent<ActivatedEffect>() != null && c.GetComponent<ActivatedEffect>().firstPlayed) {
    //                c.GetComponent<ActivatedEffect>().firstPlayed = false;
    //            }


    //            if (c.GetComponent<CountTowardEffect>() != null && c.GetComponent<CountTowardEffect>().conditions == CountTowardEffect.CountConditions.AutoIncrament) {
    //                c.GetComponent<CountTowardEffect>().AddToCounter(1);
    //                Debug.Log("Incramenting " + c.cardName);
    //            }

    //        }

    //        foreach (DomainTile dt in activeDomain.GetComponent<DomainManager>().domainTiles) {
    //            if (dt.myDomainCard.GetComponent<CardModel>().parentDeck.decktype == Deck.DeckType.Battlefield && dt.GetComponent<Image>().color == Color.gray) {
    //                //dt.GetComponent<Image>().color = Color.blue;
    //                dt.gameObject.GetPhotonView().RPC("ExhaustTile", PhotonTargets.All, false);
    //            }
    //        }

    //    }
    //    gameState = GameStates.Upkeep;
    //}

    public void PlayerUpkeep() {
        AdjustMaxEssence(1);
        FillEssence();

        //for (int i = 0; i < gameResourceDisplay.resourceDisplayInfo.Count; i++) {
        //    if (gameResourceDisplay.resourceDisplayInfo[i].addPerTurn) {
        //        gameResourceDisplay.resourceDisplayInfo[i].resource.IncreaseMaximum(1);
        //        gameResourceDisplay.resourceDisplayInfo[i].resource.AddResource(1);
        //    }
        //}


        RPCUpdateResources(PhotonTargets.AllBufferedViaServer);

        //RenewOncePerTurnCounters();

        //EventManager.EffectOnUpkeep();

        gameState = GameStates.Draw;
    }



    public void DrawFromGrimoire() {
        if (firstTurn) {
            firstTurn = false;
            gameState = GameStates.Main;
        }
        else {
            activeGrimoire.GetComponent<Deck>().DrawCard();

            //if (!Mulligan.choosingMulligan)
            //    CreateOrActivateCardGlow();

            gameState = GameStates.Main;
        }
    }

    public IEnumerator DrawStartingHand() {
        for (int i = 0; i < startingHandSize; i++) {
            activeGrimoire.GetComponent<Deck>().DrawCard();
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void AdjustCurEssence(int value) {
        //curEssence += value;
        //UpdateEssenceText();
    }
    public void AdjustMaxEssence(int value) {
        //if (maxEssence < 10) {
        //    maxEssence += value;
        //    UpdateEssenceText();
        //}
    }
    public void FillEssence() {
        //curEssence = maxEssence;
        //UpdateEssenceText();
    }

    void UpdateEssenceText() {
        //myEssenceText.text = curEssence + "/" + maxEssence;
        //RPCUpdateEnemyEssence(PhotonTargets.Others, curEssence, maxEssence);

        //CreateOrActivateCardGlow();
    }





    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }

    public void SetUpDecks() {
        GameObject battleField = PhotonNetwork.Instantiate("Battlefield", transform.position, Quaternion.identity, 0) as GameObject;
        battleField.transform.SetParent(transform, false);
        battleField.GetComponent<Deck>().owner = this;
        battleField.GetComponent<Deck>().RPCAssignDeckInfo(PhotonTargets.AllBufferedViaServer, photonView.viewID);
        battlefield = battleField.GetComponent<Deck>();

        for (int i = 0; i < deckInfo.Count; i++) {
            GameObject activeDeck = PhotonNetwork.Instantiate(deckInfo[i].deck.name, transform.position, Quaternion.identity, 0) as GameObject;
            activeDeck.transform.SetParent(transform, false);
            activeDeck.GetComponent<Deck>().owner = this;
            activeDeck.GetComponent<Deck>().RPCAssignDeckInfo(PhotonTargets.AllBufferedViaServer, photonView.viewID);
        }


    }





    #region RPCs

    public void RPCCheckOpponents(PhotonTargets targets) {
        photonView.RPC("CheckOpponents", targets);
    }

    [PunRPC]
    public void CheckOpponents() {
        Player[] allPlayers = FindObjectsOfType<Player>();

        foreach (Player p in allPlayers) {
            if (p != this) {
                opponent = p;
                p.opponent = this;
            }
        }
        //Debug.Log(opponent.gameObject.name);
    }

    public void RPCResetState(PhotonTargets targets) {
        photonView.RPC("ResetState", targets);
    }

    [PunRPC]
    public void ResetState() {
        if (!myTurn && photonView.isMine) {
            myTurn = true;
            gameState = GameStates.Refresh;
        }
    }

    public void RPCTransferTest(PhotonTargets targets, string message) {
        photonView.RPC("TransferTest", targets, message);
    }

    [PunRPC]
    public void TransferTest(string message) {
        Debug.Log(message);
    }


    public void RPCAmPlayer2(PhotonTargets targets) {
        photonView.RPC("AmPlayer2", targets);
    }

    [PunRPC]
    public void AmPlayer2() {
        player2 = true;
    }


    public void RPCUpdateEnemyEssence(PhotonTargets targets, int cur, int max) {
        photonView.RPC("UpdateEnemyEssence", targets, cur, max);
    }

    [PunRPC]
    public void UpdateEnemyEssence(int cur, int max) {
        //opponent.enemyEssenceText.text = cur.ToString() + "/" + max.ToString();
    }


    //The call for this is in Combat Manager
    [PunRPC]
    public void BroadcastAttacker(int id) {
        CardVisual attacker = Finder.FindCardByID(id);

        EventData data = new EventData();
        data.AddMonoBehaviour("Card", attacker);
        Grid.EventManager.SendEvent(Constants.GameEvent.CharacterAttacked, data);

    }

    public IEnumerator RPCBroadcastTurnStart(PhotonTargets targets, Player player) {
        yield return new WaitForSeconds(0.5f);

        int playerID = player.photonView.viewID;
        photonView.RPC("BroadcastTurnStart", targets, playerID);
    }


    [PunRPC]
    public void BroadcastTurnStart(int playerID) {
        Player p = Finder.FindPlayerByID(playerID);

        EventData data = new EventData();
        data.AddMonoBehaviour("Player", p);

        Grid.EventManager.SendEvent(Constants.GameEvent.TurnStarted, data);
    }

    public void RPCBroadcastTurnEnd(PhotonTargets targets, Player player) {
        int playerID = player.photonView.viewID;
        photonView.RPC("BroadcastTurnEnd", targets, playerID);
    }

    [PunRPC]
    public void BroadcastTurnEnd(int playerID) {
        Player p = Finder.FindPlayerByID(playerID);

        EventData data = new EventData();
        data.AddMonoBehaviour("Player", p);

        Grid.EventManager.SendEvent(Constants.GameEvent.TurnEnded, data);
    }

    public void RPCSetupResources(PhotonTargets targets, bool firstResource, 
        GameResource.ResourceType resourceType = GameResource.ResourceType.None, 
        int current = 0, 
        int max = 0,
        string resourceName = "",
        int resourceCap = 0) {


        int resourceTypeEnum = (int)resourceType;

        GameObject resourceText = PhotonNetwork.Instantiate("ResourceText", transform.position, Quaternion.identity, 0) as GameObject;

        //HUDRegistrar.AddHudElement(resourceText);
        int textID = resourceText.GetPhotonView().viewID;


        photonView.RPC("SetupResources", targets, textID, firstResource, resourceTypeEnum, current, max, resourceName, resourceCap);
    }

    [PunRPC]
    public void SetupResources(int textID, bool firstResource, int resourceTypeEnum, int current, int max, string name, int cap) {

        GameResource.ResourceType type = (GameResource.ResourceType)resourceTypeEnum;


        //Debug.Log(textID + " is the view id Sent");

        GameObject newTextGO = HUDRegistrar.FindHudElementByID(textID);

        if(newTextGO == null) {

            GameObject[] allHUD = GameObject.FindGameObjectsWithTag("HUD");

            foreach(GameObject go in allHUD) {
                if (go.GetPhotonView().viewID == textID) {
                    newTextGO = go;
                    break;
                }
            }
        }

        //Debug.Log(newTextGO);

        Text newText = newTextGO.GetComponent<Text>();

        if (firstResource) {

            GameResource essence = new GameResource(GameResource.ResourceType.Essence, 0, 0, "Essence", gameResourceDisplay, 10);

            gameResources.Add(essence);

            gameResourceDisplay.Initialize(this, essence, newText);
        }
        else {

            GameResource newResource = new GameResource(type, current, max, name, gameResourceDisplay, cap);

            gameResources.Add(newResource);

            gameResourceDisplay.AddNewResource(newResource, newText, false);

        }

    }


    public void RPCUpdateResources(PhotonTargets targets) {

        photonView.RPC("UpdateResources", targets);
    }

    [PunRPC]
    public void UpdateResources() {
        for (int i = 0; i < gameResourceDisplay.resourceDisplayInfo.Count; i++) {
            if (gameResourceDisplay.resourceDisplayInfo[i].addPerTurn) {
                gameResourceDisplay.resourceDisplayInfo[i].resource.IncreaseMaximum(1);
                //gameResourceDisplay.resourceDisplayInfo[i].resource.AddResource(1);
                gameResourceDisplay.resourceDisplayInfo[i].resource.RefreshResource();
            }
        }
    }


    public void RPCConfirmPos(PhotonTargets targets) {
        photonView.RPC("ConfirmPos", targets);
    }

    [PunRPC]
    public void ConfirmPos() {
        transform.position = Vector3.zero;
    }


    //[PunRPC]
    //public void ActivateEndGameButton() {
    //    returnToMenuButton.SetActive(true);
    //    if (opponent != null)
    //        opponent.returnToMenuButton.SetActive(true);
    //}

    #endregion


    [System.Serializable]
    public class DeckInfo {
        public Constants.DeckType deckType;
        public GameObject deck;
    }





    //[PunRPC]
    //public void RemoteCombatEventLog(int firstCardID, int secondCardID) {
    //    CardModel first = null;
    //    CardModel second = null;

    //    foreach (GameObject card in Deck._allCards.activeCards) {
    //        if (card.GetPhotonView().viewID == firstCardID) {
    //            first = card.GetComponent<CardModel>();
    //        }
    //        if (card.GetPhotonView().viewID == secondCardID) {
    //            second = card.GetComponent<CardModel>();
    //        }
    //        if (first != null && second != null) {
    //            break;
    //        }
    //    }

    //    PlayEvent combatEvent = new PlayEvent(PlayEvent.EventType.SoulAttack, first, second);
    //    //PlayEventManager.playEventManager.playEvents.Add(combatEvent);
    //    PlayEventManager.LogPlayEvent(combatEvent);
    //}

}