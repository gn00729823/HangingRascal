using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneSwitchManager : MonoBehaviour {

	private static string floorSceneName = "FloorScene";

	public static string LoadingName;

	private Slider slider;
	public float tempValue;
	AsyncOperation asyn;

	public bool allowChangeScene;

	public static void LoadFloorScene(int nextFloor){
		GameData.CurrentFloor = nextFloor;
		Application.LoadLevelAsync (floorSceneName);
	}

	void Awake(){
		slider = transform.FindChild ("Slider").GetComponent<Slider>();
	}
	// Use this for initialization
	void Start () 
	{
		StartCoroutine ("BeginLoading");
	}

	// Update is called once per frame
	void LateUpdate () 
	{
		slider.value = asyn.progress;

		if (slider.value < 0.9f) 
		{
			tempValue = slider.value;
		} 
		else 
		{
			slider.value = tempValue;
			float rnd = Random.Range (0.1f,0.2f);
			tempValue += Time.deltaTime * rnd;
			tempValue = Mathf.Clamp(tempValue,0.9f,1);
		}

		if (tempValue >= 1 && allowChangeScene) 
		{
			asyn.allowSceneActivation = true;
		}
	}

	IEnumerator BeginLoading()
	{
		asyn = Application.LoadLevelAsync (floorSceneName);
		asyn.allowSceneActivation = false;
		yield return asyn;
	}

	public static void LoadScene(string value)
	{
		LoadingName = value;
		Application.LoadLevel ("LoadingScene");
	}

}
