using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardSupportData))]
public class CardSupportDataEditor : CardDataEditor {

    public override void OnInspectorGUI() {


        CardSupportData _supportData = (CardSupportData)target;

        EditorGUILayout.LabelField("Support Fields", EditorStyles.boldLabel);
        _supportData.supportValue = EditorGUILayout.IntField("Value", _supportData.supportValue);



        base.OnInspectorGUI();

    }
}
