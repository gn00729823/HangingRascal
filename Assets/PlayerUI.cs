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
    }

	void Start () {
        NewUiObj();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

   private GameObject NewUiObj()
    {
        GameObject obj = Instantiate(uiPrefab, uiPrefab.transform.position, Quaternion.identity) as GameObject;
        obj.transform.parent = transform.GetChild(1).transform;
        obj.transform.localScale = uiPrefab.transform.localScale;
        obj.SetActive(true);
        return obj;
    }


}
