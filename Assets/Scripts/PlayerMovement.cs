using UnityEngine;
using Cinemachine; // <-- BU SATIR �OK �NEML�

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    // Animator referans�n� �imdilik kald�r�yoruz.

    [Header("Movement")]
    public float speed = 6.0f;

    [Header("Camera & Rotation")]
    public Transform cam;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Audio")]
    public AudioClip gunshotSound; //
    private AudioSource audioSource;

    [Header("Aiming")]
    public CinemachineFreeLook aimCamera; // Ni�an kameras� referans�
    public bool isAiming = false;
    private int aimPriority = 20; // Ni�an al�rken kameran�n �nceli�i (normal kameradan (10) y�ksek olmal�)

    [Header("Shooting")] 
    public LayerMask aimLayerMask; 
    public GameObject hitEffectPrefab;

    [Header("Physics (Gravity)")]
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float gravityValue = -9.81f;




    void Start()
    {
        controller = GetComponent<CharacterController>();
 
        audioSource = GetComponent<AudioSource>(); 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 1. Ni�an Almay� Kontrol Et
        if (Input.GetMouseButton(1)) // Sa� tu�a bas�l� tutuluyorsa
        {
            isAiming = true;
            aimCamera.Priority = aimPriority; // Aim kameras�n�n �nceli�ini art�r
        }
        else
        {
            isAiming = false;
            aimCamera.Priority = 5; // �nceli�i normale (varsay�lan kameradan (10) d���k) �ek
        }

        // 2. Yerde miyiz diye kontrol et
        isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // 3. Girdileri Al (Input)
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(horizontal, 0f, vertical);
        Vector3 direction = inputDir.normalized;

        // 4. Hareket ve D�n��

        // E�er ni�an al�yorsak VEYA hareket ediyorsak karakter d�ns�n
        if (direction.magnitude >= 0.1f || isAiming)
        {
            float targetAngle;

            if (isAiming)
            {
                // Ni�an al�rken, karakter her zaman kameran�n bakt��� y�ne d�ns�n
                targetAngle = cam.eulerAngles.y;
            }
            else
            {
                // Normal hareket ederken, kameraya g�re bas�lan y�ne d�ns�n
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            }

            // D�n��� yumu�at
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Sadece hareket girdisi varsa karakteri ilerlet
            if (direction.magnitude >= 0.1f)
            {
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

        }

        // 5. Yer�ekimini Uygula
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 6. ATE� ETME (YEN� KOD)

        if (Input.GetMouseButtonDown(0)) // Sol fare tu�una bas�ld���nda
        {
            audioSource.PlayOneShot(gunshotSound);  
            Debug.DrawRay(cam.position, cam.forward * 100f, Color.red, 1.0f);

            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, 100f, aimLayerMask))
            {
                // Vurdu�umuz objenin "EnemyHealth" script'i var m� diye kontrol et
                EnemyHealth enemy = hitInfo.collider.GetComponent<EnemyHealth>();

                // E�er varsa (yani vurdu�umuz �ey bir d��mansa)
                if (enemy != null)
                {
                    // EFEKT� OLU�TUR
                    // Instantiate(prefab, position, rotation);
                    // hitInfo.point = merminin �arpt��� tam nokta
                    // hitInfo.normal = merminin �arpt��� y�zeyin bakt��� y�n (efektin d��a do�ru bakmas� i�in)
                    Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                    // D��man�n TakeDamage fonksiyonunu �a��r ve 25 hasar ver
                    enemy.TakeDamage(25);
                }
                else
                {
                    // D��man de�ilse, neye vurdu�umuzu yaz
                    Debug.Log("Vurulan Obje: " + hitInfo.collider.name);
                }
            }
            else
            {
                Debug.Log("Bo�a ate� edildi.");
            }
        }

    }
}