using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    private GameObject uiPrefab;
    private GameObject canvas;
	// Use this for initialization
    void Awake(){
        canvas = transform.GetChild(1).gameObject;
        uiPrefab = canvas.transform.GetChild(0).gameObject;
        uiPrefab.SetActive(false);
        canvas.SetActive(true);
    }

	void Start () {
       // NewUiObj();

    }
	
	// Update is called once per frame
	void FixedUpdate() {
        Debug.Log("??");
	}

   private GameObject NewUiObj(string _message, _MessageType type)
    {
        GameObject obj = Instantiate(uiPrefab, uiPrefab.transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = transform.GetChild(1).transform;
        obj.transform.localScale = uiPrefab.transform.localScale;

        if(type == _MessageType.Damage)
        {
            obj.GetComponent<UnityEngine.UI.Text>().text = "-" + _message;
        }

        obj.SetActive(true);
        return obj;
    }

    public void showUi(Character_ShowUI _ui)
    {
        NewUiObj(_ui.message,_ui.type);
    }

}
