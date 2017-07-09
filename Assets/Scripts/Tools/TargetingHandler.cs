using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TargetingHandler {

    public static List<TargetingInfo> targetInfo = new List<TargetingInfo>();
 

    public static void AddTargetInfo(TargetingInfo info) {
        targetInfo.Add(info);
    }

    public static void RemoveTargetInfo(TargetingInfo info) {
        if (targetInfo.Contains(info)) {
            targetInfo.Remove(info);
        }
    }

    public static void ClearTargetInfo() {
        targetInfo.Clear();
    }



    public static void CreateTargetInfoListing(CardVisual source, List<CardVisual> targets = null) {
        TargetingInfo newInfo = new TargetingInfo();
        newInfo.source = source;

        if (targets != null)
            newInfo.targets = targets;

        AddTargetInfo(newInfo);

        //return newInfo;
    }

    public static void CreateTargetInfoListing(CardVisual source, CardVisual target = null) {
        TargetingInfo newInfo = new TargetingInfo();
        newInfo.source = source;

        if (target != null)
            newInfo.targets.Add(target);

        AddTargetInfo(newInfo);

        //return newInfo;
    }


    public static void AddTargetsToExistingInfo(CardVisual source, List<CardVisual> targets) {
        for(int i = 0; i < targetInfo.Count; i++) {
            if(targetInfo[i].source == source) {
                targetInfo[i].targets.AddRange(targets);
                break;
            }
        }

    }









    [System.Serializable]
    public class TargetingInfo {
        public CardVisual source;
        public List<CardVisual> targets = new List<CardVisual>();
    }
}