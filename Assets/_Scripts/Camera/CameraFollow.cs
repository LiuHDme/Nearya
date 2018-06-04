using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;

	public float smoothing = 5f;
	public float distance_v;
	public float distance_h;
	public float rotation_H_speed = 1;
	public float rotation_V_speed = 1;
	public float max_up_angle = 20;               //越大，头抬得越高
	public float max_down_angle = -20;            //越小，头抬得越低

	private Vector3 localEulerAngles;
	private Vector3 targetLocalEulerAngles;

	private float current_rotation_H;  // 水平旋转结果
	private float current_rotation_V;  // 垂直旋转结果

	void LateUpdate() {
		// 控制旋转
		current_rotation_H += Input.GetAxis("Mouse X") * rotation_H_speed;
		current_rotation_V += Input.GetAxis("Mouse Y") * rotation_V_speed;
		current_rotation_V = Mathf.Clamp(current_rotation_V, max_down_angle, max_up_angle);  // 限制垂直旋转角度
		// 自己旋转
		localEulerAngles.Set(- current_rotation_V, 0f, 0f);
		transform.localEulerAngles = localEulerAngles;
		// 控制目标旋转
		targetLocalEulerAngles.Set(0f, current_rotation_H, 0f);
		target.transform.localEulerAngles = targetLocalEulerAngles;
	}

}
