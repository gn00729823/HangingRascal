using UnityEngine;
using System.Collections;

[System.Serializable]
public class MapData{
	public int floor;
	public Vector2 MapSize;
	public GroundData[] groundData;
	public HiddenData[] hiddenData;
}

[System.Serializable]
public class GroundData{
	public GroundType type;
	public Vector3 position;
	public Vector3 Scale;
}

[System.Serializable]
public class HiddenData{
	public HiddenType type;
	public Vector3 position;
}

public enum GroundType{
	BASIC
}

public enum HiddenType{
	NULL = 0,
	GRASS,
	BOX
}

public class MapDataHolder{
	public static HiddenType getHiddenType(string name){
		HiddenType type = HiddenType.NULL;
		switch (name) {
		case "Grass":
			type = HiddenType.GRASS;
			break;
		case "Box":
			type = HiddenType.BOX;
			break;
		}

		return type;
	}

	public static HiddenType getHiddenTypeByInt(string name){
		HiddenType type = HiddenType.NULL;
		switch (name) {
		case "Grass":
			type = HiddenType.GRASS;
			break;
		case "Box":
			type = HiddenType.BOX;
			break;
		}

		return type;
	}

	public static GameObject getGroundObject(GroundType type){
		GameObject obj = null;
		switch (type) {
		case GroundType.BASIC:
			obj = Resources.Load ("Ground/Basic_Ground") as GameObject;
			break;
		}
		if (obj == null)
			Debug.LogWarning ("Ground Type Error : " + type.ToString());

		return obj;
	}

	public static GameObject getHiddenObject(HiddenType type){
		GameObject obj = null;
		switch (type) {
		case HiddenType.BOX:
			obj = Resources.Load ("Hidden/Box") as GameObject;
			break;
		case HiddenType.GRASS:
			obj = Resources.Load ("Hidden/Grass") as GameObject;
			break;
		}

		if (obj == null)
			Debug.LogWarning ("Hidden Type Erroe : " + type.ToString());

		return obj;
	}
}

