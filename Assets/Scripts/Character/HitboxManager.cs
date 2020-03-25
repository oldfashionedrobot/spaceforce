﻿using System.Collections;
using System.Collections.Generic;
using SpaceForce.Combat;
using UnityEngine;

namespace SpaceForce.Character {
  public class HitboxManager : MonoBehaviour {

    private Hitbox[] bodyParts;

    void Start() {
      bodyParts = GetComponentsInChildren<Hitbox>();

      ToggleRagdoll(false);
    }

    public void TurnOnRagdoll() {
      ToggleRagdoll(true);
    }

    private void ToggleRagdoll(bool onOff) {
      foreach (Hitbox part in bodyParts) {
        part.GetComponent<Rigidbody>().isKinematic = !onOff;
      }
    }
  }
}