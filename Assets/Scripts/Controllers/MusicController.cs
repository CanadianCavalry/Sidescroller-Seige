using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {

	public AudioClip menuMusic;
	public AudioClip prepMusic;
	public AudioClip siegeMusic;
	public AudioClip deathMusic;
	public AudioClip labMusic;

	private AudioSource source;

	public float volumeModifier;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
	}

	public void startMusic() {
		source.Play ();
	}

	public void stopMusic () {
		source.Stop ();
	}

	public void updateVolume(float volume) {
		source.volume = volume * volumeModifier;
	}

	public void fadeMusicOut() {
		StartCoroutine ("FadeMusicOut");
	}

	public void changeMusic(string clipName) {
		switch (clipName) {
		case "siege":
			source.clip = siegeMusic;
			break;
		case "prep":
			source.clip = prepMusic;
			break;
		case "lab":
			source.clip = labMusic;
			break;
		case "menu":
			source.clip = menuMusic;
			break;
		case "death":
			source.clip = deathMusic;
			break;
		default:
			break;
		}

		source.Play ();
	}

	IEnumerator FadeMusicOut(float interval=0.01f) {
		float startVol = source.volume;
		for (float f = startVol; f < 1.0f; f -= interval) {
			float newVol = f * startVol;
			source.volume = newVol;

			yield return null;
		}

		stopMusic ();
		source.volume = startVol;
	}
}
