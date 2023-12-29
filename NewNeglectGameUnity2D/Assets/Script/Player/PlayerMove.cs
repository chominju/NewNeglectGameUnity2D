using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    private float movementSpeed;
    Vector2 movement = new Vector2();
    Animator animator;

    Rigidbody2D rb2D;
	GameObject weaponCollider;
	GameObject playerCollider;
	PlayerData data;


	bool changeFilp;
    // Start is called before the first frame update
    void Start()
    {
		movementSpeed = 2;// GetComponent<Player>().GetPlayerData().speed;
        rb2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		data = DataManager.GetDataManager().GetPlayerData();
		weaponCollider = transform.Find(data.playerName + "WeaponCollider").gameObject;
		playerCollider = transform.Find(data.playerName + "Collider").gameObject;

		changeFilp = false;

	}

    // Update is called once per frame
    void Update()
    {
		if (Input.GetKeyUp(KeyCode.B))
		{
			AttackOff();
		}
		//if(Input.GetKeyDown(KeyCode.B))
		if(Input.GetKey(KeyCode.B))
        {
			Attack();
        }
	}

    private void FixedUpdate()
    {
        MoveCharacter();
	}

    void MoveCharacter()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement.Normalize();

		if (movement.x != 0.0f)
		{
			if (animator.GetBool("Attack") == false)
			 {
				Walk();
			}
		}
		else
        {
				WalkOff();
        }
		rb2D.velocity = movement * movementSpeed;

    }












	public void Jump()
	{
		animator.SetBool("Jump", true);
	}

	public void JumpOff()
	{
		animator.SetBool("Jump", false);
	}

	public void Dead()
	{
		animator.SetBool("Dead", true);
	}

	public void DeadOff()
	{
		animator.SetBool("Dead", false);
	}
	public void Walk()
	{
		if(movement.x <=0)
        {
			if(!changeFilp)
            {
				changeFilp = true;
                Vector3 temp = new Vector3(-1.5f, 0, 0);
                GetComponent<Transform>().position += temp;


				//Vector3 temp2 = new Vector3(+1.3f, 0);

				Vector3 weaponPos = weaponCollider.GetComponent<Transform>().localPosition;
				weaponPos.x *= -1.0f;
				weaponCollider.transform.localPosition = weaponPos;


				Vector3 colliderPos = playerCollider.GetComponent<Transform>().localPosition;
				colliderPos.x *= -1.0f;
				playerCollider.transform.localPosition = colliderPos;


				//Set(weaponCollider2D.transform.position.x * -1, weaponCollider2D.transform.position.x, weaponCollider2D.transform.position.z); ;
				//playerCollider2D.transform.position.Set(playerCollider2D.transform.position.x * -1, playerCollider2D.transform.position.x, playerCollider2D.transform.position.z); ;
				//playerCollider2D.transform.position += temp2;


			}
			GetComponent<SpriteRenderer>().flipX = true;
        }
		else
        {
			if(changeFilp)
            {
				changeFilp = false;
                Vector3 temp = new Vector3(1.5f, 0, 0);
				Vector3 temp2 = new Vector3(-1.3f, 0);
				GetComponent<Transform>().position += temp;
				//weaponCollider2D.transform.position += temp;
				//playerCollider2D.transform.position += temp2;
				Vector3 weaponPos = weaponCollider.GetComponent<Transform>().localPosition;
				weaponPos.x *= -1.0f;
				weaponCollider.GetComponent<Transform>().localPosition = weaponPos;


				Vector3 colliderPos = playerCollider.GetComponent<Transform>().localPosition;
				colliderPos.x *= -1.0f;
				playerCollider.GetComponent<Transform>().localPosition= colliderPos;


				//.transform.position.Set(weaponCollider2D.transform.position.x * -1, weaponCollider2D.transform.position.x, weaponCollider2D.transform.position.z); ;
				//playerCollider2D.transform.position.Set(playerCollider2D.transform.position.x * -1, playerCollider2D.transform.position.x, playerCollider2D.transform.position.z); ;

			}
			GetComponent<SpriteRenderer>().flipX = false;
		}
		animator.SetBool("Walk", true);
	}

	public void WalkOff()
	{
		animator.SetBool("Walk", false);
	}
	public void Run()
	{
		animator.SetBool("Run", true);
	}
	public void RunOff()
	{
		animator.SetBool("Run", false);
	}
	public void Attack()
	{
		animator.SetBool("Attack", true);
	}
	public void AttackOff()
	{
		animator.SetBool("Attack", false);

	}
}
