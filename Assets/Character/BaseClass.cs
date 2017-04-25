using UnityEngine;
public class Character_Login
{
    public int hp;		        //血量
    public Vector3 pos;        //出現位置
    public string nicName;     //角色暱稱
    public string uid;
    public bool isMainPlayer;
    public Character_Login(string _nicName, string _uid, int _hp, Vector3 _pos,bool _isMainPlayer = false)
    {
        hp = _hp;
        pos = _pos;
        nicName = _nicName;
        uid = _uid;
        isMainPlayer = _isMainPlayer;
    }
}
public class Character_Move
{
    public Vector3 pos;
    public bool isFromRight;
    public string uid;
    public Animator_Clip Clip;
	public int Health;
	public Character_Move(string _uid, Vector3 _pos, Animator_Clip _Clip,bool _isFromRight,int _health)
    {
        pos = _pos;
        uid = _uid;
        Clip = _Clip;
        isFromRight = _isFromRight;
		Health = _health;
    }
}
class PlayerData
{

}



