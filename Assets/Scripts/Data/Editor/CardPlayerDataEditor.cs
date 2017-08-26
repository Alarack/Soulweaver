using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardPlayerData))]
public class CardPlayerDataEditor : CardDataEditor {

    public override void OnInspectorGUI() {

        CardPlayerData _playerData = (CardPlayerData)target;

        EditorGUILayout.LabelField("General Domains", EditorStyles.boldLabel);
        _playerData.domainPowers = EditorHelper.DrawList("Domains", _playerData.domainPowers, true, null, true, DrawDomainPowers);

        EditorGUILayout.Separator();

        EditorGUILayout.LabelField("Stats", EditorStyles.boldLabel);
        _playerData.attack = EditorGUILayout.IntField("Attack", _playerData.attack);
        _playerData.size = EditorGUILayout.IntField("Size", _playerData.size);
        _playerData.health = EditorGUILayout.IntField("Health", _playerData.health);


        base.OnInspectorGUI();
    }



    private CardDomainData DrawDomainPowers (List<CardDomainData> domainData, int index) {
        CardDomainData result = EditorHelper.ObjectField<CardDomainData>("Domain", domainData[index]);
        return result;
    }

}
