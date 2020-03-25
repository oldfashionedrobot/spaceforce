using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceForce.Combat {
  public class Projectile : MonoBehaviour {
    private Vector3 startPos;
    private Vector3 targetPos;
    private float lerpTime;
    private float moveTime;

    void Update() {
      lerpTime += Time.deltaTime;
      transform.position = Vector3.Lerp(startPos, targetPos, lerpTime / moveTime);

      if (lerpTime >= moveTime) {
        Destroy(gameObject);
      }
    }

    public void ShootTarget(Vector3 target, float mvTime = 0.1f) {
      startPos = transform.position;
      targetPos = target;
      lerpTime = 0f;
      moveTime = mvTime;
    }

    public void ShootMiss(Vector3 direction) {
      startPos = transform.position;
      targetPos = startPos + direction * 5f;
      lerpTime = 0f;
      moveTime = 10f;
    }
  }
}