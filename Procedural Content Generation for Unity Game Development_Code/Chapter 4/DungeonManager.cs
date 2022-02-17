using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour {

	[Serializable]
	public class PathTile {

		public string type;
		public Vector2 position;
		public List<Vector2> adjacentPathTiles;

		public PathTile (string t, Vector2 p, int min, int max, Dictionary<Vector2, Vector2> currentTiles) {
			type = t;
			position = p;
			adjacentPathTiles = getAdjacentPath(min, max, currentTiles);
		}

		public List<Vector2> getAdjacentPath(int minBound, int maxBound, Dictionary<Vector2, Vector2> currentTiles) {
			List<Vector2> pathTiles = new List<Vector2> ();
			if (position.y + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x, position.y + 1))) {
				pathTiles.Add(new Vector2(position.x, position.y + 1));
			}
			if (position.x + 1 < maxBound && !currentTiles.ContainsKey(new Vector2(position.x + 1, position.y))) {
				pathTiles.Add(new Vector2(position.x + 1, position.y));
			}
			if (position.y - 1 > minBound && !currentTiles.ContainsKey(new Vector2(position.x, position.y - 1))) {
				pathTiles.Add(new Vector2(position.x, position.y - 1));
			}
			if (position.x - 1 >= minBound && !currentTiles.ContainsKey(new Vector2(position.x - 1, position.y)) && type != "E") {
				pathTiles.Add(new Vector2(position.x - 1, position.y));
			}
			return pathTiles;
		}
	}

	public Dictionary<Vector2, Vector2> gridPositions = new Dictionary<Vector2, Vector2> ();

	public int minBound = 0, maxBound;

	public static Vector2 startPos;

	public Vector2 endPos;

	public void StartDungeon () {
		//Random.seed = 1;
		gridPositions.Clear ();
		maxBound = Random.Range (50, 101);

		BuildEssentialPath ();

		BuildRandomPath ();
	}

	private void BuildEssentialPath () {
		int randomY = Random.Range (0, maxBound + 1);
		PathTile ePath = new PathTile ("E", new Vector2 (0, randomY), minBound, maxBound, gridPositions);
		startPos = ePath.position;

		int boundTracker = 0;

		while (boundTracker < maxBound) {

			gridPositions.Add (ePath.position, ePath.position);

			int adjacentTileCount = ePath.adjacentPathTiles.Count;

			int randomIndex = Random.Range (0, adjacentTileCount);

			Vector2 nextEPathPos;
			if (adjacentTileCount > 0) {
				nextEPathPos = ePath.adjacentPathTiles[randomIndex];
			} else {
				break;
			}

			PathTile nextEPath = new PathTile ("E", nextEPathPos, minBound, maxBound, gridPositions);
			if (nextEPath.position.x > ePath.position.x || (nextEPath.position.x == maxBound - 1 && Random.Range (0,2) == 1)) { // Update boundtracker before EPath update
				++boundTracker;
			} 

			ePath = nextEPath;
		}

		if (!gridPositions.ContainsKey (ePath.position))
			gridPositions.Add (ePath.position, ePath.position);

		endPos = new Vector2 (ePath.position.x, ePath.position.y);

	}

	private void BuildRandomPath () {

		List<PathTile> pathQueue = new List<PathTile> ();
		foreach (KeyValuePair<Vector2,Vector2> tile in gridPositions) {
			Vector2 tilePos = new Vector2(tile.Value.x, tile.Value.y);
			pathQueue.Add(new PathTile("R", tilePos, minBound, maxBound, gridPositions));
		}


		pathQueue.ForEach (delegate (PathTile tile) {

			int adjacentTileCount = tile.adjacentPathTiles.Count;
			if (adjacentTileCount != 0) {
				if (Random.Range(0, 5) == 1) {
					BuildRandomChamber (tile);
				}
				else if (Random.Range (0, 5) == 1 || (tile.type == "R" && adjacentTileCount > 1)) {

					int randomIndex = Random.Range (0, adjacentTileCount);

					Vector2 newRPathPos = tile.adjacentPathTiles[randomIndex];

					if (!gridPositions.ContainsKey(newRPathPos)) {
						gridPositions.Add (newRPathPos, newRPathPos);

						PathTile newRPath = new PathTile ("R", newRPathPos, minBound, maxBound, gridPositions);
						pathQueue.Add (newRPath);
					}
				}
			}
		});
	}

	private void BuildRandomChamber (PathTile tile) {
		int chamberSize = 3,
			adjacentTileCount = tile.adjacentPathTiles.Count,
			randomIndex = Random.Range (0, adjacentTileCount);
		Vector2 chamberOrigin = tile.adjacentPathTiles[randomIndex];

		for (int x = (int) chamberOrigin.x; x < chamberOrigin.x + chamberSize; x++) {
			for (int y = (int) chamberOrigin.y; y < chamberOrigin.y + chamberSize; y++) {
				Vector2 chamberTilePos = new Vector2 (x, y);
				if (!gridPositions.ContainsKey(chamberTilePos) && 
				    chamberTilePos.x < maxBound && chamberTilePos.x > 0 &&
				    chamberTilePos.y < maxBound && chamberTilePos.y > 0)

					gridPositions.Add (chamberTilePos, chamberTilePos);
			}
		}
	}
}
