using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class WeaponComponents : MonoBehaviour {

	public Sprite[] modules;
	
	private Weapon parent;
	private SpriteRenderer spriteRenderer;
	
	void Start () {
		parent = GetComponentInParent<Weapon> ();
		spriteRenderer = GetComponent<SpriteRenderer> ();
		spriteRenderer.sprite = modules [Random.Range(0, modules.Length)];
	}

	void Update () {
		transform.eulerAngles = parent.transform.eulerAngles;
	}
	
	public SpriteRenderer getSpriteRenderer () {
		return spriteRenderer;
	}
}
