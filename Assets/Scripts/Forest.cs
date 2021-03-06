﻿using UnityEngine;
using System.Collections;

public class Forest : MonoBehaviour {
	
	public SpriteRenderer[] renderers;
	
	private float time = 0f;
	private int idx = 0;
	private int[] chattyHappy = {0};
	private int[] chattyInd = {1};
	private int[] chattySad = {2,3};
	private int[] clingyHappy = {4};
	private int[] clingyInd = {5};
	private int[] clingySad = {6,7};	
	
	private int[] currentScenario;
	
	private Color hidden() {
		return new Color (1, 1, 1, 0);
	}
	private Color shown() {
		return new Color (1, 1, 1, 1);
	}
	
	private float transitionTime = .6f;
	
	void Start () {
		switch (LevelState.levelState) {
		case LevelState.State.ChattyHappy:
			currentScenario = chattyHappy;
			break;
		case LevelState.State.ChattyIndifferent:
			currentScenario = chattyInd;
			break;
		case LevelState.State.ChattySad:
			currentScenario = chattySad;
			break;
		case LevelState.State.ClingyHappy:
			currentScenario = clingyHappy;
			break;
		case LevelState.State.ClingyIndifferent:
			currentScenario = clingyInd;
			break;
		case LevelState.State.ClingySad:
			currentScenario = clingySad;
			break;
		default:
			currentScenario = chattyHappy;
			break;
		}
	}
	
	void Update() {
		var sceneLength = 5f;
		var currentIdx = idx < currentScenario.Length ? currentScenario[idx] : 0;
		var currentSprite = renderers[currentIdx];
		
		// Set the colors on the previous and current scenes.
		var interpolation = Mathf.Min(time/transitionTime, 1);
		
		currentSprite.color = Color.Lerp (hidden (), shown (), interpolation);
		
		// Switch to the next scene if needed.
		time += Time.deltaTime;
		if ((time > sceneLength || Input.GetButtonDown("Jump")) && idx < currentScenario.Length) {
			idx++;
			time = 0;
			
			// Did we show the last cutscene?
			if (idx >= currentScenario.Length) {
				loadNextLevel();
			} else {
				// set the order of the layer to show up in front of the previous one
				var newIdx = currentScenario[idx];
				renderers[newIdx].sortingOrder = currentSprite.sortingOrder + 1;
			}
		}
	}
	
	public void loadNextLevel() {
		switch (LevelState.levelState) {
		case LevelState.State.ChattyHappy:
			LevelState.levelState = LevelState.State.Chatty;
			Application.LoadLevel((int)LevelState.levelSequence);
			break;
			
		case LevelState.State.ChattyIndifferent:
			LevelState.levelState = LevelState.State.Clingy;
			Application.LoadLevel(0);
			break;			
			
		case LevelState.State.ChattySad:
			LevelState.levelState = LevelState.State.Chatty;
			Application.LoadLevel((int)LevelState.levelSequence);
			break;
			
		case LevelState.State.ClingyHappy:
			LevelState.levelState = LevelState.State.Clingy;
			Application.LoadLevel((int)LevelState.levelSequence);
			break;
			
		case LevelState.State.ClingyIndifferent:
			LevelState.levelState = LevelState.State.Chatty;
			Application.LoadLevel(0);
			break;			
			
		case LevelState.State.ClingySad:
			LevelState.levelState = LevelState.State.Clingy;
			Application.LoadLevel((int)LevelState.levelSequence);
			break;
		}
	}
}
