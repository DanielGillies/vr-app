using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour {
	
	Text mph;

	// Use this for initialization
	void Start () {
		mph = GetComponent<Text>();
		StartCoroutine(WaitFor());
	
	}
	
	IEnumerator WaitFor() {
		yield return new WaitForSeconds(2);
		int temp = int.Parse(mph.text);
		mph.text = (temp + 3).ToString();
		StopCoroutine(WaitFor());
		StartCoroutine(WaitFor());
 	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
