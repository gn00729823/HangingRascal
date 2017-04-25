using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiddenObjectScript : MonoBehaviour {
	public HiddenType type;
	private Dictionary<string, BattleSpriteAction> inSidePlayer = new Dictionary<string, BattleSpriteAction> ();


	public void AddPlayer(string uid, BattleSpriteAction target){
		if (!inSidePlayer.ContainsKey (uid))
			inSidePlayer.Add (uid, target);
	}

	public void RemovePlayer(string uid){
		if (inSidePlayer.ContainsKey (uid))
			inSidePlayer.Remove (uid);
	}

	public List<BattleSpriteAction> getInsidePlayer(){
		List<BattleSpriteAction> result = new List<BattleSpriteAction> ();
		foreach (KeyValuePair<string, BattleSpriteAction> obj in inSidePlayer) {
			result.Add (obj.Value);
		}
		return result;
	}

//	void OnTriggerEnter2D(Collider2D collider){
//		if (collider.gameObject.tag == "MainPlayer") {
//			Material mat = gameObject.GetComponent<MeshRenderer> ().material;
//			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 0.5f);
//		}
//	}
//	void OnTriggerExit2D(Collider2D collider){
//		if (collider.gameObject.tag == "MainPlayer") {
//			Material mat = gameObject.GetComponent<MeshRenderer> ().material;
//			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 1);
//		}
//	}


}
	
