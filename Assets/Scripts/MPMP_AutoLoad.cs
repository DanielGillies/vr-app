using UnityEngine;
using System.Collections;

 public class MPMP_AutoLoad : MonoBehaviour {
    public monoflow.MPMP mpmp;//reference to your mpmp instance
	
	IEnumerator Start () {
		mpmp.OnInit+=MyOnInit;//When mpmp is ready your method is called
		return null;
	}

	//This is the method that should be called when mpmp fires OnInit event
	public void MyOnInit(monoflow.MPMP mpmp){
		// DebugConsole.Log("GOT TO THE LOAD2", "normal");
		mpmp.Load();//mpmp must have a valid videopath
	}

}
