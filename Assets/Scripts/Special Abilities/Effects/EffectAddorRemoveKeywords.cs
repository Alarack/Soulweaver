using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Keywords = Constants.Keywords;

public class EffectAddorRemoveKeywords : Effect {

    public enum AddOrRemove {
        Add,
        Remove
    }


    public List<Keywords> keywords = new List<Keywords>();
    public AddOrRemove addOrRemove;

    public override void Apply(CardVisual target) {
        switch (addOrRemove) {
            case AddOrRemove.Add:
                AddKeywords(target);
                break;

            case AddOrRemove.Remove:
                RemoveKeywords(target);
                break;
        }
    }


    private void AddKeywords(CardVisual target) {
        for (int i = 0; i < keywords.Count; i++) {
            target.RPCAddKeyword(PhotonTargets.All, keywords[i], true);
        }
    }


    private void RemoveKeywords(CardVisual target) {
        for (int i = 0; i < keywords.Count; i++) {
            target.RPCAddKeyword(PhotonTargets.All, keywords[i], false);
        }
    }



}
