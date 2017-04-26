using UnityEngine;
using System.Collections;


public class UI_PlayerHpMove : MonoBehaviour {
    private GameObject hp_obj ;
    private float MaxPosX = -3.4f;
    private float MixPosX = -91f;
    private float OneChagePosX;
    private BattleSpriteAction myTarget;
	// Use this for initialization
    void Awake()
    {
        myTarget = transform.parent.parent.gameObject.GetComponent<BattleSpriteAction>();
        hp_obj = this.transform.FindChild("_hp").gameObject;
        OneChagePosX = (Mathf.Abs(MixPosX) - Mathf.Abs(MaxPosX))/100;
        
      
    }

	void Start () {
        if (!myTarget.isMainPlayer)
        {
            Destroy(this.gameObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
        hp_obj.transform.localPosition = new Vector3(MixPosX + (myTarget.hp*OneChagePosX),0.4f,0);
	}
}
