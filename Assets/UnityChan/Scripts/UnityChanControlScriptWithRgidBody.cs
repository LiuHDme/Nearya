//
// 当Mecanim的动画数据不在原点处移动时，带有刚体的控制器
// 样本
// 2014/03/13 N.Kobyasahi
//
using UnityEngine;
using System.Collections;

// 必要组件列表
[RequireComponent(typeof (Animator))]
[RequireComponent(typeof (CapsuleCollider))]
[RequireComponent(typeof (Rigidbody))]

public class UnityChanControlScriptWithRgidBody : MonoBehaviour {

		public float animSpeed = 1.5f;				// 动画播放速度设置
		public float lookSmoother = 3.0f;			// 照相机运动的平滑设置
		public bool useCurves = true;				  // 设置是否使用Mecanim进行曲线调整
																					// 除非打开此开关，否则不会使用曲线
		public float useCurvesHeight = 0.5f;		// 曲线修正的有效高度（易于通过地面时增加）

		// 字符控制器的参数如下
		// 前進速度
		public float forwardSpeed = 7.0f;
		// 後退速度
		public float backwardSpeed = 2.0f;
		// 旋回速度
		public float rotateSpeed = 2.0f;
		// 跳跃的力量
		public float jumpPower = 3.0f;
		// 角色控制器（胶囊对撞机）的参考
		private CapsuleCollider col;
		private Rigidbody rb;
		// 角色控制器（胶囊对撞机）的移动量
		private Vector3 velocity;
		// 一个变量，可以保存CapsuleCollider设置的对撞机中心Heiht的初始值
		private float orgColHight;
		private Vector3 orgVectColCenter;

		private Animator anim;							// 参考附在角色上的动画师
		private AnimatorStateInfo currentBaseState;			// 对基础层中使用的动画器的当前状态的引用

		private GameObject cameraObject;	// 参考主相机

		// 甲参照动画的每个状态
		static int idleState = Animator.StringToHash("Base Layer.Idle");
		static int locoState = Animator.StringToHash("Base Layer.Locomotion");
		static int jumpState = Animator.StringToHash("Base Layer.Jump");
		static int restState = Animator.StringToHash("Base Layer.Rest");

// 初始化
		void Start () {
				// 检索Animator组件
				anim = GetComponent<Animator>();
				// 检索CapsuleCollider组件（胶囊碰撞）
				col = GetComponent<CapsuleCollider>();
				rb = GetComponent<Rigidbody>();
				// 获取主相机
				cameraObject = GameObject.FindWithTag("MainCamera");
				// 保存高度的初始值，CapsuleCollider组件的中心
				orgColHight = col.height;
				orgVectColCenter = col.center;
}


// 以下是主要处理。由于它与刚体有关，所以在FixedUpdate内完成处理。
		void FixedUpdate () {
				float h = Input.GetAxis("Horizontal");				// 将输入设备的水平轴定义为h
				float v = Input.GetAxis("Vertical");				// 输入设备的垂直轴由v定义
				anim.SetFloat("Speed", v);							// 将v传递给Animator端的“Speed”参数
				anim.SetFloat("Direction", h); 						// 将h传递给Animator端的“方向”参数集
				anim.speed = animSpeed;								// 将animSpeed设置为Animator的动画播放速度
				currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// 将基础层（0）的当前状态设置为参考状态变量
				rb.useGravity = true;	// 我们会在跳跃过程中减少重力，否则我们会受到重力的影响

				// 在下文中，角色移动过程
				velocity = new Vector3(0, 0, v);		// 从上下键输入获取Z轴方向的移动量
				// 转换角色在本地空间的方向
				velocity = transform.TransformDirection(velocity);
				// v的以下阈值与Mecanim的转换一起调整
				if (v > 0.1) {
						velocity *= forwardSpeed;		// 乘以移动速度
				} else if (v < -0.1) {
						velocity *= backwardSpeed;	// 乘以移动速度
				}

				if (Input.GetButtonDown("Jump")) {	// 输入空格键后

						// 动画状态只能在运动过程中跳转
						if (currentBaseState.nameHash == locoState) {
								// 如果状态转换没有进行，您可以跳转
								if(!anim.IsInTransition(0)) {
									rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);
									anim.SetBool("Jump", true);		// 发送标志切换到动画师
								}
						}
				}


