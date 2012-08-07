using System.Collections;
using UnityEngine;

public class CountDownExplosive : Explosive
{
    // Materials assigned in the inspector
    public Material tnt1, tnt2, tnt3;

    // Boolean flag to make sure the explosion doesn't start more than once
    private bool didStartCountDown;

    public void StartCountDown()
    {
        // Check if the explosion has already been triggered
        if (didStartCountDown)
            return;

        // Start the count down
        StartCoroutine(CountDownExplode());

        // Set our boolean flag
        didStartCountDown = true;
    }

    IEnumerator CountDownExplode()
    {
        // Apply new material
        renderer.material = tnt3;
        // Play countdown sound
        AudioHandler.Instance.PlaySound(AudioHandler.Instance.countDown);
        // Wait a second...
        yield return new WaitForSeconds(1);

        // Apply new material
        renderer.material = tnt2;
        // Wait a second...
        yield return new WaitForSeconds(1);

        // Apply new material
        renderer.material = tnt1;
        // Wait a second...
        yield return new WaitForSeconds(1);

        // Call on explode from base class
        Explode();
    }

    // Override toggle method from base class, to start explosion countdown
    public override void Toggle()
    {
        StartCountDown();
    }
}
