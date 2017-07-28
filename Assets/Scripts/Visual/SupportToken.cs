using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupportToken : BoardToken {


    public Text supportText;
    public GameObject supportTokenGlow;

    private CardSupportData _supportData;


    public override void Initialize(CardData data, CardVisual card) {
        base.Initialize(data, card);

        _supportData = parentCardData as CardSupportData;
    }

    public void SetUpSupportText() {
        if(_supportData.supportValue != 0)
            supportText.text = _supportData.supportValue.ToString();
        else {
            supportText.gameObject.SetActive(false);
        }
    }

    public void UpdateSupportText(Constants.CardStats statToUpdate, int value) {
        if (_supportData.supportValue == 0)
            return;


        switch (statToUpdate) {
            case Constants.CardStats.SupportValue:
                TextTools.AlterTextColor(value, _supportData.supportValue, supportText);
                supportText.text = value.ToString();
                break;
        }
    }

}
