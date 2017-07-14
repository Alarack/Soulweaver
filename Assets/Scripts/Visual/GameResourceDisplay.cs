using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResourceDisplay : Photon.MonoBehaviour {


    public List<ResourceDisplayInfo> resourceDisplayInfo;

    public GameObject textTemplate;

    public GameObject canvas;

    private Player owner;


	public void Initialize (Player owner, GameResource essence) {
        this.owner = owner;

        GameObject essenceTextGO = Instantiate(textTemplate) as GameObject;
        essenceTextGO.transform.SetParent(canvas.transform, false);
        Text essenceText = essenceTextGO.GetComponent<Text>();
        essenceTextGO.SetActive(true);

        ResourceDisplayInfo firstResource = new ResourceDisplayInfo(essence, essenceText, true);
        resourceDisplayInfo.Add(firstResource);

	}

	void Update () {
		
	}



    public void UpdateResourceText(GameResource resource, string value) {

        for(int i = 0; i < resourceDisplayInfo.Count; i++) {
            if(resourceDisplayInfo[i].resource == resource) {
                resourceDisplayInfo[i].resourceText.text = value;
                break;
            }
        }

    }










    [System.Serializable]
    public class ResourceDisplayInfo {
        public GameResource resource;
        public Text resourceText;
        public bool addPerTurn;

        public ResourceDisplayInfo(GameResource resource, Text textField, bool addPerTurn) {
            this.resource = resource;
            this.resourceText = textField;
            this.addPerTurn = addPerTurn;

        }

    }
}
