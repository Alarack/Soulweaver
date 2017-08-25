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
        _cardData.faction = EditorHelper.EnumPopup("Faction", _cardData.faction);
        
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
        if (entry is EffectZoneChange) {
            EffectZoneChange zoneChange = entry as EffectZoneChange;
            zoneChange.targetLocation = EditorHelper.EnumPopup("Target Zone", zoneChange.targetLocation);
        }

        //Spawn Tokens
        if (entry is EffectSpawnToken) {
            EffectSpawnToken spawnToken = entry as EffectSpawnToken;

            spawnToken.spawnMethod = EditorHelper.EnumPopup("Spawn Method", spawnToken.spawnMethod);

            spawnToken.numberOfSpawns = EditorGUILayout.IntField("Number of Spawns", spawnToken.numberOfSpawns);
            spawnToken.spawnTokenLocation = EditorHelper.EnumPopup("Send Token Where?", spawnToken.spawnTokenLocation);

            if (spawnToken.spawnMethod != EffectSpawnToken.SpawnMethod.CopyStats)
                spawnToken.spawnForOpponent = EditorGUILayout.Toggle("Spawn for Opponent?", spawnToken.spawnForOpponent);

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
                    spawnToken.spawnableTokenDataName = EditorGUILayout.TextField("Token Data Name", spawnToken.spawnableTokenDataName);
                    spawnToken.spawnCardType = EditorHelper.EnumPopup("Token Card Type", spawnToken.spawnCardType);

                    if (spawnToken.spawnForOpponent)
                        spawnToken.spawnForOpponent = false;
                    break;

                default:
                    break;
            }
        }//End of Spawn Token

        //Stat Adjustments
        if (entry is EffectStatAdjustment) {
            EffectStatAdjustment adjustment = entry as EffectStatAdjustment;


            adjustment.valueSetmethod = EditorHelper.EnumPopup("Stat Adjustment Method", adjustment.valueSetmethod);
            adjustment.setStatToValue = EditorGUILayout.Toggle("Set stat to BE Value?", adjustment.setStatToValue);
            //adjustment.adjustments = EditorHelper.DrawExtendedList("Stat Adjustments", )

            switch (adjustment.valueSetmethod) {
                case EffectStatAdjustment.ValueSetMethod.Manual:
                    adjustment.adjustments = EditorHelper.DrawExtendedList("Adjustments", adjustment.adjustments, "Adjustment", DrawManualStatAdjustment);
                    break;

                case EffectStatAdjustment.ValueSetMethod.DeriveValueFromTargetStat:

                    if (adjustment.deriveStatsFromWhom == EffectStatAdjustment.DeriveStatsFromWhom.TargetOFEffect)
                        adjustment.targetAbilityName = EditorGUILayout.TextField("Target From What Ability?", adjustment.targetAbilityName);

                    adjustment.deriveStatsFromWhom = EditorHelper.EnumPopup("Derive Stat from what Target?", adjustment.deriveStatsFromWhom);
                    adjustment.targetStat = EditorHelper.EnumPopup("Which Stat to derive value from?", adjustment.targetStat);
                    adjustment.maxValue = EditorGUILayout.IntField("Max Value? Leave 0 if Infinite", adjustment.maxValue);
                    adjustment.invertValue = EditorGUILayout.Toggle("Invert Value?", adjustment.invertValue);
                    adjustment.adjustments = EditorHelper.DrawExtendedList("Adjustments", adjustment.adjustments, "Adjustment", DrawDerivedStatAdjustment);

                    break;

                case EffectStatAdjustment.ValueSetMethod.DeriveValueFromCardsInZone:
                    adjustment.zoneToCount = EditorHelper.EnumPopup("Which Zone?", adjustment.zoneToCount);
                    adjustment.maxValue = EditorGUILayout.IntField("Max Value? Leave 0 if Infinite", adjustment.maxValue);
                    adjustment.invertValue = EditorGUILayout.Toggle("Invert Value?", adjustment.invertValue);
                    adjustment.constraints.types = EditorHelper.DrawList("Zone Constraints", adjustment.constraints.types, true, ConstraintType.None, true, DrawConstraintTypes);
                    for (int i = 0; i < adjustment.constraints.types.Count; i++) {
                        ShowConstraintsOfType(adjustment.constraints.types[i], adjustment.constraints, "Counted Target");
                    }

                    adjustment.adjustments = EditorHelper.DrawExtendedList("Adjustments", adjustment.adjustments, "Adjustment", DrawDerivedStatAdjustment);

                    break;

                case EffectStatAdjustment.ValueSetMethod.DeriveValueFromResource:
                    adjustment.targetResource = EditorHelper.EnumPopup("What Resource?", adjustment.targetResource);
                    adjustment.maxValue = EditorGUILayout.IntField("Max Value? Leave 0 if Infinite", adjustment.maxValue);
                    adjustment.invertValue = EditorGUILayout.Toggle("Invert Value?", adjustment.invertValue);
                    adjustment.adjustments = EditorHelper.DrawExtendedList("Adjustments", adjustment.adjustments, "Adjustment", DrawDerivedStatAdjustment);

                    break;

                case EffectStatAdjustment.ValueSetMethod.SwapStats:
                    adjustment.sourceStat = EditorHelper.EnumPopup("First Stat", adjustment.sourceStat);
                    adjustment.destinationStat = EditorHelper.EnumPopup("Second Stat", adjustment.destinationStat);



                    break;

                default:
                    break;
            }
        }//End of Stat Adjustments

        //Generate Resource
        if (entry is EffectGenerateResource) {
            EffectGenerateResource generateResource = entry as EffectGenerateResource;

            generateResource.resourceType = EditorHelper.EnumPopup("What Kind of Resource?", generateResource.resourceType);
            generateResource.resourceName = EditorGUILayout.TextField("Resource Name: ", generateResource.resourceName);
            generateResource.cap = EditorGUILayout.IntField("Resource Cap? Leave 0 if Inifnite", generateResource.cap);
            generateResource.amount = EditorGUILayout.IntField("Generate How Much?", generateResource.amount);
        }

        //Add or Remove Keywords
        if (entry is EffectAddorRemoveKeywords) {
            EffectAddorRemoveKeywords keywords = entry as EffectAddorRemoveKeywords;
            keywords.addOrRemove = EditorHelper.EnumPopup("Add or Remove?", keywords.addOrRemove);
            keywords.keywords = EditorHelper.DrawList("Keywords", keywords.keywords, true, Keywords.None, true, DrawKeywords);

        }

        //Add or Remove Special Attributes
        if (entry is EffectAddorRemoveSpecialAttribute) {
            EffectAddorRemoveSpecialAttribute specialAttribute = entry as EffectAddorRemoveSpecialAttribute;
            //specialAttribute.attributeType = EditorHelper.EnumPopup("Attribute Type", specialAttribute.attributeType);
            specialAttribute.attributeAction = EditorHelper.EnumPopup("Add, Modify, or Remmove?", specialAttribute.attributeAction);

            specialAttribute.specialAttributes = EditorHelper.DrawExtendedList("Special Attributes", specialAttribute.specialAttributes, "Adjustment", DrawManualSpecialAttribute);

            //switch (specialAttribute.attributeAction) {
            //    case EffectAddorRemoveSpecialAttribute.AttributeAction.Add:
            //        //specialAttribute.value = EditorGUILayout.IntField("Value", specialAttribute.value);

                    

            //        break;

            //    //case EffectAddorRemoveSpecialAttribute.AttributeAction.Modify:
            //    //    specialAttribute.modificationValue = EditorGUILayout.IntField("Modifier Value", specialAttribute.modificationValue);
            //    //    break;
            //}
        }

        //Choose One
        if (entry is EffectChooseOne) {
            EffectChooseOne chooseOne = entry as EffectChooseOne;

            chooseOne.cardType = EditorHelper.EnumPopup("Card Type", chooseOne.cardType);
            chooseOne.choices = EditorHelper.DrawList("Choice Data Names", chooseOne.choices, true, "", true, DrawListOfStrings);
        }

        //Bestow Ability
        if(entry is EffectBestowSpecialAbility) {
            EffectBestowSpecialAbility bestow = entry as EffectBestowSpecialAbility;

            bestow.targetAbiliity = EditorGUILayout.TextField("Target Ability Name", bestow.targetAbiliity);



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


    private SpecialAttribute DrawManualSpecialAttribute(SpecialAttribute entry) {
        entry.attributeType = EditorHelper.EnumPopup("Attribute Type", entry.attributeType);
        entry.attributeValue = EditorGUILayout.IntField("Value", entry.attributeValue);

        return entry;
    }




    private void DrawInspirePreset(SpecialAbility entry) {

        if (entry.trigger.Contains(AbilityActivationTrigger.EntersZone))
            return;

        entry.trigger.Add(AbilityActivationTrigger.EntersZone);
        entry.triggerConstraints.types.Add(ConstraintType.CurrentZone);
        entry.triggerConstraints.types.Add(ConstraintType.PreviousZone);
        entry.triggerConstraints.currentZone.Add(DeckType.Battlefield);
        entry.triggerConstraints.previousZone.Add(DeckType.Hand);
        entry.triggerConstraints.thisCardOnly = true;
    }

    private void DrawFinalePreset(SpecialAbility entry) {

        if (entry.trigger.Contains(AbilityActivationTrigger.EntersZone))
            return;

        entry.trigger.Add(AbilityActivationTrigger.EntersZone);
        entry.triggerConstraints.types.Add(ConstraintType.CurrentZone);
        entry.triggerConstraints.types.Add(ConstraintType.PreviousZone);

        entry.triggerConstraints.currentZone.Add(DeckType.SoulCrypt);
        entry.triggerConstraints.previousZone.Add(DeckType.Battlefield);
        entry.triggerConstraints.thisCardOnly = true;
    }

    private void DrawDeathwatchPreset(SpecialAbility entry) {
        if (entry.trigger.Contains(AbilityActivationTrigger.EntersZone))
            return;

        entry.trigger.Add(AbilityActivationTrigger.EntersZone);
        entry.triggerConstraints.types.Add(ConstraintType.CurrentZone);
        entry.triggerConstraints.types.Add(ConstraintType.PreviousZone);
        entry.triggerConstraints.types.Add(ConstraintType.PrimaryType);

        entry.triggerConstraints.currentZone.Add(DeckType.SoulCrypt);
        entry.triggerConstraints.previousZone.Add(DeckType.Battlefield);
        entry.triggerConstraints.primaryType.Add(CardType.Soul);

        entry.sourceConstraints.types.Add(ConstraintType.CurrentZone);
        entry.sourceConstraints.currentZone.Add(DeckType.Battlefield);

    }

    private void DrawBloodthirstPreset(SpecialAbility entry) {

        entry.trigger.Add(AbilityActivationTrigger.CreatureStatChanged);
        entry.triggerConstraints.statChanged = Constants.CardStats.Health;
        entry.triggerConstraints.statGainedOrLost = SpecialAbility.GainedOrLost.Lost;
        //entry.processTriggerOnWhom = SpecialAbility.ApplyEffectToWhom.TriggeringCard;

        entry.sourceConstraints.types.Add(ConstraintType.CurrentZone);
        entry.sourceConstraints.currentZone.Add(DeckType.Battlefield);


    }

    private void DrawSendToCryptPreset(SpecialAbility entry) {
        entry.abilityName = "Send to Crypt";

        entry.trigger.Add(AbilityActivationTrigger.SecondaryEffect);

        entry.effect = EffectType.ZoneChange;

        EffectZoneChange toTheCrypt = new EffectZoneChange();
        toTheCrypt.targetLocation = DeckType.SoulCrypt;

        entry.effectHolder.zoneChanges.Add(toTheCrypt);

        entry.targetConstraints.thisCardOnly = true;

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.AllValidTargets;
        }

    }

    private void DrawExaustAbilityPreset(SpecialAbility entry) {
        entry.trigger.Add(AbilityActivationTrigger.UserActivated);
        entry.sourceConstraints.types.Add(ConstraintType.CurrentZone);
        entry.sourceConstraints.types.Add(ConstraintType.Keyword);
        entry.sourceConstraints.currentZone.Add(DeckType.Battlefield);
        entry.sourceConstraints.keyword.Add(Keywords.Exhausted);
        entry.sourceConstraints.notKeyword = true;
    }

    private void DrawExhaustSelfPreset(SpecialAbility entry) {
        entry.abilityName = "Exhaust Self";

        entry.trigger.Add(AbilityActivationTrigger.SecondaryEffect);
        entry.effect = EffectType.AddOrRemoveKeywordAbilities;

        EffectAddorRemoveKeywords addExhaust = new EffectAddorRemoveKeywords();
        addExhaust.addOrRemove = EffectAddorRemoveKeywords.AddOrRemove.Add;
        addExhaust.keywords.Add(Keywords.Exhausted);
        entry.effectHolder.addOrRemoveKeywords.Add(addExhaust);
        entry.targetConstraints.thisCardOnly = true;

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.AllValidTargets;
        }

    }

    private void DrawArtifactPreset(SpecialAbility entry) {
        entry.trigger.Add(AbilityActivationTrigger.CreatureStatChanged);
        entry.triggerConstraints.statChanged = Constants.CardStats.Health;
        entry.triggerConstraints.statGainedOrLost = SpecialAbility.GainedOrLost.Lost;
        entry.processTriggerOnWhom = SpecialAbility.ApplyEffectToWhom.TriggeringCard;

        entry.triggerConstraints.types.Add(ConstraintType.Owner);
        entry.triggerConstraints.owner = OwnerConstraints.Mine;

        entry.triggerConstraints.types.Add(ConstraintType.PrimaryType);
        entry.triggerConstraints.primaryType.Add(CardType.Player);

        entry.sourceConstraints.types.Add(ConstraintType.CurrentZone);
        entry.sourceConstraints.currentZone.Add(DeckType.Battlefield);

        entry.effect = EffectType.StatAdjustment;

        EffectStatAdjustment durabilityLoss = new EffectStatAdjustment();

        SpecialAbility.StatAdjustment adj = new SpecialAbility.StatAdjustment();
        adj.stat = Constants.CardStats.SupportValue;
        adj.value = -1;

        durabilityLoss.adjustments.Add(adj);

        durabilityLoss.valueSetmethod = EffectStatAdjustment.ValueSetMethod.Manual;

        entry.effectHolder.statAdjustments.Add(durabilityLoss);

        entry.targetConstraints.thisCardOnly = true;

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.AllValidTargets;
        }
    }

    private void DrawPhalanxOtherEnterPreset(SpecialAbility entry) {
        entry.trigger.Add(AbilityActivationTrigger.EntersZone);
        entry.triggerConstraints.types.Add(ConstraintType.CardAdjacentToSource);

        entry.sourceConstraints.types.Add(ConstraintType.CurrentZone);
        entry.sourceConstraints.currentZone.Add(DeckType.Battlefield);


        entry.effectDuration = Constants.Duration.WhileInZone;
        entry.targetConstraints.types.Add(ConstraintType.PrimaryType);
        entry.targetConstraints.primaryType.Add(CardType.Soul);

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.OnlyTargetTriggeringCard;
        }

    }

    private void DrawPhalanxInspirePreset(SpecialAbility entry) {
        DrawInspirePreset(entry);

        entry.sourceConstraints.types.Add(ConstraintType.CurrentZone);
        entry.sourceConstraints.currentZone.Add(DeckType.Battlefield);


        entry.effectDuration = Constants.Duration.WhileInZone;

        entry.targetConstraints.types.Add(ConstraintType.PrimaryType);
        entry.targetConstraints.primaryType.Add(CardType.Soul);

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.AdjacentToSource;
        }

    }

    private void DrawTargetSoulsOnBattlefieldPreset(SpecialAbility entry) {
        entry.targetConstraints.types.Add(ConstraintType.CurrentZone);
        entry.targetConstraints.currentZone.Add(DeckType.Battlefield);
        entry.targetConstraints.types.Add(ConstraintType.PrimaryType);
        entry.targetConstraints.primaryType.Add(CardType.Soul);

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.AllValidTargets;
        }
    }

    private void DrawTargetOwnGeneralPreset(SpecialAbility entry) {
        entry.targetConstraints.types.Add(ConstraintType.CurrentZone);
        entry.targetConstraints.currentZone.Add(DeckType.Battlefield);
        entry.targetConstraints.types.Add(ConstraintType.PrimaryType);
        entry.targetConstraints.primaryType.Add(CardType.Player);
        entry.targetConstraints.types.Add(ConstraintType.Owner);
        entry.targetConstraints.owner = OwnerConstraints.Mine;

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.AllValidTargets;
        }



    }

    private void DrawCardDrawPreset(SpecialAbility entry) {

        entry.effect = EffectType.ZoneChange;

        EffectZoneChange drawCards = new EffectZoneChange();
        drawCards.targetLocation = DeckType.Hand;

        entry.effectHolder.zoneChanges.Add(drawCards);

        entry.targetConstraints.types.Add(ConstraintType.CurrentZone);
        entry.targetConstraints.types.Add(ConstraintType.Owner);

        entry.targetConstraints.owner = OwnerConstraints.Mine;
        entry.targetConstraints.currentZone.Add(DeckType.Grimoire);

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.NumberOfValidTargets;
            lta.numberofTargets = 1;
        }
    }

    private void DrawStatAdjustmentPreset(SpecialAbility entry) {
        entry.effect = EffectType.StatAdjustment;

        EffectStatAdjustment adj = new EffectStatAdjustment();
        adj.valueSetmethod = EffectStatAdjustment.ValueSetMethod.Manual;

        SpecialAbility.StatAdjustment newADJ = new SpecialAbility.StatAdjustment();

        adj.adjustments.Add(newADJ);

        entry.effectHolder.statAdjustments.Add(adj);
    }

    private void DrawDiscardPreset(SpecialAbility entry) {

        entry.effect = EffectType.ZoneChange;

        EffectZoneChange discard = new EffectZoneChange();
        discard.targetLocation = DeckType.SoulCrypt;

        entry.effectHolder.zoneChanges.Add(discard);

        entry.targetConstraints.types.Add(ConstraintType.CurrentZone);
        entry.targetConstraints.currentZone.Add(DeckType.Hand);
        entry.targetConstraints.types.Add(ConstraintType.Owner);
        entry.targetConstraints.owner = OwnerConstraints.Mine;

        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility lta = entry as LogicTargetedAbility;

            lta.logicTargetingMethod = LogicTargetedAbility.LogicTargeting.NumberOfValidTargets;
            lta.numberofTargets = 1;
        }
    }

    private void DrawSpawnTokenPreset(SpecialAbility entry) {
        entry.effect = EffectType.SpawnToken;

        EffectSpawnToken spawner = new EffectSpawnToken();

        spawner.spawnMethod = EffectSpawnToken.SpawnMethod.Basic;
        spawner.numberOfSpawns = 1;
        spawner.spawnCardType = CardType.Soul;
        spawner.spawnTokenLocation = DeckType.Battlefield;

        entry.effectHolder.tokenSpanws.Add(spawner);

    }

    private void DrawLightforgedPreset(SpecialAbility entry) {
        entry.abilityName = "Lightforged";

        entry.trigger.Add(AbilityActivationTrigger.EntersZone);
        entry.triggerConstraints.types.Add(ConstraintType.CurrentZone);
        entry.triggerConstraints.types.Add(ConstraintType.PreviousZone);
        entry.triggerConstraints.currentZone.Add(DeckType.SoulCrypt);
        entry.triggerConstraints.previousZone.Add(DeckType.Battlefield);
        entry.triggerConstraints.thisCardOnly = true;


        entry.effect = EffectType.GenerateResource;

        EffectGenerateResource genResource = new EffectGenerateResource();
        genResource.amount = 1;
        genResource.resourceType = GameResource.ResourceType.Hardlight;
        genResource.resourceName = "Hardlight";

        entry.effectHolder.generateResources.Add(genResource);


    }





    private SpecialAbility DrawSpecalAbilities(SpecialAbility entry) {

        GUIStyle boldRed = new GUIStyle(EditorStyles.boldLabel);
        boldRed.normal.textColor = Color.red;

        GUIStyle boldTeal = new GUIStyle(EditorStyles.boldLabel);
        boldTeal.normal.textColor = Color.cyan;

        //EditorGUILayout.BeginVertical();
        EditorHelper.DrawInspectorSectionFoldout(ref entry.togglePresets, "Presets", DrawPresets, entry);


        entry.abilityName = EditorGUILayout.TextField("Name of Ability (Optional) ", entry.abilityName);

        EditorGUILayout.LabelField("Start of " + entry.abilityName + " section", boldRed);

        //Trigger Logic
        //EditorGUILayout.BeginVertical();
        EditorHelper.DrawInspectorSectionFoldout(ref entry.toggleTriggerOptions, "Trigger Logic", DrawTriggerOptions, entry);

        EditorGUILayout.Separator();

        //Source of Effect Logic
        //EditorGUILayout.BeginVertical();
        EditorHelper.DrawInspectorSectionFoldout(ref entry.toggleSourceOptions, "Source Constraints", DrawSourceOptions, entry);

        EditorGUILayout.Separator();

        //Additional Requirements
        //EditorGUILayout.BeginVertical();
        EditorHelper.DrawInspectorSectionFoldout(ref entry.toggleAdditonalRequirementOptions, "Additional Requirements", DrawAdditionalRequirementsOptions, entry);

        EditorGUILayout.Separator();

        //Effect Logic
        //EditorGUILayout.BeginVertical();
        EditorHelper.DrawInspectorSectionFoldout(ref entry.toggleEffectOptions, "Effect", DrawEffectOptions, entry);
    
        EditorGUILayout.Separator();

        //Targeting Logic
        //EditorGUILayout.BeginVertical();
        EditorHelper.DrawInspectorSectionFoldout(ref entry.toggleTargetOptions, "Target Constraints", DrawTargetOptions, entry);

   

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("End of " + entry.abilityName + " section", boldRed);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        return entry;
    }



    private void DrawTriggerOptions(SpecialAbility entry) {

        //Trigger Logic
        EditorHelper.DrawInspectorSectionHeader("Effect Triggers:");

        entry.trigger = EditorHelper.DrawList("Triggers", entry.trigger, true, AbilityActivationTrigger.None, true, DrawActivationTrigger);
        entry.triggerConstraints.oncePerTurn = EditorGUILayout.Toggle("Can only trigger once per turn?", entry.triggerConstraints.oncePerTurn);
        entry.triggerConstraints.requireMultipleTriggers = EditorGUILayout.Toggle("Require Multiple Triggers?", entry.triggerConstraints.requireMultipleTriggers);
        if (entry.triggerConstraints.requireMultipleTriggers) {
            entry.triggerConstraints.triggersRequired = EditorGUILayout.IntField("How many?", entry.triggerConstraints.triggersRequired);
            entry.triggerConstraints.resetCountAtTurnEnd = EditorGUILayout.Toggle("Reset Counter On Turn End?", entry.triggerConstraints.resetCountAtTurnEnd);
        }
        entry.triggerDuration = EditorHelper.EnumPopup("Trigger Duration?", entry.triggerDuration);

        for (int i = 0; i < entry.trigger.Count; i++) {
            switch (entry.trigger[i]) {

                case AbilityActivationTrigger.CreatureStatChanged:
                    entry.triggerConstraints.statChanged = EditorHelper.EnumPopup("Which Stat Changed?", entry.triggerConstraints.statChanged);
                    entry.triggerConstraints.statGainedOrLost = EditorHelper.EnumPopup("Gained Or Lost?", entry.triggerConstraints.statGainedOrLost);
                    entry.processTriggerOnWhom = EditorHelper.EnumPopup("Process on Which card?", entry.processTriggerOnWhom);
                    EditorGUILayout.Separator();

                    if(!entry.triggerConstraints.thisCardAdjusted)
                        entry.triggerConstraints.thisCardAdjusts = EditorGUILayout.Toggle("This Card Caused Adjustment?", entry.triggerConstraints.thisCardAdjusts);

                    if (!entry.triggerConstraints.thisCardAdjusts)
                        entry.triggerConstraints.thisCardAdjusted = EditorGUILayout.Toggle("This Card Was Adjusted?", entry.triggerConstraints.thisCardAdjusted);

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

                case AbilityActivationTrigger.ResourceChanged:
                    entry.triggerConstraints.resourceThatChanged = EditorHelper.EnumPopup("Resource", entry.triggerConstraints.resourceThatChanged);
                    entry.triggerConstraints.resourceGainedOrLost = EditorHelper.EnumPopup("Gained or Lost?", entry.triggerConstraints.resourceGainedOrLost);

                    break;

                case AbilityActivationTrigger.Defends:
                    if(!entry.triggerConstraints.thisCardAttacks)
                        entry.triggerConstraints.thisCardDefends = EditorGUILayout.Toggle("This card is Defender?", entry.triggerConstraints.thisCardDefends);

                    if(!entry.triggerConstraints.thisCardDefends)
                        entry.triggerConstraints.thisCardAttacks = EditorGUILayout.Toggle("This card is Attacker?", entry.triggerConstraints.thisCardAttacks);

                    entry.processTriggerOnWhom = EditorHelper.EnumPopup("Process on Which card?", entry.processTriggerOnWhom);
                    break;

            }
        }

        EditorGUILayout.Separator();

        entry.triggerConstraints.thisCardOnly = EditorGUILayout.Toggle("Only this card can trigger this effect?", entry.triggerConstraints.thisCardOnly);
        entry.triggerConstraints.neverTargetSelf = EditorGUILayout.Toggle("This card CANNOT trigger this effect?", entry.triggerConstraints.neverTargetSelf);
        entry.triggerConstraints.types = EditorHelper.DrawList("Trigger Constraints", entry.triggerConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);
        for (int i = 0; i < entry.triggerConstraints.types.Count; i++) {
            ShowConstraintsOfType(entry.triggerConstraints.types[i], entry.triggerConstraints, "Trigger");

        }
        EditorHelper.DrawInspectorSectionFooter();


    }

    private void DrawSourceOptions(SpecialAbility entry) {
        EditorHelper.DrawInspectorSectionHeader("Source of Effect:");
        entry.sourceConstraints.types = EditorHelper.DrawList("Source Constraints", entry.sourceConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);
        for (int i = 0; i < entry.sourceConstraints.types.Count; i++) {
            ShowConstraintsOfType(entry.sourceConstraints.types[i], entry.sourceConstraints, "Source of this Effect");
        }

        EditorHelper.DrawInspectorSectionFooter();

    }

    private void DrawEffectOptions(SpecialAbility entry) {

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Draw Cards")) {
            DrawCardDrawPreset(entry);
        }

        if (GUILayout.Button("Discard Cards")) {
            DrawDiscardPreset(entry);
        }

        if (GUILayout.Button("Stat Adjustment")) {
            DrawStatAdjustmentPreset(entry);
        }

        if (GUILayout.Button("Spawn Token")) {
            DrawSpawnTokenPreset(entry);
        }
        EditorGUILayout.EndHorizontal();



        EditorHelper.DrawInspectorSectionHeader("Effect:");
        entry.abilityVFX = EditorGUILayout.TextField("Effect VFX Name", entry.abilityVFX);
        entry.movingVFX = EditorGUILayout.Toggle("Moving VFX?", entry.movingVFX);
        entry.effect = (Constants.EffectType)EditorGUILayout.EnumPopup(entry.effect);
        entry.effectDuration = EditorHelper.EnumPopup(" Effect Duration", entry.effectDuration);

        if(!entry.clearTriggeringTargetFromOtherAbility)
            entry.clearTargetsOnEffectComplete = EditorGUILayout.Toggle("Clear ALL targets after this effect?", entry.clearTargetsOnEffectComplete);


        if (!entry.clearTargetsOnEffectComplete) {
            entry.clearTriggeringTargetFromOtherAbility = EditorGUILayout.Toggle("Clear THIS target from other Ability targets?", entry.clearTriggeringTargetFromOtherAbility);

            if(entry.clearTriggeringTargetFromOtherAbility)
                entry.triggerConstraints.abilityToGatherTargetsFrom = EditorGUILayout.TextField("Ability Name", entry.triggerConstraints.abilityToGatherTargetsFrom);
        }


        switch (entry.effect) {
            case EffectType.SpawnToken:
                entry.effectHolder.tokenSpanws = EditorHelper.DrawExtendedList("Spawn Token Effects", entry.effectHolder.tokenSpanws, "Spawn Token", DrawEffectList);

                break;

            case EffectType.ZoneChange:
                entry.effectHolder.zoneChanges = EditorHelper.DrawExtendedList("Zone Change Effect", entry.effectHolder.zoneChanges, "Zone Change", DrawEffectList);

                break;

            case EffectType.AddOrRemoveKeywordAbilities:
                //entry.keywordsToAddorRemove = EditorHelper.DrawList("Keywords", entry.keywordsToAddorRemove, true, Keywords.None, true, DrawKeywords);

                entry.effectHolder.addOrRemoveKeywords = EditorHelper.DrawExtendedList("Add or Remove Keyword Effect", entry.effectHolder.addOrRemoveKeywords, "Keyword", DrawEffectList);

                break;

            case EffectType.GenerateResource:
                entry.effectHolder.generateResources = EditorHelper.DrawExtendedList("Resource Generation Effect", entry.effectHolder.generateResources, "Generate Resource", DrawEffectList);

                break;

            case EffectType.StatAdjustment:
                entry.effectHolder.statAdjustments = EditorHelper.DrawExtendedList("Stat Adjustment Effects", entry.effectHolder.statAdjustments, "Stat Adjustments", DrawEffectList);

                break;

            case EffectType.AddOrRemoveSpecialAttribute:
                entry.effectHolder.addOrRemoveSpecialAttribute = EditorHelper.DrawExtendedList("Special Attribute Effects", entry.effectHolder.addOrRemoveSpecialAttribute, "Special Attribute", DrawEffectList);

                break;

            case EffectType.ChooseOne:
                entry.effectHolder.chooseOne = EditorHelper.DrawExtendedList("Choose One Effects", entry.effectHolder.chooseOne, "Choose One", DrawEffectList);

                break;

            case EffectType.BestowAbility:
                entry.effectHolder.bestowAbility = EditorHelper.DrawExtendedList("Bestow Ability Effects", entry.effectHolder.bestowAbility, "Bestow Ability", DrawEffectList);

                break;

            case EffectType.RemoveOtherEffect:
                entry.targetConstraints.abilityToRemove = EditorGUILayout.TextField("Name of Ability to Remove", entry.targetConstraints.abilityToRemove);
                break;

            case EffectType.RetriggerOtherEffect:
                entry.targetConstraints.abilityToRetrigger = EditorGUILayout.TextField("Name of Ability to Trigger", entry.targetConstraints.abilityToRetrigger);

                break;

        }//End of Effects

        EditorHelper.DrawInspectorSectionFooter();
    }

    private void DrawTargetOptions(SpecialAbility entry) {
        EditorHelper.DrawInspectorSectionHeader("Target Constraints:");

        if (!entry.targetConstraints.neverTargetSelf)
            entry.targetConstraints.thisCardOnly = EditorGUILayout.Toggle("This card only targets itself?", entry.targetConstraints.thisCardOnly);

        if (!entry.targetConstraints.thisCardOnly)
            entry.targetConstraints.neverTargetSelf = EditorGUILayout.Toggle("This card can't target itself?", entry.targetConstraints.neverTargetSelf);

        //entry.targetConstraints.targetAdjacency = EditorGUILayout.Toggle("Include Adjacent Targets on Battlefield?", entry.targetConstraints.targetAdjacency);


        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Souls On Field")) {
            DrawTargetSoulsOnBattlefieldPreset(entry);
        }

        if (GUILayout.Button("My General")) {
            DrawTargetOwnGeneralPreset(entry);
        }

        EditorGUILayout.EndHorizontal();



        if (entry is LogicTargetedAbility) {
            LogicTargetedAbility logicTargeted = entry as LogicTargetedAbility;

            logicTargeted.logicTargetingMethod = EditorHelper.EnumPopup("Targeting Method", logicTargeted.logicTargetingMethod);

            switch (logicTargeted.logicTargetingMethod) {
                case LogicTargetedAbility.LogicTargeting.NumberOfValidTargets:
                    logicTargeted.numberofTargets = EditorGUILayout.IntField("Number of Targets", logicTargeted.numberofTargets);
                    break;

                case LogicTargetedAbility.LogicTargeting.UseTargetsFromOtherAbility:
                    logicTargeted.targetAbilityName = EditorGUILayout.TextField("Ability Name", logicTargeted.targetAbilityName);
                    break;

                case LogicTargetedAbility.LogicTargeting.AdjacentToTarget:
                    logicTargeted.targetAbilityName = EditorGUILayout.TextField("Ability Name", logicTargeted.targetAbilityName);
                    break;
            }



            //if (logicTargeted.logicTargetingMethod == LogicTargetedAbility.LogicTargeting.NumberOfValidTargets) {

            //}

            //if (logicTargeted.logicTargetingMethod == LogicTargetedAbility.LogicTargeting.UseTargetsFromOtherAbility) {

            //}
        }

        entry.targetConstraints.types = EditorHelper.DrawList("Target Constraints", entry.targetConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);


        for (int i = 0; i < entry.targetConstraints.types.Count; i++) {
            ShowConstraintsOfType(entry.targetConstraints.types[i], entry.targetConstraints, "Target");
        }


        EditorGUILayout.Separator();
        EditorHelper.DrawInspectorSectionHeader("Manual Triggers:");
        entry.manualTriggerAbilityNames = EditorHelper.DrawList("Manual Triggers", entry.manualTriggerAbilityNames, true, "", true, DrawListOfStrings);

        EditorHelper.DrawInspectorSectionFooter();
        EditorGUILayout.Separator();

        EditorHelper.DrawInspectorSectionFooter();

    }

    private void DrawAdditionalRequirementsOptions(SpecialAbility entry) {
        EditorHelper.DrawInspectorSectionHeader("Additional Requirements:");

        entry.additionalRequirements = EditorHelper.DrawList("Requirement", entry.additionalRequirements, true, Constants.AdditionalRequirement.None, true, DrawAdditionalRequirements);

        for (int i = 0; i < entry.additionalRequirements.Count; i++) {
            switch (entry.additionalRequirements[i]) {
                case Constants.AdditionalRequirement.NumberofCardsInZone:
                    entry.additionalRequirementConstraints.zoneToCheckForNumberOfCards = EditorHelper.EnumPopup("Zone to Check", entry.additionalRequirementConstraints.zoneToCheckForNumberOfCards);
                    entry.additionalRequirementConstraints.numberOfcardsInZone = EditorGUILayout.IntField("How many?", entry.additionalRequirementConstraints.numberOfcardsInZone);
                    entry.additionalRequirementConstraints.moreOrLessCards = EditorHelper.EnumPopup("At Least Or Less Than?", entry.additionalRequirementConstraints.moreOrLessCards);

                    EditorGUILayout.Separator();
                    entry.additionalRequirementConstraints.types = EditorHelper.DrawList("Number of cards Constraint", entry.additionalRequirementConstraints.types, true, ConstraintType.None, true, DrawConstraintTypes);

                    for (int j = 0; j < entry.additionalRequirementConstraints.types.Count; j++) {
                        ShowConstraintsOfType(entry.additionalRequirementConstraints.types[j], entry.additionalRequirementConstraints, "Extra Requirement");
                    }

                    break;

                case Constants.AdditionalRequirement.RequireResource:
                    entry.additionalRequirementConstraints.requiredResourceType = EditorHelper.EnumPopup("What kind of Resource?", entry.additionalRequirementConstraints.requiredResourceType);
                    entry.additionalRequirementConstraints.moreOrlessResource = EditorHelper.EnumPopup("More or less?", entry.additionalRequirementConstraints.moreOrlessResource);
                    entry.additionalRequirementConstraints.amountOfResourceRequried = EditorGUILayout.IntField("How Much?", entry.additionalRequirementConstraints.amountOfResourceRequried);
                    entry.additionalRequirementConstraints.consumeResource = EditorGUILayout.Toggle("Consume this resource?", entry.additionalRequirementConstraints.consumeResource);


                    break;
            }
        }


        EditorHelper.DrawInspectorSectionFooter();

    }



    private void DrawPresets(SpecialAbility entry) {
        GUIStyle boldRed = new GUIStyle(EditorStyles.boldLabel);
        boldRed.normal.textColor = Color.red;

        EditorGUILayout.LabelField("Presets", boldRed);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Inspire")) {
            DrawInspirePreset(entry);
        }

        if (GUILayout.Button("Finale")) {
            DrawFinalePreset(entry);
        }

        if (GUILayout.Button("Deathwatch")) {
            DrawDeathwatchPreset(entry);
        }

        if (GUILayout.Button("Bloodthirst")) {
            DrawBloodthirstPreset(entry);
        }

        if (GUILayout.Button("Spell")) {
            DrawSendToCryptPreset(entry);
        }

        if (GUILayout.Button("Exhaust Ability")) {
            DrawExaustAbilityPreset(entry);
        }

        if (GUILayout.Button("Exhaust Self")) {
            DrawExhaustSelfPreset(entry);
        }

        if (GUILayout.Button("Artifact")) {
            DrawArtifactPreset(entry);
        }

        EditorGUILayout.EndHorizontal();

        //Second Row
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("PhalanxInspire")) {
            DrawPhalanxInspirePreset(entry);
        }

        if (GUILayout.Button("PhalanxOthersEnter")) {
            DrawPhalanxOtherEnterPreset(entry);
        }

        if (GUILayout.Button("Lightforged")) {
            DrawLightforgedPreset(entry);
        }

        EditorGUILayout.EndHorizontal();

    }


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

            case ConstraintType.SpecialAttribute:
                EditorGUILayout.Separator();

                entry.notSpecialAttribute = EditorGUILayout.Toggle("Not?", entry.notSpecialAttribute);

                EditorGUILayout.LabelField("Which Special Attributes are" + ShowNot(entry.notSpecialAttribute) + "valid for the " + constraintName + " ?", EditorStyles.boldLabel);

                entry.specialAttributes = EditorHelper.DrawList("Special Attribute", entry.specialAttributes, true, SpecialAttribute.AttributeType.None, true, DrawSpecialAttributeTypes);

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

            case ConstraintType.OtherTargets:
                EditorGUILayout.Separator();

                EditorGUILayout.LabelField("Does the " + constraintName + " exist in another ability's target list?", EditorStyles.boldLabel);
                entry.abilityToGatherTargetsFrom = EditorGUILayout.TextField("Ability Name", entry.abilityToGatherTargetsFrom);
                break;

            case ConstraintType.CanAttack:
                EditorGUILayout.Separator();
                EditorGUILayout.LabelField("Can the " + constraintName + " attack?", EditorStyles.boldLabel);
                entry.creatureCanAttack = EditorGUILayout.Toggle("Can Attack", entry.creatureCanAttack);
                break;

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

    private SpecialAttribute.AttributeType DrawSpecialAttributeTypes(List<SpecialAttribute.AttributeType> list, int index) {
        SpecialAttribute.AttributeType result = EditorHelper.EnumPopup("Special Attributes", list[index]);
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
