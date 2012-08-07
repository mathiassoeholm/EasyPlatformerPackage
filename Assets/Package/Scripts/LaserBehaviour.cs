using UnityEngine;
using System.Collections;

public class LaserBehaviour : ToggleBehaviour
{
    // How fast the animatios runs in frames per second, assigned in the inspector
    public float animationFPS;

    // An array of materials, that the animation shifts between as frames, assigned in the inspector
    public Material[] textures;

    // The width of the lasers line renderer, assigned in the inspector
    public float width;

    // A linerenderer used to render the laser effect
    private LineRenderer lineRenderer;

    // Current index for the texture used for animation
    private int currentTexture;
    
    public override void Toggle()
    {
        // Toggle the laser
        lineRenderer.enabled = !lineRenderer.enabled;
    }

	// Use this for initialization
    new void Start()
    {
        // Call base classes start method first
        base.Start();
        
        // Add a line renderer component to this gameobject
        lineRenderer = gameObject.AddComponent<LineRenderer>();

        // Sets the number of line segments
        lineRenderer.SetVertexCount(2);

        // Set the width of the line renderer
        lineRenderer.SetWidth(width, width);

        // Assign materil '0' as the material for the line renderer
        lineRenderer.material = textures[0];

        // Start line renderer animation, that shifts bewteen materials
        StartCoroutine(ChangeTexture());
	}
	
	IEnumerator ChangeTexture()
    {
        // While(true) is an infinite loop and will always run
        while (true)
        {
            int i;

            // Assign i a random value, that is not equal to our current texture
            while ((i = Random.Range(0, textures.Length)) == currentTexture) { };

            currentTexture = i;

            // Assign new material
            lineRenderer.material = textures[currentTexture];

            // Wait for fps
            yield return new WaitForSeconds(1 / animationFPS);
        }
	}

    // Update is called once per frame
    void Update()
    {
        // Stop if the renderer is not enabled
        if (!lineRenderer.enabled)
            return;

        // Set linerenders end points
        lineRenderer.SetPosition(0, transform.GetChild(0).position);
        lineRenderer.SetPosition(1, transform.GetChild(1).position);

        // Set width
        lineRenderer.SetWidth(width, width);

        // Set material scale
        lineRenderer.material.SetTextureScale("_MainTex", new Vector2(Vector2.Distance(transform.GetChild(0).position, transform.GetChild(1).position) / width, 1));
        
        // Variable to store raycast hit information
        RaycastHit hitInfo;

        Debug.DrawRay(transform.GetChild(0).position, transform.GetChild(1).position - transform.GetChild(0).position, Color.green);

        // Check if hit anything
        if (Physics.Raycast(transform.GetChild(0).position, transform.GetChild(1).position - transform.GetChild(0).position, out hitInfo))
        {
            // Check if we hit the character
            if (hitInfo.collider.GetComponent<Character>() != null)
                hitInfo.collider.GetComponent<Character>().Respawn();
            // Check if we hit countdown explosive (TNT)
            else if (hitInfo.collider.GetComponent<CountDownExplosive>() != null)
                hitInfo.collider.GetComponent<CountDownExplosive>().Explode();
            // Check if we hit Nitro explosion
            else if (hitInfo.collider.GetComponent<Explosive>() != null)
                hitInfo.collider.GetComponent<Explosive>().Explode();
            // Check if we hit an enemy
            else if(hitInfo.collider.GetComponent<EnemyBehaviour>())
                hitInfo.collider.GetComponent<EnemyBehaviour>().Die();
        }
    }
}
