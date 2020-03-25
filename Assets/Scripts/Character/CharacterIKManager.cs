using System.Collections;
using System.Collections.Generic;
using SpaceForce.Utils;
using UnityEngine;

namespace SpaceForce.Character {

  [RequireComponent(typeof(Animator))]
  public class CharacterIKManager : MonoBehaviour {

    private Animator anim;

    public bool ikEnabled = false;
    private float lookAngle = 80;
    private float lookAtLerpSpeed = 4f;
    private float lookAwayLerpSpeed = 2f;
    private float lookSmoothSpeed = 30f;
    private float bodyOffFactor = 0.1f;
    private float bodyOnFactor = 0.7f;
    private float aimAtSpeed = 6f;

    [SerializeField]
    private Transform ikAimRShoulder = null;
    [SerializeField]
    private Transform ikAimRHand = null;
    [SerializeField]
    private Transform ikAimLHand = null;
    [SerializeField]
    private Transform ikAimRElbow = null;
    [SerializeField]
    private Transform ikAimLElbow = null;

    // public Transform lookObj = null;
    private Vector3 lookPos = Vector3.zero;

    private float lookWeightTarget = 0f;
    private float lookWeightCurrent = 0f;
    private Vector3 lastLookAt = Vector3.zero;
    private Vector3 lookAtCurrent = Vector3.zero;
    private bool weaponAiming = false;
    private float aimWeightCurrent = 0f;

    void Awake() {
      anim = GetComponent<Animator>();
    }

    void Start() {
      // Initialize weights
      anim.SetLookAtWeight(0f, 1f, 1f, 1f, 0f);
    }

    void OnAnimatorIK(int layerIdx) {
      // check if active
      if (anim != null && ikEnabled) {
        Vector3 lookAt = GetLookPosition();

        HandleHeadLook(lookAt);
        HandleWeaponAim(lookAt);
      } else {
        DeactivateIK();
      }
    }

    public void SetLeftArmIK(Transform handTarget, Transform elbowTarget) {
      ikAimLHand.GetComponent<TransformConstraint>().target = handTarget;
      ikAimLElbow.GetComponent<TransformConstraint>().target = elbowTarget;
    }

    public void SetLookPosition(Vector3 target) {
      lookPos = target;
    }

    public void ToggleAim(bool onOff) {
      weaponAiming = onOff;
    }

    private void HandleHeadLook(Vector3 lookAt) {

      if (CheckLookAngle(lookAt)) {
        lastLookAt = lookAt;
        anim.SetLookAtPosition(lookAt);
        lookWeightTarget = 1f;
        LerpLookWeight();

      } else {
        // else look at nothing
        anim.SetLookAtPosition(lastLookAt);
        lookWeightTarget = 0f;
        LerpLookWeight();
      }
    }

    private void HandleWeaponAim(Vector3 lookAt) {
      if (weaponAiming) {
        LerpAimWeight(1f);
        ikAimRShoulder.LookAt(lookAt);

        anim.SetIKPosition(AvatarIKGoal.RightHand, ikAimRHand.position);
        anim.SetIKRotation(AvatarIKGoal.RightHand, ikAimRHand.rotation);
        anim.SetIKHintPosition(AvatarIKHint.RightElbow, ikAimRElbow.position);

        if (ikAimLHand != null) {
          anim.SetIKPosition(AvatarIKGoal.LeftHand, ikAimLHand.position);
          anim.SetIKRotation(AvatarIKGoal.LeftHand, ikAimLHand.rotation);

          if (ikAimLElbow != null)
            anim.SetIKHintPosition(AvatarIKHint.LeftElbow, ikAimLElbow.position);
        }
      } else {
        // Animation transition to idle pose handles this smoothing
        DeactivateWeaponAimIK();
      }
    }

    private void LerpLookWeight() {
      lookWeightCurrent = Mathf.Lerp(
        lookWeightCurrent,
        lookWeightTarget,
        (lookWeightCurrent > lookWeightTarget ? lookAwayLerpSpeed : lookAtLerpSpeed) * Time.deltaTime
      );
      anim.SetLookAtWeight(lookWeightCurrent, lookWeightCurrent * (weaponAiming ? bodyOnFactor : bodyOffFactor), lookWeightCurrent);
    }

    private void LerpAimWeight(float aimWeightTarget) {
      aimWeightCurrent = Mathf.Lerp(
        aimWeightCurrent,
        aimWeightTarget,
        (aimWeightCurrent > aimWeightTarget ? lookAwayLerpSpeed : aimAtSpeed) * Time.deltaTime
      );
      LerpWeaponAimIK(aimWeightCurrent);
    }

    private Vector3 GetLookPosition() {
      Vector3 lookTarget;

      if (lookPos != Vector3.zero) {
        lookTarget = lookPos;
      }
      // else if (lookObj != null) {
      //   lookTarget = lookObj.position;
      // } 
      else {
        lookTarget = Vector3.zero;
      }

      if (lookTarget != Vector3.zero) {
        if (lookAtCurrent == Vector3.zero) lookAtCurrent = lookTarget;

        lookAtCurrent = Vector3.Lerp(lookAtCurrent, lookTarget, lookSmoothSpeed * Time.deltaTime);
      }

      return lookAtCurrent;
    }

    private bool CheckLookAngle(Vector3 lookPos) {
      if (lookPos == Vector3.zero || Vector3.Angle(transform.forward, lookPos - transform.position) > lookAngle) {
        return false;
      }

      return true;
    }

    private void DeactivateIK() {
      DeactivateWeaponAimIK();
      anim.SetLookAtWeight(0);
    }

    private void LerpWeaponAimIK(float aimIkWeight) {
      anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimIkWeight);
      anim.SetIKRotationWeight(AvatarIKGoal.RightHand, aimIkWeight);
      anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, aimIkWeight);

      if (ikAimLHand != null) {
        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, aimIkWeight);
        anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, aimIkWeight);

        if (ikAimRElbow != null)
          anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, aimIkWeight);
      }
    }

    private void DeactivateWeaponAimIK() {
      aimWeightCurrent = 0f;
      anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
      anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
      anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);

      anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
      anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
      anim.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 0);
    }
  }
}