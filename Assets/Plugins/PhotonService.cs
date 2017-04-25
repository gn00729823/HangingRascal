using System.Collections;
using System.Collections.Generic;
using System;
using ExitGames.Client.Photon;
using ServerOperationCode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PhotonService : IPhotonPeerListener {

	protected PhotonPeer peer;
	protected bool ServerConnected;
	protected string DebugMessage;

	public PhotonService(){
		peer = null;
		ServerConnected = false;
		DebugMessage = "";
	}

	public PhotonPeer ezPeer{
		get{ 
			return this.peer;
		}
	}

	public bool ezServerConnecting{
		get{ 
			return this.ServerConnected;
		}
	}

	public string ezDebugMessage{
		get{ 
			return this.DebugMessage;
		}
	}

	//Connect to Server
	public void Connect(string IP , int port, string ServerName){
		try{
			string ServerAddress = IP + ":" + port.ToString ();
			this.peer = new PhotonPeer (this, ConnectionProtocol.Tcp);
			if(!this.peer.Connect (ServerAddress,ServerName)){
				ConnectEvent(false);
			}
		}catch(Exception EX){
			ConnectEvent(false);
			throw EX;
		}
	}

	public void DisConnect(){
		try{
			if(peer != null){
				this.peer.Disconnect();
			}	
		}catch (Exception EX){
			throw EX;
		}
	}

	public void Service(){
		try{
			if(this.peer != null){
				this.peer.Service();
			}	
		}catch(Exception EX){
			throw EX;
		}
	}

	public void DebugReturn (DebugLevel level, string message)
	{
		this.DebugMessage = message;
	}

	public void OnOperationResponse (OperationResponse operationResponse)
	{
		switch (operationResponse.OperationCode) {
		//Login

		case (byte)OperationCode.Join:
			MessageEvent ("Join Success");
			if (operationResponse.ReturnCode == (short)0) {
				string message = "";
				foreach (KeyValuePair<byte,object> obj in operationResponse.Parameters) {
					MessageEvent (obj.Key + " || " + obj.Value );
				}
				MemberGlobal.UniqueID = Convert.ToString (operationResponse.Parameters [(byte)ActorParameter.UniqueID]);
				onMapData (Convert.ToString(operationResponse.Parameters [(byte)GameParameter.GameMap]));
				onChangeScene( Convert.ToString(operationResponse.Parameters [(byte)ActorParameter.Floor] ));

			} else {
				DebugMessage = operationResponse.DebugMessage;
			}
			break;

		case (byte)ServerOperationCode.MapParameter.MapParm:
			

			break;

		case (byte)ServerOperationCode.OperationCode.GetProperties:
			Debug.Log ("GetProperties");
			foreach (KeyValuePair<byte,object> dic in operationResponse.Parameters) {
				Dictionary<byte,object> innerDic = dic.Value as Dictionary<byte,object>;
				string name = Convert.ToString (innerDic [(byte)ServerOperationCode.ActorParameter.NickName]);
				string uID = Convert.ToString (innerDic [(byte)ServerOperationCode.ActorParameter.UniqueID]);
				int hp = Convert.ToInt32 (innerDic [(byte)ServerOperationCode.GameParameter.Health]);
				float x = float.Parse (innerDic [(byte)ServerOperationCode.GameParameter.PositionX].ToString ());
				float y = float.Parse (innerDic [(byte)ServerOperationCode.GameParameter.PositionY].ToString ());
				float z = float.Parse (innerDic [(byte)ServerOperationCode.GameParameter.PositionZ].ToString ());
				Vector3 pos = new Vector3 (x, y, z);

				LoginEvent (name, uID, hp, pos);
			}
			break;
		}

	}

	public void OnStatusChanged (StatusCode statusCode)
	{
		switch (statusCode) {
		case StatusCode.Connect:
			ServerConnected = true;
			ConnectEvent(true);

			break;
		case StatusCode.Disconnect:
			this.peer = null;
			ServerConnected = false;
			ConnectEvent(false);
			break;
		}
	}

	//Server Event
	public void OnEvent (EventData eventData)
	{

		switch (eventData.Code) {
		case (byte)ServerOperationCode.MapParameter.MapParm:
			{
				MessageEvent ("get MapParm");
				if (onMapEvent != null) {
					onMapEvent (
						Convert.ToString (eventData.Parameters [(byte)ServerOperationCode.MapParameter.UID]),
						Convert.ToInt32 (eventData.Parameters [(byte)ServerOperationCode.MapParameter.Type]),
						Convert.ToBoolean (eventData.Parameters [(byte)ServerOperationCode.MapParameter.Create]),
						Convert.ToInt32 (eventData.Parameters [(byte)ServerOperationCode.MapParameter.PositionX]),
						Convert.ToInt32 (eventData.Parameters [(byte)ServerOperationCode.MapParameter.PositionY]),
						Convert.ToInt32 (eventData.Parameters [(byte)ServerOperationCode.MapParameter.PositionZ])
					);
				}
			}
			break;

		case (byte)OperationCode.Join:
			{
				MessageEvent ("get Join Event");
				//string _nicName, string _uid, int _hp, Vector3 _pos,bool _isMainPlayer = false
				string name = Convert.ToString (eventData.Parameters [(byte)ServerOperationCode.ActorParameter.NickName]);
				string uID = Convert.ToString (eventData.Parameters [(byte)ServerOperationCode.ActorParameter.UniqueID]);
				int hp = Convert.ToInt32 (eventData.Parameters [(byte)ServerOperationCode.GameParameter.Health]);
				float x = float.Parse (eventData.Parameters [(byte)ServerOperationCode.GameParameter.PositionX].ToString ());
				float y = float.Parse (eventData.Parameters [(byte)ServerOperationCode.GameParameter.PositionY].ToString ());
				float z = float.Parse (eventData.Parameters [(byte)ServerOperationCode.GameParameter.PositionZ].ToString ());

				Vector3 pos = new Vector3 (x, y, z);

				LoginEvent (name, uID, hp, pos);

			}
			break;

		case (byte)OperationCode.Leave:
			{
				string uID = Convert.ToString (eventData.Parameters [(byte)ServerOperationCode.ActorParameter.UniqueID]);
				LeaveEvent (uID);
			}

			break;

		case (byte)ServerOperationCode.GameParameter.GameParm:
			{
				foreach (KeyValuePair<byte,object> obj in eventData.Parameters) {
					Dictionary<byte,object> innerDic = (Dictionary<byte,object>)obj.Value;

					float X = float.Parse (innerDic [(byte)ServerOperationCode.GameParameter.PositionX].ToString ());
					float Y = float.Parse (innerDic [(byte)ServerOperationCode.GameParameter.PositionY].ToString ());
					float Z = float.Parse (innerDic [(byte)ServerOperationCode.GameParameter.PositionZ].ToString ());
					Vector3 posision = new Vector3 (X, Y, Z);
					
					int state = Convert.ToInt32 (innerDic [(byte)ServerOperationCode.GameParameter.ActionState].ToString ());
					string UID = Convert.ToString (innerDic [(byte)ServerOperationCode.ActorParameter.UniqueID]);
					bool facing = Convert.ToBoolean (innerDic [(byte)ServerOperationCode.GameParameter.Facing]);
					int health = Convert.ToInt32 (innerDic [(byte)ServerOperationCode.GameParameter.Health].ToString ());
					CharacterUpdateEvent (UID, health, posision, state, health, facing);
				}
			}
			break;
		}
	}

	public void JoinRoom(int RoomIndex, Vector3 position = new Vector3()){
		try{
			var request = new OperationRequest{OperationCode = (byte)OperationCode.Join, Parameters = new Dictionary<byte, object>()};

			request.Parameters.Add((byte)ParameterKey.GameId,RoomIndex.ToString());

			var parameter = new ExitGames.Client.Photon.Hashtable{ 
				{(byte)ActorParameter.NickName,MemberGlobal.NickName},
				{(byte)ActorParameter.Money,MemberGlobal.Money},
				{(byte)ActorParameter.AccountID,MemberGlobal.AccountID},
				{(byte)GameParameter.Health,MemberGlobal.Health},
				{(byte)GameParameter.PositionX,position.x},
				{(byte)GameParameter.PositionY,position.y},
				{(byte)GameParameter.PositionZ,position.z}
			};

			request.Parameters.Add((byte)ParameterKey.ActorProperties,parameter);
			this.peer.OpCustom(request.OperationCode,request.Parameters,true,0);
			onPlayerChangeRoom();
		}catch(Exception EX){
			throw EX;
		}
	}

	public void CreateItem(int type, float X, float Y,float Z){
		try{
			var request = new OperationRequest{OperationCode = (byte)ServerOperationCode.MapParameter.MapParm, Parameters = new Dictionary<byte, object>()};
			request.Parameters.Add((byte)ServerOperationCode.MapParameter.PositionX,X);
			request.Parameters.Add((byte)ServerOperationCode.MapParameter.PositionY,Y);
			request.Parameters.Add((byte)ServerOperationCode.MapParameter.PositionZ,Z);
			request.Parameters.Add((byte)ServerOperationCode.MapParameter.Type,type);
			request.Parameters.Add((byte)ServerOperationCode.MapParameter.Create,true);

			this.peer.OpCustom(request.OperationCode,request.Parameters,true,0);
		}catch(Exception EX){
			throw EX;
		}
	}


	public void LeaveRoom(){
		try{
			var request = new OperationRequest{OperationCode = (byte)OperationCode.Leave, Parameters = new Dictionary<byte, object>()};
			request.Parameters.Add((byte)ActorParameter.UniqueID,MemberGlobal.UniqueID);
			this.peer.OpCustom(request.OperationCode,request.Parameters,true,0);
		}catch(Exception EX){
			throw EX;
		}
	}

	public void UpdateCharacterInfo(string UID, Vector3 position ,int clip,bool facing,int health){
		try{
			var request = new OperationRequest{OperationCode = (byte)ServerOperationCode.GameParameter.GameParm, Parameters = new Dictionary<byte, object>()};
			request.Parameters.Add((byte)ServerOperationCode.GameParameter.PositionX,position.x);
			request.Parameters.Add((byte)ServerOperationCode.GameParameter.PositionY,position.y);
			request.Parameters.Add((byte)ServerOperationCode.GameParameter.PositionZ,position.z);
			request.Parameters.Add((byte)ServerOperationCode.GameParameter.ActionState,clip);
			request.Parameters.Add((byte)ServerOperationCode.ActorParameter.UniqueID,UID);
			request.Parameters.Add((byte)ServerOperationCode.GameParameter.Facing,facing);
			request.Parameters.Add((byte)ServerOperationCode.GameParameter.Health,health);

			this.peer.OpCustom(request.OperationCode,request.Parameters,true,0);
		}catch(Exception EX){
			throw EX;
		}
	}


	public class RoomInfo{
		public string RoomName;
		public int limit;
		public string Description;
		public int PlayerCount;
		public double MaxBet;
		public double MinBet;
	}



	//Delegate for ConnectEvent
	public delegate void ConnectEventHandler(bool ConnectStatus);
	public event ConnectEventHandler ConnectEvent;

	public delegate void LoginEventHandler(string _nicName, string _uid, int _hp, Vector3 _pos,bool _isMainPlayer = false);
	public event LoginEventHandler LoginEvent;

	public delegate void CharacterDataHandler(string _uid, int _hp, Vector3 _pos,int _clip,int health,bool facing = false);
	public event CharacterDataHandler CharacterUpdateEvent;

	public delegate void CommonHandler();
	public event CommonHandler onPlayerChangeRoom;

	public delegate void LeaveEventHandler(string uid);
	public event LeaveEventHandler LeaveEvent;

	public delegate void GetAllRoomInfoEventHandler(bool RetStatus, string DebugMessage, List<RoomInfo> rooms);
	public event GetAllRoomInfoEventHandler GetAllRoomEvent;

	public delegate void RoomAutoBroadcast(string RoomName,int Limit, string Description, int PlayerCount,double MaxBet, double MinBet);
	public event RoomAutoBroadcast RoomAutoBroadcastEvent;

	public delegate void MessageHandler(string message);
	public event MessageHandler MessageEvent;
	public event MessageHandler onMapData;
	public event MessageHandler onChangeScene;

	public delegate void MapEvent( string UID , int type, bool create,float X, float Y,float Z);
	public event MapEvent onMapEvent;
}
