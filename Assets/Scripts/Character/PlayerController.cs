using System.Collections;
using UnityEngine;

public class InputKeys {
  public static KeyCode Dodge = KeyCode.Space;
  public static KeyCode Crouch = KeyCode.LeftShift;
  public static KeyCode EquipWeapon1 = KeyCode.Alpha1;
  public static KeyCode EquipWeapon2 = KeyCode.Alpha2;
  public static KeyCode EquipWeapon3 = KeyCode.Alpha3;
  public static KeyCode EquipWeapon4 = KeyCode.Alpha4;
  public static KeyCode EquipWeapon5 = KeyCode.Alpha5;
  public static KeyCode EquipWeapon6 = KeyCode.Alpha6;
  public static KeyCode UnequipWeapon = KeyCode.Q;
  public static KeyCode SwapCamShouler = KeyCode.V;
  public static KeyCode ReloadWeapon = KeyCode.R;
}

public enum ControlState {
  Locomotion,
  Dodge,
  Hit,
}

public class PlayerController : DudeController {

  protected Camera mainCam;
  protected CameraManager camManager;

  private float groundCheckDistance = 0.4f;
  private LayerMask groundCheckLayer;
  private LayerMask shootCheckLayer;
  private float fallMultiplier = 1.5f;
  private float rollSpeed = 10f;
  private float weaponIdleTimeout = 1f;
  private float turnLerpSpeed = 10f;
  private float shootTurnLerpSpeed = 3f;

  private ControlState controlState = ControlState.Locomotion;
  private Vector3 moveVector;

  private bool dodgeMovement = false;
  private bool isGrounded = true;
  private bool isAiming = false;
  private bool isEquipped = false;
  private bool weaponControlDisabled = false;
  private float lastShotTime = 0f;

  #region Init
  public void SetUp(LayerMasks layerMasks) {
    groundCheckLayer = layerMasks.groundLayerMask;
    shootCheckLayer = layerMasks.shootLayerMask;
  }

  new void Awake() {
    base.Awake();

    mainCam = Camera.main;
    Cursor.lockState = CursorLockMode.Locked;

    camManager = FindObjectOfType<CameraManager>();
  }
  #endregion Init

  #region Update
  void OnAnimatorMove() {
    if (controlState != ControlState.Dodge) {
      controller.Move(anim.deltaPosition);
    }
  }

  void Update() {
    ApplyGravity();

    float v = Input.GetAxis("Vertical");
    float h = Input.GetAxis("Horizontal");

    switch (controlState) {
      case ControlState.Locomotion:
        HandleLocomotion(h, v);
        break;
      case ControlState.Dodge:
        HandleDodge();
        break;
    }
  }
  #endregion Update

  #region MovementControl
  private void TriggerDodge(float h, float v) {
    Vector3 dodgeDir = GetInputDirectionByCamera(h, v);
    TurnTo(dodgeDir);
    anim.SetTrigger("dodge");
    controlState = ControlState.Dodge;

    EndZoom();
    EndAim();
  }

  private void HandleDodge() {
    if (dodgeMovement) {
      moveVector += transform.forward * rollSpeed;
    }

    MakeMove();
  }

  private void HandleLocomotion(float h, float v) {
    if (Input.GetKeyDown(InputKeys.Dodge)) {
      TriggerDodge(h, v);
      return;
    }

    if (Input.GetKeyDown(InputKeys.SwapCamShouler)) {
      camManager.SwapShoulder();
    }

    HandleIKLooking();
    HandleMoving(h, v);

    // TODO: set up a combat controlState vs freeMovement control state
    if (isEquipped)
      HandleWeapons();
    else {
      HandleUnarmed();
    }
  }

