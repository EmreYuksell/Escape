using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    // Efektin ne kadar s�re sonra yok olaca��n� saniye cinsinden belirler
    public float lifetime = 2.0f;

    void Start()
    {
        // Bu script'in ba�l� oldu�u GameObject'i 'lifetime' saniye sonra yok et
        Destroy(gameObject, lifetime);
    }
}