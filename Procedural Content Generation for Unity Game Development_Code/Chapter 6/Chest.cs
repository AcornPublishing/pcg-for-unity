using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Chest : MonoBehaviour {
	public Sprite openSprite;
	public Item randomItem;
	
	public Weapon weapon;

	private SpriteRenderer spriteRenderer;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	public void Open () {
		spriteRenderer.sprite = openSprite;

		GameObject toInstantiate;

		if (Random.Range (0, 2) == 1) {
			randomItem.RandomItemInit ();
			toInstantiate = randomItem.gameObject;
		} else {
			toInstantiate = weapon.gameObject;
		}
		GameObject instance = Instantiate (toInstantiate, new Vector3 (transform.position.x, transform.position.y, 0f), Quaternion.identity) as GameObject;
		instance.transform.SetParent (transform.parent);
		gameObject.layer = 10;
		spriteRenderer.sortingLayerName = "Items";
	}
}