using UnityEngine;
using System.Collections;

public class Settings : MonoBehaviour {

	public static bool VRMODE = false;
	
	public bool getVRMODE() {
		return VRMODE;
	}
	
	public void setVRMODE(bool status) {
		VRMODE = status;
	}
	
}
