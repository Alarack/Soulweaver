using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoulWeaver;
using System;

using AbilityActivationTrigger = Constants.AbilityActivationTrigger;
using EffectType = Constants.EffectType;

using OwnerConstraints = Constants.OwnerConstraints;
using Keywords = Constants.Keywords;
using Subtypes = Constants.SubTypes;
using CardType = Constants.CardType;
using Attunements = Constants.Attunements;
using DeckType = Constants.DeckType;
using ConstraintType = Constants.ConstraintType;
using CardStats = Constants.CardStats;
using GameEvent = Constants.GameEvent;
using EffectDuration = Constants.EffectDuration;

[System.Serializable]
public abstract class SpecialAbility {

    [Header("Basic Info")]
    public List<AbilityActivationTrigger> trigger = new List<AbilityActivationTrigger>();
    public EffectDuration duration;
    public ConstraintList triggerConstraints = new ConstraintList();
    public ConstraintList sourceConstraints = new ConstraintList();

    public EffectType effect;
    public string abilityName;
    public int effectValue;
    public CardVisual source;
    public List<CardVisual> targets = new List<CardVisual>();
    public ConstraintList targetConstraints = new ConstraintList();
    public int ID;
    public string abilityVFX;
    //Cards In Zone
    public ConstraintList cardsInZoneConstraints = new ConstraintList();

    //Keyword Fields
    public List<Keywords> keywordsToAddorRemove = new List<Keywords>();


    //Additional Requirements
    public List<Constants.AdditionalRequirement> additionalRequirements = new List<Constants.AdditionalRequirement>();



    public List<StatAdjustment> statAdjustments = new List<StatAdjustment>();

    public enum ApplyEffectToWhom {
        TriggeringCard,
        CauseOfTrigger
    }

    public ApplyEffectToWhom applyEffectToWhom;

    public enum GainedOrLost {
        Gained,
        Lost
    }

    public enum MoreOrLess {
        AtLeast,
        NoMoreThan
    }





    public bool applyEffectToSourceOfAdjustment;





    public virtual bool ProcessEffect(CardVisual card) {
        bool check = CheckConstraints(targetConstraints, card);

        if (!check)
            return false;

        Effect(card);

        return check;

    }

    protected virtual void Unsubscribe() {

    }

    public virtual void Initialize(CardVisual owner) {
        source = owner;

        if (source == null) {
            Debug.LogError("[Special Ability] This Ability has a null source.");
            return;
        }

        RegisterListeners();
        InitializeStatAdjusments();

        source.specialAbilities.Add(this);
        //combatManager = owner.owner.combatManager;
        //ID = IDFactory.GenerateID();

        //Debug.Log(source.gameObject.name + " has a special ability with ID " + ID);
    }

    protected virtual void Effect(CardVisual card) {

        if (!targets.Contains(card))
            targets.Add(card);


        if (abilityVFX != null && abilityVFX != "") {
            CreateVFX();
        }

        //Debug.Log("[Special Ability] Applying effect");

        switch (effect) {
            case EffectType.StatAdjustment:
                ApplyStatAdjustments(card);
                break;


            case EffectType.SpawnToken:

                //for (int i = 0; i < numberOfSpawns; i++) {
                SpawnToken(targetConstraints);
                //}
                break;

            case EffectType.ZoneChange:

                ForcedZoneChange(card, targetConstraints);

                break;

            case EffectType.GrantKeywordAbilities:
                GrantKeywords(card, keywordsToAddorRemove);
                break;

            case EffectType.None:
                Debug.Log("No effect is set to happen for " + source.gameObject.name);
                break;

        }
    }

