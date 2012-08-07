using UnityEngine;
using System.Collections.Generic;

public class SwitchBehaviour : MonoBehaviour
{
    // Switch ID, used to asscoiate with correct toggle objects, assigned in the inspecter
    public int switchId;

    // A static list of all toggle objects in the scene
    public static List<ToggleBehaviour> toggleObjects = new List<ToggleBehaviour>();

    // Awake is called once and before Start()
    public void Awake()
    {
        // Get a reference to the TriggerToggle animation state
        AnimationState anim = transform.GetChild(0).animation["TriggerToggle"];
        
        // Make the animation go in reverse to start with
        anim.speed *= -1;
    }

    // Searches for associated objects, and call their Toggle method
    public void SearcForTriggeredObjects()
    {
        for (int i = toggleObjects.Count - 1; i >= 0; i--)
        {
            // Check if id's match, and call Toggle() if true
            if(switchId == toggleObjects[i].switchID)
                toggleObjects[i].Toggle();
        }
        
    }
}