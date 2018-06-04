using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	
	public GameObject cam;		// 摄像机

	// 以下是远程射击参数
	public Transform shotSpanw;	// 发射位置
	public float fireRate = 0.1f;	// 子弹发射的间隔时间
	private float nextFire;	// 下一发子弹的时间

	// 以下是近战攻击连招参数
	private float nextHit;	// 下一次攻击的时间
	private bool firstHit;	// 连招_1
	private bool secondHit;	// 连招_2
	private bool thirdHit;	// 连招_3
	private bool attackStop;
	GameObject swordAudio_1;
	AudioSource audio_1;

	public float animSpeed = 1f;	// 动画速度

	// 前进速度
	public float forwardSpeed = 7.0f;
	// 后退速度
	public float backwardSpeed = 7.0f;
	// 被击退速度
	public float knockbackSpeed = 5.0f;

	private bool isRunning;
	Vector3 movement;
	Animator anim;
	Rigidbody playerRigidbody;

	// 以下是控制一些双击或连击时间的参数
	private float run_lastTime;
	private float run_curTime;
	private float hit_lastTime;
	private float hit_curTime;
	private float firstHit_lastTime;
	private float firstHit_curTime;
	private float secondHit_lastTime;
	private float thirdHit_lastTime;
	private float hitStop_lastTime;

	// 剑和枪
	public GameObject Sword;
	public GameObject Gun;
	public bool swordBack = true;	// 剑在背上
	private GameObject swordBackPosition;
	private GameObject swordHandPosition;
	private GameObject gunBackPosition;
	private GameObject gunHandPosition;

	void Start() {
		run_lastTime = Time.time;
		hit_lastTime = Time.time;
	}

	// 检测动作
	void Update () {
		// 判断双击W
		if (Input.GetKeyDown (KeyCode.W)) {
			run_curTime = Time.time;
			if (run_curTime - run_lastTime <= 0.5) {
				anim.SetBool ("IsRunning", true);
				isRunning = true;
			}
			run_lastTime = run_curTime;
		}
		if (Input.GetKeyUp (KeyCode.W)) {
			anim.SetBool ("IsRunning", false);
			isRunning = false;
		}

		// 切换武器
		if (Input.GetKeyDown (KeyCode.Q)) {
			if (swordBack) {	// 如果此时剑在背上就把它绑定到手上，并把枪绑定到背上
				Sword.transform.SetParent (swordHandPosition.transform, false);
				Gun.transform.SetParent (gunBackPosition.transform, false);
				swordBack = false;
			} else {
				Sword.transform.SetParent (swordBackPosition.transform, false);
				Gun.transform.SetParent (gunHandPosition.transform, false);
				swordBack = true;
			}
		}

		if (!swordBack) {
			// 近战攻击
			meleeAttack ();
		} else {
			// 远程射击
			if (Input.GetButton ("Fire2") && Time.time > nextFire) {
				anim.SetBool ("IsShooting", true);
				nextFire = Time.time + fireRate;
			}
			if (Input.GetButtonUp ("Fire2")) {
				anim.SetBool ("IsShooting", false);
			}
		}

	}

	void Awake() {
		anim = GetComponent<Animator> ();
		playerRigidbody = GetComponent<Rigidbody> ();
		swordAudio_1 = GameObject.Find ("Audio_1");
		audio_1 = swordAudio_1.GetComponent<AudioSource> ();
		swordBackPosition = GameObject.Find ("SwordBackPosition");
		swordHandPosition = GameObject.Find ("SwordHandPosition");
		gunBackPosition = GameObject.Find ("GunBackPosition");
		gunHandPosition = GameObject.Find ("GunHandPosition");
	}

	// 控制移动
	void FixedUpdate() {
		float v = Input.GetAxis ("Vertical");
		float h = Input.GetAxis ("Horizontal");
		Move (h, v);	// 移动（旋转由 Camera 控制）
		Animating (h, v);
	}

	void Move(float h, float v) {
		movement.Set(h, 0f, v); 
		if (v > 0.1) {	// 前进
			movement *= forwardSpeed;
		} else if (v < -0.1) {	// 后退
			movement *= backwardSpeed;
		}
		// 归一化，以免斜方向速度过快
		movement = movement.normalized * Time.deltaTime;
		// 重新使前进后退的速度改变（此时不影响斜方向）
		if (v > 0.1) {
			if (isRunning)
				movement.z *= forwardSpeed * 2;
			else {
				movement.z *= forwardSpeed;
			}
		} else if (v < -0.1) {
			movement.z *= backwardSpeed;
		}
		movement.x *= 3;
		// 使位移相对于自身坐标系而不是世界坐标系
		movement = transform.TransformDirection(movement);
//		playerRigidbody.MovePosition (transform.position + movement);
		transform.position += movement;
	}

	// 判断近战攻击以及连击状态
	void meleeAttack() {
		// 近战攻击
		if (Input.GetButtonDown ("Fire1")) {
			hit_curTime = Time.time;
			if (hit_curTime - hit_lastTime <= 0.8) {
				if (secondHit) {
					AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
					// 如果已经处于第三段攻击，则重新开始第一段攻击
					if (stateInfo.fullPathHash == Animator.StringToHash ("Base Layer.Attack3")) {
						audio_1.Play ();
						anim.SetBool ("Attacking_3", false);
						thirdHit = false;
						secondHit = false;
						anim.SetBool ("Attacking_1", true);
						hit_lastTime = Time.time;
						firstHit_lastTime = Time.time;
					} else {
						audio_1.Play ();
						anim.SetBool ("Attacking_3", true);
						thirdHit = true;
						anim.SetBool ("Attacking_2", false);
						secondHit = false;
						hit_lastTime = Time.time;
						thirdHit_lastTime = Time.time;
					}
				} else if (firstHit) {
					audio_1.Play ();
					anim.SetBool ("Attacking_1", false);
					anim.SetBool ("Attacking_2", true);
					firstHit = false;
					secondHit = true;
					hit_lastTime = Time.time;
					secondHit_lastTime = Time.time;
				} else {
					if (thirdHit) {
						audio_1.Play ();
						anim.SetBool ("Attacking_3", false);
						thirdHit = false;
					}
					audio_1.Play ();
					anim.SetBool ("Attacking_1", true);
					firstHit = true;
					hit_lastTime = Time.time;
					firstHit_lastTime = Time.time;
				}
			} else {	// 如果离上次攻击大于 0.8 秒
				audio_1.Play ();
				anim.SetBool ("Attacking_1", true);
				firstHit = true;
				hit_lastTime = Time.time;
				firstHit_lastTime = Time.time;
			}
		} else {
			if (firstHit && Time.time - firstHit_lastTime >= 0.8) {
				anim.SetBool ("Attacking_1", false);
				anim.SetBool ("AttackStop", true);
				firstHit = false;
				attackStop = true;
				hitStop_lastTime = Time.time;
			}
			else if (secondHit && Time.time - secondHit_lastTime >= 0.8) {
				anim.SetBool ("Attacking_2", false);
				anim.SetBool ("AttackStop", true);
				secondHit = false;
				attackStop = true;
				hitStop_lastTime = Time.time;
			}
			else if (thirdHit && Time.time - thirdHit_lastTime >= 0.8) {
				anim.SetBool ("Attacking_3", false);
				anim.SetBool ("AttackStop", true);
				thirdHit = false;
				attackStop = true;
				hitStop_lastTime = Time.time;
			}
			else if (attackStop && Time.time - hitStop_lastTime >= 0.2) {
				anim.SetBool ("AttackStop", false);
				attackStop = false;
			}
		}
	}

	void Animating(float h, float v) {
		anim.speed = animSpeed;	// 动画播放速度
		anim.SetFloat("V_Speed", v);	// 将 v 传给 Animator 的 V_Speed
		anim.SetFloat("H_Speed", h);	// 将 h 传给 Animator 的 H_Speed
	}

	public void Knockback() {
		if (GetComponent<PlayerHealth> ().currentHealth < 0)
			return;
		anim.SetTrigger ("Knockback");
		transform.Translate (Vector3.back * knockbackSpeed / 3);
	}

}
