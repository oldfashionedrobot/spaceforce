using System.Collections;
using System.Collections.Generic;
using SpaceForce.Character;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceForce.UI {
  public class PlayerHUD : MonoBehaviour {
    [SerializeField]
    private Text ammoText;

    [SerializeField]
    private GameObject player;

    private WeaponManager playerWeaponManager;

    void Awake() {
      playerWeaponManager = player.GetComponent<WeaponManager>();
    }

    void Update() {
      ammoText.text = "Ammo: " + playerWeaponManager.GetCurrentAmmo() + " / " + playerWeaponManager.GetClipSize();
    }
  }
}