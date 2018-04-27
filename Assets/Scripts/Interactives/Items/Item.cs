﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Interactive {

	[HideInInspector]
	public bool isHeld = false;
	[HideInInspector]
	public bool isThrown = false;
	[HideInInspector]
	public bool isBouncing = false;
	[HideInInspector]
	public bool isAttacking = false;
	[HideInInspector]
	public bool flipped = false;

	public float xOffset;
	public float yOffset;
	public float zRotation;
	public float restingHeight;
	public float restingRotation;
	public int thrownDamage;
	public int thrownKnockback;
	public int throwDirection = 0;
	public string type;
	public string description;

	protected int state = 0;

	public Sprite bloodySprite1;
	public Sprite bloodySprite2;
	public Sprite bloodySprite3;

	public BoxCollider2D pickupCollider;
	public Collider2D hitCollider;
	public GameObject frontHand;
	public GameObject backHand;

	protected AudioSource source;
	public AudioClip throwSound;
	public AudioClip throwImpact;
	public AudioClip craftSound;
	public AudioClip pickupSound;
	public AudioClip swapSound;
	public AudioClip breakSound;
	public AudioClip craftOverrideSound;

	private SpriteRenderer sprite;
	protected SoundController soundController;

	[HideInInspector]
	public GameObject player;
	[HideInInspector]
	public PlayerController playerCon;

	// Use this for initialization
	protected void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		playerCon = player.GetComponent<PlayerController> ();
		source = gameObject.GetComponent<AudioSource> ();
		soundController = GameObject.FindGameObjectWithTag ("SoundController").GetComponent<SoundController> ();
		description = description.Replace ("\\n", "\n");
		sprite = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	protected void Update () {
		if (isBouncing) {
			Rigidbody2D itemBody = gameObject.GetComponent<Rigidbody2D> ();
			float xVel = itemBody.velocity.x;
			float yVel = itemBody.velocity.y;
			//Once it has come to rest
			if (Mathf.Approximately (xVel, 0.0f) && Mathf.Approximately (yVel, 0.0f)) {
				isBouncing = false;
				//Turn physics effects off for the item
				itemBody.bodyType = RigidbodyType2D.Kinematic;

				pickupCollider.enabled = true;
				hitCollider.enabled = false;
				gameObject.layer = 13;
			}
		}
	}

	protected void OnCollisionEnter2D(Collision2D other) {
		if (isThrown) {
			if (other.gameObject.tag == "Enemy") {
				if (throwImpact) {
					source.PlayOneShot (throwImpact);
				}
				float direction = transform.position.x - other.transform.position.x;
				other.gameObject.GetComponent<Enemy> ().takeHit (thrownDamage, thrownKnockback, direction);
				if ((state == 0) && (bloodySprite1 != null)) {
					GetComponent<SpriteRenderer> ().sprite = bloodySprite1;
					state++;
				} else if ((state == 1) && (bloodySprite2 != null)) {
					GetComponent<SpriteRenderer> ().sprite = bloodySprite2;
					state++;
				} else if ((state == 2) && (bloodySprite3 != null)) {
					GetComponent<SpriteRenderer> ().sprite = bloodySprite3;
					state++;
				}
			}

			isThrown = false;
			isBouncing = true;
			gameObject.layer = 11;
			sprite.sortingLayerName = "Items";
			sprite.sortingOrder = 5;
		}
	}

	public void disableAnimator() {
		Animator anim = GetComponent<Animator> ();
		if (anim) {
			anim.enabled = false;
		}
	}

	public void enableAnimator() {
		Animator anim = GetComponent<Animator> ();
		if (anim) {
			anim.enabled = true;
		}
	}

	override public void disableHighlight() {
		StopCoroutine ("highlightGlow");
		GetComponent<SpriteOutline> ().enabled = false;
	}

	override public void enableHighlight() {
		GetComponent<SpriteOutline> ().enabled = true;
		StartCoroutine ("highlightGlow");
	}

	override public void updateHighlightColor() {
		GameObject heldItem = playerCon.heldItem;
		if (heldItem && !RecipeBook.canCraft(type, heldItem.GetComponent<Item>().type)) {
			GetComponent<SpriteOutline> ().color = negativeColor;
		} else {
			GetComponent<SpriteOutline> ().color = positiveColor;
		}

	}

	public void flipItem() {
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;
		flipped = !flipped;
	}

	public void pickupItem(bool playerFlipX, bool playSound=true) {
		if (playSound && pickupSound && source) {
			source.PlayOneShot (pickupSound);
		}
			
		if (playerFlipX != flipped)
		{
			flipItem ();
		}
			
		gameObject.layer = 16;
		//Sometimes while manually instantiating prefabs Start() has not yet been called
		if (!playerCon) {
			Start ();
		}
		sprite.sortingLayerName = playerCon.playerSprite.sortingLayerName;
		sprite.sortingOrder = playerCon.playerSprite.sortingOrder + 1;

		//disable pickup trigger, enable hit trigger
		pickupCollider.enabled = false;
		hitCollider.enabled = true;

		enableAnimator ();

		frontHand.SetActive (true);
		backHand.SetActive (true);

		isHeld = true;

		onPickup ();
	}

	public void dropItem() {
		transform.parent = null;
		pickupCollider.enabled = true;
		hitCollider.enabled = false;
		isHeld = false;

		disableAnimator ();

		frontHand.SetActive (false);
		backHand.SetActive (false);

		gameObject.layer = 13;
		moveToResting ();

		onDrop ();
	}

	public void moveToResting() {
		transform.position = new Vector2 (transform.position.x, restingHeight);
		transform.eulerAngles = new Vector3 (0, 0, restingRotation);

		sprite.sortingLayerName = "Items";
		sprite.sortingOrder = 5;
	}

	public void playCraftingSound(AudioClip overrideSound=null) {
		//Since this object was just created, the source variable is not initialized yet
		//so we need to get the AudioSource directly
		AudioSource tempSource = gameObject.GetComponent<AudioSource>();
		if (overrideSound) {
			tempSource.PlayOneShot (overrideSound);
		} else if (craftSound) {
			tempSource.PlayOneShot (craftSound);
		}
	}

	public void playSwappingSound() {
		if (craftSound) {
			source.PlayOneShot (swapSound);
		}
	}

	public void playThrowSound() {
		if (throwSound) {
			source.PlayOneShot (throwSound);
		}
	}

	public virtual void use() {

	}

	public virtual void onTerrainImpact() {

	}

	public virtual void onPickup() {
		return;
	}

	public virtual void onDrop() {
		return;
	}

	public virtual void onThrow() {
		return;
	}

	public void breakItem() {
		bool breakImmed = onBreak ();

		playBreakSound ();

		if (isHeld) {
			playerCon.heldItem = null;
			playerCon.activeSlot.setEmpty();
			gameObject.transform.parent = null;

			disableAnimator ();
			frontHand.SetActive (false);
			backHand.SetActive (false);

			playerCon.alignHands ();
			playerCon.showPlayerHands ();
		}

		//If we don't want to play the break animation, destroy the object now
		if (breakImmed) {
			Destroy (gameObject);
			return;
		}

		Rigidbody2D body = GetComponent<Rigidbody2D>();

		gameObject.layer = 11;

		int xBreakForce = Random.Range(-100, 100);
		int yBreakForce = Random.Range(60, 100);

		isHeld = false;

		//Reset the x and y rotation as these can change during attack animations
		transform.eulerAngles = new Vector3 (0, 0, transform.rotation.z);

		body.bodyType = RigidbodyType2D.Dynamic;
		body.AddForce (new Vector2 (xBreakForce, yBreakForce));
		body.AddTorque (25.0f);

		StartCoroutine ("beginSpriteFlash");
		StartCoroutine ("destroyAfterTime", 1.5f);
	}

	public void playBreakSound() {
		if (breakSound) {
			source.PlayOneShot (breakSound);
		}
	}

	//Return true if item should be destroyed immediately (no animation)
	public virtual bool onBreak() {
		return false;
	}
		
	public virtual void onTravel() {

	}

	public virtual void onArrival() {
		
	}

	/**** Coroutines ****/ 
	IEnumerator destroyAfterTime(float time) {
		yield return new WaitForSeconds(time);

		Destroy (gameObject);
	}

	IEnumerator beginSpriteFlash() {
		while (true) {
			SpriteRenderer sprite = gameObject.GetComponent<SpriteRenderer> ();
			if (sprite.enabled) {
				sprite.enabled = false;
			} else {
				sprite.enabled = true;
			}

			yield return new WaitForSeconds (0.05f);
		}
	}

	IEnumerator highlightGlow() {
		SpriteOutline outline = GetComponent<SpriteOutline> ();
		//Fade out the highlight
		for (float f = 1f; f >= 0.3; f -= 0.01f) {
			Color c = outline.color;
			c.a = f;
			outline.color = c;

			yield return null;
		}
		//Fade the highlight back in
		for (float f = 0.3f; f <= 1; f += 0.01f) {
			Color c = outline.color;
			c.a = f;
			outline.color = c;

			yield return null;
		}
		Color orig = outline.color;
		orig.a = 1;
		outline.color = orig;


		StartCoroutine ("highlightGlow");
	}
}
