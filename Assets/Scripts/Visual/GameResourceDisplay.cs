using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameResourceDisplay : Photon.MonoBehaviour {


    public List<ResourceDisplayInfo> resourceDisplayInfo;

    public GameObject textTemplate;

    public GameObject canvas;

    public Player owner;


	public void Initialize (Player owner, GameResource essence, Text newText) {
        this.owner = owner;

        //GameObject essenceTextGO = Instantiate(textTemplate) as GameObject;
        //essenceTextGO.transform.SetParent(canvas.transform, false);
        //Text essenceText = essenceTextGO.GetComponent<Text>();
        //essenceTextGO.SetActive(true);

        newText.transform.SetParent(canvas.transform, false);


        ResourceDisplayInfo firstResource = new ResourceDisplayInfo(essence, newText, true);
        resourceDisplayInfo.Add(firstResource);

    }

	void Update () {
		
	}

    public void AddNewResource(GameResource resource, Text textField, bool addPerTurn) {

        textField.transform.SetParent(canvas.transform, false);

        ResourceDisplayInfo newResource = new ResourceDisplayInfo(resource, textField, addPerTurn);
        resourceDisplayInfo.Add(newResource);

        UpdateResourceText(newResource.resource, newResource.resource.currentValue.ToString());
    }



    public void UpdateResourceText(GameResource resource, string value) {

        for(int i = 0; i < resourceDisplayInfo.Count; i++) {
            if(resourceDisplayInfo[i].resource == resource) {
                resourceDisplayInfo[i].resourceText.text = resource.resourceName + " " + value;
                break;
            }
        }

    }


    public int GetCurrentResourceValueByType(GameResource.ResourceType type) {
        for(int i = 0; i < resourceDisplayInfo.Count; i++) {
            if(resourceDisplayInfo[i].resource.resourceType == type) {
                return resourceDisplayInfo[i].resource.currentValue;
            }
        }

        return 0;
    }


    #region RPCs



    public void RPCAddResource(PhotonTargets targets, GameResource.ResourceType type, int value) {
        int resourceTypeEnum = (int)type;
        photonView.RPC("AddResource", targets, resourceTypeEnum, value);
    }

    [PunRPC]
    public void AddResource(int resourceTypeEnum, int value) {

        GameResource.ResourceType resourceType = (GameResource.ResourceType)resourceTypeEnum;

        for (int i = 0; i < resourceDisplayInfo.Count; i++) {
            if(resourceDisplayInfo[i].resource.resourceType == resourceType) {

                resourceDisplayInfo[i].resource.AddResource(value);
                break;
            }
        }
    }

    public void RPCRemoveResource(PhotonTargets targets, GameResource.ResourceType type, int value) {
        int resourceTypeEnum = (int)type;
        photonView.RPC("RemoveResource", targets, resourceTypeEnum, value);

    }

    [PunRPC]
    public void RemoveResource(int resourceTypeEnum, int value) {

        GameResource.ResourceType resourceType = (GameResource.ResourceType)resourceTypeEnum;

        for (int i = 0; i < resourceDisplayInfo.Count; i++) {
            if (resourceDisplayInfo[i].resource.resourceType == resourceType) {

                resourceDisplayInfo[i].resource.RemoveResource(value);
                break;
            }
        }
    }
    

    public void RPCUpdateResourceText(PhotonTargets targets, GameResource.ResourceType resourceToUpdate) {
        int resourceTypeEnum = (int)resourceToUpdate;
        photonView.RPC("UpdateResourceText", targets, resourceTypeEnum);
    }

    [PunRPC]
    public void UpdateResourceText(int resourceTypeEnum) {
        GameResource.ResourceType resourceType = (GameResource.ResourceType)resourceTypeEnum;

        //Debug.Log(resourceType.ToString() + " has been sent to update resource text");

        for (int i = 0; i < resourceDisplayInfo.Count; i++) {
            if (resourceDisplayInfo[i].resource.resourceType == resourceType) {

                resourceDisplayInfo[i].resource.UpdateText();
                break;
            }
        }



    }



    #endregion







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
