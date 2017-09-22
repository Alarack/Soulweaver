using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;
using System;

using OwnerConstraints = Constants.OwnerConstraints;
using Keywords = Constants.Keywords;
using Subtypes = Constants.SubTypes;
using CardType = Constants.CardType;
using Attunements = Constants.Attunements;
using DeckType = Constants.DeckType;

public class CombatManager : Photon.MonoBehaviour {


    public enum TargetingMode {
        CombatTargeting,
        SpellAbilityTargetng
    }


    public static CombatManager combatManager;
    public DrawLine lineDrawer;
    public TargetingMode targetingMode;
    public LayerMask whatIsCard;
    public bool isChoosingTarget;
    public bool isInCombat;
    public bool selectingDefender;
    [Space(10)]
    public CreatureCardVisual attacker;
    public CreatureCardVisual defender;

    public Vector3 selectedPos;

    //public Action<CardVisual> targetCallback;
    public Func<CardVisual, bool> confirmedTargetCallback;
    //public CardVisual sourceOfTargetingEffect;

    //private CreatureCardVisual tempAttacker;
    //private CreatureCardVisual tempDefender;

    private Ray clickRay;
    private RaycastHit clickRayHit;
    private Player owner;

    private List<CardVisual> currentInterceptors = new List<CardVisual>();

    void Start() {
        combatManager = this;
        owner = GetComponentInParent<Player>();
        //Debug.Log(combatManager.gameObject.name + " is where combat manager is assigned");
        Grid.EventManager.RegisterListener(Constants.GameEvent.TurnStarted, OnTurnStart);
    }

    void Update() {
        if (Camera.main != null)
            clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        else {
            Debug.LogError("Camera is null");
        }

        if (Input.GetMouseButtonDown(1) && isInCombat && owner.myTurn) {
            EndCombat();
        }

        if (isInCombat && attacker != null) {

            lineDrawer.BeginDrawing(attacker.battleToken.incomingEffectLocation.position, Input.mousePosition);
            //lineDrawer.RPCBeginDrawing(PhotonTargets.Others, attacker.battleToken.incomingEffectLocation.position, Input.mousePosition);
        }


        if (Input.GetMouseButtonDown(0) && targetingMode == TargetingMode.SpellAbilityTargetng && isChoosingTarget && !isInCombat && owner.myTurn) {
            //Debug.Log("Trying to do stuff on a target");
            DoStuffOnTarget();
        }

        if (Input.GetMouseButtonDown(0) && targetingMode == TargetingMode.CombatTargeting && !isChoosingTarget && !isInCombat && owner.myTurn) {
            SelectAttacker();
        }

        if (Input.GetMouseButtonDown(0) && targetingMode == TargetingMode.CombatTargeting && isInCombat && selectingDefender && owner.myTurn) {
            SelectDefender();
        }

        if (attacker != null && defender != null) {
            DoCombat();
        }

    }


    public void ActivateSpellTargeting() {
        targetingMode = TargetingMode.SpellAbilityTargetng;
    }

    public void ResetTargeting() {
        StartCoroutine(Reset());
    }

    private IEnumerator Reset() {
        yield return new WaitForSeconds(0.2f);
        targetingMode = TargetingMode.CombatTargeting;
    }


    private void SelectAttacker() {
        CreatureCardVisual currentTarget = CardClicked() as CreatureCardVisual;
        //CardCreatureData creatureData = currentTarget.cardData as CardCreatureData;

        //Debug.Log("Selecting Attacker");

        if (!ConfirmCardClicked(currentTarget, DeckType.Battlefield, true))
            return;

        if (!Finder.CardHasPrimaryType(currentTarget, CardType.Soul) && !Finder.CardHasPrimaryType(currentTarget, CardType.Player)) {
            Debug.LogError("That cannot attack");
            return;
        }

        //if (currentTarget.hasAttacked) {
        //    Debug.LogError("That has already attacked");
        //    return;
        //}

        if (!currentTarget.CanAttack()) {
            Debug.LogError("That cannot attack");
            return;
        }

        //if (Finder.CardHasKeyword(currentTarget, Keywords.Exhausted)) {
        //    Debug.LogError("That is exhausted");
        //    return;
        //}

        //if (currentTarget.attack < 1) {
        //    Debug.LogError("That has no attack value");
        //    return;
        //}


        attacker = currentTarget;
        isInCombat = true;
        selectingDefender = true;

        attacker.battlefieldPos.position += selectedPos;

        currentInterceptors = SortInterceptors();

    }

