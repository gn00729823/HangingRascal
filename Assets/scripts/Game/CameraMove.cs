using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour {
	private GameObject MainPlayer;
	public float cameraOffSetX;
	public float cameraOffSetY;

	private Vector2 horizontalEdge;
	private Vector2 verticalEdge;
	// Use this for initialization
	void Start () {
		MainPlayer = MemberGlobal.mainPlayer;

		horizontalEdge = new Vector2 (-1f + cameraOffSetX, MapCreator.getInstance ().MapSize.x + 1 - cameraOffSetX);
		verticalEdge = new Vector2 (-1f + cameraOffSetY, MapCreator.getInstance ().MapSize.y + 1 - cameraOffSetY);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (MainPlayer != null) {
			float PosX = Mathf.Clamp (MainPlayer.transform.position.x, horizontalEdge.x, horizontalEdge.y);
			float PosY = Mathf.Clamp (MainPlayer.transform.position.y, verticalEdge.x, verticalEdge.y);
			Vector3 position = new Vector3 (PosX, PosY, this.transform.position.z);

			this.transform.position = position;
		}

	}
}
