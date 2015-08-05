using UnityEngine;
using System.Collections;

public class BackKey : MonoBehaviour
{
	public void Update () {
		if (Application.platform == RuntimePlatform.Android) {
			if (Input.GetKeyUp(KeyCode.Escape)) {
				//quit application on return button
				Application.Quit();
				return;
			}
		}
	}
}