using UnityEngine;
using System.Collections;

public class GlobalVarDisplayer : MonoBehaviour {
	public bool LoginStatus = false;
	public string UniqueID = "";
	public string AccountID = "";
	public string NickName = "";
	public double Money = 0;
	public  int Status = 0;
	public int RoomLevel = 0;
	public string RoomName = "";
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		LoginStatus = MemberGlobal.LoginStatus;
		UniqueID = MemberGlobal.UniqueID;
		AccountID = MemberGlobal.AccountID;
		NickName = MemberGlobal.NickName;
		Money = MemberGlobal.Money;
		Status = MemberGlobal.Status;
	}
}
