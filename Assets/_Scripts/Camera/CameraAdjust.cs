using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjust : MonoBehaviour {

	private Transform player;
	private Vector3 offset; // 摄像机和 Player 相对位置
	private float distance;	// 距离
	private Vector3[] points = new Vector3[5];	// 5 个点作为摄像机位置的选择

	private Vector3 targetPos;	// 筛选后的摄像机位置s 

	void Awake() {
		player = GameObject.FindWithTag("Player").transform;
		offset = transform.position - player.position;
		distance = offset.magnitude;
	}

	void FixedUpdate() {
		// 更新5个点的位置
		points[0] = player.position + offset;
		points[4] = player.position + Vector3.up * distance;

		points[1] = Vector3.Lerp(points[0], points[4], 0.25f);
		points[2] = Vector3.Lerp(points[0], points[4], 0.5f);
		points[3] = Vector3.Lerp(points[0], points[4], 0.75f);
		points[4] = Vector3.Lerp(points[0], points[4], 0.9f);

		targetPos = FindCameraTarget();

		AdjustCamera();
	}

	private Vector3 FindCameraTarget() {
		Vector3 result = points[points.Length - 1];//头顶位置

		// 从低到高遍历
		for (int i = 0; i < points.Length; ++i) {
			if (IsHitPlayer(points[i], player.position)) {
				result = points[i];
				break;
			}
		}

		return result;
	}

	private Ray ray;
	private RaycastHit hit;
	// 从 origin 发射一条射线检测是否碰到 player，
	// 碰到则表示摄像机在此位置可以看到 player
	bool IsHitPlayer(Vector3 origin, Vector3 target) {
		bool result = false;

		Vector3 dir = target - origin;
		ray = new Ray(origin,dir);
		if (Physics.Raycast(ray, out hit)) {
			if (hit.transform.tag == "Player") {
				result = true;
			}
		}
		return result;
	}

	// 调整摄像机位置
	void AdjustCamera() {

		transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * 6);

		Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 33f);

	}
}
