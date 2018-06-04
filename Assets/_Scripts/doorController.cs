using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doorController : MonoBehaviour {

	Animator anim;
	GameObject player;
	GameObject bg;

	AudioSource bossAudio;
	AudioSource bgAudio;

	bool if_enter = false;

	void Awake() {
		player = GameObject.FindGameObjectWithTag ("Player");
		anim = GetComponent<Animator> ();
		bossAudio = GetComponent<AudioSource> ();
		bg = GameObject.Find ("GameController");
		bgAudio = bg.GetComponent<AudioSource> ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == player) {
			anim.SetBool ("character_nearby", true);
			if (!if_enter) {
				if (this.gameObject.name == "BossBgmController") {
					bossAudio.Play ();
					bgAudio.Stop ();
				}
				if_enter = true;
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject == player) {
			anim.SetBool ("character_nearby", false);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
