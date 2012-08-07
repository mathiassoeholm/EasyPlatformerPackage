using UnityEngine;
using System.Collections;

public abstract class ToggleBehaviour : MonoBehaviour
{
    // Switch ID, used to asscoiate with correct switch, assigned in the inspecter
    public int switchID = -1;

    // The Start method heres is protected so it can be called from in derived class
    protected void Start()
    {
        // Check if the swicth ID is bigger than or equal to 0, to check if user changed it. Default is -1, which has no effect
        if(switchID >= 0)
        {
            // Add this toggle object to the static list
            SwitchBehaviour.toggleObjects.Add(this);

            // Write a clarification message to the console
            Debug.Log("Added " + gameObject.name + " with trigger id " + switchID);
        } 
    }

    public void OnDestroy()
    {
        // Remove this object from the list of toggle object in case it gets destroyed (Explosions get destroyed, and are toggle objects)
        SwitchBehaviour.toggleObjects.Remove(this);
    }

    // Supply an abstract Toggle() method, that every derived class need to implement
    public abstract void Toggle();
}
