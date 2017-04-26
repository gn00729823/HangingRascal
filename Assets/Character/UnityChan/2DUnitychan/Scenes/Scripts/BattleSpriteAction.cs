using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public enum HitType
{
    noHit=0,
    hit,
    borkenDef,
    defense,
}
public enum AttackType
{
    normal=0,
    heavy,
    heavy_Lset,
}
public enum PlayerFront
{
    right=0,
    left,
}
enum AnimationTag
{
    Speed=0,
    FallSpeed,
    GroundDistance,
    IsCrouch,
    Attack1,
    Attack2,
    Defense,
    Damage,
    IsDead,
    Action,
}
public enum Animator_Clip
{
    Idle=0,
    Run,
    Crouch,
    Jump_Fall,
    Jump_MidAir,
    Jump_up,
    Landing,
    Damage,
    Knockdown,
    Attack1,
    Attack2,
    Defense,
    StandUp,
}
public enum Animator_State
{
    Stand=0,
    Crouch,
    StandUp,
    Jumping,
    Landing,
    Damage,
    Knockdown,
    Attack1,
    Attack2,
    Defense,
}


public class BattleSpriteAction : MonoBehaviour
{
    public string uid = "123";
    public string nikName = "pig";
    public PlayerFront playerFront = PlayerFront.right;
    public Animator_Clip animatorClip = Animator_Clip.Idle;
    public Animator_State animatorState = Animator_State.Stand;
    private float speed = 1f;
    public float moveSpeed = 4;
    public float jumpSpeed = 10;
    private bool canAction = true;
    private bool canAttack = true;
    private bool Stiff = false;
    private string[] ClipStringAry; 
    private Dictionary<string, int> TagHashpool = new Dictionary<string, int>();
    private Dictionary<int, Animator_State> AnimatorHashpool = new Dictionary<int, Animator_State>();
    private AnimatorStateInfo curAnimatorInfo;
    private int lestAniHash =0;
    private int curAniHash = 0;
    private bool canJump2 = true;
    public  delegate void HitCallBackCommen (HitType HitResoul);
    private Character_Move updateClass = new Character_Move("",new Vector3(),Animator_Clip.Idle,true,100);
    private float axis;
    private bool longCrouch;
    [SerializeField] private float characterHeightOffset = 0.2f;
	[SerializeField] LayerMask groundMask;
    [SerializeField] LayerMask plyaerMask; 
    [SerializeField, HideInInspector] Animator animator;
	[SerializeField, HideInInspector]SpriteRenderer spriteRenderer;
	[SerializeField, HideInInspector]public Rigidbody2D rig2d;
    public bool isMainPlayer = true;
	public int hp = 100;
    public HitCallBackCommen HitCallBack ;
    private RaycastHit2D distanceFromGround;
    public Animator effecter;
    private bool isDeath;
    void Awake ()
	{
        effecter = transform.GetChild(0).GetComponent<Animator>();
        animator = GetComponent<Animator> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		rig2d = GetComponent<Rigidbody2D> ();
        ClipStringAry = Enum.GetNames(typeof(Animator_State));
        for (int i=0;i< ClipStringAry.Length;i++)
        {
            AnimatorHashpool[Animator.StringToHash(ClipStringAry[i])] = (Animator_State)i;
        }

        HitCallBack = DoHitCallBack;
        
    }
    void Start()
    {   
        if(isMainPlayer)
        {
            this.transform.tag = "MainPlayer";
        }
        else
        {
            this.transform.tag = "Player";
        }
        plyaerMask = LayerMask.GetMask("Player");
    }
	void FixedUpdate(){
		if (isMainPlayer) {
			PhotonGlobal.PS.UpdateCharacterInfo (MemberGlobal.UniqueID.ToString(), transform.position,(int)updateClass.Clip,updateClass.isFromRight,hp);
		}
	}
	void Update ()
	{
        //---------------屬性偵測----
        curAnimatorInfo = animator.GetCurrentAnimatorStateInfo(0);
        if(curAnimatorInfo.shortNameHash!=curAniHash)
        {
            this.OnAniStateCheange(curAnimatorInfo.shortNameHash);
        }
        distanceFromGround = Physics2D.Raycast(transform.position, Vector3.down, 2, groundMask);
        this.CheckAnimatorClip();
        
        //---------------屬性偵測結束----

        //僵直為真時 持續保持受傷畫面
        if (Stiff)
        {
            animator.SetTrigger(getTagHash(AnimationTag.Damage));
        }


        //主要玩家 才偵測來自鍵盤的控制
        onMainPlayerMove();

        // 角色翻轉
        if (axis != 0 && !this.IsAction())
        {       
            spriteRenderer.flipX = axis < 0;
        }
			
    }
    void onMainPlayerMove() {

        if (isMainPlayer)
        {

            this.updateCharacterData();
            if(this.isDeath)
            {
                return;
            }
            if (canAction)
            {
               
                axis = Input.GetAxisRaw("Horizontal");

                if (Input.GetButtonDown("Jump") && !this.IsAction())
                {
                    if ((distanceFromGround.distance == 0 ? 99 : distanceFromGround.distance - characterHeightOffset) < 0.1f)
                    {
                        canJump2 = true;
                        rig2d.velocity = new Vector2(rig2d.velocity.x, jumpSpeed * speed);
                    }
                    else if (canJump2)
                    {
                        canJump2 = false;
                        rig2d.velocity = new Vector2(rig2d.velocity.x, (jumpSpeed*0.7f) * speed);
                    }


                }
                if(longCrouch)
                {
                    animator.SetBool(getTagHash(AnimationTag.IsCrouch), true);
                }
                else
                {
                    bool isDown = Input.GetAxisRaw("Vertical") < 0;
                    animator.SetBool(getTagHash(AnimationTag.IsCrouch), isDown);
                }
               if(axis!=0 || rig2d.velocity.y!=0)
                {
                    longCrouch = false;
                }
                if (canAttack)
                {
                    if (Input.GetKeyDown(KeyCode.Z) && !IsAction())
                    {
                        animator.SetTrigger(getTagHash(AnimationTag.Action));
                        animator.SetTrigger(getTagHash(AnimationTag.Attack1));
                    }
                    if (Input.GetKeyDown(KeyCode.X) && !IsAction())
                    {
                        animator.SetTrigger(getTagHash(AnimationTag.Action));
                        animator.SetTrigger(getTagHash(AnimationTag.Attack2));
                    }
                    if (Input.GetKeyDown(KeyCode.C) && !IsAction())
                    {
                        animator.SetTrigger(getTagHash(AnimationTag.Action));
                        animator.SetTrigger(getTagHash(AnimationTag.Defense));
                    }
                    if (Input.GetKeyDown(KeyCode.H) && !IsAction())
                    {
                        longCrouch = true;
                    }                 
                }
                //角色移動
                if (!this.IsAction())
                {
                    animator.SetFloat(getTagHash(AnimationTag.Speed), Mathf.Abs(axis));
                    if (axis > 0)
                    {

                        rig2d.velocity = new Vector2(moveSpeed * speed, rig2d.velocity.y);
                        playerFront = PlayerFront.right;
                    }
                    else if (axis < 0)
                    {

                        rig2d.velocity = new Vector2((moveSpeed * speed) * -1, rig2d.velocity.y);
                        playerFront = PlayerFront.left;
                    }
                }
                animator.SetFloat(getTagHash(AnimationTag.GroundDistance), distanceFromGround.distance == 0 ? 99 : distanceFromGround.distance - characterHeightOffset);
                animator.SetFloat(getTagHash(AnimationTag.FallSpeed), rig2d.velocity.y);
            }

        }
    }
    public void setAnimation(Vector3 pos,Animator_Clip clip,bool isRight,int health)
    {
		

        this.transform.position = pos;
        if(isRight)
        {
            spriteRenderer.flipX = false;
			playerFront = PlayerFront.right;
        }
        else
        {
            spriteRenderer.flipX = true;
			playerFront = PlayerFront.left;
        }

		if(clip != Animator_Clip.Crouch)
			animator.SetBool(getTagHash(AnimationTag.IsCrouch), false);

        switch(clip)
        {
            case Animator_Clip.Idle:
                this.animator.Play(Animator_State.Stand.ToString(), 0);
                animator.SetFloat(getTagHash(AnimationTag.Speed), 0);
                onAir(false);
                break;
            case Animator_Clip.Run:
                this.animator.Play(Animator_State.Stand.ToString(), 0);
                animator.SetFloat(getTagHash(AnimationTag.Speed), 99);               
                onAir(false);
                break;
            case Animator_Clip.Crouch:
                this.animator.Play(Animator_State.Crouch.ToString(), 0);
                animator.SetBool(getTagHash(AnimationTag.IsCrouch), true);
                animator.SetFloat(getTagHash(AnimationTag.Speed), 0);
                break;
            case Animator_Clip.StandUp:
                this.animator.Play(Animator_State.StandUp.ToString(), 0);
                animator.SetBool(getTagHash(AnimationTag.IsCrouch), false);
                break;
            case Animator_Clip.Jump_Fall:
                this.animator.Play(Animator_State.Jumping.ToString(), 0);
                animator.SetFloat(getTagHash(AnimationTag.FallSpeed),-3);
                onAir(true);
                break;
            case Animator_Clip.Jump_MidAir:
                this.animator.Play(Animator_State.Jumping.ToString(), 0);
                animator.SetFloat(getTagHash(AnimationTag.FallSpeed), 0);
                onAir(true);
                break;
            case Animator_Clip.Jump_up:
                this.animator.Play(Animator_State.Jumping.ToString(), 0);
                animator.SetFloat(getTagHash(AnimationTag.FallSpeed), 3);
                onAir(true);
                break;
            case Animator_Clip.Landing:
                this.animator.Play(Animator_State.Landing.ToString(), 0);
                animator.SetFloat(getTagHash(AnimationTag.Speed), 0);
                onAir(false);
                break;
            case Animator_Clip.Attack1:
                this.animator.Play(Animator_State.Attack1.ToString(), 0);          
                break;
            case Animator_Clip.Attack2:
                this.animator.Play(Animator_State.Attack2.ToString(), 0);                        
                break;
            case Animator_Clip.Defense:
                this.animator.Play(Animator_State.Defense.ToString(), 0);     
                break;
            case Animator_Clip.Damage:
                this.animator.Play(Animator_State.Damage.ToString(), 0);
                break;
			case Animator_Clip.Knockdown:
				animator.SetBool(getTagHash(AnimationTag.IsDead), true);
				this.animator.Play(Animator_State.Knockdown.ToString(), 0);
			break;
        }
    }
    private void onAir(bool isTrue)
    {
        if(isTrue)
        {
            animator.SetFloat(getTagHash(AnimationTag.GroundDistance), 99);
        }
        else
        {
            animator.SetFloat(getTagHash(AnimationTag.GroundDistance), 0);
            animator.SetFloat(getTagHash(AnimationTag.FallSpeed), 0);
        }
    }
    private void updateCharacterData()
    {

        if (updateClass.Clip != animatorClip || updateClass.pos != this.transform.position)
        {
            updateClass.Clip = animatorClip;
            updateClass.pos = this.transform.position;
            updateClass.uid = this.uid;
            updateClass.isFromRight = this.playerFront == PlayerFront.right ? true: false;
            //廣播角色事件
            //CharacterControler.OnCharacterEvent(updateClass);
            Messenger.Broadcast<Character_Move>(GameEvent.Character_Update, this.updateClass);
        }


    }
    public HitType HitChaeck(object[] parm)
    {
        if (isDeath)
            return HitType.noHit;

        AttackType type = (AttackType)parm[0];
        HitCallBackCommen callback = (HitCallBackCommen)parm[1];
        PlayerFront enemyfront = (PlayerFront)parm[2];
        HitType _type = HitType.noHit;

        switch (type)
        {
            case AttackType.normal:
               
                if (curAnimatorInfo.shortNameHash == getTagHash(AnimationTag.Defense) && this.playerFront != enemyfront)
                {
                    _type = HitType.defense;
                }
                else
                {
                    this.OnHit(AttackType.normal, enemyfront);
                    _type = HitType.hit;
                }
            break;
            case AttackType.heavy:
            case AttackType.heavy_Lset:
                if (curAnimatorInfo.shortNameHash == getTagHash(AnimationTag.Defense) && this.playerFront != enemyfront)
                {
                    this.OnBorkenDef();
                }
                this.OnHit(type, enemyfront);
                Debug.Log("??");
                _type = HitType.hit;
             break;
            default:
                _type = HitType.noHit;
                break;
        }

        callback(_type);
        return _type;

    }
    public void DoHitCallBack(HitType HitResoul)
    {
        Debug.Log(HitResoul);
        if (HitResoul == HitType.defense)
        {
            StartCoroutine("PunchStiff", 5f);
            // animator.SetTrigger(getTagHash(AnimationTag.Damage));
        }
    }
    private bool IsAction()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if(info.shortNameHash == getTagHash(AnimationTag.Attack1) || info.shortNameHash == getTagHash(AnimationTag.Attack2) || info.shortNameHash == getTagHash(AnimationTag.Defense))
        {
            return true;
        }

