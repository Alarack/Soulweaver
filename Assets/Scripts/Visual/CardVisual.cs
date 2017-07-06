using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

public class CardVisual : Photon.MonoBehaviour {

    public enum CardVisualState {
        ShowFront,
        ShowBack,
        ShowBattleToken
    }


    [Header("Data Info")]
    public CardData cardData;
    public CardVisual cardPreview;
    [Header("Basic Text Info")]
    public Text cardNameText;
    public Text cardTypeText;
    public Text cardDescriptionText;
    public Text cardCostText;
    [Header("Image Info")]
    public Image cardImage;
    public GameObject glow;
    public GameObject cardFront;
    public GameObject cardBack;
    [Header("Colors Info")]
    public Color32 activeColor;
    public Color32 targetedColor;
    public Color32 castColor;
    [Header("Location and Ownership Info")]
    public Player owner;
    public Deck currentDeck;
    public Deck previousDeck;
    [Space(10)]
    public Transform handPos;
    public Transform battlefieldPos;
    public bool active;
    [Header("Colliders")]
    public BoxCollider cardCollider;
    [Header("Current Stats")]
    [Header("Types and Attunements")]
    public int essenceCost;
    public Constants.CardType primaryCardType;
    public List<Constants.CardType> otherCardTypes = new List<Constants.CardType>();
    public List<Constants.Attunements> attunements = new List<Constants.Attunements>();
    public List<Constants.SubTypes> subTypes = new List<Constants.SubTypes>();
    [Header("Keywords")]
    public List<Constants.Keywords> keywords = new List<Constants.Keywords>();
    [Header("Special Abilities")]
    public List<EffectOnTarget> specialAbilities = new List<EffectOnTarget>();

    protected float lerpSmoothing = 10f;
    protected CardVisualState _visualState;
    protected Vector3 position;

    protected CombatManager combatManager;

    protected virtual void Start() {
        combatManager = FindObjectOfType<CombatManager>();

        //Debug.Log(specialAbilities[0].source.gameObject.name + " is my ability source");
    }



