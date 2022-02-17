using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MovingObject {
	
	public static Vector2 position;
	
	public int wallDamage = 1;
	public Text healthText;	
	public bool onWorldBoard, dungeonTransition;
	
	public Image glove;
	public Image boot;
	
	private Animator animator;
	private int health;
	// Chapter 5
	public int attackMod = 0, defenseMod = 0;
	
	// Chapter 5
	private Dictionary<String, Item> inventory;
	
	
	protected override void Start () {
		animator = GetComponent<Animator>();
		
		health = GameManager.instance.healthPoints;
		
		healthText.text = "Health: " + health;
		
		position.x = position.y = 2;
		
		onWorldBoard = true;
		dungeonTransition = false;
		
		inventory = new Dictionary<String, Item> ();
		
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
				if (onWorldBoard)
					canMove = AttemptMove<Wall> (horizontal, vertical);
				else 
					canMove = AttemptMove<Chest> (horizontal, vertical);
				
				if(canMove && onWorldBoard) {
					position.x += horizontal;
					position.y += vertical;
					GameManager.instance.updateBoard(horizontal, vertical);
				}
			}
		}
	}
	
	protected override bool AttemptMove <T> (int xDir, int yDir) {	
		//Debug.Log (typeof(T));
		bool hit = base.AttemptMove <T> (xDir, yDir);
		//Debug.Log (hit);
		GameManager.instance.playersTurn = false;
		
		return hit;
	}
	
	protected override void OnCantMove <T> (T component) {
		//Debug.Log (typeof(T));
		// Chapter 5
		// TODO check if the type of T is Wall or Chest
		if (typeof(T) == typeof(Wall)) {
			Wall blockingObj = component as Wall;
			blockingObj.DamageWall (wallDamage);
		}
		else if (typeof(T) == typeof(Chest)) {
			Chest blockingObj = component as Chest;
			blockingObj.Open ();
		}
		
		animator.SetTrigger ("playerChop");
	}
	
	public void LoseHealth (int loss) {
		animator.SetTrigger ("playerHit");
		
		health -= loss;
		
		healthText.text = "-" + loss + " Health: " + health;
		
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
	
	//Chapter 5
	private void UpdateHealth (Collider2D item) {
		if (health < 100) {
			if (item.tag == "Food") {
				health += Random.Range (1,4);
			} else {
				health += Random.Range (4,11);
			}
			GameManager.instance.healthPoints = health;
			healthText.text = "Health: " + health;
		}
	}
	
	private void UpdateInventory (Collider2D item) {
		Item itemData = item.GetComponent<Item> ();
		switch(itemData.type) {
		case itemType.glove:
			if (!inventory.ContainsKey("glove"))
				inventory.Add("glove", itemData);
			else 
				inventory["glove"] = itemData;
			
			glove.color = itemData.level;
			break;
		case itemType.boot:
			if (!inventory.ContainsKey("boot"))
				inventory.Add("boot", itemData);
			else 
				inventory["boot"] = itemData;
			
			boot.color = itemData.level;
			break;
		}
		
		attackMod = 0;
		defenseMod = 0;
		
		foreach (KeyValuePair<String, Item> gear in inventory) {
			attackMod += gear.Value.attackMod;
			defenseMod += gear.Value.defenseMod;
		}
	}
	
	private void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Exit") {
			dungeonTransition = true;
			Invoke ("GoDungeonPortal", 0.5f);
			Destroy (other.gameObject);
		} else if (other.tag == "Food" || other.tag == "Soda") {
			UpdateHealth(other);
			Destroy (other.gameObject);
		} else if (other.tag == "Item") {
			UpdateInventory(other);
			Destroy (other.gameObject);
		}
	}
}

