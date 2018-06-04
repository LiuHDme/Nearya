using UnityEngine;

public class PlayerShooting : MonoBehaviour {
	public float damagePerShot = 0.5f;	// 每次伤害
    public float timeBetweenBullets = 0.15f;	// 射击间隔
    public float range = 100f;	// 范围
	public Camera myCamera;

	GameObject Player;
    float timer;
	Ray mouseRay = new Ray();
    RaycastHit shootHit;	// 返回击中的物体
    int shootableMask;
    ParticleSystem gunParticles;
    AudioSource gunAudio;
    Light gunLight;
    float effectsDisplayTime = 0.2f;	// 效果消失前的持续时间

    void Awake () {
        shootableMask = LayerMask.GetMask ("Shootable");	// 只能攻击该层
        gunParticles = GetComponent<ParticleSystem> ();		// 枪的粒子效果
        gunAudio = GetComponent<AudioSource> ();			// 枪击声
        gunLight = GetComponent<Light> ();					// 射击光照
		Player = GameObject.FindWithTag("Player");
    }
		
    void Update () {
        timer += Time.deltaTime;	// 该值随着时间不断增大

		if (Player.GetComponent<PlayerController> ().swordBack) {	// 枪在手上时才能射击

			if (Input.GetButton ("Fire2") && timer >= timeBetweenBullets && Time.timeScale != 0) {
				Shoot ();
			}

			if (timer >= timeBetweenBullets * effectsDisplayTime) {	// 当开火过了足够多的时间
				DisableEffects ();
			}
		}
    }

	// 该方法能被其他脚本调用
    public void DisableEffects () {
        gunLight.enabled = false;	// 停用射击光照
    }


    void Shoot () {
        timer = 0f;

		mouseRay = myCamera.ScreenPointToRay(new Vector3(Screen.width/2, Screen.height / 2, 0));

        gunAudio.Play ();	// 播放枪声

        gunLight.enabled = true;	// 启用射击光照

        gunParticles.Stop ();	// 先停用粒子效果
		gunParticles.Play ();	// 再打开粒子效果

		// 射线、击中的物体、范围、层
		if (Physics.Raycast (mouseRay, out shootHit, range, shootableMask)) {	// 如果击中了物体
			// 获取敌人生命值脚本（如果击中的是敌人的话）
			EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
			if (enemyHealth != null) { // 如果击中的的确是敌人
				enemyHealth.TakeDamage (damagePerShot, shootHit.point);	// 调用 TakeDamage，将击中的位置作为参数传入
				// 用来敌人的粒子效果的位置
			}
		}
    }
}
