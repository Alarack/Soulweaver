using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Keywords = Constants.Keywords;

[System.Serializable]
public class EffectAddorRemoveKeywords : Effect {

    public enum AddOrRemove {
        Add,
        Remove
    }


    public List<Keywords> keywords = new List<Keywords>();
    public AddOrRemove addOrRemove;

    public override void Apply(CardVisual target) {
        //Debug.Log("Add Or Remove Keywords Apply method");

        switch (addOrRemove) {
            case AddOrRemove.Add:
                AddKeywords(target);
                break;

            case AddOrRemove.Remove:
                RemoveKeywords(target);
                break;
        }
    }

    public override void Remove(CardVisual target) {
        switch (addOrRemove) {
            case AddOrRemove.Remove:
                AddKeywords(target);
                break;

            case AddOrRemove.Add:
                RemoveKeywords(target);
                break;
        }
    }


    private void AddKeywords(CardVisual target) {
        for (int i = 0; i < keywords.Count; i++) {

            //Debug.Log("adding " + keywords[i].ToString() + " to " + target.gameObject.name);

            target.RPCAddKeyword(PhotonTargets.All, keywords[i], true);
        }
    }


    private void RemoveKeywords(CardVisual target) {
        for (int i = 0; i < keywords.Count; i++) {
            target.RPCAddKeyword(PhotonTargets.All, keywords[i], false);
        }
    }



}
