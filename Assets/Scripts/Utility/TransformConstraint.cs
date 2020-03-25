using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceForce.Utils {
  public class TransformConstraint : MonoBehaviour {
    public Transform target;
    public bool constrainPosition = false;
    public bool constrainRotation = false;

    void FixedUpdate() {
      if (target == null) return;
      if (constrainPosition) transform.position = target.position;
      if (constrainRotation) transform.rotation = target.rotation;
    }
  }
}