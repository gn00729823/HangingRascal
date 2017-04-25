using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapCreator)),ExecuteInEditMode]
public class MapCreatorEditor : Editor {

	public override void OnInspectorGUI ()
	{
		MapCreator mapCreator = (MapCreator)target;

		if (!mapCreator.modeSelected) {
			GUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Edit new map", GUILayout.Width (100))) {
				mapCreator.modeSelected = true;
				mapCreator.isEditor = true;
			}
			if (GUILayout.Button ("Read map data", GUILayout.Width (100))) {
				mapCreator.modeSelected = true;
				mapCreator.isEditor = false;
			}
			GUILayout.EndHorizontal ();
			return;
		}

		if (!mapCreator.isEditor) {
			GUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField ("Floor :", GUILayout.Width (100));
			mapCreator.editorString = EditorGUILayout.TextField (mapCreator.editorString, GUILayout.Width (150));
			GUILayout.EndHorizontal ();

			if (mapCreator.editorString != "" && int.TryParse (mapCreator.editorString, out mapCreator.Floor)) {
				GUILayout.BeginHorizontal ();
				mapCreator.Floor = int.Parse (mapCreator.editorString);
				if (GUILayout.Button ("Load Data", GUILayout.Width (100))) {
					mapCreator.modeSelected = true;
					mapCreator.isEditor = true;
					mapCreator.mapData = MapSaver.LoadData (mapCreator.Floor);
					mapCreator.createMapByData (mapCreator.mapData);
				}
				if (GUILayout.Button ("Back", GUILayout.Width (100))) {
					mapCreator.modeSelected = false;
					mapCreator.isEditor = false;
				}
				GUILayout.EndHorizontal ();
			} else
				return;
			
			return;
		}

		if (!mapCreator.DataReady ()) {
			mapCreator.init ();
			return;
		}
			
		
		base.OnInspectorGUI ();

		if (GUILayout.Button ("Create Border")) {
			if (mapCreator.Border != null) {
				if (mapCreator.MapSize.x != 0 && mapCreator.MapSize.y != 0) {
					mapCreator.ClearBorder ();
					mapCreator.CreateBorder ();
				} else {
					Debug.LogWarning ("Map size must greater than 0");
				}
			} else {
				Debug.LogWarning ("Please Insert Border Object");
			}
		}
		if (GUILayout.Button ("ClearBorder")) {
			mapCreator.ClearBorder ();
		}

		GUILayout.Space (15);

		if (GUILayout.Button ("Create Ground")) {
			mapCreator.CreateGround ();
		}
		if (GUILayout.Button ("Delete Last Ground")) {
			mapCreator.DeleteLastGround ();
		}
		if (GUILayout.Button ("RandomGround")) {
			mapCreator.RandomCreateGround ();
		}
		if (GUILayout.Button ("Reset")) {
			mapCreator.ClearGround ();
		}

		mapCreator.DetectGroundObject ();

		GUILayout.Space (15);
		if (GUILayout.Button ("Create Hidden Object")) {
			mapCreator.CreateRandomGrass ();
		}
		if (GUILayout.Button ("Clear Hidden Object")) {
			mapCreator.ClearHiddenObject ();
		}

		mapCreator.DetectHiddenObject ();

		GUILayout.Space (15);
		if (GUILayout.Button ("Save")) {
			mapCreator.Save ();
		}
		if (GUILayout.Button ("Reset All")) {
			mapCreator.Reset ();
		}

	}

	public void OnSceneGUI(){
		
	}

}
