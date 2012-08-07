using UnityEngine;

public class CameraFade : MonoBehaviour
{
    //The position where we place the overlay screen
    //Vector3 targetOverlayPosition = new Vector3(0,0,-9);
    
    //The overlay
    Texture2D overlay;

    //The speed of our scaling
    public float fadeFactor;

    //The state of the overlay
    private enum OverlayState
    {
        FadingIn,
        FadingOut,
        FullyFaded,
        FullyShown
    };
    private OverlayState state = OverlayState.FullyFaded;

    //The current alpha level
    public float Alpha
    {
        get
        {
            return overlay.GetPixel(1, 1).a;
        }
        set
        {
            if (value >= 1)
            {
                overlay.SetPixels(new Color[1] { new Color(0, 0, 0, 1) });
                state = OverlayState.FullyShown;
            }
            else if (value <= 0)
            {
                overlay.SetPixels(new Color[1] { new Color(0, 0, 0, 0) });
                state = OverlayState.FullyFaded;
            }
            else
            {
                overlay.SetPixels(new Color[1] { new Color(0, 0, 0, value) });
            }

            overlay.Apply();
        }
    }

    void Awake()
    {
        overlay = new Texture2D(1, 1);
    }

    void Update()
    {
        switch (state)
        {
            case OverlayState.FadingIn:
                Alpha=overlay.GetPixel(0,0).a + (Time.deltaTime * (1 / fadeFactor));
                break;
            case OverlayState.FadingOut:
                    Alpha= overlay.GetPixel(0, 0).a - (Time.deltaTime * (1 / fadeFactor ));
                break;
            case OverlayState.FullyFaded:
                Alpha =0;
                break;
            case OverlayState.FullyShown:
                Alpha = 1;

                break;
        }
    }

    void OnGUI()
    {
        switch (state)
        {
            case OverlayState.FadingIn:
            case OverlayState.FadingOut:
            case OverlayState.FullyShown:
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), overlay);
                break;
        }
    }

    //Triggers the fading in
    public void FadeIn()
    {
        state = overlay.GetPixel(1, 1).a < 1 ? OverlayState.FadingIn : OverlayState.FullyShown;
    }

    //Triggers the fading in
    public void FadeOut()
    {
        state = overlay.GetPixel(1, 1).a > 0 ? OverlayState.FadingOut : OverlayState.FullyFaded;
    }
}