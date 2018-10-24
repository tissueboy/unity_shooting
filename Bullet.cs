using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will handle the bullet adding itself back to the pool
public class Bullet : MonoBehaviour
{
	public int speed = 10;			//How fast the bullet moves
	public float lifeTime = 1;		//How long the bullet lives in seconds
	public int power = 1;			//Power of the bullet

	public GameObject gameobject;
	public Vector2 vec;
	private GameObject _parent;
	public Player script;

	void Start()
	{
		//_parent = transform.root.gameObject;

		//unitychan = GameObject.Find ("unitychan");

		//Debug.Log("test="+gameobject.GetComponent<Player>().GetBulletVecor());

		//vec = gameobject.GetComponent<Player>().GetBulletVecor();

		gameobject = GameObject.FindWithTag("Player");
		script = gameobject.GetComponent<Player>();

		vec = script.last_direction;

		Debug.Log("test2="+vec);

	}

	void Update()
	{
		GetComponent<Rigidbody2D>().velocity = vec;
	}


	void OnEnable ()
	{
		//Send the bullet "forward"
		//GetComponent<Rigidbody2D>().velocity = transform.up.normalized * speed;
		//GetComponent<Rigidbody2D>().velocity = gameobject.GetComponent<Player>().GetBulletVecor();
		//Debug.Log("Player.GetBulletVecor();="+player.GetBulletVecor());



		//script = gameobject.GetComponent<Player>();

		

		//Invoke the Die method
		Invoke ("Die", lifeTime);
	}

	void OnDisable()
	{
		//Stop the Die method (in case something else put this bullet back in the pool)
		CancelInvoke ("Die");
	}

	void Die()
	{
		//Add the bullet back to the pool
		ObjectPool.current.PoolObject (gameObject);
	}
}