    private void SelectDefender() {
        CreatureCardVisual currentTarget = CardClicked() as CreatureCardVisual;
        //CardCreatureData creatureData = currentTarget.cardData as CardCreatureData;

        if (!ConfirmCardClicked(currentTarget, DeckType.Battlefield)) {
            Debug.Log("Invalid Target");
            return;
        }

        if (currentTarget.owner == owner) {
            Debug.Log("That Soul is yours");
            return;
        }

        if (Finder.CardHasKeyword(currentTarget, Keywords.Invisible)) {
            Debug.Log("That Soul is invisible");
            return;
        }

        if (attacker.keywords.Contains(Keywords.Rush) && currentTarget.primaryCardType == CardType.Player) {
            Debug.Log("Rush Souls cannot attack Generals the turn they're played");
            return;
        }

        if(currentTarget.keywords.Contains(Keywords.Flight) && !attacker.keywords.Contains(Keywords.Flight) && !attacker.keywords.Contains(Keywords.Reach)){
            Debug.Log("That target is flying");
            return;
        }


        //List<CardVisual> potentialInterceptors = Finder.FindAllCardsInZone(DeckType.Battlefield, Keywords.Interceptor, OwnerConstraints.Theirs);



        if (currentInterceptors.Count > 0) {
            if (!currentInterceptors.Contains(currentTarget)) {
                Debug.Log("You must select an Interceptor");
                return;
            }

            defender = currentTarget;

        }
        else {
            defender = currentTarget;
        }

        Debug.Log(defender.cardData.cardName + " is Defending");

    }

    private List<CardVisual> SortInterceptors() {
        currentInterceptors.Clear();

        List<CardVisual> allInterceptors = Finder.FindAllCardsInZone(DeckType.Battlefield, Keywords.Interceptor, OwnerConstraints.Theirs);

        for (int i = allInterceptors.Count - 1; i >= 0; i--) {
            CreatureCardVisual interceptor = allInterceptors[i] as CreatureCardVisual;

            if (Finder.CardHasKeyword(attacker, Keywords.Invisible)) {
                allInterceptors.Remove(interceptor);
                //Debug.Log("attcker is invisable, removeing all interceptors");
                continue;
            }

            if(Finder.CardHasKeyword(interceptor, Keywords.Invisible)) {
                allInterceptors.Remove(interceptor);
                //Debug.Log("Defender is invisable, removeing");
                continue;
            }

            if (interceptor.size < attacker.size) {
                //Debug.Log(interceptor.cardData.cardName + " is smaller than " + attacker.cardData.cardName + ". Removeing");
                allInterceptors.Remove(interceptor);
                continue;
            }

            if (Finder.CardHasKeyword(attacker, Keywords.Flight) && !Finder.CardHasKeyword(interceptor, Keywords.Flight) && !Finder.CardHasKeyword(interceptor, Keywords.Reach)) {
                //Debug.Log(interceptor.cardData.cardName + " does not have flight or reach and " + attacker.cardData.cardName + " does. Removeing");
                allInterceptors.Remove(interceptor);
                continue;
            }
        }

        //for (int i = 0; i < allInterceptors.Count; i++) {
        //    Debug.Log(allInterceptors[i].cardData.name + " is a valid interceptor");
        //}

        return allInterceptors;
    }

    private void DoCombat() {

        RPCBroadcastAttacker(PhotonTargets.All, attacker);
        RPCBroadcastDefender(PhotonTargets.All, defender);

        RPCBroadcastCombat(PhotonTargets.All, attacker, defender);

        if (CheckForCombatEventDeaths()) {
            EndCombat();
            return;
        }

        if (!Finder.CardHasKeyword(attacker, Keywords.Tireless)) {
            attacker.RPCToggleExhaust(PhotonTargets.All, true);
        }

        attacker.hasAttacked = true;

        if (attacker.battleToken.battleTokenGlow.activeInHierarchy) {
            attacker.battleToken.battleTokenGlow.SetActive(false);
        }


        if (attacker.keywords.Contains(Keywords.FirstStrike) && !defender.keywords.Contains(Keywords.FirstStrike)) {
            CombatHelper(attacker, defender);

            if (defender.health <= 0) {
                EndCombat();
                return;
            }
            else {
                CombatHelper(defender, attacker);
                EndCombat();
                return;
            }
        }

        if (!attacker.keywords.Contains(Keywords.FirstStrike) && defender.keywords.Contains(Keywords.FirstStrike)) {
            CombatHelper(defender, attacker);

            if (attacker.health <= 0) {
                EndCombat();
                return;
            }
            else {
                CombatHelper(attacker, defender);
                EndCombat();
                return;
            }
        }

        if (attacker.keywords.Contains(Keywords.Ranged)) {
            CombatHelper(attacker, defender);
            EndCombat();
            return;
        }

        if (defender.keywords.Contains(Keywords.Ranged)) {
            CombatHelper(attacker, defender);
            EndCombat();
            return;
        }


        CombatHelper(attacker, defender);
        CombatHelper(defender, attacker);

        EndCombat();

    }

