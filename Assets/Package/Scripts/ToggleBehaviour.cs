using UnityEngine;
using System.Collections;

public abstract class ToggleBehaviour : MonoBehaviour
{
    public int switchID = -1;

    protected void Start()
    {
        if(switchID >= 0)
        {
            SwitchBehaviour.toggleObjects.Add(this);
            Debug.Log("Added " + gameObject.name + " with trigger id " + switchID);
        } 
    }

    public void OnDestroy()
    {
        SwitchBehaviour.toggleObjects.Remove(this);
    }

    public abstract void Toggle();
}
