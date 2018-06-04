using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour {

	public int damagePerHit = 10;	// 每次伤害

	private Animator anim;

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Enemy") {
			// 只有当正在攻击时才会触发伤害
			AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
			if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Attack1")
			    || stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Attack2")
			    || stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Attack3")) {
				EnemyHealth enemyHealth = other.GetComponent<Collider> ().GetComponent<EnemyHealth> ();
				if (enemyHealth != null) {
					enemyHealth.TakeDamageFromHit (damagePerHit);
				}
			} else {
				EnemyHealth enemyHealth = other.GetComponent<Collider> ().GetComponent<EnemyHealth> ();
				if (enemyHealth != null) {
					enemyHealth.Recover ();
				}
			}
		}
	}

	void OnTriggerExit(Collider other) {
		AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo (0);
		if (other.tag == "Enemy") {
			EnemyHealth enemyHealth = other.GetComponent<Collider> ().GetComponent<EnemyHealth> ();
			if (enemyHealth != null) {
				enemyHealth.Recover ();
			}
		}
	}

	void Awake() {
		anim = GetComponent<Animator> ();
	}
}
