using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardCreatureData))]
public class CardCreatureDataEditor : CardDataEditor {

    public override void OnInspectorGUI() {
        

        CardCreatureData _creatureData = (CardCreatureData)target;

        EditorGUILayout.LabelField("Creature Fields", EditorStyles.boldLabel);
        _creatureData.attack = EditorGUILayout.IntField("Attack", _creatureData.attack);
        _creatureData.size = EditorGUILayout.IntField("Size", _creatureData.size);
        _creatureData.health = EditorGUILayout.IntField("Health", _creatureData.health);


        base.OnInspectorGUI();

    }


}
