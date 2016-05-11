using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class TextUpdate : MonoBehaviour {
	
	Text mph;
	Text distance;

	// Use this for initialization
	void Start () {
		mph = transform.FindChild("MPH").GetComponent<Text>();
		distance = transform.FindChild("GPS").GetComponent<Text>();
		InvokeRepeating("DecreaseDistance", 8, 5F);
		StartCoroutine(ChangeSpeed(15, 5, 5)); // :05 = 2, at :10 be at 14
		StartCoroutine(ChangeSpeed(35, 5, 10)); // at :15 be at 35
		StartCoroutine(ChangeSpeed(50, 5, 15)); // at :20 be at 50
		StartCoroutine(ChangeSpeed(48, 2, 20)); // at :22 be at 48
		StartCoroutine(ChangeSpeed(50, 3, 22)); // at :25 be at 50
		StartCoroutine(ChangeSpeed(53, 3, 25)); // at :28 be at 53
		StartCoroutine(ChangeSpeed(55, 7, 28)); // at :35 be at 55
		StartCoroutine(ChangeSpeed(48, 5, 35)); // at :40 be at 48
		StartCoroutine(ChangeSpeed(50, 2, 40)); // at :45 be at 50
		StartCoroutine(ChangeSpeed(53, 5, 45)); // at :50 be at 53
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator ChangeSpeed(int end, double duration, int time) {
		yield return new WaitForSeconds(time);
		float delayMultiplier = 1;
		float delayStatic = 0.5f;
		float delay;
		// If speedDifference is positive, we are increasing speed, else decreasing speed
		int speedDifference = end - int.Parse(mph.text);
		
		double changeOverTime = speedDifference / duration;
		double changePerInterval = changeOverTime / 2;
		int intervals = Convert.ToInt32(duration * 2);
		
		if (changePerInterval < Mathf.Abs(1) ) {
			delayMultiplier = Mathf.Abs((float)(1 / changePerInterval));
			changePerInterval *= delayMultiplier;
			intervals = Convert.ToInt32(delayMultiplier);
			delay = delayMultiplier * delayStatic;
		} 
		else {
			delayMultiplier = (float)Math.Floor(changePerInterval) / (float)changePerInterval; // .952
			delay = delayMultiplier * delayStatic;
			float temp = speedDifference / (float)Math.Floor(changePerInterval * 2);
			intervals = Convert.ToInt32(temp / delay);
			changePerInterval = Math.Floor(changePerInterval);
			// Debug.Log("INTERVALS: " + temp + " / " + delay + " = " + intervals);
		}
		Debug.Log("Doing " + intervals + " intervals of " + changePerInterval + " every " + delay + " seconds for " + duration + " seconds");
		// Debug.Log("CHANGING TO " + end + " mph (" + speedDifference + ") at time " + time + " over " + duration + " seconds");
		// Debug.Log("INTERVALS: " + intervals + " -- PER INTERVAL: " + changePerInterval);
		// Debug.Log(changePerInterval / 1);
			
		StartCoroutine(Change(time, changePerInterval, intervals, delay, duration));
	}
	
	IEnumerator Change(int time, double changePerInterval, int intervals, float delay, double duration) {
		while(duration > 0) {
			mph.text = Convert.ToInt32(double.Parse(mph.text) + changePerInterval).ToString();
			duration -= delay;
			yield return new WaitForSeconds(delay);
		}
	}
	
	void DecreaseDistance() {
		double tempDist = Double.Parse(distance.text);
		distance.text = (tempDist - 0.1).ToString();
	}
}
