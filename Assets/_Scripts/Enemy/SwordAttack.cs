using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour {

	public int perDamage = 10;

	private Animator anim;
	private AnimatorStateInfo stateInfo;

	void Awake() {
		anim = GetComponent<Animator> ();
	}

	void OnCollisionEnter(Collision col) {
		if (col.collider.gameObject.tag == "Player")
			Debug.Log ("Get Player");
	}
}
