using UnityEngine;
using System.Collections;

public class EnemyBehaviour : MonoBehaviour
{
    // Enemies move speed, assigned in the inspector
    public float moveSpeed;

    // Reference to our transform component, faster to use this variable
    private Transform myTransform;

    // Awake is called once and before Start()
    void Awake()
    {
        // Save myTransform reference
        myTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        // Move at move speed on local x-axis
        myTransform.Translate(moveSpeed * Time.deltaTime, 0, 0, Space.Self);

        // Variable to store hit information from raycast
        RaycastHit hitInfo;

        // Check if a ray in front of us hit anything, we use the conditional operator (?:) to determine direction
        if (Physics.Raycast(myTransform.position, new Vector3(myTransform.rotation.eulerAngles.y < 180 ? 1 : -1, 0, 0), out hitInfo, 1))
        {
            // Check if the character was hit, and respawn if it was
            if (hitInfo.collider.GetComponent<Character>() != null)
                hitInfo.collider.GetComponent<Character>().Respawn();
            // Turn around if hit anything else
            else
                TurnAround();
        } 
    }

    // This method is called whenever we get killed by the character
    public void Die()
    {
        Destroy(gameObject);
    }

    void TurnAround()
    {
        // We use the conditional (?:) operator to determine correct rotation
        myTransform.rotation = Quaternion.Euler(0, myTransform.rotation.eulerAngles.y < 180 ? 180 : 0, 0);
    }
}