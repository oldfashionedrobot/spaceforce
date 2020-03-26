using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpaceForce.UI {
  public class WorldUI : MonoBehaviour {
    [SerializeField]
    GameObject blockedCrosshair;

    void Awake() {

    }

    public void SetBlockedCrosshair() {
      blockedCrosshair.SetActive(true);

    }

    public void DisableBlockedCrosshair() {
      blockedCrosshair.SetActive(false);
    }
  }
}