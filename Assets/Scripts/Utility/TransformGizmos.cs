using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceForce.Utils {
  public class TransformGizmos : MonoBehaviour {
    /// <summary>
    /// Callback to draw gizmos that are pickable and always drawn.
    /// </summary>
    void OnDrawGizmos() {
      Gizmos.color = Color.green;
      Gizmos.DrawWireSphere(transform.position, 0.1f);
    }
  }
}