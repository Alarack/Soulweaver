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
    public string deathEffect;

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
    public bool isMulligand;
    public Constants.CardType primaryCardType;
    public List<Constants.CardType> otherCardTypes = new List<Constants.CardType>();
    public List<Constants.Attunements> attunements = new List<Constants.Attunements>();
    public List<Constants.SubTypes> subTypes = new List<Constants.SubTypes>();
    [Header("Keywords")]
    public List<Constants.Keywords> keywords = new List<Constants.Keywords>();
    [Header("Special Abilities")]

    public List<SpecialAbility> specialAbilities = new List<SpecialAbility>();
    public List<SpecialAttribute> specialAttributes = new List<SpecialAttribute>();

    public List<EffectOnTarget> userTargtedAbilities = new List<EffectOnTarget>();
    public List<LogicTargetedAbility> multiTargetAbiliies = new List<LogicTargetedAbility>();


    public DomainTile domainTile;


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
        deathEffect = cardData.deathVFX;

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

        List<SpecialAttribute> tempAttributes = new List<SpecialAttribute>(cardData.specialAttributes);

        foreach (SpecialAttribute att in tempAttributes) {
            SpecialAttribute newAtt = ObjectCopier.Clone(att) as SpecialAttribute;
            specialAttributes.Add(newAtt);
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

        List<Constants.Keywords> tempKeywords = new List<Constants.Keywords>(cardData.keywords);
        keywords = tempKeywords;


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

        if (this is CreatureCardVisual) {
            CreatureCardVisual soul = this as CreatureCardVisual;
            value = soul.CalcProtection(value);
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
                TextTools.AlterTextColor(value, cardData.cardCost, cardCostText, true);
                //Debug.Log("altering cost " + value);
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


    public int CheckSpecialAttributes(SpecialAttribute.AttributeType attribute) {

        for (int i = 0; i < specialAttributes.Count; i++) {
            if (specialAttributes[i].attributeType == attribute) {
                return specialAttributes[i].attributeValue;
            }
        }

        return 0;
    }

    public List<SpecialAbility.StatAdjustment> GatherAllSpecialAbilityStatAdjustments() {
        List<SpecialAbility.StatAdjustment> results = new List<SpecialAbility.StatAdjustment>();

        for (int i = 0; i < specialAbilities.Count; i++) {
            results.AddRange(specialAbilities[i].GetAllStatAdjustments());
        }

        return results;
    }

    #region Private Methods





    protected virtual void OnMouseOver() {

        if (photonView.isMine || currentDeck.decktype == Constants.DeckType.Battlefield)
            CardTooltip.ShowTooltip(cardData.cardName + "\n" + "Cost: " + essenceCost.ToString() + "\n" + cardData.cardText);


        if (currentDeck.decktype == Constants.DeckType.Hand && Vector3.Distance(transform.position, handPos.position) > 10f && !Mulligan.choosingMulligan) {
            //mainHudText.text = "Play " + cardName + "?";
            if (Input.GetMouseButton(0)) {
                ActivateGlow(Color.cyan);
            }
        }

        if (currentDeck.decktype == Constants.DeckType.Hand && Vector3.Distance(transform.position, handPos.position) < 10f && !Mulligan.choosingMulligan) {
            //mainHudText.text = "Play " + cardName + "?";
            if (Input.GetMouseButton(0)) {
                ActivateGlow(Color.green);
            }
        }

        //Playing Card from hand
        if (currentDeck.decktype == Constants.DeckType.Hand && primaryCardType != Constants.CardType.Player && Input.GetMouseButtonUp(0)
            && photonView.isMine && Vector3.Distance(transform.position, handPos.position) > 10f && owner.myTurn) {

            if (owner.battleFieldManager.IsCollectionFull() && cardData.primaryCardType == Constants.CardType.Soul) {
                Debug.Log("No place to put that");
                return;
            }

            if (currentDeck.decktype != Constants.DeckType.Hand)
                return;

            owner.gameResourceDisplay.resourceDisplayInfo[0].resource.RemoveResource(essenceCost);

            currentDeck.RPCTransferCard(PhotonTargets.All, this, owner.battlefield);

        }

        //Dev delete
        if (Input.GetKeyDown(KeyCode.Delete)) {
            currentDeck.RPCTransferCard(PhotonTargets.All, this, owner.activeCrypt.GetComponent<Deck>());
        }

        //Dev Damage
        if (Input.GetKeyDown(KeyCode.K)) {
            SpecialAbility.StatAdjustment damage = new SpecialAbility.StatAdjustment(Constants.CardStats.Health, -1, false, false, this);

            RPCApplyUntrackedStatAdjustment(PhotonTargets.All, damage, this);

        }



        //Targeting
        if (currentDeck.decktype == Constants.DeckType.Battlefield && (combatManager.isChoosingTarget || combatManager.isInCombat)) {

            RPCTargetCard(PhotonTargets.All, true);

            //Drawing Lines
            if (this is CreatureCardVisual && combatManager.isInCombat) {
                CreatureCardVisual soul = this as CreatureCardVisual;
                combatManager.lineDrawer.RPCBeginDrawing(PhotonTargets.Others, combatManager.attacker.battleToken.incomingEffectLocation.position, soul.battleToken.incomingEffectLocation.position);
            }
        }

        //Intercepting
        if (Input.GetKeyDown(KeyCode.I) && owner.myTurn && !combatManager.isChoosingTarget && !combatManager.isInCombat && primaryCardType == Constants.CardType.Soul && currentDeck.decktype == Constants.DeckType.Battlefield) {

            if (keywords.Contains(Constants.Keywords.Interceptor))
                RPCToggleIntercept(PhotonTargets.All, false);
            else {
                RPCToggleIntercept(PhotonTargets.All, true);
            }
        }

        //Untargeting
        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && currentDeck.decktype == Constants.DeckType.Battlefield && (combatManager.isChoosingTarget || combatManager.isInCombat)) {
            RPCTargetCard(PhotonTargets.All, false);
        }

        //Activating Abilities
        if (owner.myTurn && photonView.isMine && (Input.GetKeyDown(KeyCode.A) || Input.GetMouseButtonUp(1)) && !combatManager.isChoosingTarget && !combatManager.isInCombat) {
            if (CheckForUserActivatedAbilities())
                ActivateAbility();
        }

        if (Mulligan.choosingMulligan && photonView.isMine && currentDeck.decktype == Constants.DeckType.Hand && Input.GetMouseButtonDown(0)) {
            ToggleMulligan();
        }

    }

    protected void ToggleMulligan() {

        isMulligand = !isMulligand;
        if (isMulligand) {
            ActivateGlow(Color.red);
        }
        else {
            ActivateGlow(Color.green);
        }

    }



    protected bool CheckForUserActivatedAbilities() {
        for (int i = 0; i < specialAbilities.Count; i++) {
            if (specialAbilities[i].trigger.Contains(Constants.AbilityActivationTrigger.UserActivated))
                return true;
        }
        return false;
    }

    protected void ActivateAbility() {

        EventData data = new EventData();
        data.AddMonoBehaviour("Card", this);

        Grid.EventManager.SendEvent(Constants.GameEvent.UserActivatedAbilityInitiated, data);

    }

    protected virtual void OnMouseExit() {

        if (CardTooltip.cardTooltip.tooltipContainer.activeInHierarchy)
            CardTooltip.HideTooltip();


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

    public void RPCSetCardStats(PhotonTargets targets, int cost, int attack, int size, int health) {
        photonView.RPC("SetCardStats", targets, cost, attack, size, health);
    }

    [PunRPC]
    public void SetCardStats(int cost, int attack, int size, int health) {

        essenceCost = cost;
        cardCostText.text = essenceCost.ToString();

        if (this is CreatureCardVisual) {
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

    public virtual void RPCApplyUntrackedStatAdjustment(PhotonTargets targets, SpecialAbility.StatAdjustment adjustment, CardVisual source) {
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

    public virtual void RPCApplySpecialAbilityStatAdjustment(PhotonTargets targets, SpecialAbility.StatAdjustment adjustment, CardVisual source) {
        int sourceID;

        //adjustment.uniqueID = IDFactory.GenerateID();

        if (source != null)
            sourceID = source.photonView.viewID;
        else {
            sourceID = photonView.viewID;
            //Debug.Log("I'm the source of me!");
        }

        photonView.RPC("ApplySpecialAbilityStatAdjustment", targets, sourceID, adjustment.uniqueID);
    }


    //[PunRPC]
    //public void ApplystatAdjustment(int sourceID, int adjID) {
    //    CardVisual source = Finder.FindCardByID(sourceID);

    //    //Debug.Log(source.gameObject.name + " is trying to apply a stat adjustment with ID " + adjID);

    //    //TODO: Make this search a single ability list
    //    for (int i = 0; i < source.specialAbilities.Count; i++) {
    //        SpecialAbility special = source.specialAbilities[i];

    //        for (int j = 0; j < special.statAdjustments.Count; j++) {
    //            SpecialAbility.StatAdjustment adj = special.statAdjustments[j];

    //            if (adj.uniqueID == adjID) {
    //                //Debug.Log("Match found : " +adj.source.gameObject.name + " and " + gameObject.name);

    //                if (adj.nonStacking && statAdjustments.Contains(adj)) {
    //                    //Debug.Log("Non Stacking effect found, aborting");
    //                    return;
    //                }

    //                // Debug.Log(adj.stat.ToString() + " is being adjusted by " + adj.value + " on " + gameObject.name);

    //                AlterCardStats(adj.stat, adj.value, adj.source);
    //                statAdjustments.Add(adj);
    //            }
    //        }
    //    }
    //}

    [PunRPC]
    public void ApplySpecialAbilityStatAdjustment(int sourceID, int adjID) {
        CardVisual source = Finder.FindCardByID(sourceID);

        List<SpecialAbility.StatAdjustment> allAdjustments = source.GatherAllSpecialAbilityStatAdjustments();

        for (int i = 0; i < allAdjustments.Count; i++) {
            if (allAdjustments[i].uniqueID == adjID) {
                if (allAdjustments[i].nonStacking && statAdjustments.Contains(allAdjustments[i])) {
                    return;
                }

                AlterCardStats(allAdjustments[i].stat, allAdjustments[i].value, allAdjustments[i].source);
                statAdjustments.Add(allAdjustments[i]);
            }
        }


    }


    public virtual void RPCRemoveSpecialAbilityStatAdjustment(PhotonTargets targets, int adjID, CardVisual source) {
        int sourceID = source.photonView.viewID;

        //Debug.Log(adjID + " is the ID I'm Sending");

        photonView.RPC("RemoveSpecialAbilityStatAdjustment", targets, adjID, sourceID);
    }

    //[PunRPC]
    //public void RemoveStatAdjustment(int adjID, int sourceID) {
    //    CardVisual source = Finder.FindCardByID(sourceID);

    //    //Debug.Log(adjID + " is the ID I'm searching for");

    //    for (int i = 0; i < source.specialAbilities.Count; i++) {
    //        SpecialAbility special = source.specialAbilities[i];

    //        for (int j = 0; j < special.statAdjustments.Count; j++) {
    //            SpecialAbility.StatAdjustment adj = special.statAdjustments[j];

    //            if (adj.uniqueID == adjID) {
    //                //Debug.Log("Match found : " + adj.source.gameObject.name + " and " + gameObject.name);

    //                if (!adj.temporary) {
    //                    //Debug.Log("This adjustment is not temporary, aborting");
    //                    return;
    //                }

    //                AlterCardStats(adj.stat, -adj.value, adj.source);
    //                statAdjustments.Remove(adj);

    //            }
    //        }
    //    }
    //}

    [PunRPC]
    public void RemoveSpecialAbilityStatAdjustment(int adjID, int sourceID) {
        CardVisual source = Finder.FindCardByID(sourceID);

        //Debug.Log(source.gameObject.name + " is removeing stat adjustments");

        List<SpecialAbility.StatAdjustment> allAdjustments = source.GatherAllSpecialAbilityStatAdjustments();

        //Debug.Log(allAdjustments.Count + " is the number of adjustments found on " + gameObject.name);

        for (int i = 0; i < allAdjustments.Count; i++) {
            if (allAdjustments[i].uniqueID == adjID) {
                if (!allAdjustments[i].temporary) {
                    return;
                }

                AlterCardStats(allAdjustments[i].stat, -allAdjustments[i].value, allAdjustments[i].source);
                statAdjustments.Remove(allAdjustments[i]);
            }
        }

    }


    public void RPCUpdateSpecialAbilityStatAdjustment(PhotonTargets targets, SpecialAbility.StatAdjustment adjustment, CardVisual source, int updatedValue) {
        int sourceID;
        int statEnum = (int)adjustment.stat;

        if (source != null)
            sourceID = source.photonView.viewID;
        else {
            sourceID = photonView.viewID;
        }

        photonView.RPC("UpdateSpecialAbilityStatAdjustment", targets, adjustment.uniqueID, sourceID, statEnum, updatedValue);
    }

    //[PunRPC]
    //public void UpdateStatAdjustment(int adjID, int sourceID, int statEnum, int updatedValue) {
    //    CardVisual source = Finder.FindCardByID(sourceID);

    //    for (int i = 0; i < source.specialAbilities.Count; i++) {
    //        SpecialAbility special = source.specialAbilities[i];

    //        for (int j = 0; j < special.statAdjustments.Count; j++) {
    //            SpecialAbility.StatAdjustment adj = special.statAdjustments[j];

    //            if (adj.uniqueID == adjID) {
    //                //Debug.Log("Match found : " + adj.source.gameObject.name + " and " + gameObject.name);
    //                adj.value = updatedValue;

    //            }
    //        }
    //    }
    //}

    [PunRPC]
    public void UpdateSpecialAbilityStatAdjustment(int adjID, int sourceID, int updatedValue) {
        CardVisual source = Finder.FindCardByID(sourceID);

        List<SpecialAbility.StatAdjustment> allAdjustments = source.GatherAllSpecialAbilityStatAdjustments();

        for (int i = 0; i < allAdjustments.Count; i++) {
            if (allAdjustments[i].uniqueID == adjID) {

                allAdjustments[i].value = updatedValue;
                break;
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

                //Debug.Log("adding " + newKeyword.ToString() + " to " + gameObject.name);

                if (newKeyword == Constants.Keywords.Interceptor) {
                    if (keywords.Contains(Constants.Keywords.Exhausted) || keywords.Contains(Constants.Keywords.NoIntercept)) {
                        return;
                    }
                }

                AddKeyword(newKeyword);
                break;

            case false:
                RemoveKeyword(newKeyword);
                break;
        }

        KeywordHelper(newKeyword, add);
    }

    protected virtual void KeywordHelper(Constants.Keywords keyword, bool add) {

        if (cardData is CardDomainData) {

            if (add) {
                if (keyword == Constants.Keywords.Exhausted) {

                    if (domainTile != null) {
                        domainTile.RPCExhaustTile(PhotonTargets.All, true);
                    }
                }
            }
            else {
                if (keyword == Constants.Keywords.Exhausted) {

                    if (domainTile != null) {
                        domainTile.RPCExhaustTile(PhotonTargets.All, false);
                    }
                }
            }
        }
    }



    public virtual void RPCDeployAttackEffect(PhotonTargets targets, int effectId, CardVisual target, bool moveingEffect = false) {
        int targetID = target.photonView.viewID;

        photonView.RPC("DeployAttackEffect", targets, effectId, targetID, moveingEffect);
    }

    [PunRPC]
    public void DeployAttackEffect(int effectID, int targetID, bool moveingEffect) {
        GameObject effect = Finder.FindEffectByID(effectID);
        CreatureCardVisual target = Finder.FindCardByID(targetID) as CreatureCardVisual;

        //Debug.Log(effect);
        //Debug.Log(effect.GetComponent<CardVFX>());

        CardVFX vfx = effect.GetComponent<CardVFX>();

        vfx.active = true;

        //if (vfx.photonView.isMine) {
        //    if (moveingEffect) {
        //        effect.transform.SetParent(transform, false);
        //        effect.transform.localPosition = Vector3.zero;
        //        vfx.target = target.battleToken.incomingEffectLocation;
        //        vfx.beginMovement = true;
        //    }
        //    else {
        //        effect.transform.SetParent(target.battleToken.incomingEffectLocation, false);
        //        effect.transform.localPosition = Vector3.zero;
        //    }
        //}
    }


    public void RPCAddSpecialAttribute(PhotonTargets targets, SpecialAttribute.AttributeType type, int value) {
        int attributeTypeEnum = (int)type;

        photonView.RPC("AddSpecialAtribute", targets, attributeTypeEnum, value);
    }


    [PunRPC]
    public void AddSpecialAtribute(int type, int value) {
        SpecialAttribute.AttributeType newType = (SpecialAttribute.AttributeType)type;

        SpecialAttribute existingAttribute = null;

        for (int i = 0; i < specialAttributes.Count; i++) {
            if (specialAttributes[i].attributeType == newType) {
                existingAttribute = specialAttributes[i];

                if (existingAttribute.attributeValue < value) {
                    existingAttribute.attributeValue = value;
                    break;
                }
            }
        }

        if (existingAttribute == null) {
            SpecialAttribute newAtt = new SpecialAttribute(newType, value);
            specialAttributes.Add(newAtt);
        }

    }


    public void RPCModifySpecialAttribute(PhotonTargets targets, SpecialAttribute.AttributeType type, int value) {
        int attributeTypeEnum = (int)type;

        photonView.RPC("ReduceSpecialAtribute", targets, attributeTypeEnum, value);
    }


    [PunRPC]
    public void ModifySpecialAtribute(int type, int value) {
        SpecialAttribute.AttributeType newType = (SpecialAttribute.AttributeType)type;

        SpecialAttribute existingAttribute = null;

        for (int i = 0; i < specialAttributes.Count; i++) {
            if (specialAttributes[i].attributeType == newType) {
                existingAttribute = specialAttributes[i];
                existingAttribute.attributeValue += value;
                break;
            }
        }
    }

    public void RPCRemoveSpecialAttributeSuspension(PhotonTargets targets, SpecialAttribute.AttributeType type) {
        int attributeTypeEnum = (int)type;
        photonView.RPC("RemoveSpecialAtribute", targets, attributeTypeEnum);
    }


    [PunRPC]
    public void RemoveSpecialAtribute(int type) {
        SpecialAttribute.AttributeType newType = (SpecialAttribute.AttributeType)type;

        SpecialAttribute existingAttribute = null;

        for (int i = 0; i < specialAttributes.Count; i++) {
            if (specialAttributes[i].attributeType == newType) {
                existingAttribute = specialAttributes[i];
                break;
            }
        }

        if(existingAttribute != null) {
            specialAttributes.Remove(existingAttribute);
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