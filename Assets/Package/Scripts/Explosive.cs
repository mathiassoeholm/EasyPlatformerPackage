using UnityEngine;

public class Explosive : ToggleBehaviour
{
    // How much the character is affected by the explosion when standing near it, assigned in the inspector
    public float pushOnCharacter = 15;

    // General explosion force, applied to all nearby rigidbodies, assigned in the inspector
    public float explosionForce = 100;
    
    // The range in which objects incl the character will get affected by the explosion, assigned in the inspector
    public float explosionRadius = 3;

    // Boolean flag to make sure the explosion doesn't start more than once
    private bool didStartExplosion;

    public void Explode()
    {
        // Check if the explosion has already been triggered
        if (didStartExplosion)
            return;

        // Set our boolean flag
        didStartExplosion = true;
        
        // Add explosion force to move direction, if the character is within radius
        if (Vector2.Distance(transform.position, Character.Instance.transform.position) < explosionRadius)
            Character.MoveDirection = (Character.Instance.transform.position - transform.position).normalized * pushOnCharacter / Vector2.Distance(transform.position, Character.Instance.transform.position);

        // Add explosion force to all physic objects
        foreach (GameObject physicObject in GameManager.Instance.PhysicObjects)
        {
            physicObject.rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }

        // Play explosion sound
        AudioHandler.Instance.PlaySound(AudioHandler.Instance.explosion);

        // Instantiate and destroy explosion particles
        Destroy(
            // We instantiate a new explosion
            Instantiate(GameManager.Instance.explosionParticles, transform.position,
                        Quaternion.LookRotation(transform.position - Character.Instance.transform.position)),
            // And remove it after 'lifetime' seconds
            GameManager.Instance.explosionLifeTime);

        // Find all nearby fragile objects
        GameObject[] fragileObjects = GameObject.FindGameObjectsWithTag("Fragile");

        // Loop through all the fragile objects
        foreach (GameObject fragileObject in fragileObjects)
        {
            // Skip fragileObject if it is out of range
            if (Vector2.Distance(transform.position, fragileObject.transform.position) > explosionRadius)
                continue;

            // Play smash crate sound
            AudioHandler.Instance.PlaySound(AudioHandler.Instance.crateSmash);

            // Destroy nearby fragile object
            Destroy(fragileObject);
        }

        // Get all other explosives
        GameObject[] explosives = GameObject.FindGameObjectsWithTag("Explosive");

        // Trigger nearby explosives
        foreach (GameObject explosive in explosives)
        {
            // Skip explosive if it is out of range or if the explosive is this explosive
            if (Vector2.Distance(transform.position, explosive.transform.position) > explosionRadius || explosive == gameObject)
                continue;

            // Check if we are dealing with countdown explosive (TNT)
            if (explosive.GetComponent<CountDownExplosive>() != null)
                // Get CountDownExplosive if that is the case, and call StartCountDown
                explosive.GetComponent<CountDownExplosive>().StartCountDown();
            else
                // Else we just get Explosive and call Explode
                explosive.GetComponent<Explosive>().Explode();
        }

        // Destroy this explosion object
        Destroy(gameObject);
    }


    public override void Toggle()
    {
        // Trigger the explosion
        Explode();
    }
}
