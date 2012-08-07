using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	// This method is called when the character enters the checkpoint
    public void SetCheckpointForCharacter()
	{
	    // Don't save checkpoint if character is dead
        if(Character.Instance.IsDead)
            return;
        
        // Save checkpoint position
        Character.LastCheckPointPos = transform.position;

        // Destroy checkpoint gameobject
        Destroy(gameObject);
	}
}
