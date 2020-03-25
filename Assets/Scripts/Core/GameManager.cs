using System.Collections;
using System.Collections.Generic;
using SpaceForce.Character;
using UnityEngine;

namespace SpaceForce.Core {
  public class GameManager : MonoBehaviour {
    [Range(0, 1)]
    public float timeScale = 1f;

    public LayerMasks layerMasks;
    public static LayerMasks layers;

    [SerializeField]
    private PlayerController player;

    void Start() {
      Time.timeScale = timeScale;

      layers = layerMasks;
    }

  }
}