using UnityEngine;
using System.Collections;

public class HiddenObjectScript : MonoBehaviour {
	public HiddenType type;

	void OnTriggerEnter2D(Collider2D collider){
		if (collider.gameObject.tag == "MainPlayer") {
			Material mat = gameObject.GetComponent<MeshRenderer> ().material;
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 0.5f);
		}
	}
	void OnTriggerExit2D(Collider2D collider){
		if (collider.gameObject.tag == "MainPlayer") {
			Material mat = gameObject.GetComponent<MeshRenderer> ().material;
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 1);
		}
	}

	public void DeleteSelf(){
		
	}
}
	
