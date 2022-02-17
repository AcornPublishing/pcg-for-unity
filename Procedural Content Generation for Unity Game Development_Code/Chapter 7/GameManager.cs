using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public float turnDelay = 0.1f;
	public int healthPoints = 100;
	public static GameManager instance = null;
	[HideInInspector] public bool playersTurn = true;
	
	public bool enemiesFaster = false;
	public bool enemiesSmarter = false;
	public int enemySpawnRatio = 20;
	
	private BoardManager boardScript;

	private DungeonManager dungeonScript;
	private Player playerScript;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	
	private bool playerInDungeon;
	
	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);	

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();
		enemiesFaster = false;
		enemiesSmarter = false;

		boardScript = GetComponent<BoardManager> ();

		dungeonScript = GetComponent<DungeonManager> ();
		playerScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();

		InitGame();
	}

	void OnLevelWasLoaded(int index) {
		InitGame();
	}

	void InitGame() {
		enemies.Clear();

		boardScript.BoardSetup();

		playerInDungeon = false;
	}

	void Update() {
		if(playersTurn || enemiesMoving)
			return;

		StartCoroutine (MoveEnemies ());
	}
	
	public void AddEnemyToList(Enemy script) {
		enemies.Add(script);
	}

	public void RemoveEnemyFromList(Enemy script) {
		enemies.Remove(script);
	}

	public void GameOver() {
		enabled = false;
	}
	
	IEnumerator MoveEnemies() {
		enemiesMoving = true;

		yield return new WaitForSeconds(turnDelay);

		if (enemies.Count == 0) {
			yield return new WaitForSeconds(turnDelay);
		}
		List<Enemy> enemiesToDestroy = new List<Enemy>();

		for (int i = 0; i < enemies.Count; i++)
		{
			if (playerInDungeon) {
				if ((!enemies[i].getSpriteRenderer().isVisible)) {
					if (i == enemies.Count - 1)
						yield return new WaitForSeconds(enemies[i].moveTime);
					continue;
				}
			} else {
				if ((!enemies[i].getSpriteRenderer().isVisible) || (!boardScript.checkValidTile (enemies[i].transform.position))) {
					enemiesToDestroy.Add(enemies[i]);
					continue;
				}
			}

			enemies[i].MoveEnemy ();

			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		playersTurn = true;

		enemiesMoving = false;

		for (int i = 0; i < enemiesToDestroy.Count; i++) {
			enemies.Remove(enemiesToDestroy[i]);
			Destroy(enemiesToDestroy[i].gameObject);
		}
		enemiesToDestroy.Clear ();
	}

	public void updateBoard (int horizantal, int vertical) {
		boardScript.addToBoard(horizantal, vertical);
	}

	public void enterDungeon () {
		dungeonScript.StartDungeon ();
		boardScript.SetDungeonBoard (dungeonScript.gridPositions, dungeonScript.maxBound, dungeonScript.endPos);
		playerScript.dungeonTransition = false;

		playerInDungeon = true;

		for (int i = 0; i < enemies.Count; i++) {
			Destroy(enemies[i].gameObject);
		}
		enemies.Clear ();
	}

	public void exitDungeon () {
		boardScript.SetWorldBoard ();
		playerScript.dungeonTransition = false;

		playerInDungeon = false;

		enemies.Clear ();
	}
}