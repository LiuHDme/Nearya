using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {
    Transform player;	// 玩家位置
    PlayerHealth playerHealth;	// 玩家生命值脚本
    EnemyHealth enemyHealth;	// 敌人生命值脚本
    UnityEngine.AI.NavMeshAgent nav;	// 寻路系统
	bool playerInRange;

	void OnTriggerEnter (Collider other) {
		if(other.gameObject == player) {	// 如果玩家进入攻击范围
			playerInRange = true;	// 设置状态
		}
	}
		
	void OnTriggerExit (Collider other) {
		if(other.gameObject == player) { // 如果玩家离开攻击范围
			playerInRange = false;	// 设置状态
		}
	}

    void Awake () {
		// 获取引用
        player = GameObject.FindGameObjectWithTag ("Player").transform;
        playerHealth = player.GetComponent <PlayerHealth> ();
        enemyHealth = GetComponent <EnemyHealth> ();
        nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
    }
		
    void Update () {
		if (!playerInRange) {
			if (enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0) {	// 如果敌人和玩家都活着
				nav.SetDestination (player.position);	// 向玩家的方向前进

			} else {	// 否则停止寻路
				nav.enabled = false;
			}
		} else {
			nav.enabled = true;
		}
	}

	public void StopNav() {
		nav.enabled = false;
	}

	public void enableNav() {
		nav.enabled = true;
	}
}
