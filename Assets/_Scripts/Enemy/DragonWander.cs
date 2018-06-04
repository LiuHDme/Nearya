using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonWander : MonoBehaviour {

	private GameObject playerUnit;          // 获取玩家单位  
	private Animator thisAnimator;          // 自身动画组件  
	private Vector3 initialPosition;        // 初始位置  

	public float wanderRadius;          // 游走半径，移动状态下，如果超出游走半径会返回出生位置
	public float defendRadius;          // 自卫半径，玩家进入后怪物会追击玩家，当距离<攻击距离则会发动攻击（或者触发战斗）  
	public float chaseRadius;           // 追击半径，当怪物超出追击半径后会放弃追击，返回追击起始位置  

	public float attackRange;        // 攻击距离  
	public float walkSpeed;          // 移动速度  
	public float runSpeed;           // 跑动速度  
	public float turnSpeed;          // 转身速度，建议

	public float[] actionWeight = {3000, 3000, 4000};         // 设置待机时各种动作的权重，顺序依次为呼吸、观察、移动  
	public float actRestTime;            // 更换待机指令的间隔时间  
	private float lastActTime;           // 最近一次指令时间  

	private float distanceToPlayer;         // 怪物与玩家的距离  
	private float diatanceToInitial;        // 怪物与初始位置的距离  
	private Quaternion targetRotation;      // 怪物的目标朝向  

	public bool is_Running = false;  
	private bool is_Dying = false;
	AnimatorStateInfo stateInfo;

	private AudioSource audio;
	private GameObject go_audioScream;
	private AudioSource audioScream;

	void Start () {  
		playerUnit = GameObject.FindGameObjectWithTag("Player");   
		// 保存初始位置信息  
		initialPosition = gameObject.GetComponent<Transform>().position;  
		// 检查并修正怪物设置   
		attackRange = Mathf.Min(defendRadius, attackRange);  
		wanderRadius = Mathf.Min(chaseRadius, wanderRadius);  
		// 随机一个待机动作  
		RandomAction();  

		go_audioScream = GameObject.Find ("dragonScream_audio");
		audioScream = go_audioScream.GetComponent<AudioSource> ();
	}

	void Awake() {
		thisAnimator = GetComponent<Animator>();
		audio = GetComponent<AudioSource> ();
	}

	// 根据权重随机待机指令
	void RandomAction() {
		// 获取当前动作
		stateInfo = thisAnimator.GetCurrentAnimatorStateInfo (0);
		// 更新行动时间
		lastActTime = Time.time;
		// 根据权重随机
		float number = Random.Range(0, actionWeight[0] + actionWeight[1] + actionWeight[2]);
		if (number <= actionWeight[0]) {  
			thisAnimator.SetTrigger("Idle");  
		} else if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1]) {
			thisAnimator.SetTrigger("Idle02");  
		}  
		if (actionWeight[0] + actionWeight[1] < number && number <= actionWeight[0] + actionWeight[1] + actionWeight[2]) {
			// 随机一个朝向  
			targetRotation = Quaternion.Euler(0, Random.Range(1, 5) * 90, 0);
			thisAnimator.SetTrigger("Walk");
		}
	}

	void Update () {
		if (is_Dying || GetComponent<EnemyHealth>().isDead) {
			
			return;
		}
		stateInfo = thisAnimator.GetCurrentAnimatorStateInfo (0);
		// 待机状态，等待actRestTme后重新随机指令
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Idle")) {
//			Debug.Log ("idle");
			if (Time.time - lastActTime > actRestTime) {
				RandomAction ();         // 随机切换指令  
			}
			// 该状态下的检测指令  
			EnemyDistanceCheck ();
		} else if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Idle02")) {
//			Debug.Log ("idle02");
			if (Time.time - lastActTime > actRestTime) {
				RandomAction ();         // 随机切换指令  
			}
			// 该状态下的检测指令  
			EnemyDistanceCheck ();
		} else if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Walk")) {
//			Debug.Log ("walk");
			transform.Translate (Vector3.forward * Time.deltaTime * walkSpeed);  
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, turnSpeed);  
			if (Time.time - lastActTime > actRestTime) {  
				RandomAction ();         // 随机切换指令  
			}  
			// 该状态下的检测指令
			WanderRadiusCheck ();
		} else if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Chease")) {
