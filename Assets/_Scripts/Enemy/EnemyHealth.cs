using UnityEngine;

public class EnemyHealth : MonoBehaviour {
    public int startingHealth = 100;	// 初始生命值
	public float currentHealth;			// 当前生命值
    public float sinkSpeed = 2.5f;		// 下沉速度
    public int scoreValue = 10;			// 击杀分数
    public AudioClip deathClip;			// 死亡音效

    Animator anim;		// 动画
    AudioSource enemyAudio;	// 声音
    ParticleSystem hitParticles;	// 粒子效果
    CapsuleCollider capsuleCollider;	// 胶囊碰撞器
	float recover_timer;	// 控制恢复时间
	float timeBetweenHurt = 2f;
    
	public bool isDead;	// 是否死亡
    bool isSinking;	// 是否正在下沉

    void Awake () {
        anim = GetComponent <Animator> ();
        enemyAudio = GetComponent <AudioSource> ();
        hitParticles = GetComponentInChildren <ParticleSystem> ();
        capsuleCollider = GetComponent <CapsuleCollider> ();
        currentHealth = startingHealth;	// 游戏开始时的当前生命值为初始生命值
    }
		
    void Update () {
		recover_timer += Time.deltaTime;
		if (recover_timer > timeBetweenHurt) {
			anim.SetBool ("ReturnWalk", true);
		}
        if(isSinking) {	// 如果下沉
            transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);	// 向下移动
        }
    }

	// 该方法可被其他脚本调用
	public void TakeDamage (float amount, Vector3 hitPoint) {	// hitPoint 表示击中位置
		if (isDead)	// 如果已经死亡就返回
            return;
		if (this.gameObject.name == "RedDragon")
			return;
		enemyAudio.Play ();	// 受伤音效
		currentHealth -= amount;	// 生命值减少
		hitParticles.transform.position = hitPoint;	// 粒子效果位置为被击中位置
		hitParticles.Play ();	// 启动粒子效果
		if (currentHealth <= 0) { // 如果生命值 ≤ 0
			Death ();	// 去死吧
		}
	}

	public void TakeDamageFromHit (int amount) {
		if (isDead)	// 如果已经死亡就返回
			return;
		enemyAudio.Play ();	// 受伤音效
		currentHealth -= amount;	// 生命值减少
		anim.SetBool("Hurt", true);	// 播放受伤动画
		recover_timer = 0;
		anim.SetBool ("ReturnWalk", false);
		if (currentHealth <= 0) { // 如果生命值 ≤ 0
			Death ();	// 去死吧
		}
	}

	public void Recover() {
		anim.SetBool ("Hurt", false);
	}

    void Death () {
        isDead = true;	// 设置死亡状态
        capsuleCollider.isTrigger = true;	// 打开 Trigger，停止物理碰撞
        anim.SetTrigger ("Dead");		// 播放死亡动画
        enemyAudio.clip = deathClip;	// 当前声音为死亡音效
        enemyAudio.Play ();				// 播放死亡音效
    }


    public void StartSinking () {
        GetComponent <UnityEngine.AI.NavMeshAgent> ().enabled = false;	// 停止寻路
        GetComponent <Rigidbody> ().isKinematic = true;	// 打开运动学开关
        isSinking = true;	// 设置下沉状态
        //ScoreManager.score += scoreValue;	// 加分
        Destroy (gameObject, 2f);	// 2 秒后销毁
    }
}
