using UnityEngine;
using System.Collections.Generic;

public class SwitchBehaviour : MonoBehaviour
{
    // Switch ID, used to asscoiate with correct toggle objects, assigned in the inspecter
    public int switchId;

    // A static list of all toggle objects in the scene
    public static List<ToggleBehaviour> toggleObjects = new List<ToggleBehaviour>(); 
    

    public void Awake()
    {
        AnimationState anim = transform.GetChild(0).animation["TriggerToggle"];
        if (anim)
        {
            anim.speed *= -1;
        }
    }

    // Character is calling it 
    public void SearcForTriggeredObjects()
    {
        for (int i = toggleObjects.Count - 1; i >= 0; i--)
        {
            if(switchId == toggleObjects[i].switchID)
                toggleObjects[i].Toggle();
        }
        
    }
}