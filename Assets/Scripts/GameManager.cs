using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  [Range(0, 1)]
  public float timeScale = 1f;

  public LayerMasks layerMasks;

  [SerializeField]
  private PlayerController player;

  void Start() {
    Time.timeScale = timeScale;

    player.SetUp(layerMasks);
  }
}