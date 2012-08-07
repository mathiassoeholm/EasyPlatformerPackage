using UnityEngine;

class CameraBehaviour : MonoBehaviour
{
    // Move speed assigned in the inspector
    public float moveSpeed = 2f;

    // LateUpdate is called once per frame, but after all other update methods
    void LateUpdate()
    {
        // Calculate move direction by using delta positions (Gives a smooth follow)
        Vector3 moveDirection = new Vector3(Character.Instance.transform.position.x - transform.position.x,
                                            Character.Instance.transform.position.y - transform.position.y, 0);

        // Move in move direction
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
    }
}

