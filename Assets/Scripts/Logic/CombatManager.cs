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
    public TargetingMode targetingMode;
    public LayerMask whatIsCard;
    public bool isChoosingTarget;
    public bool isInCombat;
    public bool selectingDefender;
    [Space(10)]
    public CreatureCardVisual attacker;
    public CreatureCardVisual defender;

    //public Action<CardVisual> targetCallback;
    public Func<CardVisual, bool> confirmedTargetCallback;
    //public CardVisual sourceOfTargetingEffect;

    //private CreatureCardVisual tempAttacker;
    //private CreatureCardVisual tempDefender;

    private Ray clickRay;
    private RaycastHit clickRayHit;
    private Player owner;

    void Start () {
        combatManager = this;
        owner = GetComponentInParent<Player>();
        //Debug.Log(combatManager.gameObject.name + " is where combat manager is assigned");
        //Grid.EventManager.RegisterListener(Constants.GameEvent.SpecialAbilityTriggered, OnSpecialAbilityTriggered);
    }

	void Update () {
        if (Camera.main != null)
            clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        else {
            Debug.LogError("Camera is null");
        }

        if(Input.GetMouseButtonDown(1) && isInCombat && owner.myTurn) {
            EndCombat();
        }
        if (Input.GetKeyDown(KeyCode.T)) {
            ActivateSpellTargeting();
        }


        if (Input.GetMouseButtonDown(0) && targetingMode == TargetingMode.SpellAbilityTargetng && isChoosingTarget && !isInCombat && owner.myTurn) {
            //Debug.Log("Trying to do stuff on a target");
            DoStuffOnTarget();
        }

        if(Input.GetMouseButtonDown(0) && targetingMode == TargetingMode.CombatTargeting && !isChoosingTarget && !isInCombat && owner.myTurn) {
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

        if (currentTarget.hasAttacked) {
            Debug.LogError("That has already attacked");
            return;
        }

        if (currentTarget.keywords.Contains(Keywords.Pacifist) || currentTarget.keywords.Contains(Keywords.Defender)) {
            Debug.LogError("That has pacifist or defender");
            return;
        }

        if (Finder.CardHasKeyword(currentTarget, Keywords.Exhausted)) {
            Debug.LogError("That is exhausted");
            return;
        }

        if (currentTarget.attack < 1) {
            Debug.LogError("That has no attack value");
            return;
        }
            

        attacker = currentTarget;
        isInCombat = true;
        selectingDefender = true;
        //Debug.Log(attacker.cardData.cardName + " is Attacking");


        RPCBroadcastAttacker(PhotonTargets.All, attacker);

        //EventData data = new EventData();
        //data.AddMonoBehaviour("Card", attacker);
        //Grid.EventManager.SendEvent(Constants.GameEvent.CharacterAttacked, data);

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

        if(Finder.CardHasKeyword(currentTarget, Keywords.Invisible)){
            Debug.Log("That Soul is invisible");
            return;
        }

        List<CardVisual> potentialInterceptors = Finder.FindAllCardsInZone(DeckType.Battlefield, Keywords.Interceptor, OwnerConstraints.Theirs);

        List<CardVisual> resultingInterceptors = SortInterceptors(potentialInterceptors);

        if(resultingInterceptors.Count > 0) {
            if (!resultingInterceptors.Contains(currentTarget)) {
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

    private List<CardVisual> SortInterceptors(List<CardVisual> potentalBlockers) {
        List<CardVisual> results = potentalBlockers;

        for (int i = 0; i < results.Count; i++) {
            CreatureCardVisual interceptor = potentalBlockers[i] as CreatureCardVisual;

            if (Finder.CardHasKeyword(attacker, Keywords.Invisible)) {
                results.Remove(interceptor);
            }

            if (interceptor.size < attacker.size) {
                results.Remove(interceptor);
            }

            if (Finder.CardHasKeyword(attacker, Keywords.Flight) && !Finder.CardHasKeyword(interceptor, Keywords.Flight))
                results.Remove(interceptor);
        }

        return results;
    }

    private void DoCombat() {

        //TODO: Combat Events happen first for stuff like OnAttack: Effect

        if (CheckForCombatEventDeaths()) {
            EndCombat();
            return;
        }
        
        if(!Finder.CardHasKeyword(attacker, Keywords.Tireless)) {
            attacker.RPCToggleExhaust(PhotonTargets.All, true);
        }

        attacker.hasAttacked = true;

        if (attacker.battleToken.battleTokenGlow.activeInHierarchy) {
            attacker.battleToken.battleTokenGlow.SetActive(false);
        }

        CombatHelper(attacker, defender);
        CombatHelper(defender, attacker);

        EndCombat();

    }

    private void CombatHelper(CreatureCardVisual damageDealer, CreatureCardVisual damageTaker) {

        SpecialAbility.StatAdjustment adj = new SpecialAbility.StatAdjustment(Constants.CardStats.Health, -damageDealer.attack, false, false, damageDealer);


        damageTaker.RPCApplyCombatDamage(PhotonTargets.All, adj, attacker);

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

        //TODO: End of Combat Events

        if(attacker != null && defender != null) {
            attacker.RPCCheckDeath(PhotonTargets.All, defender);
            defender.RPCCheckDeath(PhotonTargets.All, attacker);

            defender.RPCTargetCard(PhotonTargets.All, false);
        }


        Debug.Log("Ending Combat");

        if (attacker != null)
            attacker = null;

        if (defender != null)
            defender = null;

        isInCombat = false;
        selectingDefender = false;

    }

    private void DoStuffOnTarget() {
        CardVisual currentTarget = CardClicked();

        if (!ConfirmCardClicked(currentTarget, DeckType.Battlefield))
            return ;


        //TargetingHandler.CreateTargetInfoListing(sourceOfTargetingEffect, currentTarget);

        if(confirmedTargetCallback != null) {

            if (confirmedTargetCallback(currentTarget)) {

                Debug.Log(currentTarget.cardData.cardName + " has been clicked");
                if (isChoosingTarget)
                    isChoosingTarget = false;

                //sourceOfTargetingEffect = null;
                StartCoroutine(Reset());


            }

        }
        else {
            Debug.LogError("[Combat Manager - DoStuffOnTarget] Callback was null");
        }

    }

    //private void OnSpecialAbilityTriggered(EventData data) {
    //    CardVisual source = data.GetMonoBehaviour("Source") as CardVisual;
    //    int abilityID = data.GetInt("ID");
    //    SpecialAbility ability = Finder.FindSpecialAbilityByID(source, abilityID);

    //    ability.targets.Add(DoStuffOnTarget());


    //}



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

    private bool ConfirmCardClicked(CardVisual card, DeckType location, bool mineOnly = false) {

        if (card == null) {
            //Debug.LogError("Card Clicked was Null");
            return false;
        }

        if (card.currentDeck.decktype != location) {
            //Debug.LogError("Card Clicked was not in " + location.ToString());
            return false;
        }

        if (mineOnly && !card.photonView.isMine) {
            Debug.LogError("Card Clicked was not mine");
            return false;
        }


        return true;

    }


    #region RPCs


    public void RPCBroadcastAttacker(PhotonTargets targets, CardVisual attacker) {
        int attackerID = attacker.photonView.viewID;

       owner.photonView.RPC("BroadcastAttacker", targets, attackerID);
    }




    #endregion

}
