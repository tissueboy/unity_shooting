using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour {

	public float speed;
	public Vector2 keyDownPos;
	//public Vector2 keyDownBeforePos;
	public Vector2 pos;
	public bool isDown = false;
	public bool isAttack = false;
	public float jumpAfterCountMax = 1;
	public float jumpAfterCount = 0;

	private Animator anim;
	public GameObject yajirushi;
	public GameObject slash;

	Rigidbody2D m_rigidbody;
	Vector2 m_lookTarget;

	public LayerMask groundLayer;
	public bool isGrounded;//着地判定


	void Awake () {

		Application.targetFrameRate = 30;

	}

	void Start () {

		m_rigidbody = GetComponent<Rigidbody2D> ();
		
		yajirushi = transform.Find("yajirushi").gameObject;
		slash = transform.Find("slash").gameObject;
		
		isGrounded = false;

		anim = GetComponent<Animator>();
	}

	void Update () {

		Transform groundCheck = transform.Find("GroundCheck");

		isGrounded = (Physics2D.OverlapPoint(groundCheck.position) != null ) ? true : false;

		// 動く入力
		Vector2 moveInput = new Vector2 (
			CrossPlatformInputManager.GetAxis ("Horizontal"),
			CrossPlatformInputManager.GetAxis ("Vertical")
		);

		// 動く方向をみる
		m_lookTarget = moveInput;


		// 動く方向があるとき
		if (m_lookTarget != Vector2.zero) {
			// 向きを更新
			if (m_lookTarget.y < -0.3) {
			}else{
				//右移動中の方向
				if(m_lookTarget.x > 0.1){
					transform.localScale = new Vector3(3, 3, 1);
				}
				//左移動中の方
				if(m_lookTarget.x < 0.1){
					transform.localScale = new Vector3(-3, 3, 1);
				}
			}
		}
		if(isDown == true ){
			anim.SetBool("down", true);
		}

	}

	void FixedUpdate () {

		// 動く方向があるときだけ進む
		Vector2 movement = new Vector2 (m_lookTarget.x, m_lookTarget.y);

		//anim.SetTrigger("jump");

		if (m_lookTarget != Vector2.zero) {

			if ( m_lookTarget.y < -0.3 ) {//下に入力

				PlayerKeyDown(true);

			} else {

				PlayerKeyDown(false);

				//ジャンプ中
				if (isGrounded == false) {

					m_rigidbody.velocity = new Vector2 ( movement.x*0.6f, -1f);

				}else{

					m_rigidbody.velocity = new Vector2 (movement.x * speed, 0);

					jumpAfterCount = 0;

				}

				//攻撃
				if (m_lookTarget.x != 0 && isAttack == true) {

					anim.SetBool("run", false);

					anim.SetBool("attack", true);

				}else{

					anim.SetBool("attack", false);

				}

			}

		} else {

			//離した時（入力なし）
			anim.SetBool("attack", false);

			anim.SetBool("down", false);

			anim.SetBool("run", false);

			Debug.Log("isDown="+isDown);

			if (isDown == true) {//下に入力が入っていた時

				anim.SetTrigger("jump");

				m_rigidbody.velocity = new Vector2(keyDownPos.x * -6, keyDownPos.y * -6);

				//PlayerJump();

			}else{//下に入力がない時

				if (isGrounded) {

					isDown = false;

					yajirushi.SetActive (false);//下に入力していることを非表示

					//地面に着地している時は動かさない
					m_rigidbody.velocity = new Vector2 (0, 0);

				}else{

					//PlayerJump();
					//ジャンプ中		
					//m_rigidbody.velocity = new Vector2(0, m_rigidbody.velocity.y);			

				}
			}
		}
	}
	public void PlayerKeyDown(bool flg){

		if(flg == true){

			isDown = true;//下に入力をしていることを登録

			keyDownPos = new Vector2 (m_lookTarget.x, m_lookTarget.y);//離した場合のベクトルを保存

			pos = new Vector2 (
				Mathf.Abs(m_lookTarget.x),
				Mathf.Abs(m_lookTarget.y)
			);

			yajirushi.SetActive (true);//下に入力していることを視覚化

			yajirushi.transform.localScale = new Vector2(0.1f,-1-(pos.magnitude));

	    float rad = Mathf.Atan2(m_lookTarget.x, m_lookTarget.y);

	    float degree = rad * Mathf.Rad2Deg * -1.0f;

	    yajirushi.transform.rotation = Quaternion.Euler(0, 0, degree);

	    anim.SetBool("run", false);

		}else{

			isDown = false;//下に入力をしていることを解除

			yajirushi.SetActive (false);//下に入力時の矢印を非表示

			anim.SetBool("down", false);

			anim.SetBool("run", true);//走るアニメーションをON				
		}

	}

	public void PlayerJump(){

		//anim.SetBool("jump",true);

		//下に入力していた時は逆のベクトルを渡す。
		

		if(keyDownPos.x > 0){
			transform.localScale = new Vector3(-3, 3, 1);
		}else{
			transform.localScale = new Vector3(3, 3, 1);
		}

		//m_rigidbody.velocity = new Vector2 (m_lookTarget.x * speed, 0);

		//右
		if(m_lookTarget.x > 0){
			transform.localScale = new Vector3(3, 3, 1);						
		}

		//右
		if(m_lookTarget.x < 0){
			transform.localScale = new Vector3(-3, 3, 1);						
		}

	}

	void OnCollisionStay2D (Collision2D col){

		if (col.gameObject.tag == "Enemy") {
			isAttack = true;
			slash.SetActive (true);
		}else{
			slash.SetActive (false);
		}

	}

	void OnCollisionExit2D (Collision2D col){

		if (col.gameObject.tag == "Enemy") {

			isAttack = false;

		}

	}

}

