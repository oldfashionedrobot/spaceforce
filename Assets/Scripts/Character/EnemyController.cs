using System.Collections;
using System.Collections.Generic;
using SpaceForce.Combat;
using SpaceForce.Core;
using UnityEngine;
using UnityEngine.AI;

namespace SpaceForce.Character {
  public class EnemyController : DudeController {

    private NavMeshAgent navAgent;

    new void Awake() {
      base.Awake();
      navAgent = GetComponent<NavMeshAgent>();
    }

    void Start() {
      EquipWeapon(1);
    }

    void Update() {
      if (isDead) return;
      Transform player = GameObject.FindGameObjectWithTag("Player").transform;
      Vector3 lookPos = player.position;
      lookPos.y += 1.5f;
      ikManager.SetLookPosition(lookPos);

      // navAgent.SetDestination(player.position);

      anim.SetFloat("verticalSpeed", navAgent.velocity.magnitude / navAgent.speed);

      // weaponManager.PerformAimRaycast(lookPos, GameManager.layers.shootLayerMask);
      // if (Random.Range(0f, 1f) > 0.9 && !weaponControlDisabled) {
      //   TriggerAim();

      //   int shotResult = weaponManager.Shoot();

      //   if (shotResult == -1) {
      //     TriggerReload();
      //   }
      // }
    }

    new void Die(Hitbox hit, Vector3 hitPoint, Vector3 hitDirection) {
      navAgent.enabled = false;

      base.Die(hit, hitPoint, hitDirection);
    }
  }
}