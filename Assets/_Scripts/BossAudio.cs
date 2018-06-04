using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour {

	GameObject Player;
	GameObject backGroundAudio;
	AudioSource audioBoss;
	AudioSource audioBackGround;

	void Awake() {
		Player = GameObject.Find ("Player");
	}

	// Use this for initialization
	void Start () {
		backGroundAudio = GameObject.Find ("GameController");
		audioBoss = GetComponent<AudioSource> ();
		audioBackGround = backGroundAudio.GetComponent<AudioSource> ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == Player) {
			Debug.Log ("player enters");
		}
	}
}
