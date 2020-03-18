using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {
  private Animator anim;
  private CharacterIKManager ikManager;
  private NavMeshAgent navAgent;

  void Start() {
    anim = GetComponent<Animator>();
    ikManager = GetComponent<CharacterIKManager>();
    navAgent = GetComponent<NavMeshAgent>();
  }

  void Update() {
    Transform player = GameObject.FindGameObjectWithTag("Player").transform;
    Vector3 lookPos = player.position;
    lookPos.y += 1.5f;
    ikManager.SetLookPosition(lookPos);

    // navAgent.SetDestination(player.position);

    anim.SetFloat("verticalSpeed", navAgent.velocity.magnitude / navAgent.speed);
  }
}