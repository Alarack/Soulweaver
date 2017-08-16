using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDFactory  {


    public static int uniqueAdjID = 0;
    public static int uniqueAdjID2 = 0;



    public static int uniqueAttID = 0;
    public static int uniqueAttID2 = 0;


    public static int GenerateAdjID(Player player) {

        if (player.player2) {
            //Debug.Log(uniqueAdjID2.ToString() + " is an adj ID for player 2");

            uniqueAdjID2++;

            return uniqueAdjID2;
        }
        else {

            //Debug.Log(uniqueAdjID.ToString() + " is an adj ID for player 1");

            uniqueAdjID++;

            return uniqueAdjID;

        }
    }

    public static int GenerateAttID(Player player) {

        if (player.player2) {
            uniqueAttID2++;

            return uniqueAttID2;
        }
        else {
            uniqueAttID++;

            return uniqueAttID;
        }

    }



}