    private void CombatHelper(CreatureCardVisual damageDealer, CreatureCardVisual damageTaker) {

        int tempAttackvalue = 0;

        if (damageDealer.attack < 0) {
            tempAttackvalue = 0;
        }
        else {
            tempAttackvalue = -damageDealer.attack;
        }

        SpecialAbility.StatAdjustment adj = new SpecialAbility.StatAdjustment(Constants.CardStats.Health, tempAttackvalue, false, false, damageDealer);

        bool hasVFX = String.IsNullOrEmpty(damageDealer.attackEffect);

        if (damageDealer == attacker) {
            if (damageDealer.keywords.Contains(Keywords.Cleave)) {
                CardVisual rightOfTarget = damageTaker.owner.battleFieldManager.GetCardToTheRight(damageTaker);
                CardVisual leftOfTarget = damageTaker.owner.battleFieldManager.GetCardToTheLeft(damageTaker);

                if (rightOfTarget != null) {
                    rightOfTarget.RPCApplyUntrackedStatAdjustment(PhotonTargets.All, adj, damageDealer, false);
                }

                if (leftOfTarget != null) {
                    leftOfTarget.RPCApplyUntrackedStatAdjustment(PhotonTargets.All, adj, damageDealer, false);
                }
            }
        }


        damageTaker.RPCApplyUntrackedStatAdjustment(PhotonTargets.All, adj, damageDealer, !hasVFX);

        if (!hasVFX) {
            LaunchVFX(damageDealer, damageTaker);
        }


    }

    private bool CheckForCombatEventDeaths() {
        bool result = false;

        bool attackerDied = false;
        bool defenderDied = false;

        if (attacker.health <= 0) {
            attackerDied = true;
        }
        if (defender.health <= 0) {
            defenderDied = true;
        }

        if (attackerDied || defenderDied) {
            Debug.Log("The attack or defender died prematurely");
            result = true;
        }

        return result;
    }


    public void EndCombat() {
        attacker.battlefieldPos.position -= selectedPos;
        //TODO: End of Combat Events

        if (attacker != null && defender != null) {
            defender.RPCTargetCard(PhotonTargets.All, false);

            attacker = null;
            defender = null;
        }

        isInCombat = false;
        selectingDefender = false;

        if (lineDrawer.lineRenderer.enabled) {
            lineDrawer.RPCStopDrawing(PhotonTargets.All);
        }
    }

    private void DoStuffOnTarget() {
        CardVisual currentTarget = CardClicked();

        if (!ConfirmCardClicked(currentTarget, DeckType.Battlefield))
            return;

        //TargetingHandler.CreateTargetInfoListing(sourceOfTargetingEffect, currentTarget);

        if (confirmedTargetCallback != null) {

            if (confirmedTargetCallback(currentTarget)) {

                //Debug.Log(currentTarget.cardData.cardName + " has been clicked");
                if (isChoosingTarget)
                    isChoosingTarget = false;


                StartCoroutine(Reset());
            }
        }
        else {
            Debug.LogError("[Combat Manager - DoStuffOnTarget] Callback was null");
        }

    }

    public CardVisual CardClicked() {
        CardVisual cardSelected = null;

        //Debug.Log("Raycasting");

        if (Physics.Raycast(clickRay, out clickRayHit, Mathf.Infinity, whatIsCard)) {

            //Debug.Log(clickRayHit.collider.gameObject.name + " was clicked");

            cardSelected = clickRayHit.collider.GetComponent<CardVisual>();
        }


        EventData data = new EventData();
        data.AddMonoBehaviour("Card", cardSelected);
        Grid.EventManager.SendEvent(Constants.GameEvent.CardClicked, data);

        return cardSelected;
    }

    private bool ConfirmCardClicked(CardVisual card, DeckType location = DeckType.None, bool mineOnly = false) {

        if (card == null) {
            //Debug.LogError("Card Clicked was Null");
            return false;
        }

        if (location != DeckType.None && card.currentDeck.decktype != location) {
            //Debug.LogError("Card Clicked was not in " + location.ToString());
            return false;
        }

        if (mineOnly && !card.photonView.isMine) {
            //Debug.LogError("Card Clicked was not mine");
            return false;
        }


        return true;

    }



