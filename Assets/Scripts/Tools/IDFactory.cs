using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class IDFactory  {


    public static int uniqueAdjID = 0;
    public static int uniqueAttID = 0;


    public static int GenerateAdjID() {

        //Debug.Log(uniqueAdjID.ToString() + " is the global id for adjustments before incramenting");

        uniqueAdjID++;

        return uniqueAdjID;
    }

    public static int GenerateAttID() {

        //Debug.Log(uniqueAttID.ToString() + " is the global id for Attributes before incramenting");

        uniqueAttID++;

        return uniqueAttID;
    }



}
