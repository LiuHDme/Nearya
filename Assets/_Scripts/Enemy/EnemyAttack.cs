using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {
    public float timeBetweenAttacks = 4f;	// 攻击间隔
    public int attackDamage = 10;	// 攻击伤害

	// 一些私有变量
    Animator anim;
    GameObject player;
    PlayerHealth playerHealth;	// 玩家生命脚本
    EnemyHealth enemyHealth;	// 敌人生命脚本
    bool playerInRange;		// 玩家是否在攻击范围内
    float timer;
	AnimatorStateInfo stateInfo;

    void Awake () {
		// 获得引用
        player = GameObject.FindGameObjectWithTag ("Player");
        playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent<EnemyHealth>();
        anim = GetComponent <Animator> ();
    }

    void OnTriggerEnter (Collider other) {
        if(other.gameObject == player) {	// 如果玩家进入攻击范围
			anim.SetBool("PlayerInRange", true);
            playerInRange = true;	// 设置状态
        }
    }

    void OnTriggerExit (Collider other) {
        if(other.gameObject == player) { // 如果玩家离开攻击范围
			anim.SetBool("PlayerInRange", false);
            playerInRange = false;	// 设置状态
        }
    }

    void Update () {
        timer += Time.deltaTime;

        if(timer >= timeBetweenAttacks && playerInRange && enemyHealth.currentHealth > 0) {
			stateInfo = anim.GetCurrentAnimatorStateInfo(0);
			if(stateInfo.fullPathHash != Animator.StringToHash("Base Layer.Hurt")) {
            	Attack ();	// 攻击
			}
        }

        if(playerHealth.currentHealth <= 0) {	// 如果玩家生命值 ≤ 0
            anim.SetTrigger ("PlayerDead");		// 设置动画（此时为 IDle）
        }
    }

    void Attack () {
        timer = 0f;
        if(playerHealth.currentHealth > 0) {	// 确认玩家是否还活着
            playerHealth.TakeDamage (attackDamage);	// 损失玩家的生命值
        }
    }
}
