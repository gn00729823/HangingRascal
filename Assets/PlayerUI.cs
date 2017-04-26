using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    private GameObject uiPrefab;
    private GameObject canvas;

    private List<playerUiObj> UIObjList = new List<playerUiObj>();
    //private Dictionary<string, playerUiObj> UIObjList = new Dictionary<string, playerUiObj>();
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
       if(UIObjList.Count!=0)
        {
            foreach (playerUiObj obj in UIObjList)
            {
                if(obj.moveRange!=0)
                {               
                    obj.moveRange--;
                    float y = obj.UIobj.transform.localPosition.y +3;
                    float x = obj.UIobj.transform.localPosition.x;
                    obj.UIobj.transform.localPosition = new Vector3(x,y,0);
                }
                else 
                {
                    Destroy(obj.UIobj);                                   
                    obj.needDestroy = true;
                }
            }
            int listCount = UIObjList.Count;
            if(listCount!=0)
            {
                for (int i = 0; i < listCount; i++)
                {
                    if (UIObjList[i].needDestroy)
                    {
                        UIObjList.RemoveAt(i);
                        listCount--;
                    }
                }
            }
        }     
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
        playerUiObj obj = new playerUiObj();
        obj.moveRange = 50;
        obj.UIobj = NewUiObj(_ui.message,_ui.type);
        float x = obj.UIobj.transform.localPosition.x;
        float y = obj.UIobj.transform.localPosition.y;
        obj.UIobj.transform.localPosition = new Vector3(x+=Random.Range(-100,100),y+= Random.Range(-25, 25),0);
        UIObjList.Add(obj);
    }

    [ContextMenu("Do testUi")]
    public void testUi()
    {
        Character_ShowUI ui = new Character_ShowUI("123","100",_MessageType.Damage);
        showUi(ui);
        
    }

}

class playerUiObj
{
    public float moveRange;
    public GameObject UIobj;
    public bool needDestroy=false;
}