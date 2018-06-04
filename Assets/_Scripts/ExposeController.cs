using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ExposeController : MonoBehaviour {

	public GameObject exposion_1;
	public GameObject exposion_2;
	public GameObject exposion_3;

	GameObject Player;
	GameObject MainCamera;
	GameObject BossBgmController;
	GameObject lastBgm;
	ParticleSystem exposionPS_1;
	ParticleSystem exposionPS_2;
	ParticleSystem exposionPS_3;
	AudioSource exposionAS_1;
	AudioSource exposionAS_2;
	AudioSource exposionAS_3;

	GameObject exposionTextGO;
	Text exposionText;
	GameObject endGO;
	Text endText;

	public Image textImage;
	public float flashSpeed = 1f;
	public Color flashColor = new Color (0f, 0f, 0f);

	float timer = 0;
	float exposionTimer = 0;
	bool ifExpose = false;
	bool firstEnd = true;

	void Awake () {
		Player = GameObject.FindGameObjectWithTag ("Player");
		MainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		exposionPS_1 = exposion_1.GetComponent<ParticleSystem> ();
		exposionPS_2 = exposion_2.GetComponent<ParticleSystem> ();
		exposionPS_3 = exposion_3.GetComponent<ParticleSystem> ();
		exposionAS_1 = exposion_1.GetComponent<AudioSource> ();
		exposionAS_2 = exposion_2.GetComponent<AudioSource> ();
		exposionAS_3 = exposion_3.GetComponent<AudioSource> ();
		exposionTextGO = GameObject.Find ("ExposionInformation");
		exposionText = exposionTextGO.GetComponent<Text> ();
		BossBgmController = GameObject.Find ("BossBgmController");
		lastBgm = GameObject.Find ("LastBgm");
		endGO = GameObject.Find ("GameOver");
		endText = endGO.GetComponent<Text> ();
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject == Player) {
			ifExpose = true;
			timer = Time.time;
		}
	}

	void Update() {
		if (ifExpose) {
			// 显示爆炸前的信息
			textImage.color = Color.Lerp (textImage.color, flashColor, flashSpeed * Time.deltaTime);
			exposionText.enabled = true;
		}

		if (timer != 0 && Time.time - timer > 5) {
			exposionTimer = Time.time;
			// 关闭爆炸前的信息
			textImage.color = Color.clear;
			exposionText.enabled = false;
			// 切换镜头，并播放爆炸粒子效果
			MainCamera.GetComponent<Camera> ().enabled = false;
			exposionPS_1.Play ();
			exposionPS_2.Play ();
			exposionPS_3.Play ();
			exposionAS_1.Play ();
			exposionAS_2.Play ();
			exposionAS_3.Play ();
			// 切换音乐
			BossBgmController.GetComponent<AudioSource>().Stop();
			timer = 0;
			ifExpose = false;
		}

		if (exposionTimer != 0 && Time.time - exposionTimer > 8) {
			if (firstEnd) {
				// 结束爆炸声，终曲响起
				exposionAS_1.Stop ();
				exposionAS_2.Stop ();
				exposionAS_3.Stop ();
				lastBgm.GetComponent<AudioSource> ().Play ();
			}
			// 显示文字信息
			endText.enabled = true;
			textImage.color = flashColor;
			firstEnd = false;
		}
	}
}
