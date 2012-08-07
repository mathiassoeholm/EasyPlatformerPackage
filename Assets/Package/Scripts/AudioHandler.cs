using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    // The music volume when fully faded in, maximum is 1
    public float musicFullVolume = 0.5f;

    // The speed in which the music fades
    public float fadeFactor = 0.15f;

    // Audio clips, assigned in the inspector
    public AudioClip crateSmash;
    public AudioClip explosion;
    public AudioClip countDown;

    // Current state of the music (FullVolume, FadingIn, FadingOut or Muted)
    private MusicState musicState = MusicState.Muted;

    // List keeping track of all audio sources in the scene, used to play sound effects
    private List<AudioSource> audioSources = new List<AudioSource>();

    // Static instance property, used to get a single instance of the audio handler from other classes
    public static AudioHandler Instance { get; private set; }

    // Use this for initialization
    void Start()
    {
        // Set the instance property equal to 'this' instance
        Instance = gameObject.GetComponent<AudioHandler>();

        // Make the music fade in
        musicState = MusicState.FadingIn;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMusicFade();
    }

    void HandleMusicFade()
    {
        // We use a switch statement to handle music state
        switch (musicState)
        {
            case MusicState.FadingIn:
                // Add to volume, and use Mathf.Min() to make sure we don't go above full volume
                audio.volume = Mathf.Min(audio.volume + fadeFactor * Time.deltaTime, musicFullVolume);
               
                // Check if we reached full volume and switch state to FullVolume
                if (audio.volume >= musicFullVolume)
                    musicState = MusicState.FullVolume;

                break;
            case MusicState.FadingOut:
                // Subtract from volume, and use Mathf.Max() to make sure we don't go below 0
                audio.volume = Mathf.Max(audio.volume - fadeFactor * Time.deltaTime, 0);

                // Check if volume reached 0 and switch state to mutes
                if (audio.volume <= 0)
                    musicState = MusicState.Muted;

                break;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        // We need an audio source to play a sound
        AudioSource audioSource = new AudioSource();
        bool didFindAudioSource = false;
        
        // Loops through all audio sources we've created so far
        foreach (AudioSource source in audioSources)
        {
            // If an existing audio source is not playing any sound, select that one
            if (!source.isPlaying)
            {
                audioSource = source;
                didFindAudioSource = true;
                break;
            }
        }

        // If we didn't find a usable audiosource in the scene, create a new one
        if (!didFindAudioSource)
        {
            // Create audio source
            audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();

            // Add new audio source to our list
            audioSources.Add(audioSource);
        }

        // Assign the clip to the selected audio source
        audioSource.clip = clip;

        // Play the clip with the selected audio source
        audioSource.Play();
    }
}