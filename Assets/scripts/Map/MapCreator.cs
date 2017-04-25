using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ServerOperationCode;


public class MapCreator : MonoBehaviour {
	public bool DEBUG_MODE;
	public bool isEditor = false;
	public bool modeSelected = false;
	public MapData mapData;
	[HideInInspector]
	public string editorString = "0";
	public Vector2 MapSize;
	public GameObject Border;
	public GameObject MapRoot;
	public GameObject Ground;
	public GameObject[] HidenObjectArray;

	public int Floor;
	public GroundType groundType;
	public RandomGround randomGround;
	public RandomHiddenData HiddenObjectData;

	private List<GameObject> Borders = new List<GameObject>();

	private List<GameObject> Grounds = new List<GameObject>();

	private List<GameObject> HiddenObjects = new List<GameObject> ();

	private static MapCreator _mapCreator;
	public static MapCreator getInstance(){
		return _mapCreator;
	}
	// Use this for initialization
	void Awake () {
		_mapCreator = this;
		//mapData = MapSaver.LoadData (GameData.CurrentFloor);
		//this.isEditor = true;
		//this.modeSelected = true;
		//createMapByData (mapData);
	}

	void Start(){
		mapData = JsonUtility.FromJson<MapData>(GameData.MapDataJson);
		createMapByData (mapData);

		PhotonGlobal.PS.onMapEvent += onMapEvent;
	}

	public bool DataReady(){
		if (MapRoot != null) {
			if (Border != null) {
				return true;
			}
		}
		return false;
	}

	public void ClearGround(){
		foreach (GameObject obj in Grounds)
			DestroyImmediate (obj);
		Grounds = new List<GameObject> ();
	}

	public void CreateGround(){
		GameObject ground = Instantiate (Ground);
		Transform groundRoot = MapRoot.transform.FindChild ("Ground");
		ground.transform.parent = groundRoot;
		ground.name = "ground" + groundRoot.childCount;
		ground.transform.localPosition = Vector3.zero;
		Grounds.Add (ground);
	}

	public void createRandomGround(){

			float randomSize = Random.Range (randomGround.MinSize, randomGround.MaxSize);
			float randomPosX = Random.Range ( (randomSize/2), MapSize.x - (randomSize/2));
			float randomPosY = Random.Range ( 0.5f, MapSize.y - 0.5f);
			GameObject ground = Instantiate (Ground);
			Transform groundRoot = MapRoot.transform.FindChild ("Ground");
			ground.transform.parent = groundRoot;
			ground.name = "ground" + groundRoot.childCount;
			ground.transform.localScale = new Vector3 (randomSize,1,1);
			ground.transform.localPosition = new Vector3(randomPosX,randomPosY,0);
			Grounds.Add (ground);

	}

	public void CreateRandomGrass(){
		if (Grounds.Count == 0) {
			Debug.LogWarning ("Ground have not been created!");
			return;
		}
		if (HiddenObjectData.fillAmount == 0)
			return;

		Transform hidenRoot = MapRoot.transform.FindChild ("HidenObject");
		//ButtonGrass
		GameObject groundObj = getRandomHideObject();
		for (int i = 1; i < Borders [1].transform.localScale.x-2; i++) {
			if (Random.Range (0, 100) <= HiddenObjectData.grassIntensive) {
				GameObject obj = Instantiate (groundObj);
				obj.name = groundObj.name;
				obj.transform.position = new Vector3(i-0.5f, -0.5f,0);
				obj.transform.parent = hidenRoot;
				HiddenObjects.Add (obj);
			}
		}

		for (int i = 0; i < Grounds.Count; i++) {
			if (Random.Range (0, 100) > HiddenObjectData.fillAmount)
				continue;
			GameObject hidObj = getRandomHideObject ();
			for (int x = 1; x < Grounds[i].transform.localScale.x - 2; x++) {
				if (Random.Range (0, 100) <= HiddenObjectData.grassIntensive) {
					GameObject hidObject = Instantiate (hidObj);
					hidObject.name = hidObj.name;
					float scale = Grounds [i].transform.localScale.x;
					hidObject.transform.position = new Vector3 (Grounds [i].transform.position.x - (scale / 2) + x + 0.5f, Grounds [i].transform.position.y + 1f, 0);
					hidObject.transform.parent = hidenRoot;
					HiddenObjects.Add (hidObject);
				}
			}
		}
	}

