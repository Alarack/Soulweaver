using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SoulWeaver;

[System.Serializable]
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
    [Header("Particle Info")]
    public string attackEffect;

    [Header("Colors Info")]
    public Color32 activeColor;
    public Color32 targetedColor;
    public Color32 castColor;
    [Header("Anim Stuff")]
    public CardAnimatonManager animationManager;
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
    public List<SpecialAbility.StatAdjustment> statAdjustments = new List<SpecialAbility.StatAdjustment>();
    [Header("Types and Attunements")]
    public int essenceCost;
    public bool isToken;
    public Constants.CardType primaryCardType;
    public List<Constants.CardType> otherCardTypes = new List<Constants.CardType>();
    public List<Constants.Attunements> attunements = new List<Constants.Attunements>();
    public List<Constants.SubTypes> subTypes = new List<Constants.SubTypes>();
    [Header("Keywords")]
    public List<Constants.Keywords> keywords = new List<Constants.Keywords>();
    [Header("Special Abilities")]

    public List<SpecialAbility> specialAbilities = new List<SpecialAbility>();


    public List<EffectOnTarget> userTargtedAbilities = new List<EffectOnTarget>();
    public List<LogicTargetedAbility> multiTargetAbiliies = new List<LogicTargetedAbility>();


    protected float lerpSmoothing = 10f;
    protected CardVisualState _visualState;
    protected Vector3 position;

    protected CombatManager combatManager;

    protected virtual void Start() {
        combatManager = FindObjectOfType<CombatManager>();
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
        cardDescriptionText.text = cardData.cardText;

        cardImage.sprite = cardData.cardImage;
        cardImage.transform.localPosition = cardData.cardImagePos;
        attackEffect = cardData.attackEffect;

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

        List<EffectOnTarget> tempSpecials = new List<EffectOnTarget>(cardData.userTargtedAbilities);

        foreach (EffectOnTarget effect in tempSpecials) {
            EffectOnTarget newEffect = ObjectCopier.Clone(effect) as EffectOnTarget;
            userTargtedAbilities.Add(newEffect);
        }

        List<LogicTargetedAbility> tempMulti = new List<LogicTargetedAbility>(cardData.multiTargetAbilities);

        foreach (LogicTargetedAbility effect in tempMulti) {
            LogicTargetedAbility newEffect = ObjectCopier.Clone(effect) as LogicTargetedAbility;
            multiTargetAbiliies.Add(newEffect);
        }

        for (int i = 0; i < userTargtedAbilities.Count; i++) {
            //userTargtedAbilities[i].source = this;
            userTargtedAbilities[i].Initialize(this);
        }

        for (int i = 0; i < multiTargetAbiliies.Count; i++) {
            //userTargtedAbilities[i].source = this;
            multiTargetAbiliies[i].Initialize(this);
        }

        //Debug.Log(specialAbilities.Count + " is the total number of specials on " + cardData.cardName);
    }


    public virtual void RestCardData() {
        statAdjustments.Clear();

    }

    //public void InitializeSpecialAbilities(int id) {
    //    CardVisual card = Finder.FindCardByID(id);

    //    for (int i = 0; i < userTargtedAbilities.Count; i++) {

    //        Debug.Log(card.name + " is being assigned to " + gameObject.name + "'s special abilities");

    //        userTargtedAbilities[i].Initialize(card);
    //    }
    //}

    //public void SourceAssignment() {
    //    for (int i = 0; i < userTargtedAbilities.Count; i++) {

    //        userTargtedAbilities[i].abilityName = gameObject.name + " Effect On Target";

    //        Debug.Log(gameObject.name + " is being assigned to " + userTargtedAbilities[i].abilityName + "'s special abilities");

    //        userTargtedAbilities[i].Initialize(this);
    //    }
    //}

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

    public virtual void AlterCardStats(Constants.CardStats stat, int value, CardVisual source, bool sendEvent = true) {

        if (animationManager != null) {
            animationManager.BounceText(stat);
        }



        if (sendEvent) {
            EventData data = new EventData();

            data.AddInt("Stat", (int)stat);
            data.AddInt("Value", value);
            data.AddGameObject("Target", gameObject);
            data.AddMonoBehaviour("Source", source);

            Grid.EventManager.SendEvent(Constants.GameEvent.CreatureStatAdjusted, data);
        }



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

        if (currentDeck.decktype == Constants.DeckType.Hand && primaryCardType != Constants.CardType.Player && Input.GetMouseButtonUp(0)
            && photonView.isMine && Vector3.Distance(transform.position, handPos.position) > 10f && owner.myTurn) {

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


        if (Input.GetKeyDown(KeyCode.Delete)) {
            currentDeck.RPCTransferCard(PhotonTargets.All, this, owner.activeCrypt.GetComponent<Deck>());
        }


        if (currentDeck.decktype == Constants.DeckType.Battlefield && (combatManager.isChoosingTarget || combatManager.isInCombat)) {
            //Debug.Log("Targeting");

            RPCTargetCard(PhotonTargets.All, true);

            if (this is CreatureCardVisual && combatManager.isInCombat) {
                CreatureCardVisual soul = this as CreatureCardVisual;

                combatManager.lineDrawer.RPCBeginDrawing(PhotonTargets.Others, combatManager.attacker.battleToken.incomingEffectLocation.position, soul.battleToken.incomingEffectLocation.position);
            }



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


            if (this is CreatureCardVisual) {
                CreatureCardVisual soul = this as CreatureCardVisual;

                combatManager.lineDrawer.RPCStopDrawing(PhotonTargets.Others);
            }


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

                if (this is CreatureCardVisual) {
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



    ////TODO: Make stat adjustments carry multiple stats?
    //public virtual void RPCApplyStatAdjustments(PhotonTargets targets, List<SpecialAbility.StatAdjustment> statAdjs, CardVisual source) {

    //    int sourceID;

    //    if (source != null)
    //        sourceID = source.photonView.viewID;
    //    else {
    //        sourceID = photonView.viewID;
    //    }


    //    int[] statEnums = new int[statAdjs.Count];
    //    int[] values = new int[statAdjs.Count];

    //    for(int i = 0; i < statAdjs.Count; i++) {
    //        statEnums[i] = (int)statAdjs[i].stat;
    //        values[i] = statAdjs[i].value;
    //    }

    //}


    //[PunRPC]
    //public void ApplyMultipleStatAdjustments(int[] statEnums, int[] statValues) {

    //}

    public void RPCSetCardStats(PhotonTargets targets, int cost, int attack, int size, int health) {


        photonView.RPC("SetCardStats", targets, cost, attack, size, health);

    }

    [PunRPC]
    public void SetCardStats(int cost, int attack, int size, int health) {

        essenceCost = cost;
        cardCostText.text = essenceCost.ToString();

        if( this is CreatureCardVisual) {
            CreatureCardVisual soul = this as CreatureCardVisual;

            soul.attack = attack;
            soul.cardAttackText.text = attack.ToString();
            soul.battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Attack, attack);

            soul.size = size;
            soul.cardSizeText.text = size.ToString();
            soul.battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Size, size);

            soul.health = health;
            soul.cardHealthText.text = health.ToString();
            soul.battleToken.UpdateBattleTokenTokenText(Constants.CardStats.Health, health);


        }


    }


    //This doesn't make sense. it isn't only for combat damage.
    public virtual void RPCApplyCombatDamage(PhotonTargets targets, SpecialAbility.StatAdjustment adjustment, CardVisual source) {
        int statEnum = (int)adjustment.stat;
        int value = adjustment.value;

        int sourceID;

        if (source != null)
            sourceID = source.photonView.viewID;
        else {
            sourceID = photonView.viewID;
        }
        photonView.RPC("ApplyCombatDamage", targets, statEnum, value, sourceID);
    }

    [PunRPC]
    public void ApplyCombatDamage(int statEnum, int value, int sourceID) {
        CardVisual source = Finder.FindCardByID(sourceID);
        Constants.CardStats stat = (Constants.CardStats)statEnum;

        AlterCardStats(stat, value, source);
    }


    //This only works for special abilities. change the name
    public virtual void RPCApplyStatAdjustment(PhotonTargets targets, SpecialAbility.StatAdjustment adjustment, CardVisual source) {
        int sourceID;

        if (source != null)
            sourceID = source.photonView.viewID;
        else {
            sourceID = photonView.viewID;
            //Debug.Log("I'm the source of me!");
        }

        photonView.RPC("ApplystatAdjustment", targets, sourceID, adjustment.uniqueID);
    }

    [PunRPC]
    public void ApplystatAdjustment(int sourceID, int adjID) {
        CardVisual source = Finder.FindCardByID(sourceID);

        //TODO: Make this search a single ability list
        for (int i = 0; i < source.specialAbilities.Count; i++) {
            SpecialAbility special = source.specialAbilities[i];

            for (int j = 0; j < special.statAdjustments.Count; j++) {
                SpecialAbility.StatAdjustment adj = special.statAdjustments[j];

                if (adj.uniqueID == adjID) {
                    //Debug.Log("Match found : " +adj.source.gameObject.name + " and " + gameObject.name);

                    if (adj.nonStacking && statAdjustments.Contains(adj)) {
                        //Debug.Log("Non Stacking effect found, aborting");
                        return;
                    }

                    AlterCardStats(adj.stat, adj.value, adj.source);
                    statAdjustments.Add(adj);
                }
            }
        }
    }

    //public virtual void RPCRemoveStatAdjustment(PhotonTargets targets, SpecialAbility.StatAdjustment adjustment, CardVisual source) {
    //    int statEnum = (int)adjustment.stat;
    //    int value = adjustment.value;
    //    int sourceID = source.photonView.viewID;
    //    bool stacks = adjustment.nonStacking;
    //    bool temp = adjustment.temporary;

    //    //if (removeStat) {
    //    //    value = -value;
    //    //}

    //    photonView.RPC("RemoveStatAdjustment", targets, statEnum, value, sourceID, stacks, temp);
    //}

    public virtual void RPCRemoveStatAdjustment(PhotonTargets targets, int adjID, CardVisual source) {
        int sourceID = source.photonView.viewID;

        //Debug.Log(adjID + " is the ID I'm Sending");

        photonView.RPC("RemoveStatAdjustment", targets, adjID, sourceID);
    }

    [PunRPC]
    public void RemoveStatAdjustment(int adjID, int sourceID) {
        CardVisual source = Finder.FindCardByID(sourceID);

        //Debug.Log(adjID + " is the ID I'm searching for");

        for (int i = 0; i < source.specialAbilities.Count; i++) {
            SpecialAbility special = source.specialAbilities[i];

            for (int j = 0; j < special.statAdjustments.Count; j++) {
                SpecialAbility.StatAdjustment adj = special.statAdjustments[j];

                if (adj.uniqueID == adjID) {
                    //Debug.Log("Match found : " + adj.source.gameObject.name + " and " + gameObject.name);

                    if (!adj.temporary) {
                        //Debug.Log("This adjustment is not temporary, aborting");
                        return;
                    }

                    AlterCardStats(adj.stat, -adj.value, adj.source);
                    statAdjustments.Remove(adj);

                }
            }
        }
    }

    public virtual void RPCTargetCard(PhotonTargets targets, bool target) {
        if (target)
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

    public void RPCAddKeyword(PhotonTargets targets, Constants.Keywords keyword, bool add) {
        photonView.RPC("ToggleKeyword", targets, add, (int)keyword);
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



    public virtual void RPCDeployAttackEffect(PhotonTargets targets, int effectId, CardVisual target) {
        int targetID = target.photonView.viewID;

        photonView.RPC("DeployAttackEffect", targets, effectId, targetID);
    }

    [PunRPC]
    public void DeployAttackEffect(int effectID, int targetID) {
        GameObject effect = Finder.FindEffectByID(effectID);
        CreatureCardVisual target = Finder.FindCardByID(targetID) as CreatureCardVisual;

        if (target.battleToken.incomingEffectLocation != null) {
            effect.transform.SetParent(target.battleToken.incomingEffectLocation, false);
            effect.transform.localPosition = Vector3.zero;
        }
    }








    #endregion






    #region PhotonNetwork Methods

    protected void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(transform.position);
        }
        else {
            position = (Vector3)stream.ReceiveNext();
        }
    }

    #endregion
}