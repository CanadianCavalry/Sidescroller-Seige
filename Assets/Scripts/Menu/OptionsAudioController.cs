﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsAudioController : MonoBehaviour {
	public GameObject optionsMenu;
	public GameObject mainMenu;

	public Slider masterSlider;
	public Slider musicSlider;
	public Slider effectsSlider;

	void Start() {
		if (!PlayerPrefs.HasKey ("masterVolume")) {
			PlayerPrefs.SetFloat ("masterVolume", 1.0f);
		}

		if (!PlayerPrefs.HasKey ("musicVolume")) {
			PlayerPrefs.SetFloat ("masterVolume", 1.0f);
		}

		if (!PlayerPrefs.HasKey ("effectsVolume")) {
			PlayerPrefs.SetFloat ("masterVolume", 1.0f);
		}

		masterSlider.value = PlayerPrefs.GetFloat ("masterVolume");
		musicSlider.value = PlayerPrefs.GetFloat ("musicVolume");
		effectsSlider.value = PlayerPrefs.GetFloat ("effectsVolume");
	}

	public void setMasterVolume(float volume) {
		PlayerPrefs.SetFloat ("masterVolume", volume);
	}

	public void setMusicVolume(float volume) {
		PlayerPrefs.SetFloat ("musicVolume", volume);
	}

	public void setEffectsVolume(float volume) {
		PlayerPrefs.SetFloat ("effectsVolume", volume);
	}

	public void saveAudioSettings() {
		setMasterVolume (masterSlider.value);
		setMusicVolume (musicSlider.value);
		setEffectsVolume (effectsSlider.value);
	}

	public void showOptionsMenu() {
		optionsMenu.SetActive (true);
		mainMenu.SetActive (false);
	}

	public void showMainMenu() {
		optionsMenu.SetActive (false);
		mainMenu.SetActive (true);
	}
}