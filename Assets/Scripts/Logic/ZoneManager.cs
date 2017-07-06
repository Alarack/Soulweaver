﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;

public class ZoneManager : MonoBehaviour {

    private Player owner;
    private CombatManager combatManager;
    

    private void Start() {
        owner = GetComponentInParent<Player>();
        combatManager = GetComponent<CombatManager>();
    }


    private void OnEnable() {
        Grid.EventManager.RegisterListener(Constants.GameEvent.CardEnteredZone, OnCardEnteringZone);
    }

    private void OnDisable() {
        Grid.EventManager.RemoveMyListeners(this);
    }



    private void OnCardEnteringZone(EventData data) {

        if (!combatManager.gameObject.activeInHierarchy) {
            Debug.Log("No Combat Manager found");
            return;
        }

        if (!gameObject.activeInHierarchy) {
            Debug.Log("inactive");
        }


        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        switch (deck.decktype) {
            case Constants.DeckType.Battlefield:

                Debug.Log(card.cardData.cardName + " has enterd the Battlefield");

                ActivateTargeting(card);




                break;
        }




    } // End of Enter Zone


    private void ActivateTargeting(CardVisual card) {
        if (!card.photonView.isMine)
            return;

        //List<SpecialAbility> cardAbilities = card.specialAbilities;
        List<EffectOnTarget> effects = new List<EffectOnTarget>();

        for(int i = 0; i < card.specialAbilities.Count; i++) {
            if(card.specialAbilities[i] is EffectOnTarget) {
                effects.Add(card.specialAbilities[i]);
            }
        }

        if(effects.Count < 1) {
            return;
        }

        Debug.Log(card.cardData.cardName + " is mine and has an effect on target");

        combatManager.isChoosingTarget = true;
        combatManager.ActivateSpellTargeting();

        for(int i = 0; i < effects.Count; i++) {
            combatManager.targetCallback += effects[i].Effect;
        }




    }


}
