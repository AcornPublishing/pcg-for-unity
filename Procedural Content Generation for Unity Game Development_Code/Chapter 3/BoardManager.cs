using UnityEngine;
using System;
using System.Collections.Generic; 		//Allows us to use Lists.
using Random = UnityEngine.Random; 		//Tells Random to use the Unity Engine random number generator.

	
public class BoardManager : MonoBehaviour
{
	// Using Serializable allows us to embed a class with sub properties in the inspector.
	[Serializable]
	public class Count
	{
		public int minimum; 			//Minimum value for our Count class.
		public int maximum; 			//Maximum value for our Count class.
		
		
		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
	
	public int columns = 5; 			//Number of columns in our game board.
	public int rows = 5;				//Number of rows in our game board.

	public GameObject[] floorTiles;		//Array of floor prefabs.
	public GameObject[] wallTiles;

	private Transform boardHolder;		//A variable to store a reference to the transform of our Board object.

	private Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2> ();

	public void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
		
		//Loop along x axis
		for(int x = 0; x < columns; x++)
		{
			//Loop along y axis
			for(int y = 0; y < rows; y++)
			{
				//Add tile to list for future reference
				gridPositions.Add(new Vector2(x,y), new Vector2(x,y));

				//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];

				//Instantiate the GameObject instance using the prefab chosen for toInstantiate at the Vector3 corresponding to current grid position in loop, cast it to GameObject.
				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
				
				//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
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
			
			//Choose at random a wall tile to lay
			if (Random.Range (0, 3) == 1) {
				toInstantiate = wallTiles[Random.Range (0,wallTiles.Length)];
				instance = Instantiate (toInstantiate, new Vector3 (tileToAdd.x, tileToAdd.y, 0f), Quaternion.identity) as GameObject;
				instance.transform.SetParent (boardHolder);
			}
		}
	}

	public void addToBoard (int horizontal, int vertical) {
		if (horizontal == 1) {
			//Check if tiles exist
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

}
