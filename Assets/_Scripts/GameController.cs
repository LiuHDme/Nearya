using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class GameController : MonoBehaviour {

	public Image backgroundImage;
	public float flashSpeed = 1f;
	public Color flashColor = new Color (0f, 0f, 0f);

	private GameObject informationGO;
	private bool firstStart = true;
	private float timer;
	private Text informationText;

	void Start () {
		Cursor.visible = false;
		timer = Time.time;
	}

	void Awake() {
		informationGO = GameObject.Find ("BackGroundInformation");
		informationText = informationGO.GetComponent<Text> ();
	}

	void Update () {
		if (Input.GetKey (KeyCode.Escape)) {
			Cursor.visible = true;
		}
		if (Input.GetKeyUp (KeyCode.Escape)) {
			Cursor.visible = false;
		}
		if (Input.GetButtonDown ("Fire1"))
			firstStart = false;
		if (firstStart) {
			backgroundImage.color = flashColor;
		}
		else {
			backgroundImage.color = Color.Lerp (backgroundImage.color, Color.clear, flashSpeed * Time.deltaTime);
			informationText.enabled = false;
		}
	}
}
