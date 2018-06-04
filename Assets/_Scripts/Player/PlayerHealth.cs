using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerHealth : MonoBehaviour {
    public int startingHealth = 100;	// 初始生命值
    public int currentHealth;			// 当前生命值
    public Slider healthSlider;			// 血条
    public Image damageImage;			// 受伤闪屏
    public AudioClip deathClip;			// 音频
    public float flashSpeed = 5f;		// 闪屏消去速度
    public Color flashColour = new Color(1f, 0f, 0f);	 // 闪屏颜色

	// 部分私有变量
    Animator anim;
    AudioSource playerAudio;
	PlayerController playerMovement;
    PlayerShooting playerShooting;
    
	bool isDead;
    bool damaged;

    void Awake () {
		// 获取引用
        anim = GetComponent <Animator> ();
        playerAudio = GetComponent <AudioSource> ();
		playerMovement = GetComponent <PlayerController> ();
        playerShooting = GetComponentInChildren <PlayerShooting> ();
        currentHealth = startingHealth;
    }


    void Update () {
        if(damaged) { // 正在受到攻击
            damageImage.color = flashColour;
        }
        else {	// 消去闪屏图片
            damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
        }
        damaged = false;	// 设置状态

		if (isDead) {
			if (Input.GetButtonDown ("Fire1")) {
				SceneManager.LoadScene (0);
			}
		}
    }

    public void TakeDamage (int amount) {
        damaged = true;

        currentHealth -= amount;	// 生命值减少

        healthSlider.value = currentHealth;	// 血条减少

        playerAudio.Play ();	// 播放受伤音频

        if(currentHealth <= 0 && !isDead) {	// 检查死亡状态
            Death ();
        }
    }


    void Death () {
        isDead = true;	// 设置死亡状态

        playerShooting.DisableEffects ();	// 停止射击效果

        anim.SetTrigger ("Die");	// 播放死亡动画

        playerAudio.clip = deathClip;
        playerAudio.Play ();	// 播放玩家死亡音频

        playerMovement.enabled = false;	// 停止移动
        playerShooting.enabled = false;	// 停止射击

    }


    public void RestartLevel () {	// 复活点
        SceneManager.LoadScene (0);
    }
}
