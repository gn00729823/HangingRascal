using UnityEngine;
using System.Collections;
using System.IO;

public class MapSaver : MonoBehaviour {
	static string path = Application.dataPath + "/StreamingAssets/";


	public static void SaveData(MapData data){
		string json = JsonUtility.ToJson (data);

		if (File.Exists (path + data.floor.ToString () + ".txt"))
			File.Delete (path + data.floor.ToString () + ".txt");

		System.IO.File.WriteAllText(path + data.floor.ToString() +".txt", json);
	}

	public static MapData LoadData(int floor){
		MapData mapData = new MapData ();
		if (File.Exists (path + floor.ToString () + ".txt")) {
			FileStream file = new FileStream (path + floor.ToString() + ".txt", FileMode.Open, FileAccess.Read);
			StreamReader reader = new StreamReader (file);
			string data = reader.ReadToEnd ();
			mapData = JsonUtility.FromJson<MapData>(data);
		} else {
			Debug.LogWarning("No Floor "+ floor.ToString() +" File");
		}
		return mapData;
	}
}
 