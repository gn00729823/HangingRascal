using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HiddenObjectScript : MonoBehaviour {
	public HiddenType type;
	private Dictionary<string, BattleSpriteAction> inSidePlayer = new Dictionary<string, BattleSpriteAction> ();


	public void AddPlayer(string uid, BattleSpriteAction target){
		if (!inSidePlayer.ContainsKey (uid)) {
			inSidePlayer.Add (uid, target);
			target.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
		}
	}

	public void RemovePlayer(string uid){
		if (inSidePlayer.ContainsKey (uid)) {
			inSidePlayer[uid].gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			inSidePlayer.Remove (uid);
		}
	}

	public List<BattleSpriteAction> getInsidePlayer(){
		List<BattleSpriteAction> result = new List<BattleSpriteAction> ();
		foreach (KeyValuePair<string, BattleSpriteAction> obj in inSidePlayer) {
			result.Add (obj.Value);
		}
		return result;
	}


}
	
