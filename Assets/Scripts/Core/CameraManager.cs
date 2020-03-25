using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace SpaceForce.Core {

  public class CameraManager : MonoBehaviour {
    private CinemachineFreeLook camScript;
    private CinemachineCameraOffset offsetScript;

    // private float standardFov = 40f;
    // private float zoomFov = 30f;
    private Vector3 standardOffset = new Vector3(0.6f, 0f, 0f);
    private Vector3 zoomOffset = new Vector3(0.6f, -0.2f, 2f);
    // private Vector3 zoomOffset = new Vector3(0.6f, -.2f, 2.6f);
    private float zoomLerpSpeed = 20f;

    private bool isZoomed = false;

    void Awake() {
      camScript = GetComponent<CinemachineFreeLook>();
      offsetScript = GetComponent<CinemachineCameraOffset>();
    }

    void Update() {
      // TODO: run this in a coroutine
      // if (isZoomed && camScript.m_Lens.FieldOfView != zoomFov) {
      //   camScript.m_Lens.FieldOfView = Mathf.Lerp(camScript.m_Lens.FieldOfView, zoomFov, zoomLerpSpeed * Time.deltaTime);
      // } else if (!isZoomed && camScript.m_Lens.FieldOfView != standardFov) {
      //   camScript.m_Lens.FieldOfView = Mathf.Lerp(camScript.m_Lens.FieldOfView, standardFov, zoomLerpSpeed * Time.deltaTime);
      // }

      // This pulls the camera over the shoulder
      if (isZoomed && offsetScript.m_Offset != zoomOffset) {
        offsetScript.m_Offset = Vector3.Lerp(offsetScript.m_Offset, zoomOffset, zoomLerpSpeed * Time.deltaTime);
      } else if (!isZoomed && offsetScript.m_Offset != standardOffset) {
        offsetScript.m_Offset = Vector3.Lerp(offsetScript.m_Offset, standardOffset, zoomLerpSpeed * Time.deltaTime);
      }
    }

    public void ToggleZoom(bool onOff) {
      isZoomed = onOff;
    }

    internal void SwapShoulder() {
      standardOffset.x *= -1;
      zoomOffset.x *= -1;
    }
  }
}