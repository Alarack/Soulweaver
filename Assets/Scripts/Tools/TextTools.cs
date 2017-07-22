using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TextTools {


    public static void AlterTextColor(int value, int initValue, Text text, bool reverse = false) {

        if (!reverse) {
            if (value > initValue)
                text.color = Color.green;
            else if (value < initValue)
                text.color = Color.red;
            else
                text.color = Color.white;
        }
        else {
            if (value < initValue)
                text.color = Color.green;
            else if (value < initValue)
                text.color = Color.red;
            else
                text.color = Color.white;
        }


    }

    public static void SetTextColor(Text text, Color color) {
        text.color = color;
    }



}