	public void ClearHiddenObject(){
		foreach (GameObject obj in HiddenObjects)
			DestroyImmediate (obj);
		HiddenObjects = new List<GameObject> ();
	}

	public void DetectHiddenObject(){
		Transform HiddenObjectRoot = MapRoot.transform.FindChild ("HidenObject");
		if (HiddenObjectRoot.childCount != HiddenObjects.Count) {
			HiddenObjects = new List<GameObject> ();
			foreach (Transform obj in HiddenObjectRoot.GetComponentsInChildren<Transform>()) {
				if (obj.gameObject.name != "HidenObject") {
					HiddenObjects.Add (obj.gameObject);
				}

			}
		}
	}

	private GameObject getRandomHideObject(){
		return HidenObjectArray [Random.Range (0, HidenObjectArray.Length)];
	}

	public void DetectGroundObject(){
		Transform groundRoot = MapRoot.transform.FindChild ("Ground");
		if (groundRoot.childCount != Grounds.Count) {
			Grounds = new List<GameObject> ();
			foreach (Transform obj in groundRoot.GetComponentsInChildren<Transform>()) {
				if (obj.gameObject.name != "Ground") {
					Grounds.Add (obj.gameObject);
					obj.name = "ground" + Grounds.Count;
				}
					
			}
		}
	}

	public void DeleteLastGround(){
		if (Grounds.Count > 0) {
			DestroyImmediate (Grounds [Grounds.Count - 1]);
			Grounds [Grounds.Count - 1] = null;
		}
	}

	public void CreateBorder(){
		//Top
		GameObject top = Instantiate(Border);
		top.name = "TopBorder";
		top.transform.parent = MapRoot.transform;
		top.transform.localScale = new Vector3 (MapSize.x+2,1,1);
		top.transform.localPosition = new Vector3 (MapSize.x / 2-1, MapSize.y + 0.5f-1, 0);
		//Button
		GameObject buttom = Instantiate(Border);
		buttom.name = "ButtomBorder";
		buttom.transform.parent = MapRoot.transform;
		buttom.transform.localScale = new Vector3 (MapSize.x+2,1,1);
		buttom.transform.localPosition = new Vector3 (MapSize.x / 2-1, -0.5f-1, 0);
		//Left
		GameObject left = Instantiate(Border);
		left.name = "LeftBorder";
		left.transform.parent = MapRoot.transform;
		left.transform.localScale = new Vector3 (1,MapSize.y,1);
		left.transform.localPosition = new Vector3 (-0.5f-1, MapSize.y /2-1, 0);
		//Right
		GameObject right = Instantiate(Border);
		right.name = "RightBorder";
		right.transform.parent = MapRoot.transform;
		right.transform.localScale = new Vector3 (1,MapSize.y,1);
		right.transform.localPosition = new Vector3 (MapSize.x + 0.5f-1, MapSize.y /2-1, 0);


		Borders.Add (top);
		Borders.Add (buttom);
		Borders.Add (left);
		Borders.Add (right);
	}

	public void ClearBorder(){
		if (Borders.Count > 0) {
			foreach (GameObject obj in Borders) {
				DestroyImmediate (obj);
			}
			Borders = new List<GameObject> ();
		}

	}

	public void RandomCreateGround(){
		for (int i = 0; i < randomGround.GroundAmount; i++) {
			createRandomGround ();
		}
	}

	public void Save(){
		mapData = GenerateMapData ();
		MapSaver.SaveData (mapData);
	}

