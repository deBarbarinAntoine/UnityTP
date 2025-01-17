using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Vitesse de d�placement
    public float lookSensitivity = 2f; // Sensibilit� de la souris
    public Camera playerCamera; // Cam�ra du joueur
    public Rigidbody playerRigidbody; // Rigidbody pour le d�placement physique

    private Toast inputActions;
    private Vector2 moveInput; // Valeur des axes de d�placement
    private Vector2 lookInput; // Valeur des axes de la souris
    private float verticalRotation = 0f; // Rotation verticale (limit�e � -90/+90)

    private void Awake()
    {
        inputActions = new Toast();
    }

    private void OnEnable()
    {
        // Activer les actions d'Input
        inputActions.Player.Enable();

        // Associer les actions
        inputActions.Player.Move.performed += context => moveInput = context.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += context => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += context => lookInput = context.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += context => lookInput = Vector2.zero;
    }

    private void OnDisable()
    {
        // D�sactiver les actions d'Input
        inputActions.Player.Disable();
    }

    private void Update()
    {
        // Gestion de la rotation (cam�ra + joueur)
        RotatePlayer();
    }

    private void FixedUpdate()
    {
        // Gestion du mouvement physique
        MovePlayer();
    }

    private void MovePlayer()
    {
        // Calculer le d�placement
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Appliquer le mouvement au Rigidbody
        playerRigidbody.MovePosition(playerRigidbody.position + move * moveSpeed * Time.fixedDeltaTime);
    }

    private void RotatePlayer()
    {
        // Rotation horizontale (joueur)
        float mouseX = lookInput.x * lookSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // Rotation verticale (cam�ra)
        float mouseY = lookInput.y * lookSensitivity;
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f); // Limiter la rotation verticale
        playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }
}
