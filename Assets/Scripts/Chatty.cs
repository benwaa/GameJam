using UnityEngine;
using System.Collections.Generic;
using System;

public class Chatty : MonoBehaviour {
	
	// The font to use.
	public Font font;
	
	// The object that displays the sprites.
	public GameObject background;
	
	// The backgrounds to display.
	private Sprite[] sprites;
	
	// Questions/answers/moods
	private string[] questions;
	private string[] answers;
	private int[] moods; 
	private int questionIdx;
	
	// [0,1]
	// The current mood.
	private float mood = 1;
	
	// Whether the buttons are being hovered over.
	private bool[] hoverOnButton;

	// The last stable mood.
	private int lastStableSpriteIndex;
	
	// The temporary mood.
	private int temporarySpriteIndex;
	
	// The time for the transition between moods.
	private float transitionPeriod = .75f;
	
	// The time in the current transition, or 0 if there is no transition at the moment.
	private float transitionTime = 0;

	void Start() {
		questions = new string[]{
			"Hello, my prince.",
			"What do you think about the new flowers in the garden?",
			"They're imported from Westeros... the flowers, I mean.",
			"And the bedsheets... I think I prefer the silk ones over the Egyptian cotton ones.",
			"Did you want to attend the Eriksen dinner party next week? I hear they're serving Kobe lobster.",
			"It's been a while since we've gone riding together. Would you like to go later this evening?",
			"Does this robe make me look fat?",
			"The oracle says she sees many offspring in our future.",
			"We don't even talk any more. We don't even know what we argue about.",
			"Are you happy?",
		};
		answers = new string[]{
			"Hello, my darling.",
			"Hey.",
			"(Head Nod)",
			
			"What was that, darling?",
			"They seem nice.",
			"The blue ones are quite striking, but pale in comparison to your beautiful eyes.",
			
			"Nice.",
			"Nice!",
			"Noice!",
			
			"Then we should go with the silk ones. They do seem warmer.",
			"Cool with me.",
			"Honestly, I could not care less.",
			
			"Do I have a choice?",
			"Of course! I know you and the Eriksens are like besties.",
			"You know how I feel about crustaceans. How is that not like eating giant insects?",
			
			"Yes.",
			"No.",
			"Well, we kind of did when we visited your aunt last week...",
			
			"Yes.",
			"Not more than usual.",
			"You make the robe look good.",

			"That's wonderful, darling!",
			"That bitch crazy.",
			"...",

			"Some people work things out and some just don't know how to change.",
			"Let's don't wait 'til the water runs dry.",
			"This stream dried up a long time ago, baby.",

			"Yes.",
			"No.",
			"Define \"happy\".",
		};
		moods = new int[]{
			2,
			1,
			0,
			
			0,
			1,
			2,
			
			0,
			1,
			2,
			
			2,
			1,
			0,
			
			0,
			2,
			1,
			
			2,
			0,
			1,
			
			1,
			0,
			2,
			
			2,
			0,
			1,
			
			2,
			0,
			1,
			
			2,
			0,
			1,
		};
		
		sprites = new Sprite[]{
			Resources.Load<Sprite>("chatty/chatty-0"),
			Resources.Load<Sprite>("chatty/chatty-1"),
			Resources.Load<Sprite>("chatty/chatty-2"),
			Resources.Load<Sprite>("chatty/chatty-3"),
			Resources.Load<Sprite>("chatty/chatty-4"),
		};
		
		hoverOnButton = new bool[]{false, false, false};
		
		// Set the initial color.
		var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		var spriteIndex = getStableSpriteIndex();
		lastStableSpriteIndex = spriteIndex;
		spriteRenderer.sprite = sprites[spriteIndex];
	}
	
	// Returns the sprite index of the stable sprite for the current mood.
	private int getStableSpriteIndex() {
		return 2 * (int)(Mathf.Max(1, Mathf.Ceil(mood*3)) - 1);
	}
	
	private void commonAnswerUpdate(int answerOffset) {
		var moodToDelta = new Dictionary<int, float>{ 
		    {0, -.25f}, 
		    {1, -.1f}, 
		    {2, .25f}, 
		};
		var oldMood = mood;
		var oldSpriteIndex = getStableSpriteIndex();
		// Update the mood.
		mood = Mathf.Min(1, Mathf.Max(0, mood + moodToDelta[moods[questionIdx*3 + answerOffset]]));
		// Determine which transition to use.
		if (mood >= oldMood) {
			temporarySpriteIndex = Math.Min(4, oldSpriteIndex + 1);
		} else {
			temporarySpriteIndex = Math.Max(0, oldSpriteIndex - 1);
		}
		//Debug.Log("mood:" + mood);
		transitionTime = transitionPeriod;
		var moodIndex = getStableSpriteIndex();
		if (++questionIdx > questions.Length - 1) {
			// Move right to the stable mood for the end of the game.
			temporarySpriteIndex = moodIndex;
			if (moodIndex == 4) {
				// Happy
				LevelState.levelState = LevelState.State.ChattyHappy;
				// Indifferent
			} else if (moodIndex == 2) {
				LevelState.levelState = LevelState.State.ChattyIndifferent;
			} else {
				// Sad
				LevelState.levelState = LevelState.State.ChattySad;
			}
			Application.LoadLevel(5);
		}
	}

