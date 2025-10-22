using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100; // D��man�n maksimum can�
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Bu fonksiyonu Player'�n script'inden �a��raca��z
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(gameObject.name + " " + damage + " hasar ald�! Kalan can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " �ld�.");
        // Obje �ld���nde onu sahneden yok et
        Destroy(gameObject);
    }
}