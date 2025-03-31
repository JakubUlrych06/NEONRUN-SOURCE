using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hide : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void chack()
    {   
                    


        PlayerMovementAdvanced player = FindObjectOfType<PlayerMovementAdvanced>();


        if (player.killsLeft == 0)
        {
            MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
            BoxCollider boxCollider = GetComponent<BoxCollider>();

            if (meshRenderer != null)
                meshRenderer.enabled = false;

            if (boxCollider != null)
                boxCollider.enabled = false;
        }

    }
    public void RE()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        if (meshRenderer != null)
            meshRenderer.enabled = true;

        if (boxCollider != null)
            boxCollider.enabled = true;
    }
}
