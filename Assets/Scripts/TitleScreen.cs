using UnityEngine;
using System.Collections;

public class TitleScreen : MonoBehaviour {
	
	public GUISkin customSkin;
	public int buttonW = 100;
	public int buttonH = 50;
	
	private float halfScreenW;
	private float halfButtonW;
	
	// Use this for initialization
	void Start () {
		halfButtonW = buttonW / 2;
		halfScreenW = Screen.width / 2;
	}
	
	// draws the GUI every frame:
	void OnGUI () {
		GUI.skin = customSkin;
		if (GUI.Button(new Rect(halfScreenW - halfButtonW, 320, buttonW, buttonH), "Vs Player")) {
			Game.HasDumbAI = false;
			Application.LoadLevel("game");
		}

		if (GUI.Button(new Rect(halfScreenW - halfButtonW, 405, buttonW, buttonH), "Vs Dumb AI")) {
			Game.HasDumbAI = true;
			Application.LoadLevel("game");
		}
	}
}
