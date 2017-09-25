using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;

public class DurationManager : MonoBehaviour {

    public static DurationManager durationManager;


    //public static List<AbilityTargetHolder> abiliites = new List<AbilityTargetHolder>();

    //public static List<SpecialAbility> abiliites = new List<SpecialAbility>();
    public static List<AbilityHolder> savedAbilities = new List<AbilityHolder>();


    void Awake() {
        durationManager = this;
    }


    void Start() {

        Grid.EventManager.RegisterListener(Constants.GameEvent.TurnStarted, OnTurnStarted);
        Grid.EventManager.RegisterListener(Constants.GameEvent.TurnEnded, OnTurnEnded);

    }


    public static void RegisterAbility(SpecialAbility ability) {


        //abiliites.Add(ability);

        AbilityHolder holder = new AbilityHolder(ability, ability.targets, ability.source.owner);

        savedAbilities.Add(holder);

        Debug.Log(savedAbilities.Count + " is the list count");

    }

    //public static void UnRegisterAbility(CardVisual source) {

    //    SpecialAbility target = null;


    //    for(int i = 0; i < source.specialAbilities.Count; i++) {
    //        for(int j = 0; j < abiliites.Count; j++) {
    //            if(source.specialAbilities[i] == abiliites[j]) {
    //                target = abiliites[j];
    //                break;
    //            }
    //        }
    //    }

    //    if(target != null) {
    //        abiliites.Remove(target);
    //        Debug.Log("Unreging");
    //    }

    //}




    #region EVENTS


    public void OnTurnStarted(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (savedAbilities.Count < 1) {
            //Debug.Log("List empty");
            return;
        }

        for(int i = 0; i < savedAbilities.Count; i++) {
            if (savedAbilities[i].ability.effectDuration != Constants.Duration.StartOfTurn)
                continue;

            if (savedAbilities[i].ability != null) {

                RemoveAbilityEffect(savedAbilities[i], player);

                //if (savedAbilities[i].owner != player)
                //    continue;

                //savedAbilities[i].ability.ExternalRemoval(savedAbilities[i].ability.targets);
                //savedAbilities.Remove(savedAbilities[i]);
            }
            else {
                Debug.LogError("Ability is null");
            }
        }

    }



    //private void OnTurnStarted(EventData data) {
    //    Player player = data.GetMonoBehaviour("Player") as Player;

    //    if (abiliites.Count < 1) {
    //        Debug.Log("List empty");
    //        return;
    //    }

    //    for (int i = abiliites.Count - 1; i >= 0; i--) {
    //        if (abiliites[i].effectDuration != Constants.Duration.StartOfTurn)
    //            continue;

    //        //Debug.Log(abiliites[i].abilityName + " is in the duration manager and a turn has just ended");
    //        //Debug.Log(abiliites[i].source.gameObject.name + " is the source of this ability");

    //        Debug.Log(i + " is the index");

    //        //if(abiliites[i].effectHolder.statAdjustments[0] != null) {
    //        //    Debug.Log(abiliites[i].effectHolder.statAdjustments[0].adjustments[0].uniqueID + " is the id when REMVOED");
    //        //}

    //        if (abiliites[i] != null) {

    //            if (abiliites[i].source.owner != player)
    //                continue;

    //            abiliites[i].ExternalRemoval(abiliites[i].targets);
    //            abiliites.Remove(abiliites[i]);
    //        }
    //    }

    //}

    private void OnTurnEnded(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (savedAbilities.Count < 1) {
            //Debug.Log("List empty");
            return;
        }

        for (int i = 0; i < savedAbilities.Count; i++) {
            if (savedAbilities[i].ability.effectDuration != Constants.Duration.EndOfTurn)
                continue;

            if (savedAbilities[i].ability != null) {

                RemoveAbilityEffect(savedAbilities[i], player);

                //if (savedAbilities[i].owner != player)
                //    continue;

                //savedAbilities[i].ability.ExternalRemoval(savedAbilities[i].ability.targets);
                //savedAbilities.Remove(savedAbilities[i]);
            }
            else {
                Debug.LogError("Ability is null");
            }
        }
    }



    private void RemoveAbilityEffect(AbilityHolder savedAbility, Player owner) {

        if (savedAbility.owner != owner)
            return;


        savedAbility.ability.ExternalRemoval(savedAbility.targets);

        savedAbilities.Remove(savedAbility);

    }



    public static List<SpecialAbility.StatAdjustment> GatherAllSpecialAbilityStatAdjustments() {
        List<SpecialAbility.StatAdjustment> results = new List<SpecialAbility.StatAdjustment>();

        for (int i = 0; i < savedAbilities.Count; i++) {
            results.AddRange(savedAbilities[i].ability.GetAllStatAdjustments());
        }

        return results;
    }

    public static List<SpecialAttribute> GatherAllSpecialAttributes() {
        List<SpecialAttribute> results = new List<SpecialAttribute>();

        for (int i = 0; i < savedAbilities.Count; i++) {
            results.AddRange(savedAbilities[i].ability.GetAllSpecialAttributes());
        }


        return results;
    }





    #endregion






    [System.Serializable]
    public class AbilityHolder {

        public SpecialAbility ability;
        public Player owner;
        public List<CardVisual> targets = new List<CardVisual>();

        public AbilityHolder(SpecialAbility ability, List<CardVisual> targets, Player owner) {

            this.ability = ability;
            this.targets = targets;
            this.owner = owner;

        }


    }


}