	void OnGUI() {
	    if (questionIdx > questions.Length - 1) {
	    	return;
	    }
		GUI.skin.button.wordWrap = true;
		
		// Clear button backgrounds.
		GUI.backgroundColor = new Color(1,1,1,0);
		
		// Make the font
		GUI.skin.label.fontSize = Screen.width / 31;
		GUI.skin.button.fontSize = Screen.width / 40;
		GUI.skin.label.font = font;
		GUI.skin.button.font = GUI.skin.label.font;
		
		// Default text color.
		var labelAndHighlightColor = new Color(0.184f, 0.341f, 0.498f, 1);
		GUI.contentColor = labelAndHighlightColor;
		
		// Text alignment.
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		GUI.skin.button.alignment = TextAnchor.MiddleLeft;
		
		// Pad the label and buttons.
		int padding = Screen.width/25;
		GUI.skin.label.padding = new RectOffset(padding, padding, 0, 0);
		GUI.skin.button.padding = new RectOffset(padding, padding, 0, 0);
		
		// Display the question label.
		var buttonAreaSize = new Vector2(Screen.width*.575f, Screen.height*.45f);
		GUI.Label(new Rect(0,
						   Screen.height - buttonAreaSize.y,
		                   Screen.width - buttonAreaSize.x - Screen.width * .05f,
		                   buttonAreaSize.y - padding/2), 
		           questions[questionIdx]);

		// Display/update the buttons.
		var buttonIndex = -1;
		GUILayout.BeginArea(new Rect(Screen.width - buttonAreaSize.x,
		                             Screen.height - buttonAreaSize.y,
		                             buttonAreaSize.x,
		                             buttonAreaSize.y));  
		GUI.skin.button.margin = new RectOffset(0,0,0,0);
		GUILayout.BeginVertical();
		var numButtons = 3;
		for (int i=0; i<numButtons; i++) {
			// Sets the hover color of a button.
			if (hoverOnButton[i]) {
				GUI.contentColor = labelAndHighlightColor;
			} else {
				GUI.contentColor = new Color(0.427f,0.431f,0.439f,1);
			}
			if (GUILayout.Button(answers[questionIdx*numButtons+i], GUILayout.Height(buttonAreaSize.y/3))) {
				buttonIndex = i;
			}
			if (Event.current.type == EventType.Repaint) {
				hoverOnButton[i] = GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
			}
		}

		GUILayout.EndVertical();
		GUILayout.EndArea();

		if (buttonIndex >= 0) {
			commonAnswerUpdate(buttonIndex);
		}
		
	}

	void Update() {
		var moodState = getStableSpriteIndex();
		var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		ResizeSpriteToScreen(background, Camera.main, 1, 1);
		Action<int> render = state => spriteRenderer.sprite = sprites[state];
		if (transitionTime > 0) {
			transitionTime -= Time.deltaTime;
			render(temporarySpriteIndex);
		} else {
			render(moodState);
		}
	}
	
	private void ResizeSpriteToScreen(GameObject theSprite, 
									  Camera theCamera, 
									  int fitToScreenWidth, 
									  int fitToScreenHeight) {        
		SpriteRenderer sr = theSprite.GetComponent<SpriteRenderer>();
		
		theSprite.transform.localScale = new Vector3(1,1,1);
		
		float width = sr.sprite.bounds.size.x;
		float height = sr.sprite.bounds.size.y;
		
		float worldScreenHeight = (float)(theCamera.orthographicSize * 2.0);
		float worldScreenWidth = (float)(worldScreenHeight / Screen.height * Screen.width);
		
		if (fitToScreenWidth != 0)
		{
			Vector2 sizeX = new Vector2(worldScreenWidth / width / fitToScreenWidth,theSprite.transform.localScale.y);
			theSprite.transform.localScale = sizeX;
		}
		
		if (fitToScreenHeight != 0)
		{
			Vector2 sizeY = new Vector2(theSprite.transform.localScale.x, worldScreenHeight / height / fitToScreenHeight);
			theSprite.transform.localScale = sizeY;
		}
	}
	
}
