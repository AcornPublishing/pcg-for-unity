using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

	
public class BoardManager : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum;
		public int maximum;

		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
	
	public int columns = 5;
	public int rows = 5;
	
	public GameObject exit; 

	public GameObject[] floorTiles;
	public GameObject[] wallTiles;
	public GameObject[] outerWallTiles;

	private Transform boardHolder;
	private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2> ();
	
	private Transform dungeonBoardHolder;
	private Dictionary<Vector2, Vector2> dungeonGridPositions;

	public void BoardSetup () {
		boardHolder = new GameObject ("Board").transform;

		for(int x = 0; x < columns; x++) {
			for(int y = 0; y < rows; y++) {

				gridPositions.Add(new Vector2(x,y), new Vector2(x,y));

				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;

				instance.transform.SetParent (boardHolder);
			}
		}
	}

	private void addTiles(Vector2 tileToAdd) {
		if (!gridPositions.ContainsKey (tileToAdd)) {
			gridPositions.Add (tileToAdd, tileToAdd);
			GameObject toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
			GameObject instance = Instantiate (toInstantiate, new Vector3 (tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
			instance.transform.SetParent (boardHolder);

			if (Random.Range (0, 3) == 1) {
				toInstantiate = wallTiles[Random.Range (0,wallTiles.Length)];
				instance = Instantiate (toInstantiate, new Vector3 (tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}

			if (Random.Range (0, 100) == 1) {
				toInstantiate = exit;
				instance = Instantiate (toInstantiate, new Vector3 (tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
	}

	public void addToBoard (int horizontal, int vertical) {
		if (horizontal == 1) {
			int x = (int)Player.position.x;
			int sightX = x + 2;
			for (x += 1; x <= sightX; x++) {
				int y = (int)Player.position.y;
				int sightY = y + 1;
				for (y -= 1; y <= sightY; y++) {
					addTiles(new Vector2 (x, y));
				}
			}
		} 
		else if (horizontal == -1) {
			int x = (int)Player.position.x;
			int sightX = x - 2;
			for (x -= 1; x >= sightX; x--) {
				int y = (int)Player.position.y;
				int sightY = y + 1;
				for (y -= 1; y <= sightY; y++) {
					addTiles(new Vector2 (x, y));
				}
			}
		}
		else if (vertical == 1) {
			int y = (int)Player.position.y;
			int sightY = y + 2;
			for (y += 1; y <= sightY; y++) {
				int x = (int)Player.position.x;
				int sightX = x + 1;
				for (x -= 1; x <= sightX; x++) {
					addTiles(new Vector2 (x, y));
				}
			}
		}
		else if (vertical == -1) {
			int y = (int)Player.position.y;
			int sightY = y - 2;
			for (y -= 1; y >= sightY; y--) {
				int x = (int)Player.position.x;
				int sightX = x + 1;
				for (x -= 1; x <= sightX; x++) {
					addTiles(new Vector2 (x, y));
				}
			}
		}
	}

	public void SetDungeonBoard (Dictionary<Vector2,Vector2> dungeonTiles, int bound, Vector2 endPos) {
		boardHolder.gameObject.SetActive (false);
		dungeonBoardHolder = new GameObject ("Dungeon").transform;
		GameObject toInstantiate, instance;

		foreach(KeyValuePair<Vector2,Vector2> tile in dungeonTiles) {
			toInstantiate = floorTiles [Random.Range (0, floorTiles.Length)];
			instance = Instantiate (toInstantiate, new Vector3 (tile.Value.x, tile.Value.y, 0f), Quaternion.identity) as GameObject;
			instance.transform.SetParent (dungeonBoardHolder);
		}

		for (int x = -1; x < bound + 1; x++) {
			for (int y = -1; y < bound + 1; y++) {
				if (!dungeonTiles.ContainsKey(new Vector2(x, y))) {
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					instance = Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					instance.transform.SetParent (dungeonBoardHolder);
				}
			}
		}

		toInstantiate = exit;
		instance = Instantiate (toInstantiate, new Vector3 (endPos.x, endPos.y, 0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent (dungeonBoardHolder);
	}

	public void SetWorldBoard () {
		Destroy (dungeonBoardHolder.gameObject);
		boardHolder.gameObject.SetActive (true);
	}
}