  private void HandleMoving(float h, float v) {
    if (isEquipped) {
      // strafe movement
      if (v != 0 || h != 0 || isAiming) {
        TurnToCamera();
      }

      // if the inputs are zero, dampen the animation so its not too snappy
      if (v == 0) {
        anim.SetFloat("verticalSpeed", Mathf.Lerp(anim.GetFloat("verticalSpeed"), v, 0.2f));
      } else {
        anim.SetFloat("verticalSpeed", v);
      }

      if (h == 0) {
        anim.SetFloat("horizontalSpeed", Mathf.Lerp(anim.GetFloat("horizontalSpeed"), h, 0.2f));
      } else {
        anim.SetFloat("horizontalSpeed", h);
      }

    } else {
      // free movement
      anim.SetFloat("horizontalSpeed", 0);

      Vector3 moveDir = GetInputDirectionByCamera(h, v);
      if (moveDir.magnitude == 0) {
        anim.SetFloat("verticalSpeed", Mathf.Lerp(anim.GetFloat("verticalSpeed"), moveDir.magnitude, 0.2f));
      } else {
        FreeMovementTurn(moveDir);
        anim.SetFloat("verticalSpeed", moveDir.magnitude);
      }
    }

    if (Input.GetKey(InputKeys.Crouch)) {
      anim.SetBool("crouch", true);
    } else {
      anim.SetBool("crouch", false);
    }

    MakeMove();
  }

  #endregion MovementControl

  #region WeaponControl
  private void HandleWeapons() {
    if (weaponControlDisabled) return;

    if (weaponManager.IsAutomaticWeapon() ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0)) {
      if (isAiming) {
        Shoot();
      } else {
        StartCoroutine(TurnAndShoot());
        return;
      }
    }

    if (Input.GetMouseButton(1)) {
      TriggerAim();
      TriggerZoom();
    } else if (Time.time - lastShotTime > weaponIdleTimeout) {
      EndAim();
    }

    if (Input.GetMouseButtonUp(1)) {
      EndZoom();
      EndAim();
    }

    if (Input.GetKeyDown(InputKeys.ReloadWeapon)) {
      TriggerReload();
    }

