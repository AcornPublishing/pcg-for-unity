using UnityEngine;
using System.Collections;

public class Enemy : MovingObject
{
	protected override bool AttemptMove <T> (int xDir, int yDir)
	{
		return true;
	}

	protected override void OnCantMove <T> (T component)
	{
	}
}
