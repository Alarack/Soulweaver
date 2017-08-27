using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserDeckButton : MonoBehaviour {

    public Text deckNameText;

    private DeckSelector _deckSelector;
    private DeckBuilder.DeckData _deckData;

    public void Initialize(DeckSelector parent, DeckBuilder.DeckData deckData) {
        _deckSelector = parent;
        _deckData = deckData;

        deckNameText.text = _deckData.deckName;
    }

    public void OnClick() {
        _deckSelector.AssignCustom1Deck(_deckData);
    }


}
