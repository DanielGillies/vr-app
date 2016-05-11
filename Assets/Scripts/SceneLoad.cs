using UnityEngine;
using System.Collections;

public class SceneLoad : MonoBehaviour {

    void Start() {
        GameObject camera = GameObject.Find("Main");
        Cardboard vrSetting = camera.GetComponent<Cardboard>();
        vrSetting.VRModeEnabled = Settings.VRMODE;
        Debug.Log(vrSetting.VRModeEnabled);
    }
}
