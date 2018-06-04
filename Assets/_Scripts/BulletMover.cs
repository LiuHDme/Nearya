using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMover : MonoBehaviour {

	public float speed = 1.0f;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().velocity = transform.forward * speed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