    #region Events

    public void OnTurnStart(EventData data) {
        Player p = data.GetMonoBehaviour("Player") as Player;
        if (p == owner) {
            HandleFusion();
        }

    }



    #endregion



    #region Special Keywords


    public void HandleFusion() {
        List<CardVisual> targets = Finder.FindAllCardsInZone(DeckType.Battlefield, Keywords.Fusion, OwnerConstraints.Mine);

        if (targets.Count < 2) {
            return;
        }

        List<SpecialAbility.StatAdjustment> totalStats = new List<SpecialAbility.StatAdjustment>();

        for (int i = 0; i < targets.Count; i++) {
            totalStats.AddRange(SpecialAbility.StatAdjustment.CopyStats(targets[i] as CreatureCardVisual));

            if (targets[i].keywords.Contains(Keywords.Interceptor)) {
                ((CreatureCardVisual)targets[i]).battlefieldPos.position -= ((CreatureCardVisual)targets[i]).interceptPos;
            }

            targets[i].currentDeck.RPCTransferCard(PhotonTargets.All, targets[i], owner.theVoid);
        }

        totalStats.AddRange(SpecialAbility.StatAdjustment.CreateStatSet(1, 1, 1));

        int totalAtk = 0;
        int totalSiz = 0;
        int totalHth = 0;

        for (int i = 0; i < totalStats.Count; i++) {

            switch (totalStats[i].stat) {
                case Constants.CardStats.Attack:
                    totalAtk += totalStats[i].value;
                    break;

                case Constants.CardStats.Size:
                    totalSiz += totalStats[i].value;
                    break;

                case Constants.CardStats.Health:

                    totalHth += totalStats[i].value;

                    break;
            }
        }

        string cardDataName = "FusedSoul";

        CardData tokenData = Resources.Load<CardData>("CardData/" + cardDataName) as CardData;
        CardVisual newFusion = owner.activeGrimoire.GetComponent<Deck>().CardFactory(tokenData, GlobalSettings._globalSettings.creatureCard.name, owner.battlefield);

        newFusion.RPCSetCardStats(PhotonTargets.All, totalStats.Count, totalAtk, totalSiz, totalHth);
    }






    #endregion



    #region VFX

    public void LaunchVFX(CreatureCardVisual damageDealer, CreatureCardVisual damageTaker) {

        GameObject atkVFX;
        if (damageDealer.cardData.movingVFX) {
            atkVFX = PhotonNetwork.Instantiate(damageDealer.attackEffect, damageDealer.transform.position, Quaternion.identity, 0) as GameObject;
        }
        else {
            atkVFX = PhotonNetwork.Instantiate(damageDealer.attackEffect, damageTaker.transform.position, Quaternion.identity, 0) as GameObject;
        }

        CardVFX vfx = atkVFX.GetComponent<CardVFX>();

        if (vfx.photonView.isMine) {
            vfx.Initialize(damageTaker, damageDealer.cardData.movingVFX);

            if (damageDealer.cardData.movingVFX) {
                atkVFX.transform.SetParent(damageDealer.transform, false);
                atkVFX.transform.localPosition = Vector3.zero;
                //vfx.target = damageTaker.battleToken.incomingEffectLocation;
                //vfx.beginMovement = true;
            }
            else {
                atkVFX.transform.SetParent(damageTaker.battleToken.incomingEffectLocation, false);
                atkVFX.transform.localPosition = Vector3.zero;
            }
        }

        vfx.RPCSetVFXAciveState(PhotonTargets.Others, true);
    }



    #endregion







    #region RPCs


    public void RPCBroadcastAttacker(PhotonTargets targets, CardVisual attacker) {
        int attackerID = attacker.photonView.viewID;

        owner.photonView.RPC("BroadcastAttacker", targets, attackerID);
    }

    public void RPCBroadcastDefender(PhotonTargets targets, CardVisual defender) {
        int defenderID = defender.photonView.viewID;

        owner.photonView.RPC("BroadcastDefender", targets, defenderID);
    }

    //public void RPCBroadcastInterceptor(PhotonTargets targets, CardVisual interceptor) {

    //}

    public void RPCBroadcastCombat(PhotonTargets targets, CardVisual attacker, CardVisual defender) {
        int attackerID = attacker.photonView.viewID;
        int defenderID = defender.photonView.viewID;

        owner.photonView.RPC("BroadcastCombat", targets, attackerID, defenderID);
    }




    #endregion

}