        return false;
    }
    public void NormalAttack()
    {
		Debug.Log ("in");
        RaycastHit2D[] objAry;
        objAry = RayCast(AttackType.normal);

        for(int i=0;i< objAry.Length; i++)
        {
			Debug.Log (objAry[i].transform.tag + "||" +objAry[i].transform.gameObject.Equals(this.gameObject) );
			if(objAry[i].transform.gameObject.Equals(this.gameObject))
            {
                continue;
            }
            if (objAry[i].transform == null)
            {
                return;
            }
            object[] message = new object[3];
            message[0] = AttackType.normal;
            message[1] = HitCallBack;
            message[2] = this.playerFront;
            objAry[i].transform.gameObject.SendMessage("HitChaeck", message);
            Debug.Log("NormalAttack :" + objAry[i].transform.gameObject.name);
        }
    }
    public void HeavyLastAttack()
    {
        RaycastHit2D[] objAry;
        objAry = RayCast(AttackType.heavy_Lset);

        for (int i = 0; i < objAry.Length; i++)
        {
			if (objAry[i].transform.gameObject.Equals(this.gameObject))
            {
                continue;
            }
            if (objAry[i].transform == null)
            {
                return;
            }
            object[] message = new object[3];
            message[0] = AttackType.heavy_Lset;
            message[1] = HitCallBack;
            message[2] = this.playerFront;
            objAry[i].transform.gameObject.SendMessage("HitChaeck", message);
            Debug.Log("HeavyAttack :" + objAry[i].transform.gameObject.name);
        }
    }
    public void HeavyAttack()
    {
        RaycastHit2D[] objAry;
        objAry = RayCast(AttackType.heavy);

        for (int i = 0; i < objAry.Length; i++)
        {
			if (objAry[i].transform.gameObject.Equals(this.gameObject))
            {
                continue;
            }
            if (objAry[i].transform == null)
            {
                return;
            }
            object[] message = new object[3];
            message[0] = AttackType.heavy;
            message[1] = HitCallBack;
            message[2] = this.playerFront;
            objAry[i].transform.gameObject.SendMessage("HitChaeck", message);
            Debug.Log("HeavyAttack :" + objAry[i].transform.gameObject.name);
        }

    }
    public void UseDefense()
    {
        Debug.Log("UseDefense");
        effecter.Play("Def",0);
    }
    private void OnHit(AttackType type,PlayerFront hitRoot)
    {
        effecter.SetTrigger("Hit");
        //effecter.Play("Hit", 0);
        if (playerFront == hitRoot)
        {
            spriteRenderer.flipX = spriteRenderer.flipX ? false : true;
            playerFront = playerFront == PlayerFront.right ? PlayerFront.left : PlayerFront.right;
        }
        if (hitRoot == PlayerFront.right)
        {
            rig2d.velocity = new Vector2(1f, rig2d.velocity.y);
        }
        else
        {
            rig2d.velocity = new Vector2(-1f, rig2d.velocity.y);
        }
        int damage=0;
		if (isMainPlayer) {
			switch (type)
			{
			case AttackType.normal:
                damage = 20;        
				    break;
			case AttackType.heavy:		
                damage = 10;
                    break;
			case AttackType.heavy_Lset:
                damage = 5;                
				    break;
			}
            hp -= damage;
            if(damage!=0)
            {
				PhotonGlobal.PS.sendMessage (uid, (int)_MessageType.Damage, damage.ToString ());
				Debug.Log ("Get Damage : " + damage);
                //Character_ShowUI ui = new Character_ShowUI(uid,damage.ToString(), _MessageType.Damage);
               // Debug.Log("插");
            }
			if(hp<=0)
				animator.SetBool(getTagHash(AnimationTag.IsDead),true);
			
			animator.SetTrigger(getTagHash(AnimationTag.Damage));
		}
		longCrouch = false;
       
    }
    public void OnDeath()
    {
        isDeath = true;
        effecter.SetTrigger("Death");
       // effecter.Play("Death", 0);
    }
    private void OnBorkenDef()
    {
        Debug.Log("OnBorkenDef");
        effecter.SetTrigger("DefBroken");
        //effecter.Play("DefBroken", 0);
        StartCoroutine("PunchAttack", 2f);
    }
    private IEnumerator PunchStiff(float WaitSecond)
    {
        this.canAction = false;
        this.Stiff = true;
        yield return new WaitForSeconds(WaitSecond);
        this.canAction = true;
        this.Stiff = false;
    }
    private IEnumerator PunchAttack(float WaitSecond)
    {
        this.canAttack = false;
        this.speed = 0.7f;
        yield return new WaitForSeconds(WaitSecond);
        this.canAttack = true;
        this.speed = 1f;
    }
    private RaycastHit2D[] RayCast(AttackType type)
    {
        RaycastHit2D[] objAry;
        Vector3 form;
        float lone =0f;
        if (!spriteRenderer.flipX)
        {
			form = transform.right;          
        }
        else
        {
			form = -transform.right;         
        }
        switch (type)
        {
            case AttackType.normal:
                
			lone = this.transform.localScale.x * 0.4f ;
                break;
            case AttackType.heavy:
			lone = this.transform.localScale.x *0.5f;
                break;
            case AttackType.heavy_Lset:
			lone = this.transform.localScale.x* 0.8f ;
                break;
        }

		objAry = Physics2D.RaycastAll(transform.position, form, lone);
		List<RaycastHit2D> result = new List<RaycastHit2D> ();

		foreach (RaycastHit2D hit in objAry) {
			if (hit.transform.tag == "Player" || hit.transform.tag == "MainPlayer")
				result.Add (hit);
		}
        

        // Physics2D.Raycast(transform.position, form, lone, plyaerMask);
		return result.ToArray();
    }
    private void OnAniStateCheange(int nextHash)
    {
		//Debug.Log ("Get next Hash : " + nextHash);
        animatorState = AnimatorHashpool[nextHash];
    }
    private void CheckAnimatorClip()
    {
        switch (animatorState)
        {
            case (Animator_State.Stand):
                if(Math.Abs(axis)>0.5f)
                {
                    animatorClip = Animator_Clip.Run;
                }
                else
                {
                    animatorClip = Animator_Clip.Idle;
                }
                break;
            case (Animator_State.Jumping):
                if (rig2d.velocity.y>=3)
                {
                    animatorClip = Animator_Clip.Jump_up;
                }else if(rig2d.velocity.y <1.5 && rig2d.velocity.y > 1)
                {
                    animatorClip = Animator_Clip.Jump_MidAir;
                }
                else if (rig2d.velocity.y <=-1.7 )
                {
                    animatorClip = Animator_Clip.Jump_Fall;
                }
                break;
            case (Animator_State.Crouch):
                animatorClip = Animator_Clip.Crouch;
                break;
            case (Animator_State.Attack1):
                animatorClip = Animator_Clip.Attack1;
                break;
            case (Animator_State.Attack2):
                animatorClip = Animator_Clip.Attack2;
                break;
            case (Animator_State.Defense):
                animatorClip = Animator_Clip.Defense;
                break;
            case (Animator_State.Damage):
                animatorClip = Animator_Clip.Damage;
                break;
            case (Animator_State.Knockdown):
                animatorClip = Animator_Clip.Knockdown;
                break;
            case (Animator_State.Landing):
                animatorClip = Animator_Clip.Landing;
                break;
            case (Animator_State.StandUp):
                animatorClip = Animator_Clip.StandUp;
                break;
        }
        
    }
    private int getTagHash(AnimationTag tag)
    {
        if(TagHashpool.ContainsKey(tag.ToString()))
        {
            return TagHashpool[tag.ToString()];
        }
        TagHashpool[tag.ToString()] = Animator.StringToHash(tag.ToString());
        return TagHashpool[tag.ToString()];
    }
	void OnTriggerEnter2D(Collider2D collider){
		if (collider.gameObject.tag == "HiddenObject") {
			if (isMainPlayer) {
				Material mat = collider.gameObject.GetComponent<MeshRenderer> ().material;
				mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 0.5f);

				HiddenObjectScript script = collider.GetComponent<HiddenObjectScript> ();
				List<BattleSpriteAction> players = script.getInsidePlayer ();
				foreach (BattleSpriteAction pl in players)
					pl.gameObject.GetComponent<SpriteRenderer> ().enabled = true;
			}
		
		}

	}
	void OnTriggerStay2D(Collider2D collider){
		if (collider.gameObject.tag == "HiddenObject") {
			HiddenObjectScript script = collider.GetComponent<HiddenObjectScript> ();
			if (isMainPlayer) {
				Material mat = collider.gameObject.GetComponent<MeshRenderer> ().material;
				mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 0.5f);
			}

			if (animatorState == Animator_State.Crouch && !isMainPlayer) {
				script.AddPlayer (uid, this);
				Debug.Log ("HIDE");
			} else {
				script.RemovePlayer (uid);
			}

		}
	}


	void OnTriggerExit2D(Collider2D collider){
		if (collider.gameObject.tag == "HiddenObject" && isMainPlayer) {
			Material mat = collider.gameObject.GetComponent<MeshRenderer> ().material;
			mat.color = new Color (mat.color.r, mat.color.g, mat.color.b, 1);

			HiddenObjectScript script = collider.GetComponent<HiddenObjectScript> ();

			List<BattleSpriteAction> players = script.getInsidePlayer ();
			foreach (BattleSpriteAction pl in players) {
				if (pl.gameObject.Equals (this.gameObject))
					continue;
				pl.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			}
				
		}

	}
}
