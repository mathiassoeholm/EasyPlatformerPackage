using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Inspector fields
    public GameObject explosionParticles;
    public float explosionLifeTime = 4;

    // Static instance property, used to get a single instance of the character from other classes
    public static GameManager Instance { get; private set; }

    // An array of all gameobjects affected by physics
    public GameObject[] PhysicObjects { get; private set; }

    // Awake is called once and before Start()
    void Awake ()
	{
        // Set the instance property equal to 'this' instance
        Instance = GetComponent<GameManager>();
        
        // Find all physic objects in the scene
        PhysicObjects = GameObject.FindGameObjectsWithTag("PhysicObject");

        // Reset toggle objects list
        SwitchBehaviour.toggleObjects.Clear();
	}
}