//			Debug.Log ("chease");
			if (!is_Running) {  
				thisAnimator.SetTrigger ("Chease");
				is_Running = true;  
			}
			transform.Translate (Vector3.forward * Time.deltaTime * runSpeed);  
			// 朝向玩家位置
			targetRotation = Quaternion.LookRotation (playerUnit.transform.position - transform.position, Vector3.up);  
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, turnSpeed);  
			// 该状态下的检测指令
			ChaseRadiusCheck ();  
		} else if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Return")) {
//			Debug.Log ("return");
			// 朝向初始位置移动  
			targetRotation = Quaternion.LookRotation (initialPosition - transform.position, Vector3.up);  
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, turnSpeed);  
			transform.Translate (Vector3.forward * Time.deltaTime * runSpeed);  
			// 该状态下的检测指令  
			ReturnCheck ();  
		} else if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Basic Attack")
			|| stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Claw Attack")) {
//			Debug.Log ("attack");
			if (Time.time - lastActTime > actRestTime) {
				RandomAttack ();         // 随机切换指令  
				audioScream.Play();
			}
			// 该状态下的检测指令
			AttackCheck ();
		}
		// 如果使用 Claw Attack，主角将被击退
		if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Claw Attack")) {
			playerUnit.GetComponent<PlayerController> ().Knockback ();
		}
	}

	// 原地呼吸、观察状态的检测  
	void EnemyDistanceCheck() {  
		distanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);  
		if (distanceToPlayer < attackRange) {
			thisAnimator.SetTrigger ("Basic Attack");  
		} else if (distanceToPlayer < defendRadius) {
			thisAnimator.SetTrigger ("Chease");  
		}
	}

	// 游走状态检测，检测敌人距离及游走是否越界
	void WanderRadiusCheck() {
		distanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);  
		diatanceToInitial = Vector3.Distance(transform.position, initialPosition);  

		if (distanceToPlayer < attackRange)  
		{  
			thisAnimator.SetTrigger ("Basic Attack");  
		} else if (distanceToPlayer < defendRadius)  
		{  
			thisAnimator.SetTrigger ("Chease");  
		}   
		if (diatanceToInitial > wanderRadius) {
			// 朝向调整为初始方向  
			targetRotation = Quaternion.LookRotation(initialPosition - transform.position, Vector3.up);  
		}  
	}

	// 追击状态检测，检测敌人是否进入攻击范围以及是否离开警戒范围  
	void ChaseRadiusCheck() {
		distanceToPlayer = Vector3.Distance(playerUnit.transform.position, transform.position);  
		diatanceToInitial = Vector3.Distance(transform.position, initialPosition);  

		if (distanceToPlayer < attackRange) {
			thisAnimator.SetTrigger ("Basic Attack"); 
		}  
		// 如果超出追击范围或者敌人的距离超出警戒距离就返回  
		if (diatanceToInitial > chaseRadius) {
			thisAnimator.SetTrigger ("Return"); 
		}  
	}  

	// 超出追击半径，返回状态的检测，不再检测敌人距离  
	void ReturnCheck() {
		diatanceToInitial = Vector3.Distance(transform.position, initialPosition);  
		// 如果已经接近初始位置，则随机一个待机状态  
		if (diatanceToInitial < 0.5f) {
			is_Running = false;
			RandomAction();
		}  
	} 

	// 攻击时如果玩家离开攻击区域则进行追击，如果玩家死亡则回到原处
	void AttackCheck() {
		distanceToPlayer = Vector3.Distance (playerUnit.transform.position, transform.position);
		// 如果离开攻击区域则追击
		if (distanceToPlayer > attackRange) {
			thisAnimator.SetTrigger ("Chease");
		} else if (distanceToPlayer <= attackRange) {
			// 调整面向玩家方向
			targetRotation = Quaternion.LookRotation (playerUnit.transform.position - transform.position, Vector3.up);  
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, turnSpeed);
		}
		// 如果玩家死亡则回到原处
		if (playerUnit.GetComponent<PlayerHealth> ().currentHealth <= 0) {
			thisAnimator.SetTrigger ("Return");
		}
	}

	// 根据权重随机待机指令   
	void RandomAttack() {
		// 获取当前动作
		stateInfo = thisAnimator.GetCurrentAnimatorStateInfo (0);
		// 更新行动时间
		lastActTime = Time.time;
		// 根据权重随机
		float number = Random.Range(0, actionWeight[0] + actionWeight[1]);  
		if (number <= actionWeight[0]) {
			thisAnimator.SetTrigger("Basic Attack");
		} else if (actionWeight[0] < number && number <= actionWeight[0] + actionWeight[1])  {
			thisAnimator.SetTrigger("Claw Attack");
		}
	}
}
