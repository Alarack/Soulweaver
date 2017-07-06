using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDB : MonoBehaviour {

    public static CardDB cardDB;

    public CardData[] allCardData;



    void Awake() {
        PopulateDatabase();

        cardDB = this;
    }

    private void PopulateDatabase() {
         allCardData = Resources.LoadAll<CardData>("CardData");
    }




}