				// 使用向上和向下键输入移动字符
				transform.localPosition += velocity * Time.fixedDeltaTime;

				// 用左右键输入旋转Y轴上的字符
				transform.Rotate(0, h * rotateSpeed, 0);


				// 下面，处理动画师的每个状态
				// Locomotion中
				// 当前基础层是locoState时
				if (currentBaseState.nameHash == locoState) {
						// 当您使用曲线调整碰撞器时，请将其重置以防万一
						if(useCurves) {
							resetCollider();
						}
				}
				// 跳跃处理
				// 当前基础层为jumpState时
				else if(currentBaseState.nameHash == jumpState) {
						cameraObject.SendMessage("setCameraPositionJumpView");	// 跳跃时更换相机
						// 如果状态不是转型
						if(!anim.IsInTransition(0)) {

								// 以下，说明进行曲线调整时的处理
								if(useCurves) {
										// JUMP 00下面的曲线附加到动画JumpHeight和GravityControl
										// JumpHeight：JUMP 00中跳转的高度（0到1）
										// 重力控制：1⇒跳跃（重力无效），0⇒重力有效
										float jumpHeight = anim.GetFloat("JumpHeight");
										float gravityControl = anim.GetFloat("GravityControl");
										if(gravityControl > 0)
												rb.useGravity = false;	// 切断跳跃时的重力影响

										// 掉落雷从角色中心投掷
										Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
										RaycastHit hitInfo = new RaycastHit();
										// 仅当高度超过使用曲线高度时，使用附加到JUMP 00动画的曲线调整对撞机的高度和中心
										if (Physics.Raycast(ray, out hitInfo)) {
												if (hitInfo.distance > useCurvesHeight) {
														col.height = orgColHight - jumpHeight;			// 调整后的对撞机高度
														float adjCenterY = orgVectColCenter.y + jumpHeight;
														col.center = new Vector3(0, adjCenterY, 0);	// 调整对撞机的中心
												}
												else {
													// 当它低于阈值时，将其返回到初始值（以防万一）
													resetCollider();
												}
										}
								}
								// 重置跳转布尔值（不循环）
								anim.SetBool("Jump", false);
						}
				}
				// 在IDLE中处理
				// 当前基础层处于idleState状态时
				else if (currentBaseState.nameHash == idleState) {
						// 当您使用曲线调整碰撞器时，请将其重置以防万一
						if(useCurves) {
							resetCollider();
						}
						// 进入空格键后，它将处于静止状态
						if (Input.GetButtonDown("Jump")) {
								anim.SetBool("Rest", true);
						}
				}
				// 在REST期间处理
				// 当前基础层是restState
				else if (currentBaseState.nameHash == restState) {
						//cameraObject.SendMessage("setCameraPositionFrontView");		// 将相机切换到正面
						// 当状态不转换时、Rest bool値要重置（尽量不要循环）
						if(!anim.IsInTransition(0)) {
							anim.SetBool("Rest", false);
						}
				}
		}

		// void OnGUI() {
		// 		GUI.Box(new Rect(Screen.width -260, 10 ,250 ,150), "Interaction");
		// 		GUI.Label(new Rect(Screen.width -245,30,250,30),"Up/Down Arrow : Go Forwald/Go Back");
		// 		GUI.Label(new Rect(Screen.width -245,50,250,30),"Left/Right Arrow : Turn Left/Turn Right");
		// 		GUI.Label(new Rect(Screen.width -245,70,250,30),"Hit Space key while Running : Jump");
		// 		GUI.Label(new Rect(Screen.width -245,90,250,30),"Hit Spase key while Stopping : Rest");
		// 		GUI.Label(new Rect(Screen.width -245,110,250,30),"Left Control : Front Camera");
		// 		GUI.Label(new Rect(Screen.width -245,130,250,30),"Alt : LookAt Camera");
		// }


		// 角色的对撞机尺寸重置功能
		void resetCollider() {
				// 返回高度，组件中心的初始值
				col.height = orgColHight;
				col.center = orgVectColCenter;
		}

		// 拾取
		void OnTriggerEnter(Collider other) {
				if (other.gameObject.tag == "PickUp") {
					other.gameObject.SetActive (false);
				}
		}
}
