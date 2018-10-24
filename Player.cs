using UnityEngine;
using System.Collections;
//using TouchControlsKit;
using UnityEngine.UI;

//This script manages the player object
[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Player : MonoBehaviour
{	
  //public GameObject bullet;
    public Vector2 startPos;
    public Touch touch;
    public Vector2 direction;
    public Vector2 last_direction;
    public Animator animator;
    public bool playerMoving;

  public float speed;           //Ship's speed
  public float shotDelay;         //Delay between shots
  public GameObject bullet;       //The prefab of this ship's bullet
  public bool canShoot;         //Can this ship fire?
  public GameObject explosion;      //The prefab of this ship's explosion

  protected Transform[] shotPositions;  //Fire points on the ship
  //protected Animator animator;      //Reference to the ship's animator component
  [SerializeField]
  private Message messageScript;
  private string message = "かか";
  void Start () {
    //characterController = GetComponent <CharacterController> ();
    animator = GetComponent <Animator> ();
  }

  /*void Awake ()
  {
    //Get the fire points for future reference (this is for efficiency)
    shotPositions = new Transform[transform.childCount];
    for (int i = 0; i < transform.childCount; i++) 
      shotPositions[i] = transform.GetChild (i);
    //Get a reference to the animator component
    animator = GetComponent<Animator> ();
  }*/

  protected virtual void OnEnable()
  {
    //If the game is playing and the ship can shoot...
    if (canShoot && Manager.current.IsPlaying())
      //...Start it shooting
      StartCoroutine ("Shoot");
  }

  void OnDisable()
  {
    //If the ship was able to shoot and it became disabled...
    if(canShoot)
      //...Stop shooting
      StopCoroutine ("Shoot");
  }

  protected void Explode ()
  {
    //Get a pooled explosion object
    GameObject obj = ObjectPool.current.GetObject(explosion);
    //Set its position and rotation
    obj.transform.position = transform.position;
    obj.transform.rotation = transform.rotation;
    //Activate it
    obj.SetActive (true);
  }

  //Coroutine
  IEnumerator Shoot ()
  {
    //Loop indefinitely
    //while(true)
    //{
      //If there is an acompanying audio, play it
      // if (GetComponent<AudioSource>())
      //  GetComponent<AudioSource>().Play ();
      //Loop through the fire points
      // for(int i = 0; i < shotPositions.Length; i++)
      // {
        //Debug.Log("i");
        //Get a pooled bullet
        GameObject obj = ObjectPool.current.GetObject(bullet);
        //Set its position and rotation
        //obj.transform.position = shotPositions[i].position;
        //obj.transform.rotation = shotPositions[i].rotation;
        //Activate it
        obj.SetActive(true);
        obj.transform.position = transform.position;
        obj.transform.rotation = transform.rotation;
        //obj.velocity = last_direction;
        
      //}
      //Wait for it to be time to fire another shot
      yield return new WaitForSeconds(shotDelay);
    //}
   }
    //取得用関数
    public Vector2 GetBulletVecor(){
        return last_direction;
    }

    //public Vector2 direction;

	void Update ()
	{
    playerMoving = false;
		//Get our raw inputs
		float x = Input.GetAxisRaw ("Horizontal");
		float y = Input.GetAxisRaw ("Vertical");
		//Normalize the inputs
		//Vector2 direction = new Vector2 (x, y).normalized;
    if(Input.touchCount > 0){
      //Vector2 direction = TCKInput.GetAxis( "Joystick" );
      touch = Input.GetTouch(0);
      switch (touch.phase)
      {
          //When a touch has first been detected, change the message and record the starting position
          case TouchPhase.Began:
              // Record initial touch position.
              startPos = touch.position;
              StartCoroutine("Shoot");
              break;

          //Determine if the touch is a moving touch
          case TouchPhase.Moved:
              // Determine direction by comparing the current touch position with the initial one
              direction = touch.position - startPos;
              break;
          case TouchPhase.Ended:
              direction = Vector2.zero;
              break;
      }

      Move (direction); 
    }
    // if(Input.touchCount > 1){
    //   if(Input.GetTouch(1).phase == TouchPhase.Began){
    //     StartCoroutine("Shoot");
    //     //Instantiate(bullet, transform.up.normalized * speed); 
    //   }
    // }
		//Move the player
		

	}
	
	void Move (Vector2 direction)
	{
		//Find the screen limits to the player's movement
		Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		//Get the player's current position
		Vector2 pos = transform.position;

    direction.x = direction.x * 0.01f;
    direction.y = direction.y * 0.01f;

		//Calculate the proposed position
    if(direction.x > 0.3 || direction.x < -0.3){
      //direction.x = 1;
      playerMoving = true;
      last_direction = new Vector2(direction.x,direction.y);
    }else{
      direction.x = 0;
    }
    if(direction.y > 0.3 || direction.y < -0.3){
      playerMoving = true;
      last_direction =  new Vector2(direction.x,direction.y);
      //direction.y = 1;
    }else{
      direction.y = 0;
    }
    //Debug.Log("move playerMoving="+playerMoving);

    animator.SetFloat("Direction_X", direction.x);
    animator.SetFloat("Direction_Y", direction.y);
    animator.SetFloat("Last_Direction_X", last_direction.x);
    animator.SetFloat("Last_Direction_Y", last_direction.y);
    animator.SetBool("PlayerMoving", playerMoving);

		pos += direction  * speed * Time.deltaTime;
		//Ensure that the proposed position isn't outside of the limits
		pos.x = Mathf.Clamp (pos.x, min.x, max.x);
		pos.y = Mathf.Clamp (pos.y, min.y, max.y);
		//Update the player's position
		transform.position = pos;
    //bullet.velocity = last_direction;
	}

	void OnTriggerEnter2D (Collider2D c)
	{
    Debug.Log("i");
    if (c.tag == "meet") {
      Debug.Log("2");
      messageScript.SetMessagePanel (message);
    }
		//Get the layer of the collided object
		string layerName = LayerMask.LayerToName(c.gameObject.layer);
		//If the player hit an enemy bullet or ship...
		if( layerName == "Bullet (Enemy)" || layerName == "Enemy")
		{
			//...and the object was a bullet...
			if(layerName == "Bullet (Enemy)" )
				//...return the bullet to the pool...
			    ObjectPool.current.PoolObject(c.gameObject) ;
			//...otherwise...
			else
				//...deactivate the enemy ship
				c.gameObject.SetActive(false);

			//Tell the manager that we crashed
			Manager.current.GameOver();
			//Trigger an explosion
			Explode();
			//Deactivate the player
			gameObject.SetActive(false);
		}
	}
  void OnCollisionEnter(Collision collision) {
      //衝突判定
      Debug.Log("i");
      if (collision.gameObject.tag == "meet") {
        Debug.Log("2");
          //相手のタグがBallならば、自分を消す
          messageScript.SetMessagePanel (message);
      }
  }
}