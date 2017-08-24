using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalSettings : MonoBehaviour {

    public static GlobalSettings _globalSettings;

    [Header("Prefabs and Assets")]
    public GameObject creatureCard;
    public GameObject spellCard;
    //public GameObject relicCard;
    public GameObject supportCard;
    public GameObject playerCard;
    public GameObject domainCard;
    [Header("Deckbuilder")]
    public GameObject spellDeckBuilder;
    public GameObject creatureDeckbuilder;
    public GameObject supportDeckbuilder;

    void Awake() {
        _globalSettings = this;
    }

}
