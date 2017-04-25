using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameData{

	private static GameData _gameData;
	public static GameData getInstance(){
		if (_gameData == null)
			_gameData = new GameData ();
		return _gameData;
	}
	public GameData(){
		PhotonGlobal.PS.onMapData += onGetMapJson;
		PhotonGlobal.PS.LoginEvent += onGetLogin;
		PhotonGlobal.PS.CharacterUpdateEvent += onCharacterUpdate;
		PhotonGlobal.PS.LeaveEvent += onCharacterLeave;
		PhotonGlobal.PS.onPlayerChangeRoom += onPlayerChangeRoom;
	}

	private Dictionary<string , Character_Login> characterLoginData = new Dictionary<string, Character_Login> ();


	private static int currentFloor;
	// Use this for initialization
	public static int CurrentFloor {get{return currentFloor;}set{currentFloor = value;}}

	private static string mapDataJson;
	public static string MapDataJson {get{return mapDataJson;}set{mapDataJson = value;}}

	public delegate void Command();
	public event Command onMapCreated;

	public void OnMapCreated(){
		if (onMapCreated != null) {
			onMapCreated ();
		}
	}

	private void onPlayerChangeRoom(){
		characterLoginData = new Dictionary<string, Character_Login> ();
		//Messenger.Broadcast<> (GameEvent.Character_ChangeRoom);
	}

	private void onGetMapJson(string value){
		Debug.Log (value);
		MapDataJson = value;
		//SceneSwitchManager.LoadFloorScene (currentFloor);
	}


	private Dictionary<string,GameObject> MapHiddenDictionary = new Dictionary<string, GameObject>();

	public void resetMapHiddenData(){
		MapHiddenDictionary.Clear ();
	}
	public GameObject getMapHiddenObject(string UID){
		if (!MapHiddenDictionary.ContainsKey (UID)) {
			Debug.LogWarning ("No Key Match!!!!");
			return null;
		} else {
			return MapHiddenDictionary [UID];
		}
	}
	public bool isContainUID(string UID){
		return MapHiddenDictionary.ContainsKey (UID);
	}

	public void addHiddenObject(string UID, GameObject obj){
		if (MapHiddenDictionary.ContainsKey (UID)) {
			Debug.LogWarning ("Same Key Contain!!!!");
		} else {
			MapHiddenDictionary.Add (UID, obj);
		}
	}

	public void deleteHiddenObject(string UID){
		if (!MapHiddenDictionary.ContainsKey (UID)) {
			Debug.LogWarning ("No Key Match!!!!");
		} else {
			MapHiddenDictionary.Remove (UID);
		}
	}

	public void getLogin(){
		foreach(KeyValuePair<string,Character_Login> data in characterLoginData)
			Messenger.Broadcast<Character_Login> (GameEvent.Character_Login, data.Value);
	}

	public void onGetLogin(string _nicName, string _uid, int _hp, Vector3 _pos,bool _isMainPlayer = false){
	//	Debug.Log ("GameData Get Login Event");
		Character_Login loginData = new Character_Login (_nicName,_uid,_hp,_pos,_isMainPlayer);
		Messenger.Broadcast<Character_Login> (GameEvent.Character_Login, loginData);

		if (!characterLoginData.ContainsKey (_uid)) {
			characterLoginData.Add (_uid, loginData);
		}
	}

	public void onCharacterUpdate(string _uid, int _hp, Vector3 _pos,int _clip,int health,bool facing = false){
	//	Debug.Log ("GameData Get Move Event");
		Character_Move moveData = new Character_Move (_uid, _pos, (Animator_Clip)_clip, facing,health);
		Messenger.Broadcast<Character_Move> (GameEvent.Character_Move, moveData);
	}

	public void onCharacterLeave(string uid){
		Messenger.Broadcast<string> (GameEvent.Character_Leave,uid);
	}

	//Messenger.AddListener<Character_Move>(GameEvent.Character_Move, OnCharacterMove);

}
