using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Character : MonoBehaviour
{

    #region Variables

    // Control variables, assigned in the inspector
    public string jumpKey = "up";
    public string actionKey = "down";
    public string leftKey = "left";
    public string rightKey = "right";
    public string jumpKeyAlternative = "space";

    // Public settings, assigned in the inspector
    public float walkSpeed = 6;
    public float walkAccelaration = 15;
    public float jumpSpeed = 8;
    public float gravity = 20;
    public float explosionForce = 15;
    public float pushForce = 10;

    // Misc private
    private bool headCollided;
    private float lockedZPos;
    private CharacterController controller;

    #endregion

    // Public property, are we dead or not?
    public bool IsDead { get; private set; }
    
    // Static instance property, used to get a single instance of the character from other classes
    public static Character Instance { get; private set; }

    // Static move direction, this is accesible from other classes so they can affect the characters direction
    public static Vector3 MoveDirection { get; set; }

    // Static property, used to keep track of latest checkpoint position
    public static Vector3 LastCheckPointPos { get; set; }

    // Use this for initialization
    void Start()
    {
        // Reset MoveDirection (Required because static vars maintain state between sessions)
        MoveDirection = new Vector3();
        
        // Make the camera fade in
        Camera.mainCamera.GetComponent<CameraFade>().FadeOut();

        // If the last checkpoint was at 0,0,0, it hasn't been set yet
        if (LastCheckPointPos == Vector3.zero)
            // Deafult the checkpoint pos to our start position
            LastCheckPointPos = transform.position;
        else
            // If a checkpoint has been saved, set our position to that point
            transform.position = LastCheckPointPos;
        
        // Save a reference to the character controller component
        controller = GetComponent<CharacterController>();

        // Set the instance property equal to 'this' instance
        Instance = this;

        // Save the z-pos in which we will be locked
        lockedZPos = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        // Lock z-pos
        transform.position = new Vector3(transform.position.x, transform.position.y, lockedZPos);

        // Check if the left key is down, and that we haven't exceeded the maximum walkspeed
        if (Input.GetKey(leftKey) && MoveDirection.x > -walkSpeed)
        {
            // Pull the x coordinate of our move direction to negative (left on x-axis)
            MoveDirection = new Vector3(MoveDirection.x - walkAccelaration*Time.deltaTime, MoveDirection.y, 0);

            // Rotate the robot child appropriately
            transform.GetChild(0).transform.rotation = Quaternion.Euler(270, 270, 0);
        }
        // Else, check if the right key is down, and that we haven't exceeded the maximum walkspeed
        else if (Input.GetKey(rightKey) && MoveDirection.x < walkSpeed)
        {
            // Pull the x coordinate of our move direction to positive (right on x-axis)
            MoveDirection = new Vector3(MoveDirection.x + walkAccelaration*Time.deltaTime, MoveDirection.y, 0);

            // Rotate the robot child appropriately
            transform.GetChild(0).transform.rotation = Quaternion.Euler(270, 90, 0);
        }
        // If either keys were down, or we exceeded the walkspeed begin deceleration, first check if we moved left or right
        else if (MoveDirection.x > 0)
        {
            // If we moved right, subtract from x and make sure we don't go below 0 with the Mathf.Max() method
            MoveDirection = new Vector3(Mathf.Max(0, MoveDirection.x - walkAccelaration * Time.deltaTime), MoveDirection.y, 0);
        }
        else
        {
            // If we moved left, add to x and make sure we don't go above 0 with the Mathf.Min() method
            MoveDirection = new Vector3(Mathf.Min(0, MoveDirection.x + walkAccelaration*Time.deltaTime), MoveDirection.y, 0);
        }

        // Check if we are touching the ground
        if (controller.isGrounded)
        {
            // Reset boolean flag, that determined wether something was hitting our characters head
            headCollided = false;

            // Check if the jumpkey is pressed, and apply jump to the move directions y-axis
            if (Input.GetKey(jumpKey) || Input.GetKey(jumpKeyAlternative))
                MoveDirection = new Vector3(MoveDirection.x, jumpSpeed);
        }

        // Check if our head collides with anything, and we are moving up
        if (DoesHeadCollideWhileWeHavePositiveForce())
        {
            // Set the force in y to zero (this fixes a buug where we got stuck under objects)
            MoveDirection = new Vector3(MoveDirection.x, 0, 0);
        }

        // Apply gravity to move direction, and make sure we don't go below max gravity with Mathf.Max
        MoveDirection = new Vector3(MoveDirection.x, Mathf.Max(-gravity, MoveDirection.y - gravity * Time.deltaTime));

        // Move the character with the character controller move method
        controller.Move(MoveDirection * Time.deltaTime);

        // Respawn if we jump of the platform
        if (transform.position.y < -30)
            Respawn();

        if (IsDead && GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFade>().Alpha == 1)
        {
            Application.LoadLevel(Application.loadedLevel);            
        }
    }

    // This method is called once, whenever we enter a trigger
    void OnTriggerEnter(Collider other)
    {
        // Switch statements that checks the other objects tag
        switch (other.tag)
        {
            case "Goal":
                // Break out of switch if we are dead
                if (IsDead)
                    break;
                // Local variable that save the next level number, if written as a number
                int levelNumber;

                // Reset checkpoints
                LastCheckPointPos = Vector3.zero;

                // Check if we can parse the name to a number
                if (int.TryParse(other.name, out levelNumber))
                {
                    // Load level by index number
                    Application.LoadLevel(levelNumber);
                }
                else
                {
                    // Load level by name
                    Application.LoadLevel(other.name);
                }
                break;
            case "Spikes":
                // Respawn if we hit some spikes
                Respawn();
                break;
        }

        // Check if we hit a checkpoint
        if (other.GetComponent<Checkpoint>() != null)
            other.GetComponent<Checkpoint>().SetCheckpointForCharacter();
    }

    // This method is called continuously while standing 'in' a trigger
    void OnTriggerStay(Collider other)
    {
        // Check are standing in a switch and the action key is pressed
        if (other.GetComponent<SwitchBehaviour>() != null && Input.GetKeyDown(actionKey))
        {
            // Make the switch search for associated objects and toggle them
            other.GetComponent<SwitchBehaviour>().SearcForTriggeredObjects();

            // Get a reference to the switches animation state
            AnimationState anim = other.transform.GetChild(0).animation["TriggerToggle"];

            // Reverse animation speed, to make it go back and forth
            anim.speed *= -1;

            // Set the animation start point to start or end, using the conditional operator to test if speed is + or -
            anim.time = anim.speed < 0 ? anim.length : 0;

            // Start the switch animation
            other.transform.GetChild(0).animation.Play();
        }
    }

    // This method is called once, when we collide with another object
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Check if we hit an object with a rigidbody
        if (hit.rigidbody != null)
        {
            // Calculate force vector to apply as a push to the other object
            Vector3 force = (hit.collider.transform.position - transform.position).normalized * pushForce;

            // Apply the force to the collided objects rigidbody
            hit.rigidbody.AddForce(new Vector3(force.x, force.y));
        }

        // Check if we hit a countdown explosive (TNT)
        if (hit.gameObject.GetComponent<CountDownExplosive>() != null)
            // Start tnt count down
            hit.gameObject.GetComponent<CountDownExplosive>().StartCountDown();
        // Else,  Check if we hit a normal explosive (Nitro)
        else if (hit.gameObject.GetComponent<Explosive>() != null)
            // Explode nitro
            hit.gameObject.GetComponent<Explosive>().Explode();

        // Check if we hit a jumpable object
        if (hit.gameObject.GetComponent<Jumpable>() != null)
        {
            // Check if we are directly above the jumpable object, by checking the collision normal
            if (hit.normal.y > 0.5f)
                // Apply a positive jump to the character
                hit.gameObject.GetComponent<Jumpable>().ApplyJumpToCharacter(false);
            // Else, check if we are directly beneath the jumpable object, by checking the collision normal
            else if (hit.normal.y < -0.5f)
                // Apply a negative jump to the character
                hit.gameObject.GetComponent<Jumpable>().ApplyJumpToCharacter(true);
        }

        // Check if we hit an enemy
        if (hit.collider.tag == "Enemy")
        {
            // Kill the enemy if we landed directly on top, checking the collision normal
            if (hit.normal.y > 0.5f)
                hit.gameObject.GetComponent<EnemyBehaviour>().Die();
            // Else, we die
            else
                Respawn();
        }
    }

    bool DoesHeadCollideWhileWeHavePositiveForce()
    {
        // Local variable to save raycast hit information above our head
        RaycastHit hitInfo;

        // Check if a we are applied postive force in y (are we moving up?), and check if anything is above our head
        if (!headCollided
            && MoveDirection.y > 0
            && (Physics.Raycast(transform.position + new Vector3(0.5f, 0), Vector3.up*0.3f, out hitInfo, 1.66f)
            || Physics.Raycast(transform.position + new Vector3(-0.5f, 0), Vector3.up*0.3f, out hitInfo, 1.66f)))
        {
            // Set boolean flag so we don't run this code again till we have been grounded
            headCollided = !headCollided;

            // Check if the block we hit is NOT a jumpable block
            if (hitInfo.collider.GetComponent<Jumpable>() == null)
            {
                // Our head does collide, return true
                return true; 
            }
        }

        // Our head does NOT collide, return false
        return false;
    }

    public void Respawn()
    {
        // Make the camera fade out and reload level
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFade>().FadeIn();

        // Set IsDead bool flag
        IsDead = true;
    }
    
}
