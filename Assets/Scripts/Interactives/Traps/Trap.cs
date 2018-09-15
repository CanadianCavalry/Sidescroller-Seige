﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Item {

	public int durability;
	protected int maxDurability;
	public bool friendlyFire;

	public float deployY;
	public float deployRotation;

	protected bool isDeployed;

	public Collider2D triggerCollider;

	protected override void Start() {
		usable = true;
		maxDurability = durability;

		base.Start ();
	}

	override public void use() {
		deploy ();
	}

	public virtual void deploy() {

	}

	public virtual void trigger(GameObject other=null) {
		                                                                
	}
}
