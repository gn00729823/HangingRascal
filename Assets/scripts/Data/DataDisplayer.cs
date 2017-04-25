using UnityEngine;
using System.Collections;

public class DataDisplayer : MonoBehaviour {
	public MapData data;
	public string MapJason;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		MapJason = GameData.MapDataJson;

		if (MapCreator.getInstance () != null) {
			data = MapCreator.getInstance ().mapData;
		}
	}
}
