using UnityEngine;

public class YouWinScene : MonoBehaviour
{
	// Update is called once per frame
	void Update ()
    {
	    // Check if we press enter
        if(Input.GetKeyDown("return"))
	    {
	        // Load the first assigned scene in the build settings
            Application.LoadLevel(0);
	    }
	}
}
