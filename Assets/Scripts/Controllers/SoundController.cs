﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour {

	[SerializeField]
	private int enemyQueuedSourceMax;
	[SerializeField]
	private int burningSourceMax;
	[SerializeField]
	private int splashSourceMax;
	[SerializeField]
	private int oneShotSouceMax;
	[SerializeField]
	private int environmentalSourceMax;
	//Audio Sources
	private AudioSource[] enemyWalkSources;
	private AudioSource[] enemyProwlSources;
	private AudioSource[] burningSources;
	private AudioSource[] splashSources;
	private AudioSource[] enemyOneShotSources;
	private AudioSource[] priorityOneShotSources;
	private AudioSource[] environmentalSources;
	private AudioSource playerItemSource;
	private AudioSource playerWalkSource;

	//Queues
	private List<AudioClip> walkQueue = new List<AudioClip>();
	private List<AudioClip> prowlQueue = new List<AudioClip>();
	private List<AudioClip> burningQueue = new List<AudioClip>();
	private List<AudioClip> environmentalQueue = new List<AudioClip>();
	private AudioClip playerItemLoopingQueue = new AudioClip();

	//Timers
	[SerializeField]
	private float splashInterval;
	private float splashTimer;

	// Use this for initialization
	void Start () {
		enemyWalkSources = new AudioSource[enemyQueuedSourceMax];
		enemyProwlSources = new AudioSource[enemyQueuedSourceMax];
		burningSources = new AudioSource[burningSourceMax];
		splashSources = new AudioSource[splashSourceMax];
		enemyOneShotSources = new AudioSource[oneShotSouceMax];
		priorityOneShotSources = new AudioSource[oneShotSouceMax];
		environmentalSources = new AudioSource[environmentalSourceMax];
		playerItemSource = gameObject.AddComponent<AudioSource> () as AudioSource;
		playerWalkSource = gameObject.AddComponent<AudioSource> () as AudioSource;

		for (int i = 0; i < enemyQueuedSourceMax; i++) {
			enemyWalkSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
			enemyProwlSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;

			enemyWalkSources [i].loop = true;
			enemyProwlSources [i].loop = true;
		}

		for (int i = 0; i < burningSourceMax; i++) {
			burningSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
		}

		for (int i = 0; i < splashSourceMax; i++) {
			splashSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
		}

		for (int i = 0; i < oneShotSouceMax; i++) {
			enemyOneShotSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
			priorityOneShotSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
		}

		for (int i = 0; i < enemyQueuedSourceMax; i++) {
			environmentalSources [i] = gameObject.AddComponent<AudioSource> () as AudioSource;
			environmentalSources [i].loop = true;
		}
	}

	void Update() {
		//Splash
		if (splashTimer > 0) {
			splashTimer -= Time.deltaTime;
		}

		//Player item looping
		if (playerItemSource.isPlaying == false && playerItemLoopingQueue != null) {
			playPlayerItemSound(playerItemLoopingQueue, true);

			playerItemLoopingQueue = null;
		}
	}

	public void updateVolume(float volume) {
		AudioSource[] sources = GetComponents<AudioSource> ();
		foreach (AudioSource source in sources) {
			source.volume = volume;
		}
	}

	public void pauseAll() {
		AudioSource[] sources = GetComponents<AudioSource> ();
		foreach (AudioSource source in sources) {
			source.Pause ();
		}
	}

	public void playAll() {
		AudioSource[] sources = GetComponents<AudioSource> ();
		foreach (AudioSource source in sources) {
			source.UnPause ();
		}
	}

	public void playEnemyWalk(AudioClip request) {
		bool played = false;
		for (int i = 0; i < enemyWalkSources.Length; i++) {
			if (enemyWalkSources [i].isPlaying) {
				continue;
			}

			enemyWalkSources [i].clip = request;
			enemyWalkSources [i].Play ();
			played = true;
			break;
		}

		//If we didn't have a free source to play the clip, queue it 
		if (!played) {
			queueRequest (request, walkQueue);
		}
	}

	public void stopEnemyWalk(AudioClip clip) {
		bool found = false;
		for (int i = 0; i < enemyWalkSources.Length; i++) {
			//We don't actually care if we ge tthe same source the request is playing on, as long as it is the same clip
			if (enemyWalkSources [i].isPlaying && enemyWalkSources [i].clip == clip) {
				found = true;
				enemyWalkSources [i].Stop ();
				enemyWalkSources [i].clip = null;

				playNextQueued (walkQueue, enemyWalkSources [i]);
				break;
			}
		}

		if (!found) {
			for (int i = 0; i < walkQueue.Count; i++) {
				if (walkQueue [i] != clip) {
					continue;
				}

				walkQueue.RemoveAt (i);
				break;
			}
		}
	}

	public void playEnemyProwl(AudioClip request) {
		bool played = false;
		for (int i = 0; i < enemyProwlSources.Length; i++) {
			if (enemyProwlSources [i].isPlaying) {
				continue;
			}

			enemyProwlSources [i].clip = request;
			enemyProwlSources [i].Play ();
			played = true;
			break;
		}

		//If we didn't have a free source to play the clip, queue it 
		if (!played) {
			queueRequest (request, prowlQueue);
		}
	}

	public void stopEnemyProwl(AudioClip clip) {
		bool found = false;
		for (int i = 0; i < enemyProwlSources.Length; i++) {
			//We don't actually care if we ge tthe same source the request is playing on, as long as it is the same clip
			if (enemyProwlSources [i].isPlaying && enemyProwlSources [i].clip == clip) {
				found = true;
				enemyProwlSources [i].Stop ();
				enemyProwlSources [i].clip = null;

				playNextQueued (prowlQueue, enemyProwlSources [i]);
				break;
			}
		}

		if (!found) {
			for (int i = 0; i < prowlQueue.Count; i++) {
				if (prowlQueue [i] != clip) {
					continue;
				}

				prowlQueue.RemoveAt (i);
				break;
			}
		}
	}

	public void playBurning(AudioClip request) {
		bool played = false;
		for (int i = 0; i < burningSources.Length; i++) {
			if (burningSources [i].isPlaying) {
				continue;
			}

			burningSources [i].clip = request;
			burningSources [i].Play ();
			played = true;
			break;
		}

		//If we didn't have a free source to play the clip, queue it 
		if (!played) {
			queueRequest (request, burningQueue);
		}
	}

	public void stopBurning(AudioClip clip) {
		bool found = false;
		for (int i = 0; i < burningSources.Length; i++) {
			//We don't actually care if we ge tthe same source the request is playing on, as long as it is the same clip
			if (burningSources [i].isPlaying && burningSources [i].clip == clip) {
				found = true;
				burningSources [i].Stop ();
				burningSources [i].clip = null;

				playNextQueued (burningQueue, burningSources [i]);
				break;
			}
		}

		if (!found) {
			for (int i = 0; i < burningQueue.Count; i++) {
				if (burningQueue [i] != clip) {
					continue;
				}

				burningQueue.RemoveAt (i);
				break;
			}
		}
	}

	public void playEnvironmentalSound(AudioClip request, bool loop=true) {
		bool played = false;
		for (int i = 0; i < environmentalSources.Length; i++) {
			if (environmentalSources [i].isPlaying) {
				continue;
			}

			environmentalSources [i].clip = request;
			environmentalSources [i].loop = loop;
			environmentalSources [i].Play ();
			played = true;
			break;
		}

		//If we didn't have a free source to play the clip, queue it 
		if (!played) {
			queueRequest (request, environmentalQueue);
		}
	}

	public void stopEnvironmentalSound(AudioClip clip) {
		bool found = false;
		for (int i = 0; i < environmentalSources.Length; i++) {
			//We don't actually care if we get the same source the request is playing on, as long as it is the same clip
			if (environmentalSources [i].isPlaying && environmentalSources [i].clip == clip) {
				found = true;
				environmentalSources [i].Stop ();
				environmentalSources [i].clip = null;

				playNextQueued (environmentalQueue, environmentalSources [i]);
				break;
			}
		}

		if (!found) {
			for (int i = 0; i < environmentalQueue.Count; i++) {
				if (environmentalQueue [i] != clip) {
					continue;
				}

				environmentalQueue.RemoveAt (i);
				break;
			}
		}
	}

	public void playPlayerItemSound(AudioClip startClip, bool loop=false, AudioClip loopClip=null) {
		playerItemSource.Stop ();
		playerItemSource.clip = startClip;
		playerItemSource.loop = loop;
		playerItemSource.Play ();

		playerItemLoopingQueue = loopClip;
	}

	public void stopPlayerItemSound() {
		playerItemSource.Stop ();
		playerItemLoopingQueue = null;
	}

	public void playPlayerWalkSound(AudioClip walkClip) {
		playerWalkSource.Stop ();
		playerWalkSource.clip = walkClip;
		playerWalkSource.loop = true;
		playerWalkSource.Play ();
	}

	public void stopPlayerWalkSound() {
		playerWalkSource.Stop ();
	}

	public void playSpash(AudioClip clip) {
		if (splashTimer > 0) {
			return;
		}

		for (int i = 0; i < splashSources.Length; i++) {
			//We don't actually care if we ge tthe same source the request is playing on, as long as it is the same clip
			if (splashSources [i].isPlaying) {
				continue;
			}

			splashSources [i].clip = clip;
			splashSources [i].Play ();
			splashTimer = splashInterval;
			break;
		}
	}


	public void playEnemyOneShot(AudioClip requestedClip) {
		for (int i = 0; i < enemyOneShotSources.Length; i++) {
			if (enemyOneShotSources [i].isPlaying) {
				continue;
			}

			enemyOneShotSources [i].PlayOneShot (requestedClip);
			break;
		}
		//If we get to this point without playing the clip, then all sources are full and we ignore the play request
	}

	public void playPriorityOneShot(AudioClip requestedClip) {
		for (int i = 0; i < priorityOneShotSources.Length; i++) {
			if (priorityOneShotSources [i].isPlaying) {
				continue;
			}

			priorityOneShotSources [i].PlayOneShot (requestedClip);
			break;
		}
		//If we get to this point without playing the clip, then all sources are full and we ignore the play request
	}
		
	private void queueRequest(AudioClip request, List<AudioClip> queue) {
		queue.Add (request);
	}

	private void playNextQueued(List<AudioClip> queue, AudioSource source) {
		if (queue.Count == 0) {
			//queue is empty
			return;
		}
	
		source.clip = queue [0];
		source.Play ();
		queue.RemoveAt (0);
	}
}