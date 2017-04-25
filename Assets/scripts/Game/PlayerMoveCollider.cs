using UnityEngine;
using System.Collections;

public class PlayerMoveCollider : MonoBehaviour {
	private Rigidbody2D rigid;
	public Vector2 velocity;

	private const int normalLayer = 10;
	private const int jumpingLayer = 11;


	// Use this for initialization
	void Start () {
		rigid = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		velocity = rigid.velocity;
		if (velocity.y > 0.1) {
			//Jumping
			gameObject.layer = jumpingLayer;
		}
		if (velocity.y <= 0) {
			//Falling
			gameObject.layer = normalLayer;
		}
	}

}
