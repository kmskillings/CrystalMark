using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour {

    public static SoundManager instance;

    public AudioSource musicSource;
    public AudioSource soundSource;

    public AudioClip openingMusicClip;
    public AudioClip[] gameplayMusicClips;
    public float musicVolume;

    private void Awake()
    {

        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

    }    
    
    // Use this for initialization
	void Start () {

        musicSource.volume = musicVolume;
        musicSource.clip = openingMusicClip;
        musicSource.Play();
		
	}
	
	// Update is called once per frame
	void Update () {

        if (!musicSource.isPlaying)
        {
            playNewMusic();
        }
		
	}

    public void playClip(AudioClip clip, float volume)
    {
        soundSource.PlayOneShot(clip, volume);
    }

    public void playNewMusic() //Sets up a new music clip to be played
    {
        // Selects random index of music to play
        AudioClip newClip;
        do
        {
            int musicIndex = Random.Range(0, SceneManager.GetActiveScene().name == "LevelMenu" ? gameplayMusicClips.Length+1 : gameplayMusicClips.Length); //4 possible musics if in main menu, 3 if it is
            if (musicIndex == gameplayMusicClips.Length) //Only happens if it's in LevelMenu
                newClip = openingMusicClip;
            else
                newClip = gameplayMusicClips[musicIndex];
        } while (newClip == soundSource.clip); //Don't play the same song twice in a row

        musicSource.clip = newClip;
        musicSource.Play();

    }
}
