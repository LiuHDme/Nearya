using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBullet : MonoBehaviour {

	public float lifeTime = 2f;

	void OnTriggerEnter(Collider other) {
		Destroy (this.gameObject);
	}

	// Use this for initialization
	void Start () {
		Destroy (this.gameObject, lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
