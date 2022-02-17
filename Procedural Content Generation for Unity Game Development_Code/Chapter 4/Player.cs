using UnityEngine;
using System.Collections;
using UnityEngine.UI;	

public class Player : MovingObject
{
	public int wallDamage = 1;
	public Text healthText;	
	private Animator animator;
	private int health;	
	public static Vector2 position;

	public bool onWorldBoard;
	public bool dungeonTransition;
	

	protected override void Start () {
		animator = GetComponent<Animator>();

		health = GameManager.instance.healthPoints;

		healthText.text = "Health: " + health;

		position.x = position.y = 2;

		onWorldBoard = true;
		dungeonTransition = false;

		base.Start ();
	}
	
	private void Update () {
		if(!GameManager.instance.playersTurn) return;
		
		int horizontal = 0;
		int vertical = 0;

		bool canMove = false;

		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));

		vertical = (int) (Input.GetAxisRaw ("Vertical"));

		if(horizontal != 0) {
			vertical = 0;
		}

		if(horizontal != 0 || vertical != 0) {
			if (!dungeonTransition) {
				canMove = AttemptMove<Wall> (horizontal, vertical);
				if(canMove && onWorldBoard) {
					position.x += horizontal;
					position.y += vertical;
					GameManager.instance.updateBoard(horizontal, vertical);
				}
			}
		}
	}

	protected override bool AttemptMove <T> (int xDir, int yDir) {	
		bool hit = base.AttemptMove <T> (xDir, yDir);

		GameManager.instance.playersTurn = false;

		return hit;
	}

	protected override void OnCantMove <T> (T component) {
		Wall hitWall = component as Wall;

		hitWall.DamageWall (wallDamage);

		animator.SetTrigger ("playerChop");
	}

	public void LoseHealth (int loss) {
		animator.SetTrigger ("playerHit");

		health -= loss;

		healthText.text = "-"+ loss + " Health: " + health;

		CheckIfGameOver ();
	}
	

	private void CheckIfGameOver () {
		if (health <= 0) 
		{	
			GameManager.instance.GameOver ();
		}
	}
	
	private void GoDungeonPortal () {
		if (onWorldBoard) {
			onWorldBoard = false;
			GameManager.instance.enterDungeon();
			transform.position = DungeonManager.startPos;
			
		} else {
			onWorldBoard = true;
			GameManager.instance.exitDungeon();
			transform.position = position;
		}
	}
	
	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Exit") {
			dungeonTransition = true;
			Invoke("GoDungeonPortal", 0.5f);
			Destroy (other.gameObject);
		}
	}
}

