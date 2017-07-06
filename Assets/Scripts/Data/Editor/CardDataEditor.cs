using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardData))]
public class CardDataEditor : Editor {

    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();

        CardData _cardData = (CardData)target;

        EditorGUILayout.LabelField("Card Info", EditorStyles.boldLabel);

        _cardData.cardID = (CardIDs.CardID)EditorGUILayout.EnumPopup("CardID", _cardData.cardID);
        _cardData.cardName = EditorGUILayout.TextField("Card Name", _cardData.cardName);
        _cardData.cardText = EditorGUILayout.TextField("Card Text", _cardData.cardText);
        _cardData.cardCost = EditorGUILayout.IntField("Card Cost", _cardData.cardCost);

        _cardData.cardImage = EditorHelper.ObjectField<Sprite>("Card Image", _cardData.cardImage);

        _cardData.primaryCardType = (Constants.CardType)EditorGUILayout.EnumPopup("Primary Type", _cardData.primaryCardType);
        _cardData.otherCardTypes = EditorHelper.DrawList("Other Card Types", _cardData.otherCardTypes, true, Constants.CardType.None, true, DrawCardTypes);
        _cardData.subTypes = EditorHelper.DrawList("Subtypes", _cardData.subTypes, true, Constants.SubTypes.None, true, DrawSubtypes);
        _cardData.attunements = EditorHelper.DrawList("Attunements", _cardData.attunements, true, Constants.Attunements.None, true, DrawAttunements);
        _cardData.keywords = EditorHelper.DrawList("Keywords", _cardData.keywords, true, Constants.Keywords.None, true, DrawKeywords);

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("Special Abilities", EditorStyles.boldLabel);
        _cardData.specialAbilities = EditorHelper.DrawExtendedList("Targted Effects", _cardData.specialAbilities, "Ability", DrawSpecialAbilities);
        //_cardData.specialAbilities = EditorHelper.DrawExtendedList("Targeted Effects", _cardData.specialAbilities, "Ability", DrawSpecialAbilities, AddSpecialAbility);

        EditorUtility.SetDirty(target);

        DrawDefaultInspector();
    }



    private void AddSpecialAbility(List<EffectOnTarget> list, int index) {

    }

    private EffectOnTarget DrawSpecialAbilities(EffectOnTarget entry) {
        entry.effect = (Constants.EffectType)EditorGUILayout.EnumPopup("Effect", entry.effect);

        entry.statAdjustments = EditorHelper.DrawExtendedList("Stat Adjustments", entry.statAdjustments, "Stat Adjustment", DrawStatAdjustments);

        return entry;
    }

    private SpecialAbility.StatAdjustment DrawStatAdjustments(SpecialAbility.StatAdjustment entry) {
        entry.stat = (Constants.CardStats)EditorGUILayout.EnumPopup("Stat", entry.stat);
        entry.value = EditorGUILayout.IntField("Value", entry.value);


        return entry;
    }


    private Constants.Attunements DrawAttunements(List<Constants.Attunements> list, int index) {
        Constants.Attunements result = (Constants.Attunements)EditorGUILayout.EnumPopup("Attunement", list[index]);
        return result;
    }

    private Constants.CardType DrawCardTypes(List<Constants.CardType> list, int index) {
        Constants.CardType result = (Constants.CardType)EditorGUILayout.EnumPopup("Other Card Type", list[index]);
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


}
