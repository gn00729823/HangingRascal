using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Messenger.AddListener<Character_Move>(GameEvent.Character_Update, OnCharacter_Update);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCharacter_Update(Character_Move data)
    {
        data.uid = "123";
        Messenger.Broadcast<Character_Move>(GameEvent.Character_Move, data);
    }
}