    protected virtual void Update() {

        if (photonView.isMine) {

            if (!owner.dragger.moveableIsGrabbed && currentDeck.decktype == Constants.DeckType.Hand && handPos != null && Vector3.Distance(transform.position, handPos.position) > 0.2f) {
                KeepCardInhand();
            }

            if (photonView.isMine && currentDeck.decktype == Constants.DeckType.Battlefield && battlefieldPos != null && Vector3.Distance(transform.position, battlefieldPos.position) > 0.1f) {
                KeepCardOnField();
            }
        }

        if (!photonView.isMine) {
            if (active) {
                transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * lerpSmoothing);
            }
        }
    }


    public virtual void RegisterEventListeners() {

    }


    public virtual void SetupCardData() {
        if (cardData == null) {
            Debug.LogError("[Visual Card] Card Data is Null");
            return;
        }

        if (cardData.primaryCardType == Constants.CardType.Player || cardData.primaryCardType == Constants.CardType.Domain) {
            active = true;
        }

        cardNameText.text = cardData.cardName;
        cardTypeText.text = SetupCardTypeText();
        cardCostText.text = cardData.cardCost.ToString();

        cardImage.sprite = cardData.cardImage;

        // Initalizing Current Data
        int tempCost = cardData.cardCost;
        essenceCost = tempCost;

        Constants.CardType tempPrimaryType = cardData.primaryCardType;
        primaryCardType = tempPrimaryType;

        List<Constants.CardType> tempOtherTypes = new List<Constants.CardType>(cardData.otherCardTypes);
        otherCardTypes = tempOtherTypes;

        List<Constants.Attunements> tempAttunements = new List<Constants.Attunements>(cardData.attunements);
        attunements = tempAttunements;

        List<Constants.SubTypes> tempSubtypes = new List<Constants.SubTypes>(cardData.subTypes);
        subTypes = tempSubtypes;

        List<Constants.Keywords> tempKeywords = new List<Constants.Keywords>(cardData.keywords);
        keywords = tempKeywords;

        List<EffectOnTarget> tempSpecials = new List<EffectOnTarget>(cardData.specialAbilities);
        foreach(EffectOnTarget tempEffect in tempSpecials) {
            EffectOnTarget newEffect = new EffectOnTarget();
            newEffect.abilityName = tempEffect.abilityName;
            newEffect.effect = tempEffect.effect;
            newEffect.source = tempEffect.source;
            newEffect.type = tempEffect.type;
            //newEffect.

            specialAbilities.Add(newEffect);
        }

        //if (photonView.isMine) {

        //    for (int i = 0; i < specialAbilities.Count; i++) {
        //        specialAbilities[i].source = this;
        //    }
        //}

    }

    public void InitializeSpecialAbilities(int id) {
        CardVisual card = Finder.FindCardByID(id);

        

        for (int i = 0; i < specialAbilities.Count; i++) {

            Debug.Log(card.name + " is being assigned to " + gameObject.name + "'s special abilities");

            specialAbilities[i].Initialize(card);
        }
    }

    public void SourceAssignment() {
        for (int i = 0; i < specialAbilities.Count; i++) {

            specialAbilities[i].abilityName = gameObject.name + " Effect On Target";

            Debug.Log(gameObject.name + " is being assigned to " + specialAbilities[i].abilityName + "'s special abilities");

            specialAbilities[i].Initialize(this);
        }
    }

    public void AddKeyword(Constants.Keywords keyword) {
        if (!keywords.Contains(keyword)) {
            keywords.Add(keyword);
        }
    }

    public void RemoveKeyword(Constants.Keywords keyword) {
        if (keywords.Contains(keyword)) {
            keywords.Remove(keyword);
        }
    }

    public virtual void AlterCardStats(Constants.CardStats stat, int value) {
        //Debug.Log("base card alter stat");
        switch (stat) {
            case Constants.CardStats.Cost:
                TextTools.AlterTextColor(value, cardData.cardCost, cardCostText);
                essenceCost += value;
                cardCostText.text = essenceCost.ToString();
                break;
        }
    }

    public virtual void ActivateGlow(Color32 color) {
        if (!glow.activeInHierarchy) {
            glow.SetActive(true);
        }

        if (glow.GetComponent<Image>().color != color)
            glow.GetComponent<Image>().color = color;
    }

    public virtual void DeactivateGlow() {
        if (glow.activeInHierarchy)
            glow.SetActive(false);
    }

    //public virtual void ApplyStatAdjustment(SpecialAbility.StatAdjustment adjustment) {
    //    RPCAlterStat(PhotonTargets.All, adjustment.stat, adjustment.value);
    //}


    public virtual void ApplyStatAdjustments(List<SpecialAbility.StatAdjustment> adjustments) {


    }

    #region Private Methods

    protected virtual void OnMouseOver() {

        if (currentDeck.decktype == Constants.DeckType.Hand && Vector3.Distance(transform.position, handPos.position) > 10f) {
            //mainHudText.text = "Play " + cardName + "?";
            //Debug.Log("Play " + cardData.cardName + "?");
            if (Input.GetMouseButton(0)) {
                ActivateGlow(Color.cyan);
            }
        }

        if (currentDeck.decktype == Constants.DeckType.Hand && Vector3.Distance(transform.position, handPos.position) < 10f) {
            //mainHudText.text = "Play " + cardName + "?";
            //Debug.Log("Play " + cardData.cardName + "?");
            if (Input.GetMouseButton(0)) {
                ActivateGlow(Color.green);
            }
        }

        if (primaryCardType != Constants.CardType.Player && Input.GetMouseButtonUp(0) && photonView.isMine && Vector3.Distance(transform.position, handPos.position) > 10f && owner.myTurn) {

            if (owner.battleFieldManager.IsCollectionFull() && cardData.primaryCardType == Constants.CardType.Soul) {
                Debug.Log("No place to put that");
                return;
            }

            if (currentDeck.decktype != Constants.DeckType.Hand)
                return;

            //if (cost > owner.curEssence && FindObjectOfType<Options>().enforceManaCosts) {
            //    Debug.Log("Not Enough Mana");
            //    return;
            //}

            //currentDeck.RPCTransferCard(PhotonTargets.All, this, Deck._battlefield);
            currentDeck.RPCTransferCard(PhotonTargets.All, this, owner.battlefield);

            //owner.AdjustCurEssence(-cost);
        }


        if (currentDeck.decktype == Constants.DeckType.Battlefield && (combatManager.isChoosingTarget || combatManager.isInCombat)) {
            Debug.Log("Targeting");

            RPCTargetCard(PhotonTargets.All, true);
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && currentDeck.decktype == Constants.DeckType.Battlefield && (combatManager.isChoosingTarget || combatManager.isInCombat)) {
            RPCTargetCard(PhotonTargets.All, false);
        }

        //if (GetComponent<ActivatedEffect>() != null && (Input.GetKeyDown(KeyCode.A) || Input.GetMouseButtonUp(1)) && photonView.isMine && parentDeck.decktype == Deck.DeckType.Battlefield && owner.myTurn) {
        //    ActivateCardAbility();
        //}

        //if (Mulligan.choosingMulligan && photonView.isMine && isInHand && Input.GetMouseButtonDown(0)) {
        //    ToggleMulligan();
        //}

    }

    protected virtual void OnMouseExit() {
        if (currentDeck.decktype == Constants.DeckType.Battlefield && (combatManager.isChoosingTarget || combatManager.isInCombat)) {
            RPCTargetCard(PhotonTargets.All, false);
        }

    }



    protected void KeepCardInhand() {
        transform.position = Vector3.MoveTowards(transform.position, handPos.position, 2f);
    }

    protected void KeepCardOnField() {
        if (cardData.primaryCardType != Constants.CardType.Domain  /*&& !cardTypes.Contains(CardType.Player)*/) {
            transform.position = Vector3.MoveTowards(transform.position, battlefieldPos.position, 2f);
        }
    }

    protected string SetupCardTypeText() {
        string result = "";

        if (cardData.subTypes.Count < 1) {
            if (cardData.otherCardTypes.Count > 0)
                return cardData.otherCardTypes[0].ToString();
            else {
                //Debug.LogError("[Visual Card] " + cardData.cardName + " Has no types or subtypes");
                return "";
            }
        }

        if (cardData.subTypes.Count >= 1) {
            string mainType = cardData.primaryCardType.ToString();
            string seperator = " - ";
            string subtypes = "";

            for (int i = 0; i < cardData.subTypes.Count; i++) {
                subtypes += " " + (cardData.subTypes[i].ToString());
            }

            result = mainType + seperator + subtypes;
        }

        return result;
    }

    #endregion



    #region Events
    // Event stuff
    private void OnEnterZone(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (currentDeck.decktype == Constants.DeckType.Battlefield) {

            if(deck.decktype == Constants.DeckType.Battlefield) {
                Debug.Log(card.cardData.cardName + " has entered " + deck.decktype.ToString());
                combatManager.targetCallback += DealOneDamage;
                combatManager.isChoosingTarget = true;
            }
        }
    }

    private void DealOneDamage(CardVisual target) {
        target.RPCAlterStat(PhotonTargets.All, Constants.CardStats.Health, -1);
    }

    private void OnTurnStart(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;


        if (currentDeck.decktype == Constants.DeckType.Battlefield) {
            Debug.Log(player.gameObject.name + " has started their turn");
        }
    }




    #endregion




    #region RPCs

    public void RPCSetOwner(PhotonTargets targets) {
        photonView.RPC("SetOwner", targets);
    }

    [PunRPC]
    public void SetOwner() {
        Player[] allPlayers = FindObjectsOfType<Player>();

        foreach (Player p in allPlayers) {
            if (!p.photonView.isMine) {
                owner = p;
            }
        }
    }

    public void RPCSetCardAciveState(PhotonTargets targets, bool activate) {
        photonView.RPC("SetCardActiveState", targets, activate);
    }

    [PunRPC]
    public void SetCardActiveState(bool activate) {
        active = activate;
    }

    public void RPCSetParentDeck(PhotonTargets targets, string deckName) {
        photonView.RPC("SetParentDeck", targets, deckName);
    }

    [PunRPC]
    public void SetParentDeck(string deck) {
        switch (deck) {
            case "Grimoire":
                currentDeck = owner.activeGrimoire.GetComponent<Deck>();
                currentDeck.activeCards.Add(this);
                break;

            case "Domain":
                currentDeck = owner.activeDomain.GetComponent<Deck>();
                currentDeck.activeCards.Add(this);
                break;

            case "SoulCrypt":
                currentDeck = owner.activeCrypt.GetComponent<Deck>();
                currentDeck.activeCards.Add(this);
                break;

            case "Battlefield":
                //currentDeck = Deck._battlefield;
                currentDeck = owner.battlefield;
                currentDeck.activeCards.Add(this);
                break;
        }
    }

    public void RPCRotateCard(PhotonTargets targets) {
        photonView.RPC("RotateCard", targets);
    }

    [PunRPC]
    public void RotateCard() {
        transform.localRotation = Quaternion.Euler(0, 0, 180f);
    }

    public virtual void RPCChangeCardVisualState(PhotonTargets targets, CardVisualState cardVisualState) {
        int cardStateInt = (int)cardVisualState;
        photonView.RPC("ChangeCardVisualState", targets, cardStateInt);
    }

    [PunRPC]
    public virtual void ChangeCardVisualState(int cardVisualState) {

        CardVisualState state = (CardVisualState)cardVisualState;

        if (!photonView.isMine && !active) {
            RPCSetCardAciveState(PhotonTargets.All, true);
        }

        switch (state) {
            case CardVisualState.ShowBack:
                cardBack.SetActive(true);
                cardFront.SetActive(false);

                if (this is CreatureCardVisual) {
                    CreatureCardVisual visual = this as CreatureCardVisual;
                    visual.battleToken.gameObject.SetActive(false);
                }

                break;

            case CardVisualState.ShowFront:
                cardBack.SetActive(false);
                cardFront.SetActive(true);
                cardCollider.enabled = true;

                if (cardData.primaryCardType != Constants.CardType.Player) {
                    cardCollider.center = Vector3.zero;
                    cardCollider.size = new Vector3(9f, 14f, cardCollider.size.z);
                }

                if (this is CreatureCardVisual) {
                    CreatureCardVisual visual = this as CreatureCardVisual;
                    visual.battleToken.gameObject.SetActive(false);
                }

                //battleToken.gameObject.SetActive(false);
                break;

            case CardVisualState.ShowBattleToken:

                if(this is CreatureCardVisual) {
                    CreatureCardVisual visual = this as CreatureCardVisual;

                    visual.cardBack.SetActive(false);
                    visual.cardFront.SetActive(false);
                    visual.battleToken.gameObject.SetActive(true);

                    if (cardData.primaryCardType != Constants.CardType.Player) {
                        cardCollider.center = new Vector3(0.2f, -.03f, 0f);
                        cardCollider.size = new Vector3(7f, 7f, cardCollider.size.z);
                    }

                }

                break;

            default:

                break;
        }
    }

    public void RPCRegisterCard(PhotonTargets targets, int photonViewID) {
        photonView.RPC("RegisterCard", targets, photonViewID);
    }

    [PunRPC]
    public void RegisterCard(int photonViewID) {
        Deck._allCards.activeCards.Add(this);
    }


    public void RPCSetUpCardData(PhotonTargets targets) {
        photonView.RPC("RemoteSetUpCardData", targets);
    }

    [PunRPC]
    public void RemoteSetUpCardData() {
        SetupCardData();
    }

    public virtual void RPCAlterStat(PhotonTargets targets, SpecialAbility.StatAdjustment adjustment) {
        int statEnum = (int)adjustment.stat;
        int value = adjustment.value;

        photonView.RPC("AlterStat", targets, statEnum, value);
    }

    public virtual void RPCAlterStat(PhotonTargets targets, Constants.CardStats stat, int value) {
        int statEnum = (int)stat;
        photonView.RPC("AlterStat", targets, statEnum, value);
    }

    [PunRPC]
    public void AlterStat(int stat, int value) {
        Constants.CardStats statEnum = (Constants.CardStats)stat;
        AlterCardStats(statEnum, value);

    }
    
    public virtual void RPCTargetCard(PhotonTargets targets, bool target) {
        if(target)
            photonView.RPC("TargetCard", targets);
        else {
            photonView.RPC("UntargetCard", targets);
        }
    }

    [PunRPC]
    public void TargetCard() {
        //Debug.Log("Reached RPC");
        ActivateGlow(Color.red);
    }

    [PunRPC]
    public void UntargetCard() {
        DeactivateGlow();
    }


    public void RPCToggleExhaust(PhotonTargets targets, bool exhaust) {
        photonView.RPC("ToggleKeyword", targets, exhaust, (int)Constants.Keywords.Exhausted);
    }

    public void RPCToggleIntercept(PhotonTargets targets, bool intercept) {
        photonView.RPC("ToggleKeyword", targets, intercept, (int)Constants.Keywords.Interceptor);
    }

    [PunRPC]
    public void ToggleKeyword(bool add, int keywordIndex) {

        Constants.Keywords newKeyword = (Constants.Keywords)keywordIndex;

        switch (add) {
            case true:
                AddKeyword(newKeyword);
                break;

            case false:
                RemoveKeyword(newKeyword);
                break;
        }

        KeywordHelper(newKeyword, add);
    }

    protected virtual void KeywordHelper(Constants.Keywords keyword, bool add) {

        //switch (keyword) {
        //    case Constants.Keywords.Exhausted:
        //        if (add) {
        //            cardImage.color = exhaustedColor;
        //            battleFrame.color = exhaustedColor;
        //        }
        //        else {
        //            cardImage.color = Color.white;
        //            battleFrame.color = Color.white;
        //        }
        //        break;

        //    case Constants.Keywords.Interceptor:
        //        if (add) {
        //            cardImage.color = interceptingColor;
        //            battleFrame.color = interceptingColor;
        //        }
        //        else {
        //            cardImage.color = Color.white;
        //            battleFrame.color = Color.white;
        //        }
        //        break;
        //}


    }
    #endregion


    protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(transform.position);
        }
        else {
            position = (Vector3)stream.ReceiveNext();
        }
    }

}