	public MapData GenerateMapData(){
		MapData data = new MapData ();
		data.floor = Floor;
		data.MapSize = MapSize;
		data.groundData = new GroundData[Grounds.Count];
		for (int i = 0; i < Grounds.Count; i++) {
			data.groundData [i] = new GroundData ();
			data.groundData [i].type = groundType;
			data.groundData [i].position = Grounds [i].transform.position;
			data.groundData [i].Scale = Grounds [i].transform.localScale;
		}
		data.hiddenData = new HiddenData[HiddenObjects.Count];
		for (int i = 0; i < HiddenObjects.Count; i++) {
			data.hiddenData [i] = new HiddenData ();
			data.hiddenData [i].type = MapDataHolder.getHiddenType( HiddenObjects[i].name);
			data.hiddenData [i].position = HiddenObjects [i].transform.position;
		}
		return data;
	}

	public void createMapByData(MapData data){
		init ();
		MapSize = data.MapSize;
		CreateBorder ();
		//Ground
		for (int i = 0; i < data.groundData.Length; i++) {
			Ground = MapDataHolder.getGroundObject (data.groundData [i].type);

			GameObject ground = Instantiate (Ground);
			Transform groundRoot = MapRoot.transform.FindChild ("Ground");
			ground.transform.parent = groundRoot;

			ground.name = "ground" + groundRoot.childCount;
			ground.transform.localScale = data.groundData [i].Scale;
			ground.transform.localPosition = data.groundData [i].position;
			Grounds.Add (ground);
		}

		Transform hidenRoot = MapRoot.transform.FindChild ("HidenObject");
		//ButtonGrass

		for (int i = 0; i < data.hiddenData.Length; i++) {
			GameObject hiddenObj = MapDataHolder.getHiddenObject (data.hiddenData [i].type);
			GameObject obj = Instantiate (hiddenObj);
			obj.name = NameProcess.GetObjectName (GameData.CurrentFloor, (int)data.hiddenData [i].type, i);
			obj.transform.position = data.hiddenData [i].position;
			obj.transform.parent = hidenRoot;
			HiddenObjects.Add (obj);

			GameData.getInstance ().addHiddenObject (obj.name, obj);
		}

		if (!DEBUG_MODE) {
			GameData.getInstance ().OnMapCreated();
		}
	}

	public void createHiddenObjectByServer(string UID,int type,Vector3 position){
		Transform hidenRoot = MapRoot.transform.FindChild ("HidenObject");
		GameObject hiddenObj = MapDataHolder.getHiddenObject ((HiddenType)type);
		GameObject obj = Instantiate (hiddenObj);
		obj.name = UID;
		obj.transform.position = position;
		obj.transform.parent = hidenRoot;
		HiddenObjects.Add (obj);
		GameData.getInstance ().addHiddenObject (obj.name, obj);
	}
		
	public void deleteHiddenObjectByServer(string UID){
		GameObject obj = GameData.getInstance ().getMapHiddenObject (UID);
		if (obj != null) {
			Destroy (obj);
			HiddenObjects.Remove (obj);
			GameData.getInstance ().deleteHiddenObject (UID);
		}
	}

	public void init(){
		MapRoot = GameObject.Find ("MapRoot");
		Border = Resources.Load ("Ground/Border")as GameObject;
	}

	public void Reset(){
		ClearHiddenObject ();
		ClearGround ();
		ClearBorder ();
		GameData.getInstance ().resetMapHiddenData ();
		modeSelected = false;
		isEditor = false;
	}

	public void onMapEvent( string UID , int type, bool create,float X, float Y,float Z){
		if (create) {
			if(!GameData.getInstance().isContainUID(UID))
				createHiddenObjectByServer (UID, type, new Vector3 (X, Y, Z));
		} else {
			deleteHiddenObjectByServer (UID);
		}
	}
		
	[System.Serializable]
	public class RandomGround{
		public int GroundAmount;
		public int MaxSize;
		public int MinSize;
	}

	[System.Serializable]
	public class RandomHiddenData{
		[Range(0,100)]
		public float fillAmount;
		[Range(0,100)]
		public float grassIntensive;
	}



		
}
