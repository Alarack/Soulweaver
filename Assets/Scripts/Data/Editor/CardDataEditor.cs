using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using AbilityActivationTrigger = Constants.AbilityActivationTrigger;
using EffectType = Constants.EffectType;

using OwnerConstraints = Constants.OwnerConstraints;
using Keywords = Constants.Keywords;
using Subtypes = Constants.SubTypes;
using CardType = Constants.CardType;
using Attunements = Constants.Attunements;
using DeckType = Constants.DeckType;
using ConstraintType = Constants.ConstraintType;

[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor {

    private CardData _cardData;


    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        _cardData = (CardData)target;

        EditorGUILayout.LabelField("Card Info", EditorStyles.boldLabel);

        _cardData.cardID = (CardIDs.CardID)EditorGUILayout.EnumPopup("CardID", _cardData.cardID);
        _cardData.cardName = EditorGUILayout.TextField("Card Name", _cardData.cardName);
        _cardData.cardText = EditorGUILayout.TextField("Card Text", _cardData.cardText);
        _cardData.cardCost = EditorGUILayout.IntField("Card Cost", _cardData.cardCost);


        EditorGUILayout.Separator();

        _cardData.specialAttributes = EditorHelper.DrawExtendedList("Special Attributes", _cardData.specialAttributes, "Attribute", DrawSpecialAttributes);

        EditorGUILayout.Separator();

        _cardData.cardImage = EditorHelper.ObjectField<Sprite>("Card Image", _cardData.cardImage);
        // _cardData.cardImagePos = EditorHelper.Vector2Field(_cardData.cardImagePos);
        _cardData.cardImagePos = EditorGUILayout.Vector2Field("Card Image Position", _cardData.cardImagePos);


        _cardData.attackEffect = EditorGUILayout.TextField("Attack Effect Name", _cardData.attackEffect);
        _cardData.movingVFX = EditorGUILayout.Toggle("Moving VFX?", _cardData.movingVFX);
        EditorGUILayout.Separator();
        _cardData.deathVFX = EditorGUILayout.TextField("Death Effect Name", _cardData.deathVFX);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        _cardData.primaryCardType = (Constants.CardType)EditorGUILayout.EnumPopup("Primary Type", _cardData.primaryCardType);
        _cardData.otherCardTypes = EditorHelper.DrawList("Other Card Types", _cardData.otherCardTypes, true, Constants.CardType.None, true, DrawCardTypes);
        _cardData.subTypes = EditorHelper.DrawList("Subtypes", _cardData.subTypes, true, Constants.SubTypes.None, true, DrawSubtypes);
        _cardData.attunements = EditorHelper.DrawList("Attunements", _cardData.attunements, true, Constants.Attunements.None, true, DrawAttunements);
        _cardData.keywords = EditorHelper.DrawList("Keywords", _cardData.keywords, true, Constants.Keywords.None, true, DrawKeywords);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Special Abilities", EditorStyles.boldLabel);
        _cardData.userTargtedAbilities = EditorHelper.DrawExtendedList("User Targted Effects", _cardData.userTargtedAbilities, "Ability", DrawAbilityList);
        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        _cardData.multiTargetAbilities = EditorHelper.DrawExtendedList("Logic Target Effects", _cardData.multiTargetAbilities, "Ability", DrawAbilityList);



        if (GUI.changed)
            EditorUtility.SetDirty(target);




        //DrawDefaultInspector();
    }


    private SpecialAttribute DrawSpecialAttributes(SpecialAttribute entry) {
        entry.attributeType = EditorHelper.EnumPopup("Attribute Type", entry.attributeType);
        entry.attributeValue = EditorGUILayout.IntField("Attribute Value", entry.attributeValue);

        return entry;
    }




    private T DrawAbilityList<T>(T ability) where T : SpecialAbility {

        return (T)DrawSpecalAbilities(ability);
    }

    private T DrawEffectList<T>(T effect) where T : Effect {

        return (T)DrawEffect(effect);
    }


    private Effect DrawEffect(Effect entry) {

        //Zone Change
        if(entry is EffectZoneChange) {
            EffectZoneChange zoneChange = entry as EffectZoneChange;
            zoneChange.targetLocation = EditorHelper.EnumPopup("Target Zone", zoneChange.targetLocation);
        }

        //Spawn Tokens
        if(entry is EffectSpawnToken) {
            EffectSpawnToken spawnToken = entry as EffectSpawnToken;

            spawnToken.spawnMethod = EditorHelper.EnumPopup("Spawn Method", spawnToken.spawnMethod);

            spawnToken.numberOfSpawns = EditorGUILayout.IntField("Number of Spawns", spawnToken.numberOfSpawns);
            spawnToken.spawnTokenLocation = EditorHelper.EnumPopup("Send Token Where?", spawnToken.spawnTokenLocation);

            switch (spawnToken.spawnMethod) {
                case EffectSpawnToken.SpawnMethod.Basic:
                    spawnToken.spawnableTokenDataName = EditorGUILayout.TextField("Token Data Name", spawnToken.spawnableTokenDataName);
                    spawnToken.spawnCardType = EditorHelper.EnumPopup("Token Card Type", spawnToken.spawnCardType);
                    break;

                case EffectSpawnToken.SpawnMethod.Series:
                    spawnToken.tokenSeriesNames = EditorHelper.DrawList("Token Data Names", spawnToken.tokenSeriesNames, true, "", true, DrawListOfStrings);
                    spawnToken.spawnCardType = EditorHelper.EnumPopup("Token Card Type", spawnToken.spawnCardType);
                    break;

                case EffectSpawnToken.SpawnMethod.CopyStats:
                    spawnToken.spawnCardType = EditorHelper.EnumPopup("Token Card Type", spawnToken.spawnCardType);
                    break;

                default:
                    break;
            }
        }//End of Spawn Token

        //Stat Adjustments
        if(entry is EffectStatAdjustment) {
            EffectStatAdjustment adjustment = entry as EffectStatAdjustment;


            adjustment.valueSetmethod = EditorHelper.EnumPopup("Stat Adjustment Method", adjustment.valueSetmethod);
            //adjustment.adjustments = EditorHelper.DrawExtendedList("Stat Adjustments", )

            switch (adjustment.valueSetmethod) {
                case EffectStatAdjustment.ValueSetMethod.Manual:
                    adjustment.adjustments = EditorHelper.DrawExtendedList("Adjustments", adjustment.adjustments, "Adjustment", DrawManualStatAdjustment);
                    break;

                case EffectStatAdjustment.ValueSetMethod.DeriveValueFromTargetStat:
                    adjustment.deriveStatsFromWhom = EditorHelper.EnumPopup("Derive Stat from what Target?", adjustment.deriveStatsFromWhom);
                    adjustment.targetStat = EditorHelper.EnumPopup("Which Stat to derive value from?", adjustment.targetStat);
                    adjustment.adjustments = EditorHelper.DrawExtendedList("Adjustments", adjustment.adjustments, "Adjustment", DrawDerivedStatAdjustment);

                    break;

                case EffectStatAdjustment.ValueSetMethod.DeriveValueFromCardsInZone:
                    adjustment.zoneToCount = EditorHelper.EnumPopup("Which Zone?", adjustment.zoneToCount);
                    //adjustment.constraints =

                    adjustment.constraints.types = EditorHelper.DrawList("Zone Constraints", adjustment.constraints.types, true, ConstraintType.None, true, DrawConstraintTypes);
                    for (int i = 0; i < adjustment.constraints.types.Count; i++) {
                        ShowConstraintsOfType(adjustment.constraints.types[i], adjustment.constraints, "Counted Target");
                    }

                    adjustment.adjustments = EditorHelper.DrawExtendedList("Adjustments", adjustment.adjustments, "Adjustment", DrawDerivedStatAdjustment);

                    break;

                default:
                    break;
            }
        }//End of Stat Adjustments

        //Generate Resource
        if(entry is EffectGenerateResource) {
            EffectGenerateResource generateResource = entry as EffectGenerateResource;

            generateResource.resourceType = EditorHelper.EnumPopup("What Kind of Resource?", generateResource.resourceType);
            generateResource.resourceName = EditorGUILayout.TextField("Resource Name: ", generateResource.resourceName);
            generateResource.cap = EditorGUILayout.IntField("Resource Cap? Leave 0 if Inifnite", generateResource.cap);
            generateResource.amount = EditorGUILayout.IntField("Generate How Much?", generateResource.amount);
        }

        //Add or Remove Keywords
        if(entry is EffectAddorRemoveKeywords) {
            EffectAddorRemoveKeywords keywords = entry as EffectAddorRemoveKeywords;
            keywords.addOrRemove = EditorHelper.EnumPopup("Add or Remove?", keywords.addOrRemove);
            keywords.keywords = EditorHelper.DrawList("Keywords", keywords.keywords, true, Keywords.None, true, DrawKeywords);

        }

        //Add or Remove Special Attributes
        if(entry is EffectAddorRemoveSpecialAttribute) {
            EffectAddorRemoveSpecialAttribute specialAttribute = entry as EffectAddorRemoveSpecialAttribute;
            specialAttribute.attributeType = EditorHelper.EnumPopup("Attribute Type", specialAttribute.attributeType);

            switch (specialAttribute.attributeAction) {
                case EffectAddorRemoveSpecialAttribute.AttributeAction.Add:
                    specialAttribute.value = EditorGUILayout.IntField("Value", specialAttribute.value);
                    break;

                case EffectAddorRemoveSpecialAttribute.AttributeAction.Modify:
                    specialAttribute.modificationValue = EditorGUILayout.IntField("Modifier Value", specialAttribute.modificationValue);
                    break;

            }

        }

        return entry;
    }

    private SpecialAbility.StatAdjustment DrawManualStatAdjustment(SpecialAbility.StatAdjustment entry) {

        entry.stat = EditorHelper.EnumPopup("Stat", entry.stat);
        entry.value = EditorGUILayout.IntField("Value", entry.value);

        entry.nonStacking = EditorGUILayout.Toggle("Non-Stacking?", entry.nonStacking);
        entry.temporary = EditorGUILayout.Toggle("Is this removable?", entry.temporary);
        entry.spellDamage = EditorGUILayout.Toggle("Is this Spell Damage?", entry.spellDamage);

        return entry;
    }

    private SpecialAbility.StatAdjustment DrawDerivedStatAdjustment(SpecialAbility.StatAdjustment entry) {

        entry.stat = EditorHelper.EnumPopup("Stat", entry.stat);

        entry.nonStacking = EditorGUILayout.Toggle("Non-Stacking?", entry.nonStacking);
        entry.temporary = EditorGUILayout.Toggle("Is this removable?", entry.temporary);
        entry.spellDamage = EditorGUILayout.Toggle("Is this Spell Damage?", entry.spellDamage);

        return entry;
    }

    private SpecialAbility.StatAdjustment DrawStatInformation(SpecialAbility.StatAdjustment entry) {
        entry.stat = EditorHelper.EnumPopup("Stat", entry.stat);
        entry.value = EditorGUILayout.IntField("Value", entry.value);

        return entry;
    }


    private SpecialAbility DrawSpecalAbilities(SpecialAbility entry) {

        GUIStyle boldRed = new GUIStyle(EditorStyles.boldLabel);
        boldRed.normal.textColor = Color.red;


        entry.abilityName = EditorGUILayout.TextField("Name of Ability (Optional) ", entry.abilityName);

        EditorGUILayout.LabelField("Start of " + entry.abilityName + " section", boldRed);

        //Trigger Logic
        EditorHelper.DrawInspectorSectionHeader("Effect Triggers:");

        entry.trigger = EditorHelper.DrawList("Triggers", entry.trigger, true, AbilityActivationTrigger.None, true, DrawActivationTrigger);
        entry.triggerConstraints.oncePerTurn = EditorGUILayout.Toggle("Can only trigger once per turn?", entry.triggerConstraints.oncePerTurn);
        entry.triggerConstraints.requireMultipleTriggers = EditorGUILayout.Toggle("Require Multiple Triggers?", entry.triggerConstraints.requireMultipleTriggers);
        if (entry.triggerConstraints.requireMultipleTriggers) {
            entry.triggerConstraints.triggersRequired = EditorGUILayout.IntField("How many?", entry.triggerConstraints.triggersRequired);
            entry.triggerConstraints.resetCountAtTurnEnd = EditorGUILayout.Toggle("Reset Counter On Turn End?", entry.triggerConstraints.resetCountAtTurnEnd);
        }

        for (int i = 0; i < entry.trigger.Count; i++) {
            switch (entry.trigger[i]) {

                case AbilityActivationTrigger.CreatureStatChanged:
                    entry.triggerConstraints.statChanged = EditorHelper.EnumPopup("Which Stat Changed?", entry.triggerConstraints.statChanged);
                    entry.triggerConstraints.gainedOrLost = EditorHelper.EnumPopup("Gained Or Lost?", entry.triggerConstraints.gainedOrLost);
                    entry.processTriggerOnWhom = EditorHelper.EnumPopup("Process on Which card?", entry.processTriggerOnWhom);

                    break;

                case AbilityActivationTrigger.SecondaryEffect:
                    EditorGUILayout.Separator();
                    entry.triggerConstraints.triggerbySpecificAbility = EditorGUILayout.Toggle("Trigger from specific ability?", entry.triggerConstraints.triggerbySpecificAbility);

                    if (entry.triggerConstraints.triggerbySpecificAbility) {
                        entry.triggerConstraints.triggerablePrimaryAbilityName = EditorGUILayout.TextField("Ability Name", entry.triggerConstraints.triggerablePrimaryAbilityName);
                    }

                    if (entry is LogicTargetedAbility) {
                        LogicTargetedAbility lta = entry as LogicTargetedAbility;

                        lta.processEffectOnPrimaryEffectTargets = EditorGUILayout.Toggle("Target the same targets as primary ability?", lta.processEffectOnPrimaryEffectTargets);
                    }

                    break;

                case AbilityActivationTrigger.Slain:
                    entry.processTriggerOnWhom = EditorHelper.EnumPopup("Process on Which card?", entry.processTriggerOnWhom);

                    break;
            }
        }

        EditorGUILayout.Separator();

        entry.triggerConstraints.thisCardOnly = EditorGUILayout.Toggle("Only this card can trigger this effect?", entry.triggerConstraints.thisCardOnly);
        entry.triggerConstraints.types = EditorHelper.DrawList("Trigger Constraints", entry.triggerConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);
        for (int i = 0; i < entry.triggerConstraints.types.Count; i++) {
            ShowConstraintsOfType(entry.triggerConstraints.types[i], entry.triggerConstraints, "Trigger");

        }
        EditorHelper.DrawInspectorSectionFooter();


        EditorGUILayout.Separator();


        //Source of Effect Logic
        EditorHelper.DrawInspectorSectionHeader("Source of Effect:");
        entry.sourceConstraints.types = EditorHelper.DrawList("Source Constraints", entry.sourceConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);
        for (int i = 0; i < entry.sourceConstraints.types.Count; i++) {
            ShowConstraintsOfType(entry.sourceConstraints.types[i], entry.sourceConstraints, "Source of this Effect");
        }

        EditorHelper.DrawInspectorSectionFooter();


        EditorGUILayout.Separator();

        //Additional Requirements
        EditorHelper.DrawInspectorSectionHeader("Additional Requirements:");

        entry.additionalRequirements = EditorHelper.DrawList("Requirement", entry.additionalRequirements, true, Constants.AdditionalRequirement.None, true, DrawAdditionalRequirements);

        for (int i = 0; i < entry.additionalRequirements.Count; i++) {
            switch (entry.additionalRequirements[i]) {
                case Constants.AdditionalRequirement.NumberofCardsInZone:
                    entry.additionalRequirementConstraints.zoneToCheckForNumberOfCards = EditorHelper.EnumPopup("Zone to Check", entry.additionalRequirementConstraints.zoneToCheckForNumberOfCards);
                    entry.additionalRequirementConstraints.numberOfcardsInZone = EditorGUILayout.IntField("How many?", entry.additionalRequirementConstraints.numberOfcardsInZone);
                    entry.additionalRequirementConstraints.moreOrLess = EditorHelper.EnumPopup("At Least Or Less Than?", entry.additionalRequirementConstraints.moreOrLess);

                    EditorGUILayout.Separator();
                    entry.additionalRequirementConstraints.types = EditorHelper.DrawList("Number of cards Constraint", entry.additionalRequirementConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);

                    for (int j = 0; j < entry.additionalRequirementConstraints.types.Count; j++) {
                        ShowConstraintsOfType(entry.additionalRequirementConstraints.types[j], entry.additionalRequirementConstraints, "Extra Requirement");
                    }

                    break;

                case Constants.AdditionalRequirement.RequireResource:
                    entry.additionalRequirementConstraints.requiredResourceType = EditorHelper.EnumPopup("What kind of Resource?", entry.additionalRequirementConstraints.requiredResourceType);
                    entry.additionalRequirementConstraints.amountOfResourceRequried = EditorGUILayout.IntField("How Much?", entry.additionalRequirementConstraints.amountOfResourceRequried);
                    entry.additionalRequirementConstraints.consumeResource = EditorGUILayout.Toggle("Consume this resource?", entry.additionalRequirementConstraints.consumeResource);


                    break;
            }
        }


        EditorHelper.DrawInspectorSectionFooter();


        EditorGUILayout.Separator();

        //Effect Logic
        EditorHelper.DrawInspectorSectionHeader("Effect:");
        entry.abilityVFX = EditorGUILayout.TextField("Effect VFX Name", entry.abilityVFX);
        entry.movingVFX = EditorGUILayout.Toggle("Moving VFX?", entry.movingVFX);
        entry.effect = (Constants.EffectType)EditorGUILayout.EnumPopup(entry.effect);

        switch (entry.effect) {
            case EffectType.SpawnToken:
                entry.effectHolder.tokenSpanws = EditorHelper.DrawExtendedList("Spawn Token Effects", entry.effectHolder.tokenSpanws, "Spawn Token", DrawEffectList);

                break;

            case EffectType.ZoneChange:
                entry.effectHolder.zoneChanges = EditorHelper.DrawExtendedList("Zone Change Effect", entry.effectHolder.zoneChanges, "Zone Change", DrawEffectList);

                //entry.targetConstraints.targetZone = EditorHelper.EnumPopup("Target Zone", entry.targetConstraints.targetZone);
                break;

            case EffectType.AddOrRemoveKeywordAbilities:
            case EffectType.RemoveKeywordAbilities:
                //entry.keywordsToAddorRemove = EditorHelper.DrawList("Keywords", entry.keywordsToAddorRemove, true, Keywords.None, true, DrawKeywords);

                entry.effectHolder.addOrRemoveKeywords = EditorHelper.DrawExtendedList("Add or Remove Keyword Effect", entry.effectHolder.addOrRemoveKeywords, "Keyword", DrawEffectList);


                break;

            case EffectType.GenerateResource:

                entry.effectHolder.generateResources = EditorHelper.DrawExtendedList("Resource Generation Effect", entry.effectHolder.generateResources, "Generate Resource", DrawEffectList);

                break;

            case EffectType.StatAdjustment:
                entry.effectHolder.statAdjustments = EditorHelper.DrawExtendedList("Stat Adjustment Effects", entry.effectHolder.statAdjustments, "Stat Adjustments", DrawEffectList);

                break;

            case EffectType.GrantSpecialAttribute:


                entry.effectHolder.addOrRemoveSpecialAttribute = EditorHelper.DrawExtendedList("Special Attribute Effects", entry.effectHolder.addOrRemoveSpecialAttribute, "Special Attribute", DrawEffectList);




                break;

            case EffectType.RemoveOtherEffect:
                entry.targetConstraints.abilityToRemove = EditorGUILayout.TextField("Name of Ability to Remove", entry.targetConstraints.abilityToRemove);
                break;

            case EffectType.RetriggerOtherEffect:
                entry.targetConstraints.abilityToRetrigger = EditorGUILayout.TextField("Name of Ability to Trigger", entry.targetConstraints.abilityToRetrigger);

                break;

        }//End of Effects


        entry.duration = EditorHelper.EnumPopup("Duration", entry.duration);

        //EditorHelper.DrawInspectorSectionHeader("Stat Adjustments");
        //entry.statAdjustments = EditorHelper.DrawExtendedList(entry.statAdjustments, "Stat Adjustment", DrawStatAdjustments);


        //EditorHelper.DrawInspectorSectionFooter();


        EditorGUILayout.Separator();

        if (!entry.targetConstraints.neverTargetSelf)
            entry.targetConstraints.thisCardOnly = EditorGUILayout.Toggle("This card only targets itself?", entry.targetConstraints.thisCardOnly);

        if (!entry.targetConstraints.thisCardOnly)
            entry.targetConstraints.neverTargetSelf = EditorGUILayout.Toggle("This card can't target itself?", entry.targetConstraints.neverTargetSelf);


        entry.targetConstraints.types = EditorHelper.DrawList("Target Constraints", entry.targetConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility logicTargeted = entry as LogicTargetedAbility;

            logicTargeted.logicTargetingMethod = EditorHelper.EnumPopup("Targeting Method", logicTargeted.logicTargetingMethod);

            if (logicTargeted.logicTargetingMethod == LogicTargetedAbility.LogicTargeting.NumberOfValidTargets) {
                logicTargeted.numberofTargets = EditorGUILayout.IntField("Number of Targets", logicTargeted.numberofTargets);
            }
        }

        for (int i = 0; i < entry.targetConstraints.types.Count; i++) {
            ShowConstraintsOfType(entry.targetConstraints.types[i], entry.targetConstraints, "Target");
        }

        EditorHelper.DrawInspectorSectionFooter();


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("End of " + entry.abilityName + " section", boldRed);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        return entry;
    }

    //private SpecialAbility.StatAdjustment DrawStatAdjustments(SpecialAbility.StatAdjustment entry) {
    //    entry.stat = (Constants.CardStats)EditorGUILayout.EnumPopup("Stat", entry.stat);

    //    entry.valueSetBytargetStat = EditorGUILayout.Toggle("Set Value By an Effect Target's Stat?", entry.valueSetBytargetStat);

    //    if (entry.valueSetBytargetStat) {
    //        entry.deriveStatsFromWhom = EditorHelper.EnumPopup("Derive Stats from Target or Source?", entry.deriveStatsFromWhom);

    //        entry.targetStat = EditorHelper.EnumPopup("Target Stat", entry.targetStat);
    //        entry.invertValue = EditorGUILayout.Toggle("Inverse Value?", entry.invertValue);
    //    }
    //    else {
    //        entry.value = EditorGUILayout.IntField("Value", entry.value);
    //    }


    //    entry.nonStacking = EditorGUILayout.Toggle("Non-Stacking?", entry.nonStacking);
    //    entry.temporary = EditorGUILayout.Toggle("Is this removable?", entry.temporary);
    //    entry.spellDamage = EditorGUILayout.Toggle("Is this Spell Damage?", entry.spellDamage);

    //    //if(entry.uniqueID == -1 || entry.uniqueID == 0) {
    //    //    entry.uniqueID = IDFactory.GenerateID();
    //    //}


    //    return entry;
    //}


    //private 

    private string ShowNot(bool showNot) {
        if (showNot)
            return " NOT ";
        else
            return " ";
    }


    private void ShowConstraintsOfType(ConstraintType type, SpecialAbility.ConstraintList entry, string constraintName = null) {

        switch (type) {
            case Constants.ConstraintType.Owner:
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Which Owners are Valid for the " + constraintName + "?", EditorStyles.boldLabel);
                entry.owner = EditorHelper.EnumPopup("Owner", entry.owner); //EditorHelper.DrawList("Owner", entry.owner, true, OwnerConstraints.None, true, DrawOwnerTypes);
                break;

            case ConstraintType.WhosTurn:
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("On whos turn can the " + constraintName + " occur?", EditorStyles.boldLabel);
                entry.whosTurn = EditorHelper.EnumPopup("Who's Turn?", entry.whosTurn);

                break;

            case Constants.ConstraintType.PrimaryType:
                EditorGUILayout.Separator();

                entry.notPrimaryType = EditorGUILayout.Toggle("Not?", entry.notPrimaryType);

                EditorGUILayout.LabelField("What Primary Type is" + ShowNot(entry.notPrimaryType) + "valid for the " + constraintName + "?", EditorStyles.boldLabel);

                entry.primaryType = EditorHelper.DrawList("Primary Type", entry.primaryType, true, CardType.None, true, DrawCardTypes);
                break;

            case Constants.ConstraintType.AdditionalType:
                EditorGUILayout.Separator();

                entry.notAdditionalType = EditorGUILayout.Toggle("Not?", entry.notAdditionalType);

                EditorGUILayout.LabelField("Which additional Card Types are" + ShowNot(entry.notAdditionalType) + "valid for the " + constraintName + " ?", EditorStyles.boldLabel);

                entry.additionalType = EditorHelper.DrawList("Additional Type", entry.additionalType, true, CardType.None, true, DrawCardTypes);
                break;

            case Constants.ConstraintType.Subtype:
                EditorGUILayout.Separator();

                entry.notSubtype = EditorGUILayout.Toggle("Not?", entry.notSubtype);

                EditorGUILayout.LabelField("Which Subtypes are" + ShowNot(entry.notSubtype) + "valid for the " + constraintName + " ?", EditorStyles.boldLabel);

                entry.subtype = EditorHelper.DrawList("Subtype", entry.subtype, true, Subtypes.None, true, DrawSubtypes);
                break;

            case Constants.ConstraintType.Keyword:
                EditorGUILayout.Separator();

                entry.notKeyword = EditorGUILayout.Toggle("Not?", entry.notKeyword);

                EditorGUILayout.LabelField("Which Keywords are" + ShowNot(entry.notKeyword) + "valid for the " + constraintName + " ?", EditorStyles.boldLabel);

                entry.keyword = EditorHelper.DrawList("Keyword", entry.keyword, true, Keywords.None, true, DrawKeywords);
                break;

            case Constants.ConstraintType.Attunement:
                EditorGUILayout.Separator();

                entry.notAttunement = EditorGUILayout.Toggle("Not?", entry.notAttunement);

                EditorGUILayout.LabelField("Which Attunements are" + ShowNot(entry.notAttunement) + "valid for the " + constraintName + " ?", EditorStyles.boldLabel);

                entry.attunement = EditorHelper.DrawList("Attunement", entry.attunement, true, Attunements.None, true, DrawAttunements);
                break;

            case Constants.ConstraintType.CurrentZone:
                EditorGUILayout.Separator();

                entry.notCurrentZone = EditorGUILayout.Toggle("Not?", entry.notCurrentZone);

                EditorGUILayout.LabelField("What Zones can the " + constraintName + ShowNot(entry.notCurrentZone) + "be in?", EditorStyles.boldLabel);

                entry.currentZone = EditorHelper.DrawList("Current Zone", entry.currentZone, true, DeckType.None, true, DrawDeckTypes);
                break;

            case Constants.ConstraintType.PreviousZone:
                EditorGUILayout.Separator();

                entry.notCurrentZone = EditorGUILayout.Toggle("Not?", entry.notPreviousZone);

                EditorGUILayout.LabelField("What Previous Zones could the " + constraintName + ShowNot(entry.notPreviousZone) + "have previously occupid?", EditorStyles.boldLabel);

                entry.previousZone = EditorHelper.DrawList("Previous Zone", entry.previousZone, true, DeckType.None, true, DrawDeckTypes);
                break;

            case Constants.ConstraintType.StatMinimum:
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Does the " + constraintName + " meet a minimum stat requirement?", EditorStyles.boldLabel);
                entry.minStats = EditorHelper.DrawExtendedList("Minimum Stats", entry.minStats, "Stat", DrawStatInformation);
                break;

            case Constants.ConstraintType.StatMaximum:
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Does the " + constraintName + " meet a Maximum stat requirement?", EditorStyles.boldLabel);
                entry.maxStats = EditorHelper.DrawExtendedList("Maximum Stats", entry.maxStats, "Stat", DrawStatInformation);
                break;

            //case ConstraintType.NumberofCardsInZone:
            //    EditorGUILayout.Separator();
            //    EditorGUILayout.LabelField("How many cards must be in the specified zone?", EditorStyles.boldLabel);
            //    entry.numberOfCardsInZone = EditorGUILayout.IntField("Requirement", entry.numberOfCardsInZone);

            //    break;

            case ConstraintType.CreatureStatus:
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Does the " + constraintName + " meet any special creature status requirements?", EditorStyles.boldLabel);
                entry.creatureStatus = EditorHelper.DrawList("Creature Status", entry.creatureStatus, true, Constants.CreatureStatus.None, true, DrawCreatureStatus);


                for (int i = 0; i < entry.creatureStatus.Count; i++) {

                    switch (entry.creatureStatus[i]) {
                        case Constants.CreatureStatus.Damaged:
                            EditorGUILayout.LabelField("Damaged Souls Only. No Extra Input Needed");
                            break;

                        case Constants.CreatureStatus.Undamaged:
                            EditorGUILayout.LabelField("Undamaged Souls Only. No Extra Input Needed");
                            break;

                        case Constants.CreatureStatus.MostStat:
                            EditorGUILayout.LabelField("Soul with the Most of: ");

                            entry.mostStat = EditorHelper.EnumPopup(entry.mostStat);
                            //entry.mostStat.value = EditorGUILayout.IntField("Value", entry.mostStat.value);
                            break;

                        case Constants.CreatureStatus.LeastStat:
                            EditorGUILayout.LabelField("Soul with the Least of: ");

                            entry.leastStat = EditorHelper.EnumPopup(entry.leastStat);
                            //entry.leastStat.stat = EditorHelper.EnumPopup("Stat", entry.leastStat.stat);
                            //entry.leastStat.value = EditorGUILayout.IntField("Value", entry.leastStat.value);
                            break;
                    }


                }


                break;
        }
    }



    private Constants.ConstraintType DrawConstraintTypes(List<ConstraintType> list, int index) {
        Constants.ConstraintType result = (Constants.ConstraintType)EditorGUILayout.EnumPopup("Constraint Type", list[index]);
        return result;
    }

    private OwnerConstraints DrawOwnerTypes(List<OwnerConstraints> list, int index) {
        Constants.OwnerConstraints result = (Constants.OwnerConstraints)EditorGUILayout.EnumPopup("Owners", list[index]);
        return result;
    }

    private Constants.Attunements DrawAttunements(List<Constants.Attunements> list, int index) {
        Constants.Attunements result = (Constants.Attunements)EditorGUILayout.EnumPopup("Attunement", list[index]);
        return result;
    }

    private Constants.CardType DrawCardTypes(List<Constants.CardType> list, int index) {
        Constants.CardType result = (Constants.CardType)EditorGUILayout.EnumPopup("Card Type", list[index]);
        return result;
    }

    private Constants.Keywords DrawKeywords(List<Constants.Keywords> list, int index) {
        Constants.Keywords result = (Constants.Keywords)EditorGUILayout.EnumPopup("Keywords", list[index]);
        return result;
    }

    private Constants.SubTypes DrawSubtypes(List<Constants.SubTypes> list, int index) {
        Constants.SubTypes result = (Constants.SubTypes)EditorGUILayout.EnumPopup("Subtypes", list[index]);
        return result;
    }

    private Constants.DeckType DrawDeckTypes(List<Constants.DeckType> list, int index) {
        Constants.DeckType result = (Constants.DeckType)EditorGUILayout.EnumPopup("Zones", list[index]);
        return result;
    }

    private GameResource.ResourceType DrawResourceTypes(List<GameResource.ResourceType> list, int index) {
        GameResource.ResourceType result = (GameResource.ResourceType)EditorGUILayout.EnumPopup("Resources", list[index]);
        return result;
    }

    private Constants.CreatureStatus DrawCreatureStatus(List<Constants.CreatureStatus> list, int index) {
        Constants.CreatureStatus result = (Constants.CreatureStatus)EditorGUILayout.EnumPopup("Status", list[index]);
        return result;
    }

    private AbilityActivationTrigger DrawActivationTrigger(List<AbilityActivationTrigger> list, int index) {
        AbilityActivationTrigger result = (AbilityActivationTrigger)EditorGUILayout.EnumPopup("Trigger", list[index]);
        return result;
    }

    private Constants.AdditionalRequirement DrawAdditionalRequirements(List<Constants.AdditionalRequirement> list, int index) {
        Constants.AdditionalRequirement result = EditorHelper.EnumPopup("Requirement", list[index]);
        return result;
    }

    private string DrawListOfStrings(List<string> list, int index) {
        string result = EditorGUILayout.TextField("Entry", list[index]);
        return result;
    }



    //private Constants.SpecialAbilityTypes DrawAbilityTypes(List<Constants.SpecialAbilityTypes> list, int index) {
    //    Constants.SpecialAbilityTypes result = (Constants.SpecialAbilityTypes)EditorGUILayout.EnumPopup("Specials", list[index]);
    //    return result;
    //}

}
