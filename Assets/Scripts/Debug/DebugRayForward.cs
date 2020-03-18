using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRayForward : MonoBehaviour {
  public float rayLength = 10f;
  public float rayDuration = 1f;
  public Color rayColor = Color.red;

  // Update is called once per frame
  void Update() {
    Debug.DrawRay(transform.position, transform.forward * rayLength, rayColor, rayDuration);
  }
}