    protected virtual void RemoveEffect(List<CardVisual> cards) {

        Debug.Log("Removeing Effect");
        for (int i = 0; i < cards.Count; i++) {
            switch (effect) {

                case EffectType.StatAdjustment:
                    RemoveStatAdjustments(cards[i]);
                    break;

                case EffectType.SpawnToken:

                    break;

                case EffectType.ZoneChange:

                    break;

                case EffectType.GrantKeywordAbilities:
                    RemoveKeywords(cards[i], keywordsToAddorRemove);
                    break;

            }
        }

        ClearTargets();
        Unsubscribe();
    }


    protected void ApplyStatAdjustments(CardVisual card) {
        for (int i = 0; i < statAdjustments.Count; i++) {
            card.RPCApplyStatAdjustment(PhotonTargets.All, statAdjustments[i], source);
        }
    }

    protected void RemoveStatAdjustments(CardVisual card) {
        for (int i = 0; i < statAdjustments.Count; i++) {
            card.RPCRemoveStatAdjustment(PhotonTargets.All, statAdjustments[i].uniqueID, source);
        }

    }



    public virtual void RegisterListeners() {


        if (duration == EffectDuration.EndOfTurn && source.photonView.isMine) {
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEndDuration);
            Debug.Log("Registering an end of turn duration event");
        }

        if (duration == EffectDuration.WhileInZone && source.photonView.isMine) {
            Grid.EventManager.RegisterListener(GameEvent.CardLeftZone, OnLeavesZoneEndDuration);
        }

        if (triggerConstraints.resetCountAtTurnEnd && triggerConstraints.requireMultipleTriggers)
            Grid.EventManager.RegisterListener(GameEvent.TurnEnded, OnTurnEnd);




        if (trigger.Contains(AbilityActivationTrigger.EntersZone))
            Grid.EventManager.RegisterListener(GameEvent.CardEnteredZone, OnEnterZone);

        if (trigger.Contains(AbilityActivationTrigger.CreatureStatChanged))
            Grid.EventManager.RegisterListener(GameEvent.CreatureStatAdjusted, OnCreatureStatAdjusted);

