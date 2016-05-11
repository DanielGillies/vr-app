using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {

	// Use this for initialization
	public static bool VRMODE = false;
	
	public bool getVRMODE() {
		return VRMODE;
	}
	
	public void setVRMODE(bool status) {
		VRMODE = status;
		Debug.Log("SET TO " + status);
	}
	
	void Start() {
		// GameObject camera = GameObject.Find("RearviewMain");
		// GameObject events = GameObject.Find("EventSystem");
		// Settings settings = events.GetComponent<Settings>();
		// setVRMODE(true);
		// Cardboard vrSetting = camera.GetComponent<Cardboard>();
		// vrSetting.VRModeEnabled = VRMODE;
		// Debug.Log(vrSetting.VRModeEnabled);
	}
}
