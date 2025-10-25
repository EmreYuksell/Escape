using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    // Efektin ne kadar süre sonra yok olacaðýný saniye cinsinden belirler
    public float lifetime = 2.0f;

    void Start()
    {
        // Bu script'in baðlý olduðu GameObject'i 'lifetime' saniye sonra yok et
        Destroy(gameObject, lifetime);
    }
}