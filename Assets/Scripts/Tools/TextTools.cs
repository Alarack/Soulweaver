using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TextTools {


    public static void AlterTextColor(int value, int initValue, Text text) {
        if (value > initValue)
            text.color = Color.green;
        else if (value < initValue)
            text.color = Color.red;
        else
            text.color = Color.white;
    }



}
