using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CardDomainData))]
public class CardDomainEditor : CardDataEditor {

    public override void OnInspectorGUI() {


        CardDomainData _domainData = (CardDomainData)target;

        EditorGUILayout.LabelField("Domain Fields", EditorStyles.boldLabel);
        _domainData.domainIcon = EditorHelper.ObjectField<Sprite>("Domain Icon", _domainData.domainIcon);


        base.OnInspectorGUI();
    }



}
