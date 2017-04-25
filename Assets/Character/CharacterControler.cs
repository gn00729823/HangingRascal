using UnityEngine;
using System.Collections.Generic;

public class CharacterControler : MonoBehaviour
{

    private Dictionary<string, BattleSpriteAction> characterDis = new Dictionary<string, BattleSpriteAction>();
    private UnityEngine.Object characterPrefab;
    void Awake()
    {
		//DontDestroyOnLoad (this);
		characterPrefab = Resources.Load("Prefabs/Character");
        Messenger.AddListener<Character_Login>(GameEvent.Character_Login, OnCharacterLogin);
        Messenger.AddListener<Character_Move>(GameEvent.Character_Move, OnCharacterMove);
        Messenger.AddListener<Character_ShowUI>(GameEvent.Character_ShowUI,OnShowUI);
        Messenger.AddListener<string> (GameEvent.Character_Leave,onCharacterLeave);

		GameData.getInstance ().getLogin ();
    }



    // Use this for initialization
    void Start()
    {
        
       // Character_Login obj = new Character_Login("pig", "123", 100, new Vector3(0.4f, 0.4f, 0),false);
       // Messenger.Broadcast<Character_Login>(GameEvent.Character_Login, obj);
    }


    // Update is called once per frame
    void Update()
    {

    }

    void OnShowUI(Character_ShowUI ui)
    {
        if (characterDis.ContainsKey(ui.uid))
        {
            PlayerUI playerUI =characterDis[ui.uid].gameObject.GetComponent<PlayerUI>();
            playerUI.showUi(ui);
        }
    }


	void onCharacterLeave(string uid){
		if (characterDis.ContainsKey (uid)) {
			Destroy(characterDis [uid].gameObject);
			characterDis.Remove (uid);
		}
	}

    void OnCharacterLogin(Character_Login Character)
    {
		if (characterDis.ContainsKey (Character.uid))
			return;
		
		Debug.Log ("get Event");
        GameObject obj = Instantiate(characterPrefab,Character.pos, Quaternion.identity) as GameObject;
        BattleSpriteAction script = obj.GetComponent<BattleSpriteAction>();
        script.uid = Character.uid;
        script.nikName = Character.nicName;
        script.hp = Character.hp;
		//MemberGlobal.mainPlayer
		script.isMainPlayer = script.uid == MemberGlobal.UniqueID;
		if (script.isMainPlayer) {
			MemberGlobal.mainPlayer = obj;
		} else {
			script.rig2d.gravityScale = 0;
			script.gameObject.GetComponent<CircleCollider2D> ().enabled = false;
		}
			
		
        characterDis.Add(Character.uid,script);
		Debug.Log("login :" + Character.nicName  + " | Main :" + script.isMainPlayer);
    }

    void OnCharacterMove(Character_Move Character)
    {
		if (Character.uid == MemberGlobal.UniqueID)
			return;

        if(characterDis.ContainsKey(Character.uid))
        {
			characterDis[Character.uid].setAnimation(Character.pos,Character.Clip,Character.isFromRight,Character.Health);
        }
    }
}