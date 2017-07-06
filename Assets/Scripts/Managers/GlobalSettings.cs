using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour {

    public static GlobalSettings _globalSettings;

    [Header("Prefabs and Assets")]
    public GameObject creatureCard;
    public GameObject spellCard;
    public GameObject relicCard;
    public GameObject artifactCard;
    public GameObject playerCard;
    public GameObject domainCard;



    void Awake() {
        _globalSettings = this;
    }

}
