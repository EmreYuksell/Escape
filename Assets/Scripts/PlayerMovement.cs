using UnityEngine;
using Cinemachine; // <-- BU SATIR ÇOK ÖNEMLÝ

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    // Animator referansýný þimdilik kaldýrýyoruz.

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
    public CinemachineFreeLook aimCamera; // Niþan kamerasý referansý
    public bool isAiming = false;
    private int aimPriority = 20; // Niþan alýrken kameranýn önceliði (normal kameradan (10) yüksek olmalý)

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
        // 1. Niþan Almayý Kontrol Et
        if (Input.GetMouseButton(1)) // Sað tuþa basýlý tutuluyorsa
        {
            isAiming = true;
            aimCamera.Priority = aimPriority; // Aim kamerasýnýn önceliðini artýr
        }
        else
        {
            isAiming = false;
            aimCamera.Priority = 5; // Önceliði normale (varsayýlan kameradan (10) düþük) çek
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

        // 4. Hareket ve Dönüþ

        // Eðer niþan alýyorsak VEYA hareket ediyorsak karakter dönsün
        if (direction.magnitude >= 0.1f || isAiming)
        {
            float targetAngle;

            if (isAiming)
            {
                // Niþan alýrken, karakter her zaman kameranýn baktýðý yöne dönsün
                targetAngle = cam.eulerAngles.y;
            }
            else
            {
                // Normal hareket ederken, kameraya göre basýlan yöne dönsün
                targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            }

            // Dönüþü yumuþat
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Sadece hareket girdisi varsa karakteri ilerlet
            if (direction.magnitude >= 0.1f)
            {
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * speed * Time.deltaTime);
            }

        }

        // 5. Yerçekimini Uygula
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 6. ATEÞ ETME (YENÝ KOD)

        if (Input.GetMouseButtonDown(0)) // Sol fare tuþuna basýldýðýnda
        {
            audioSource.PlayOneShot(gunshotSound);  
            Debug.DrawRay(cam.position, cam.forward * 100f, Color.red, 1.0f);

            if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hitInfo, 100f, aimLayerMask))
            {
                // Vurduðumuz objenin "EnemyHealth" script'i var mý diye kontrol et
                EnemyHealth enemy = hitInfo.collider.GetComponent<EnemyHealth>();

                // Eðer varsa (yani vurduðumuz þey bir düþmansa)
                if (enemy != null)
                {
                    // EFEKTÝ OLUÞTUR
                    // Instantiate(prefab, position, rotation);
                    // hitInfo.point = merminin çarptýðý tam nokta
                    // hitInfo.normal = merminin çarptýðý yüzeyin baktýðý yön (efektin dýþa doðru bakmasý için)
                    Instantiate(hitEffectPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));

                    // Düþmanýn TakeDamage fonksiyonunu çaðýr ve 25 hasar ver
                    enemy.TakeDamage(25);
                }
                else
                {
                    // Düþman deðilse, neye vurduðumuzu yaz
                    Debug.Log("Vurulan Obje: " + hitInfo.collider.name);
                }
            }
            else
            {
                Debug.Log("Boþa ateþ edildi.");
            }
        }

    }
}