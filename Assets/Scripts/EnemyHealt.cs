using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100; // Düþmanýn maksimum caný
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Bu fonksiyonu Player'ýn script'inden çaðýracaðýz
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log(gameObject.name + " " + damage + " hasar aldý! Kalan can: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " öldü.");
        // Obje öldüðünde onu sahneden yok et
        Destroy(gameObject);
    }
}