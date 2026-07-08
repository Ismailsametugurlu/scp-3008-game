using UnityEngine;
using UnityEngine.InputSystem;

// FPS hareket, kamera, çömelme, ıslık — direkt Input API kullanır (generated class gerektirmez)
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float gravity = -20f;

    [Header("Çömelme")]
    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float crouchTransitionSpeed = 10f;

    [Header("Kamera")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxVerticalAngle = 85f;
    [SerializeField] private Transform cameraHolder;

    [Header("Islık")]
    [SerializeField] private AudioClip whistleClip;
    [SerializeField] private float whistleCooldown = 3f;

    private CharacterController cc;
    private AudioSource audioSource;
    private PlayerStatsController statsController;

    private Vector3 verticalVelocity;
    private float verticalRotation;
    private float targetHeight;
    private bool isCrouching;
    private float lastWhistleTime = -999f;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        statsController = GetComponent<PlayerStatsController>();

        targetHeight = standHeight;
        cc.height = standHeight;
        cc.center = new Vector3(0f, standHeight * 0.5f, 0f);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleCrouch();
        HandleCrouchTransition();
        ApplyGravity();
        HandleWhistle();
    }

    // Mouse hareketi: X → oyuncu yatay dönüşü, Y → kamera dikey dönüşü
    private void HandleLook()
    {
        Vector2 delta = Mouse.current.delta.ReadValue();

        transform.Rotate(Vector3.up, delta.x * mouseSensitivity);

        verticalRotation -= delta.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // WASD + Shift ile hareket
    private void HandleMovement()
    {
        var kb = Keyboard.current;
        float x = (kb.dKey.isPressed ? 1f : 0f) - (kb.aKey.isPressed ? 1f : 0f);
        float z = (kb.wKey.isPressed ? 1f : 0f) - (kb.sKey.isPressed ? 1f : 0f);

        bool isMoving = Mathf.Abs(x) > 0.01f || Mathf.Abs(z) > 0.01f;
        bool wantsSprint = kb.leftShiftKey.isPressed;
        bool canSprint = statsController == null || statsController.CanSprint;
        // Gerçek koşma: Shift + hareket var + çömelme yok + stamina izin veriyor
        bool sprinting = wantsSprint && canSprint && isMoving && !isCrouching;

        // Stamina sistemi bu gerçek durumu kullanır (dururken Shift boşa harcamaz)
        if (statsController != null) statsController.IsActivelySprinting = sprinting;

        float statMult = statsController != null ? statsController.SpeedMultiplier : 1f;
        float speed = (isCrouching ? crouchSpeed : (sprinting ? runSpeed : walkSpeed)) * statMult;

        Vector3 move = transform.right * x + transform.forward * z;
        cc.Move(move * speed * Time.deltaTime);
    }

    // C ile çökelme toggle; ayağa kalkarken baş üstü engel kontrolü
    private void HandleCrouch()
    {
        if (!Keyboard.current.cKey.wasPressedThisFrame) return;

        if (isCrouching)
        {
            bool blocked = Physics.SphereCast(
                transform.position + Vector3.up * crouchHeight,
                cc.radius * 0.9f, Vector3.up, out _,
                standHeight - crouchHeight + 0.1f
            );
            if (blocked) return;
        }

        isCrouching = !isCrouching;
        targetHeight = isCrouching ? crouchHeight : standHeight;
    }

    // CharacterController yüksekliğini yumuşakça değiştirir
    private void HandleCrouchTransition()
    {
        if (Mathf.Abs(cc.height - targetHeight) < 0.01f) return;

        float h = Mathf.Lerp(cc.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        cc.height = h;
        cc.center = new Vector3(0f, h * 0.5f, 0f);

        if (cameraHolder != null)
        {
            Vector3 p = cameraHolder.localPosition;
            cameraHolder.localPosition = new Vector3(p.x, h * 0.85f, p.z);
        }
    }

    // Yerçekimi uygula
    private void ApplyGravity()
    {
        if (cc.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;
        else
            verticalVelocity.y += gravity * Time.deltaTime;

        cc.Move(verticalVelocity * Time.deltaTime);
    }

    // T tuşu ıslık; cooldown dolmadan çalmaz
    private void HandleWhistle()
    {
        if (!Keyboard.current.tKey.wasPressedThisFrame) return;
        if (Time.time - lastWhistleTime < whistleCooldown) return;

        lastWhistleTime = Time.time;
        if (whistleClip != null) audioSource.PlayOneShot(whistleClip);
        Debug.Log("[Player] Islık!");
    }
}
