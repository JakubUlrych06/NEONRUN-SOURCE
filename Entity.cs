using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private float startingHealth = 10f;
    private float health;
    public float originalKills;
    private Vector3 initialPosition;
    private BoxCollider boxCollider; // Reference to the BoxCollider
    private MeshRenderer meshRenderer; // Reference to the MeshRenderer

    public float Health
    {
        get { return health; }
        set
        {
            health = value;
            Debug.Log(health);

            if (health <= 0f)
            {   
                StartCoroutine(chc());
                PlayerMovementAdvanced player = FindObjectOfType<PlayerMovementAdvanced>();
                
                player.killsLeft--;
                Debug.Log("Kills left: " + player.killsLeft);

                // Disable the BoxCollider and MeshRenderer when health is 0
                if (boxCollider != null)
                {
                    boxCollider.enabled = false;
                }
                else
                {
                    Debug.LogWarning("BoxCollider is not assigned!");
                }

                if (meshRenderer != null)
                {
                    meshRenderer.enabled = false;
                }
                else
                {
                    Debug.LogWarning("MeshRenderer is not assigned!");
                }
            }
        }
    }

    void Awake() // Use Awake to ensure initialization before Start
    {
        // Get the BoxCollider and MeshRenderer components
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (boxCollider == null)
        {
            Debug.LogWarning("Entity has no BoxCollider attached!");
        }

        if (meshRenderer == null)
        {
            Debug.LogWarning("Entity has no MeshRenderer attached!");
        }
    }

    void Start()
    {
        PlayerMovementAdvanced player = FindObjectOfType<PlayerMovementAdvanced>();

        originalKills = player.killsLeft;

        initialPosition = transform.position;
        Health = startingHealth;
    }

    public void Respawn()
    {
        hide hideScript = FindObjectOfType<hide>();
        PlayerMovementAdvanced player = FindObjectOfType<PlayerMovementAdvanced>();

        hideScript.RE();
        player.killsLeft = originalKills;
        Debug.Log("Respawning...");

        transform.position = initialPosition;
        Health = startingHealth;

        // Re-enable the BoxCollider and MeshRenderer
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }
        else
        {
            Debug.LogWarning("BoxCollider is not assigned!");
        }

        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
        }
        else
        {
            Debug.LogWarning("MeshRenderer is not assigned!");
        }
    }
    IEnumerator chc()
    {
        yield return new WaitForSeconds(0.1f);

        hide hideScript = FindObjectOfType<hide>();
        hideScript.chack();
    }
}
