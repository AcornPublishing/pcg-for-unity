using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	public int playerDamage;
	public int hp = 20;

	private Animator animator;
	private Transform target;
	private bool skipMove;
	
	private SpriteRenderer spriteRenderer;

	protected override void Start () {
		GameManager.instance.AddEnemyToList (this);

		animator = GetComponent<Animator> ();

		target = GameObject.FindGameObjectWithTag ("Player").transform;

		spriteRenderer = GetComponent<SpriteRenderer> ();

		base.Start ();
	}

	protected override bool AttemptMove <T> (int xDir, int yDir) {
		if(skipMove && !GameManager.instance.enemiesFaster) {
			skipMove = false;
			return false;
		}

		base.AttemptMove <T> (xDir, yDir);

		skipMove = true;
		return true;
	}

	public void MoveEnemy () {
		int xDir = 0;
		int yDir = 0;

		if (GameManager.instance.enemiesSmarter) {
			int xHeading = (int)target.position.x - (int)transform.position.x;
			int yHeading = (int)target.position.y - (int)transform.position.y;
			bool moveOnX = false;

			if (Mathf.Abs(xHeading) >= Mathf.Abs(yHeading)) {
				moveOnX = true;
			}

			for (int attempt = 0; attempt < 2; attempt++) {
				if (moveOnX == true && xHeading < 0) {
					xDir = -1; yDir = 0;
				}
				else if (moveOnX == true && xHeading > 0) {
					xDir = 1; yDir = 0;
				}
				else if (moveOnX == false && yHeading < 0) {
					yDir = -1; xDir = 0;
				}
				else if (moveOnX == false && yHeading > 0) {
					yDir = 1; xDir = 0;
				}

				Vector2 start = transform.position;
				Vector2 end = start + new Vector2 (xDir, yDir);
				base.boxCollider.enabled = false;
				RaycastHit2D hit = Physics2D.Linecast (start, end, base.blockingLayer);
				base.boxCollider.enabled = true;

				if (hit.transform != null) {
					if (hit.transform.gameObject.tag == "Wall" || hit.transform.gameObject.tag == "Chest") {
						if (moveOnX == true)
							moveOnX = false;
						else 
							moveOnX = true;
					} else {
						break;
					}
				}
			}

		} else {
			if (Mathf.Abs (target.position.x - transform.position.x) < float.Epsilon)
				yDir = target.position.y > transform.position.y ? 1 : -1;
			else
				xDir = target.position.x > transform.position.x ? 1 : -1;
		}
		AttemptMove <Player> (xDir, yDir);

	}

	protected override void OnCantMove <T> (T component) {
		Player hitPlayer = component as Player;

		hitPlayer.LoseHealth (playerDamage);

		animator.SetTrigger ("enemyAttack");
		
	}

	public SpriteRenderer getSpriteRenderer () {
		return spriteRenderer;
	}

	public void DamageEnemy (int loss) {
		hp -= loss;
		
		if (hp <= 0) {
			GameManager.instance.RemoveEnemyFromList (this);
			Destroy (gameObject);
		}
	}
}
