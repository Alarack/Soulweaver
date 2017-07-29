using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Keyword {

    public Constants.Keywords keywordType;
    public int value;


    public Keyword() {

    }

    public Keyword(Constants.Keywords type, int value) {
        keywordType = type;
        this.value = value;
    }



}