    if (Input.GetKeyDown(InputKeys.UnequipWeapon) && weaponManager.UnequipWeapon()) {
      EndZoom();
      EndAim();
      isEquipped = false;
      GetComponent<Animator>().SetBool("weaponEquipped", false);
    }
  }

  private void HandleUnarmed() {
    if (Input.GetKeyDown(InputKeys.EquipWeapon1)) {
      EquipWeapon(1);
      return;
    }
    if (Input.GetKeyDown(InputKeys.EquipWeapon2)) {
      EquipWeapon(2);
      return;
    }
    if (Input.GetKeyDown(InputKeys.EquipWeapon3)) {
      EquipWeapon(3);
      return;
    }
    if (Input.GetKeyDown(InputKeys.EquipWeapon4)) {
      EquipWeapon(4);
      return;
    }
    if (Input.GetKeyDown(InputKeys.EquipWeapon5)) {
      EquipWeapon(5);
      return;
    }
    if (Input.GetKeyDown(InputKeys.EquipWeapon6)) {
      EquipWeapon(6);
      return;
    }
  }

  private void EquipWeapon(int weaponNum) {
    Transform leftHandIK;
    Transform leftElbowIK;
    int weaponAnimId;
    if (weaponManager.EquipWeapon(weaponNum, out leftHandIK, out leftElbowIK, out weaponAnimId)) {
      isEquipped = true;
      anim.SetFloat("weaponId", weaponAnimId);
      anim.SetBool("weaponEquipped", true);
      ikManager.SetLeftArmIK(leftHandIK, leftElbowIK);
    }
  }

  private void TriggerAim() {
    anim.SetBool("weaponAim", true);
    ikManager.ToggleAim(true);
    isAiming = true;
  }

  private void EndAim() {
    anim.SetBool("weaponAim", false);
    ikManager.ToggleAim(false);
    isAiming = false;
  }

  private void TriggerZoom() {
    camManager.ToggleZoom(true);
  }

  private void EndZoom() {
    camManager.ToggleZoom(false);
  }

  private void TriggerReload() {
    EndAim();
    EndZoom();
    anim.SetTrigger("reload");
    weaponControlDisabled = true;
  }

  private IEnumerator TurnAndShoot() {
    weaponControlDisabled = true;
    // Want to trigger the aim anim here, but not set the bool
    anim.SetBool("weaponAim", true);
    yield return StartCoroutine(ShootTurnToCamera());
    TriggerAim();
    Shoot();
    weaponControlDisabled = false;
  }

  private void Shoot() {
    if (weaponManager.Shoot()) {
      anim.SetTrigger("shoot");
      lastShotTime = Time.time;
    }
  }

  private IEnumerator ShootTurnToCamera() {
    float lerpFactor = 0f;
    Vector3 camForward = mainCam.transform.forward;
    float distance = Vector3.Angle(camForward, transform.forward);

    do {
      lerpFactor += shootTurnLerpSpeed * Time.deltaTime * (180f / distance);
      TurnTo(mainCam.transform.forward, lerpFactor);
      yield return null;
    } while ((lerpFactor < 1));
  }
  #endregion WeaponControl

  private void HandleIKLooking() {
    Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
    RaycastHit hit;
    if (Physics.Raycast(ray, out hit, 1000f, shootCheckLayer)) {
      ikManager.SetLookPosition(hit.point);
      if (isEquipped) weaponManager.PerformAimRaycast(hit.point, shootCheckLayer);
    } else {
      Vector3 lookPos = ray.GetPoint(1000f);
      ikManager.SetLookPosition(lookPos);
      if (isEquipped) weaponManager.PerformAimRaycast(lookPos, shootCheckLayer);
    }
  }

  #region MovementHelpers
  private void MakeMove() {
    controller.Move(moveVector * Time.deltaTime);
  }

  private bool ApplyGravity() {
    if (controller.isGrounded) {
      isGrounded = true;
      moveVector = Physics.gravity * Time.deltaTime;
      return true;
    } else if (moveVector.y <= 0 && Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, groundCheckDistance, groundCheckLayer)) {
      isGrounded = true;
      moveVector = (hit.point - transform.position) / Time.deltaTime;
      return true;
    }
    isGrounded = false;
    moveVector += Physics.gravity * Time.deltaTime * (moveVector.y <= 0 ? fallMultiplier : 1f);
    return false;
  }

  private void FreeMovementTurn(Vector3 turnDir) {
    TurnTo(turnDir, turnLerpSpeed * Time.deltaTime);
  }

  private void TurnToCamera(float overrideSpeed = 0f) {
    TurnTo(mainCam.transform.forward, (overrideSpeed != 0f ? overrideSpeed : turnLerpSpeed) * Time.deltaTime);
  }

  private void TurnTo(Vector3 turnDir, float lerpFactor = 1f) {
    turnDir.y = 0f;

    Quaternion targetDirection = Quaternion.LookRotation(turnDir);
    transform.rotation = Quaternion.Slerp(transform.rotation, targetDirection, lerpFactor);
  }

  private Vector3 GetInputDirectionByCamera(float h, float v) {
    //camera forward and right vectors:
    var forward = mainCam.transform.forward;
    var right = mainCam.transform.right;

    //project forward and right vectors on the horizontal plane (y = 0)
    forward.y = 0f;
    right.y = 0f;
    forward.Normalize();
    right.Normalize();

    //this is the direction in the world space we want to move:
    return forward * v + right * h;
  }
  #endregion MovementHelpers
  #region AnimationEvents
  /// ANIMATION EVENTS
  void DodgeEvent(string msg) {
    if (msg == "start") {
      dodgeMovement = true;
      return;
    }

    if (msg == "end") {
      anim.ResetTrigger("dodge");
      controlState = ControlState.Locomotion;
      dodgeMovement = false;
      return;
    }
  }

  void ReloadEvent(string msg) {
    if (msg == "start") {
      // redundant
      weaponControlDisabled = true;
      return;
    }

    if (msg == "end") {
      anim.ResetTrigger("reload");
      weaponControlDisabled = false;
      return;
    }
  }
  #endregion AnimationEvents
}