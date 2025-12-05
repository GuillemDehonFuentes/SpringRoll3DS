using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	private AudioSource audioSource;
	private bool isPlaying;
	private bool isPaused;

	private void Start() 
	{ 
		audioSource = GetComponent<AudioSource>();
		isPlaying = false;
	}

	public void PlayMusic(AudioClip music)
	{
		if (!isPlaying)
		{
			audioSource.clip = music;
			audioSource.Play();
			isPlaying = true;
		}
		else
		{
			audioSource.Stop();
            audioSource.clip = music;
            audioSource.Play();
			isPlaying = true;
		}
	}

	public void StopMusic()
	{
		if (isPlaying)
		{
			audioSource.Stop();
			isPlaying = false;
		}
		else
			Debug.LogError("There's no music playing right now to try to stop music");
	}

	public void PauseMusic()
	{
		if (!isPaused)
		{
			audioSource.Pause();
			isPaused = true;
		}
		else
		{
            Debug.LogError("The song is already paused");
        }
	}

	public void UnPauseMusic()
	{
		if (isPaused)
		{
			audioSource.UnPause();
			isPaused = false;
		}
		else
		{
            Debug.LogError("The song is already unPaused");
        }
	}
}