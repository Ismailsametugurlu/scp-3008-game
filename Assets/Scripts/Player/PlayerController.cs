using UnityEngine;
using UnityEngine.InputSystem;

// Oyuncunun FPS hareketini, kamera bakışını, çömelme ve ıslık mekaniklerini yönetir.
// CharacterController kullanır — Rigidbody değil (daha öngörülebilir FPS hareketi için).
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Header("Hareket Hızları")]
    [SerializeField] private float walkSpeed = 4f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float crouchSpeed = 2f;

    [Header("Yerçekimi")]
    [SerializeField] private float gravity = -20f;

    [Header("Çömelme")]
    [SerializeField] private float standHeight = 1.8f;
    [SerializeField] private float crouchHeight = 0.9f;
    [SerializeField] private float crouchTransitionSpeed = 10f;

    [Header("Kamera Bakışı")]
    [SerializeField] private float mouseSensitivity = 0.15f;
    [SerializeField] private float maxVerticalAngle = 85f;
    // Kameranın parent transform'u (göz hizasında boş bir GameObject)
    [SerializeField] private Transform cameraHolder;

    [Header("Islık")]
    [SerializeField] private AudioClip whistleClip;
    [SerializeField] private float whistleCooldown = 3f;

    private CharacterController cc;
    private InputSystem_Actions inputActions;
    private AudioSource audioSource;

    // Lambda event'leri doğru şekilde unsubscribe edebilmek için referans tutulur
    private System.Action<InputAction.CallbackContext> onCrouch;
    private System.Action<InputAction.CallbackContext> onWhistle;

    private Vector3 verticalVelocity;
    private float verticalRotation;
    private float targetHeight;
    private bool isCrouching;
    private float lastWhistleTime = -999f;

    private void Awake()
    {
        cc          = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        inputActions = new InputSystem_Actions();

        // CharacterController başlangıç boyutunu ayakta yüksekliğe eşitle
        targetHeight = standHeight;
        cc.height    = standHeight;
        cc.center    = new Vector3(0f, standHeight * 0.5f, 0f);
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        onCrouch  = _ => ToggleCrouch();
        onWhistle = _ => TryWhistle();

        inputActions.Player.Crouch.performed  += onCrouch;
        inputActions.Player.Whistle.performed += onWhistle;
    }

    private void OnDisable()
    {
        inputActions.Player.Crouch.performed  -= onCrouch;
        inputActions.Player.Whistle.performed -= onWhistle;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        // FPS için mouse'u ekrana kilitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible   = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleCrouchTransition();
        ApplyGravity();
    }

    // Mouse X → oyuncuyu yatayda döndür | Mouse Y → kamerayı dikeyeye döndür
    private void HandleLook()
    {
        Vector2 look = inputActions.Player.Look.ReadValue<Vector2>();

        // Oyuncu Y ekseninde döner (yatay bakış)
        transform.Rotate(Vector3.up, look.x * mouseSensitivity);

        // Kamera X ekseninde döner (dikey bakış), aşırı açı engellenir
        verticalRotation -= look.y * mouseSensitivity;
        verticalRotation  = Mathf.Clamp(verticalRotation, -maxVerticalAngle, maxVerticalAngle);

        if (cameraHolder != null)
            cameraHolder.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // WASD hareketi; çökelme/koşma durumuna göre hız seçilir
    private void HandleMovement()
    {
        Vector2 input     = inputActions.Player.Move.ReadValue<Vector2>();
        bool    sprinting = inputActions.Player.Sprint.IsPressed();

        float   speed = isCrouching ? crouchSpeed : (sprinting ? runSpeed : walkSpeed);
        Vector3 move  = transform.right * input.x + transform.forward * input.y;

        cc.Move(move * speed * Time.deltaTime);
    }

    // C tuşu: çökelme toggle. Ayağa kalkarken baş üstü engel kontrolü yapılır
    private void ToggleCrouch()
    {
        if (isCrouching)
        {
            // Üstte nesne varsa ayağa kalkma
            bool blocked = Physics.SphereCast(
                transform.position + Vector3.up * crouchHeight,
                cc.radius * 0.9f,
                Vector3.up,
                out _,
                standHeight - crouchHeight + 0.1f
            );
            if (blocked) return;
        }

        isCrouching  = !isCrouching;
        targetHeight = isCrouching ? crouchHeight : standHeight;
    }

    // CharacterController yüksekliğini hedef değere doğru yumuşakça geçirir
    private void HandleCrouchTransition()
    {
        if (Mathf.Abs(cc.height - targetHeight) < 0.01f) return;

        float newHeight = Mathf.Lerp(cc.height, targetHeight, crouchTransitionSpeed * Time.deltaTime);
        cc.height = newHeight;
        cc.center = new Vector3(0f, newHeight * 0.5f, 0f);

        // Kamera holder'ı yüksekliğe göre kaydır (göz hizasını koru)
        if (cameraHolder != null)
        {
            Vector3 p = cameraHolder.localPosition;
            cameraHolder.localPosition = new Vector3(p.x, newHeight * 0.85f, p.z);
        }
    }

    // Yerçekimi: yerde küçük negatif değer (zemine tutunma), havada artan düşüş hızı
    private void ApplyGravity()
    {
        if (cc.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;
        else
            verticalVelocity.y += gravity * Time.deltaTime;

        cc.Move(verticalVelocity * Time.deltaTime);
    }

    // T tuşu: cooldown süresi dolmadıysa ıslık çalmaz; dolduysa ses oynatır ve log basar
    private void TryWhistle()
    {
        if (Time.time - lastWhistleTime < whistleCooldown) return;

        lastWhistleTime = Time.time;

        if (whistleClip != null)
            audioSource.PlayOneShot(whistleClip);

        // TODO (Oturum 10): Network üzerinden diğer oyunculara ıslık sinyali gönder
        Debug.Log($"[PlayerController] Islık! Cooldown: {whistleCooldown}s");
    }
}
