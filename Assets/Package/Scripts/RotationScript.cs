using UnityEngine;

public class RotationScript : MonoBehaviour 
{
    // Rotation speed, assigned in the inspector
    public float speed;

    // Rotation axis, assigned in the inspector, deafult is forward
    public RotaionAxis axis = RotaionAxis.Forward;

    // Update is called once per frame
    void Update()
    {
        // Use a switch statement to rotate on the correct axis
        switch (axis)
        {
            case RotaionAxis.Forward:
                transform.RotateAround(Vector3.forward, speed * Time.deltaTime);
                break;
            case RotaionAxis.Up:
                transform.RotateAround(Vector3.up, speed * Time.deltaTime);
                break;
            case RotaionAxis.Right:
                transform.RotateAround(Vector3.right, speed * Time.deltaTime);
                break;
        }
        
    }
}

