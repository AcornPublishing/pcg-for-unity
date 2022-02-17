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
	
	private BoardManager boardScript;

	private DungeonManager dungeonScript;
	private Player playerScript;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	
	void Awake() {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);	

		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy>();

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
	}

	void Update() {
		if(playersTurn || enemiesMoving)
			return;

		StartCoroutine (MoveEnemies ());
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

		playersTurn = true;

		enemiesMoving = false;
	}

	public void updateBoard (int horizantal, int vertical) {
		boardScript.addToBoard(horizantal, vertical);
	}

	public void enterDungeon () {
		dungeonScript.StartDungeon ();
		boardScript.SetDungeonBoard (dungeonScript.gridPositions, dungeonScript.maxBound, dungeonScript.endPos);
		playerScript.dungeonTransition = false;
	}

	public void exitDungeon () {
		boardScript.SetWorldBoard ();
		playerScript.dungeonTransition = false;
	}
}