using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {

	public bool inPlayerInventory = false;
	
	private Player player;
	private WeaponComponents[] weaponsComps;
	private bool weaponUsed = false;
	
	public void AquireWeapon () {
		player = GetComponentInParent<Player> ();
		weaponsComps = GetComponentsInChildren<WeaponComponents> ();

	}

	void Update () {
		if (inPlayerInventory) {
			transform.position = player.transform.position;
			if (weaponUsed == true) {
				float degreeY = 0, degreeZ = -90f, degreeZMax = 275f;
				Vector3 returnVecter = Vector3.zero;
				if (Player.isFacingRight) {
					degreeY = 0;
					returnVecter = Vector3.zero;
				} else if (!Player.isFacingRight) {
					degreeY = 180;
					returnVecter = new Vector3(0,180,0);
				}
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (0, degreeY, degreeZ), Time.deltaTime * 20f);
				if (transform.eulerAngles.z <= degreeZMax) {
					transform.eulerAngles = returnVecter;
					weaponUsed = false;
					enableSpriteRender (false);
				}
			}
		}
	}
	
	public void useWeapon () {
		enableSpriteRender(true);
		weaponUsed = true;
	}

	public void enableSpriteRender (bool isEnabled) {
		foreach (WeaponComponents comp in weaponsComps) {
			comp.getSpriteRenderer ().enabled = isEnabled;
		}
	}

	public Sprite getComponentImage (int index) {
		return weaponsComps[index].getSpriteRenderer().sprite;
	}
}
