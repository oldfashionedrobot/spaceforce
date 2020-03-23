using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
  [SerializeField]
  private float maxHealth = 100f;

  private float currentHealth;
  private bool isDead = false;

  private DudeController dude;

  void Awake() {
    dude = GetComponent<DudeController>();
  }

  void Start() {
    currentHealth = maxHealth;
  }

  public void TakeDamage(Hitbox hit, Vector3 hitPoint, Vector3 hitDirection, float damage) {
    if (isDead) return;

    currentHealth -= damage;

    if (currentHealth <= 0) {
      Die(hit, hitPoint, hitDirection);
    }
  }

  private void Die(Hitbox hit, Vector3 hitPoint, Vector3 hitDirection) {
    isDead = true;
    dude.Die(hit, hitPoint, hitDirection);
  }
}