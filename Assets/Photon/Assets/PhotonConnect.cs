using UnityEngine;
using System.Collections;

public class PhotonConnect : MonoBehaviour {

	public string ServerIP = "10.211.55.5";
	public int ServerPort = 4530;
	public string ServerName = "Lite";

	private bool ConnectStatus = true;

	public string uniQueID;
	public string NickName;
	public string Money;
	public string AccountID;

	// Use this for initialization
	void Start () {
		DontDestroyOnLoad (this);
		if (!PhotonGlobal.PS.ezServerConnecting) {
			Debug.Log ("Start Connect");
			PhotonGlobal.PS.ConnectEvent += this.doConnectEvent;
			PhotonGlobal.PS.Connect (ServerIP,ServerPort,ServerName);
			MemberGlobal.LoginStatus = false;
			PhotonGlobal.PS.MessageEvent += this.onMessage;
			PhotonGlobal.PS.onChangeScene += this.onChangeScene;
		}
		MemberGlobal.AccountID = AccountID;
		MemberGlobal.NickName = NickName;
		MemberGlobal.Money = int.Parse(Money);
		GameData.getInstance ();
	}
	// Update is called once per frame
	void Update () {
		PhotonGlobal.PS.Service ();

		uniQueID = MemberGlobal.UniqueID;
	}
	private void onChangeScene(string message){
		GameData.CurrentFloor = int.Parse (message);
		SceneSwitchManager.LoadFloorScene (GameData.CurrentFloor);
	}

	private void onMessage(string message){
		Debug.Log (message);
	}

	private void doConnectEvent(bool Status){
		if (Status) {
			Debug.Log ("Connecting.......");
			ConnectStatus = true;
		} else {
			Debug.Log ("Connect Fail");
			ConnectStatus = false;
			PhotonGlobal.PS.ConnectEvent -= this.doConnectEvent;
			PhotonGlobal.PS.onMapData -= this.onMessage;
			PhotonGlobal.PS.onChangeScene -= this.onChangeScene;
		}
	}

	void onDestroy(){
		Debug.Log ("DisConnect");
		PhotonGlobal.PS.LeaveRoom ();
	}

	void OnGUI(){
		if (!ConnectStatus) {
			GUI.Label (new Rect ((Screen.width / 2) - 200, (Screen.height / 2) - 10, 400, 20), "Connect Fail.");
		} else {
			if (GUI.Button (new Rect (5, 5, 100, 50), "JoinRoom1")) {
				
				PhotonGlobal.PS.JoinRoom (1);
				//PhotonGlobal.PS.JoinRoom (GameData.CurrentFloor);
			}
			if (GUI.Button (new Rect (105, 5, 100, 50), "JoinRoom2")) {
				
				PhotonGlobal.PS.JoinRoom (2);
				//PhotonGlobal.PS.JoinRoom (GameData.CurrentFloor);
			}
			if (GUI.Button (new Rect (205, 5, 100, 50), "Leave Room")) {
				PhotonGlobal.PS.LeaveRoom ();
				//PhotonGlobal.PS.JoinRoom (GameData.CurrentFloor);
			}

			if (GUI.Button (new Rect (305, 5, 100, 50), "Add Item")) {
				PhotonGlobal.PS.CreateItem (1,10,10,10);
				//PhotonGlobal.PS.JoinRoom (GameData.CurrentFloor);
			}
		}
	}
}