        if (trigger.Contains(AbilityActivationTrigger.Attacks))
            Grid.EventManager.RegisterListener(GameEvent.CharacterAttacked, OnAttack);
    }

    #region EVENTS

    protected void OnTurnEndDuration(EventData data) {
        if (targets.Count < 1)
            return;

        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.owner == player) {
            return;
        }

        Debug.Log("End of turn effect is triggering");
        RemoveEffect(targets);
    }

    protected void OnLeavesZoneEndDuration(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (card == source && sourceConstraints.currentZone.Contains(deck.decktype)) {
            RemoveEffect(targets);
        }


    }

    protected void OnTurnStart(EventData data) {



    }

    protected void OnTurnEnd(EventData data) {
        Player player = data.GetMonoBehaviour("Player") as Player;

        if (!source.owner == player) {
            return;
        }

        if (triggerConstraints.resetCountAtTurnEnd)
            triggerConstraints.counter = 0;



    }


    protected void OnEnterZone(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;
        Deck deck = data.GetMonoBehaviour("Deck") as Deck;

        if (!ManageConstraints(card)) {
            return;
        }


        if (this is LogicTargetedAbility) {
            if (source.photonView.isMine) {

                ProcessEffect(source);
            }

        }


        ActivateTargeting();

        ////If a targted effect has no valid targets when it triggers, then get out of here.

        //if (source.photonView.isMine && this is EffectOnTarget) {
        //    if (CheckValidTargets().Count < 1) {
        //        Debug.Log("No Valid Targets for " + source.gameObject.name + "'s targeted ability");
        //        return;
        //    }
        //    else {

        //        Debug.Log("Good to go");
        //        CombatManager.combatManager.isChoosingTarget = true;
        //        CombatManager.combatManager.ActivateSpellTargeting();
        //        CombatManager.combatManager.confirmedTargetCallback += ProcessEffect;
        //    }
        //}
    }


    protected void OnAttack(EventData data) {
        CardVisual card = data.GetMonoBehaviour("Card") as CardVisual;

        Debug.Log(card.gameObject.name + " is attacking");
    }


    protected void OnCreatureStatAdjusted(EventData data) {
        CardStats stat = (CardStats)data.GetInt("Stat");
        int value = data.GetInt("Value");
        CardVisual target = data.GetGameObject("Target").GetComponent<CardVisual>();
        CardVisual sourceOfAdjustment = data.GetMonoBehaviour("Source") as CardVisual;

        if (!CheckCreatureStatAltered(triggerConstraints, stat, value, target))
            return;

        if (!ManageConstraints(target)) {
            return;
        }

        CardVisual effectTarget = null;

        switch (applyEffectToWhom) {
            case ApplyEffectToWhom.TriggeringCard:
                effectTarget = target;
                break;

            case ApplyEffectToWhom.CauseOfTrigger:
                effectTarget = sourceOfAdjustment;
                break;
        }

        if (this is LogicTargetedAbility && source.photonView.isMine) {
            ProcessEffect(effectTarget);
        }

        ActivateTargeting();

    }


    #endregion


    private void ActivateTargeting() {
        if (source.photonView.isMine && this is EffectOnTarget && source.owner.myTurn) {
            if (CheckValidTargets().Count < 1) {
                Debug.Log("No Valid Targets for " + source.gameObject.name + "'s targeted ability");
                return;
            }
            else {
                Debug.Log("Good to go");
                CombatManager.combatManager.isChoosingTarget = true;
                CombatManager.combatManager.ActivateSpellTargeting();
                CombatManager.combatManager.confirmedTargetCallback += ProcessEffect;

            }
        }

    }


    #region CONSTRAINT CHECKING


    protected bool ManageConstraints(CardVisual triggeringCard, CardVisual causeOfTrigger = null) {
        bool result = true;

        if (triggerConstraints.thisCardOnly && triggeringCard != source) {
            //Debug.Log(source.gameObject.name + " can only trigger its own effect and " + target.gameObject.name + " has happened");
            return false;
        }

        if (CheckConstraints(triggerConstraints, triggeringCard) == null) {
            //Debug.Log("Trigger is not in place");
            return false;
        }

        if (CheckConstraints(sourceConstraints, source) == null) {
            //Debug.Log("Source is not in place for" + source.gameObject.name + "'s ability");
            //Debug.Log("Source is not in place");
            return false;
        }


        for (int i = 0; i < additionalRequirements.Count; i++) {
            if (!CheckAdditionalRequirements(additionalRequirements[i], cardsInZoneConstraints))
                return false;
        }

        if (triggerConstraints.requireMultipleTriggers) {
            result = CheckForMultipleTriggers(triggerConstraints);
        }


        return result;
    }

    protected List<CardVisual> CheckValidTargets() {
        List<CardVisual> results = new List<CardVisual>();

        foreach (CardVisual target in Deck._allCards.activeCards) {
            if (CheckConstraints(targetConstraints, target) != null) {
                results.Add(target);
                //Debug.Log(target.gameObject.name + " is a valid target for " + source.gameObject.name + "'s targeted ability");
            }
        }

        return results;
    }

    public virtual CardVisual CheckConstraints(ConstraintList constraint, CardVisual target) {
        //CardVisual result = target;

        for (int i = 0; i < constraint.types.Count; i++) {
            if (ConstraintHelper(constraint, constraint.types[i], target) == null) {
                return null;
            }
        }

        return target;
    }

    protected void ClearTargets() {
        targets.Clear();
    }


    protected CardVisual ConstraintHelper(ConstraintList constraint, ConstraintType type, CardVisual target) {
        //bool result = true;

        if (!constraint.types.Contains(type))
            return target;


        switch (type) {
            case ConstraintType.PrimaryType:
                if (!constraint.primaryType.Contains(target.primaryCardType))
                    return null;

                break;

            case ConstraintType.Owner:
                if (DetermineOnwerConstraint(target, constraint.owner) == null)
                    return null;

                break;

            case ConstraintType.CurrentZone:
                if (!constraint.currentZone.Contains(target.currentDeck.decktype))
                    return null;

                break;

            case ConstraintType.PreviousZone:
                if (!constraint.previousZone.Contains(target.previousDeck.decktype))
                    return null;

                break;

            case ConstraintType.Subtype:
                if (!DoesListContainAny<Subtypes>(constraint.subtype, target.subTypes))
                    return null;

                break;

            case ConstraintType.Attunement:
                if (!DoesListContainAny<Attunements>(constraint.attunement, target.attunements))
                    return null;

                break;

            case ConstraintType.AdditionalType:
                if (!DoesListContainAny<CardType>(constraint.additionalType, target.otherCardTypes))
                    return null;

                break;

            case ConstraintType.Keyword:
                if (!DoesListContainAny<Keywords>(constraint.keyword, target.keywords))
                    return null;

                break;

            case ConstraintType.StatMaximum:
                return CreatureStatConstraint(constraint.maxStats, false, target);

            case ConstraintType.StatMinimum:
                return CreatureStatConstraint(constraint.minStats, true, target);

            case ConstraintType.CreatureStatus:

                for (int i = 0; i < constraint.creatureStatus.Count; i++) {
                    switch (constraint.creatureStatus[i]) {
                        case Constants.CreatureStatus.MostStat:
                            if (HasHighOrLowStat(constraint.mostStat, target, true) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.LeastStat:
                            if (HasHighOrLowStat(constraint.leastStat, target, false) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.Damaged:
                            if (IsDamagedOrUndamaged(target, true) == null)
                                return null;
                            break;

                        case Constants.CreatureStatus.Undamaged:
                            if (IsDamagedOrUndamaged(target, false) == null)
                                return null;
                            break;
                    }
                }

                break;


        }



        return target;
    }

    private bool CheckForMultipleTriggers(ConstraintList constraints) {

        constraints.counter++;

        if (constraints.counter >= constraints.triggersRequired) {
            return true;
        }
        else {
            return false;
        }

    }

    private bool CheckCreatureStatAltered(ConstraintList constraint, CardStats stat, int statChangeValue, CardVisual target = null) {
        bool result = true;

        if (stat != constraint.statChanged)
            return false;


        if(stat == CardStats.Health &&  statChangeValue < 0) {
            Debug.Log(target.gameObject.name + " has taken " + Mathf.Abs(statChangeValue) + " point(s) of damage");
        }

        result = CheckStatGainedOrLost(constraint.gainedOrLost, statChangeValue);


        return result;
    }

    private bool CheckStatGainedOrLost(GainedOrLost gainedorLost, int value) {

        switch (gainedorLost) {
            case GainedOrLost.Gained:
                return value > 0;

            case GainedOrLost.Lost:
                return value < 0;

            default:

                return false;
        }
    }


    private CardVisual DetermineOnwerConstraint(CardVisual target, OwnerConstraints limit) {
        CardVisual resul = null;

        switch (limit) {
            case OwnerConstraints.All:
                return target;

            case OwnerConstraints.Mine:
                if (target.photonView.isMine)
                    return target;

                break;

            case OwnerConstraints.Theirs:
                if (!target.photonView.isMine)
                    return target;

                break;

            default:

                return null;

        }

        return resul;
    }


    private CardVisual HasHighOrLowStat(CardStats statToCompare, CardVisual target, bool highest) {
        CardVisual result = null;

        List<CardVisual> cardsToSearch = Finder.FindCardsWithStatExtreme(statToCompare, highest);
        if (cardsToSearch.Contains(target))
            return target;


        return result;
    }

    private CardVisual IsDamagedOrUndamaged(CardVisual target, bool damaged) {
        CardVisual result = null;

        List<CardVisual> cardsToSearch = Finder.FindAllDamagedOrUndamagedCreatures(damaged);
        if (cardsToSearch.Contains(target))
            return target;


        return result;
    }

    protected CardVisual HasStatExtreme(Constants.CreatureStatus status, CardStats statToCompare, CardVisual target) {

        List<CardVisual> cardsToSearch = new List<CardVisual>();

        switch (status) {
            case Constants.CreatureStatus.MostStat:
                cardsToSearch = Finder.FindCardsWithStatExtreme(statToCompare, true);
                if (cardsToSearch.Contains(target))
                    return target;

                break;

            case Constants.CreatureStatus.LeastStat:
                cardsToSearch = Finder.FindCardsWithStatExtreme(statToCompare, false);
                if (cardsToSearch.Contains(target))
                    return target;

                break;

            case Constants.CreatureStatus.Damaged:
                cardsToSearch = Finder.FindAllDamagedOrUndamagedCreatures(true);
                if (cardsToSearch.Contains(target))
                    return target;

                break;

            case Constants.CreatureStatus.Undamaged:
                cardsToSearch = Finder.FindAllDamagedOrUndamagedCreatures(false);
                if (cardsToSearch.Contains(target))
                    return target;
                break;


        }

        return null;
    }

    protected bool DoesListContainAny<T>(List<T> list1, List<T> list2) where T : struct, IFormattable, IConvertible {
        bool result = false;

        foreach (T entry in list1) {
            if (list2.Contains(entry)) {
                result = true;
                break;
            }
        }

        return result;
    }

    protected CardVisual CreatureStatConstraint(List<StatAdjustment> statLimits, bool statHigher, CardVisual target) {
        CardVisual result = target;

        CreatureCardVisual soul = target as CreatureCardVisual;

        if (statHigher) {
            foreach (StatAdjustment stat in statLimits) {
                switch (stat.stat) {
                    case CardStats.Attack:
                        if (soul.attack < stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Size:
                        if (soul.size < stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Health:
                        if (soul.health < stat.value) {
                            return null;
                        }

                        break;
                }
            }
        }

        if (!statHigher) {
            foreach (StatAdjustment stat in statLimits) {
                switch (stat.stat) {
                    case CardStats.Attack:
                        if (soul.attack > stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Size:
                        if (soul.size > stat.value) {
                            return null;
                        }

                        break;

                    case CardStats.Health:
                        if (soul.health > stat.value) {
                            return null;
                        }

                        break;
                }
            }


        }




        return result;
    }




    protected bool CheckAdditionalRequirements(Constants.AdditionalRequirement typeOfRequirement, ConstraintList constraint) {
        bool result = true;

        switch (typeOfRequirement) {
            case Constants.AdditionalRequirement.NumberofCardsInZone:

                switch (constraint.moreOrLess) {
                    case MoreOrLess.AtLeast:
                        if (CheckNumberOfCardsInZone(constraint.zoneToCheckForNumberOfCards, cardsInZoneConstraints).Count < constraint.numberOfcardsInZone) {
                            //Debug.Log("Not enough " + cardsInZoneConstraints.subtype[0].ToString() + " in " + zoneToCheckForNumberOfCards.ToString());
                            return false;
                        }

                        break;

                    case MoreOrLess.NoMoreThan:
                        if (CheckNumberOfCardsInZone(constraint.zoneToCheckForNumberOfCards, cardsInZoneConstraints).Count > constraint.numberOfcardsInZone) {
                            //Debug.Log("Too many " + cardsInZoneConstraints.subtype[0].ToString() + " in " + zoneToCheckForNumberOfCards.ToString());
                            return false;
                        }

                        break;
                }

                break;
        }

        return result;
    }


    protected List<CardVisual> CheckNumberOfCardsInZone(DeckType zone, ConstraintList constraint) {
        List<CardVisual> results = new List<CardVisual>();

        List<CardVisual> allCardsInZone = Finder.FindAllCardsInZone(zone);

        for (int i = 0; i < allCardsInZone.Count; i++) {
            CardVisual applicant = CheckConstraints(constraint, allCardsInZone[i]);

            if (applicant != null)
                results.Add(applicant);
        }




        return results;
    }








    #endregion




    #region Effect Methods


    public List<CardVisual> SpawnToken(ConstraintList spawnConstraints) {

        List<CardVisual> tokens = new List<CardVisual>();
        int spawnIndex = targetConstraints.numberOfSpawns;

        if (spawnIndex > targets.Count)
            spawnIndex = targets.Count;


        for (int i = 0; i < targetConstraints.numberOfSpawns; i++) {

            CardData tokenData = Resources.Load<CardData>("CardData/" + spawnConstraints.spawnableTokenDataName) as CardData;


            if (targetConstraints.copyTargets) {
                spawnConstraints.spawnCardType = targets[spawnIndex - 1].primaryCardType;
                tokenData = Resources.Load<CardData>("CardData/" + targets[spawnIndex - 1].cardData.name) as CardData;
            }

            string prefabName = Deck._allCards.GetCardPrefabNameByType(spawnConstraints.spawnCardType);

            CardVisual tokenCard = source.owner.activeGrimoire.GetComponent<Deck>().CardFactory(tokenData, prefabName, GetDeckFromType(spawnConstraints.spawnTokenLocation, source));
            tokenCard.isToken = true;

            tokens.Add(tokenCard);

            spawnIndex++;
        }

        return tokens;
    }


    private Deck GetDeckFromType(DeckType type, CardVisual card) {
        switch (type) {
            case DeckType.Battlefield:
                return card.owner.battlefield;

            case DeckType.Hand:
                return card.owner.myHand;

            case DeckType.Grimoire:
                return card.owner.activeGrimoire.GetComponent<Deck>();

            case DeckType.SoulCrypt:
                return card.owner.activeCrypt.GetComponent<Deck>();

            case DeckType.None:
                return null;

            default:
                return null;
        }
    }


    public void ForcedZoneChange(CardVisual target, ConstraintList zoneChangeConstraints) {

        target.currentDeck.RPCTransferCard(PhotonTargets.All, target, GetDeckFromType(zoneChangeConstraints.targetZone, target));
    }




    public void GrantKeywords(CardVisual target, List<Keywords> keywords) {

        for (int i = 0; i < keywords.Count; i++) {
            target.RPCAddKeyword(PhotonTargets.All, keywords[i], true);
        }

        //grantedKeywords.Add(target.photonView.viewID, keywords);

    }

    public void RemoveKeywords(CardVisual target, List<Keywords> keywords) {

        for (int i = 0; i < keywords.Count; i++) {
            target.RPCAddKeyword(PhotonTargets.All, keywords[i], false);
        }

        //removedKeywords.Add(target.photonView.viewID, keywords);

    }


    #endregion



    public void CreateVFX() {

        for (int i = 0; i < targets.Count; i++) {
            GameObject atkVFX = PhotonNetwork.Instantiate(abilityVFX, targets[i].transform.position, Quaternion.identity, 0) as GameObject;

            source.RPCDeployAttackEffect(PhotonTargets.All, atkVFX.GetPhotonView().viewID, targets[i]);
        }
    }




    private void InitializeStatAdjusments() {
        for (int i = 0; i < statAdjustments.Count; i++) {
            statAdjustments[i].uniqueID = IDFactory.GenerateID();
            statAdjustments[i].source = source;
        }
    }

    [System.Serializable]
    public class StatAdjustment {
        public Constants.CardStats stat;
        public int value;
        public bool nonStacking;
        public bool temporary;
        public CardVisual source;
        public int uniqueID = -1;


        public StatAdjustment() {

        }

        public StatAdjustment(CardStats stat, int value, bool nonStacking, bool temp, CardVisual source) {
            this.stat = stat;
            this.value = value;
            this.nonStacking = nonStacking;
            this.source = source;
            temporary = temp;

            uniqueID = IDFactory.GenerateID();

        }


        public static List<StatAdjustment> CopyStats(CreatureCardVisual target) {
            List<StatAdjustment> results = new List<StatAdjustment>();

            StatAdjustment atk = new StatAdjustment(CardStats.Attack, target.attack, false, false, null);
            StatAdjustment siz = new StatAdjustment(CardStats.Size, target.size, false, false, null);
            StatAdjustment hth = new StatAdjustment(CardStats.Health, target.health, false, false, null);

            results.Add(atk);
            results.Add(siz);
            results.Add(hth);


            return results;
        }

        public static List<StatAdjustment> CreateStatSet(int atk, int siz, int hth) {
            List<StatAdjustment> results = new List<StatAdjustment>();

            StatAdjustment atka = new StatAdjustment(CardStats.Attack, atk, false, false, null);
            StatAdjustment siza = new StatAdjustment(CardStats.Size, siz, false, false, null);
            StatAdjustment htha = new StatAdjustment(CardStats.Health, hth, false, false, null);

            results.Add(atka);
            results.Add(siza);
            results.Add(htha);

            return results;
        }




    }

    //[System.Serializable]
    //public class Constraint {

    //    public ConstraintType type;

    //    public OwnerConstraints owner;
    //    public CardType primaryType;
    //    public CardType additionalType;
    //    public Subtypes subtype;
    //    public Keywords keyword;
    //    public Attunements attunement;
    //    public DeckType currentZone;
    //    public DeckType previousZone;

    //    public List<StatAdjustment> minStats = new List<StatAdjustment>();
    //    public List<StatAdjustment> maxStats = new List<StatAdjustment>();
    //    public bool damaged;
    //    public bool undamaged;



    //    private void ClearUnviewedSelection<TEnum>(TEnum chosen) where TEnum : struct, System.IConvertible, System.IComparable, System.IFormattable {
    //        if (!typeof(TEnum).IsEnum) {
    //            throw new System.ArgumentException("TEnum must be an enum.");
    //        }

    //        if (!(chosen is OwnerConstraints)) {

    //        }

    //    }

    //}


    [System.Serializable]
    public class ConstraintList {

        public List<ConstraintType> types = new List<ConstraintType>();

        //public List<OwnerConstraints> owner = new List<OwnerConstraints>();
        public OwnerConstraints owner;
        public List<CardType> primaryType = new List<CardType>();
        public List<CardType> additionalType = new List<CardType>();
        public List<Subtypes> subtype = new List<Subtypes>();
        public List<Keywords> keyword = new List<Keywords>();
        public List<Attunements> attunement = new List<Attunements>();
        public List<DeckType> currentZone = new List<DeckType>();
        public List<DeckType> previousZone = new List<DeckType>();
        public List<Constants.CreatureStatus> creatureStatus = new List<Constants.CreatureStatus>();

        public List<StatAdjustment> minStats = new List<StatAdjustment>();
        public List<StatAdjustment> maxStats = new List<StatAdjustment>();

        public CardStats mostStat;
        public CardStats leastStat;
        public bool thisCardOnly;


        //Creature Stat Adjusted
        public GainedOrLost gainedOrLost;
        public CardStats statChanged;


        //Cards In Zone
        public DeckType zoneToCheckForNumberOfCards;
        public MoreOrLess moreOrLess;
        public int numberOfcardsInZone;


        //Spawn Token
        public bool copyTargets;
        public string spawnableTokenDataName;
        public DeckType spawnTokenLocation;
        public CardType spawnCardType;
        public int numberOfSpawns;

        //Zone Change Fields
        public DeckType targetZone;

        //Count Toward Effect
        public bool requireMultipleTriggers;
        public bool resetCountAtTurnEnd;
        public int triggersRequired;
        public int counter;
    }

}
