﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Singleton method adapted from https://msdn.microsoft.com/en-us/library/ff650316.aspx
public sealed class GameState : MonoBehaviour
{
    private static GameState instance;

    private GameState() { }

    public static GameState Instance
    {
        get
        {
            return instance;
        }
    }

    // TODO: make player settings configurable in via level data
    public int currentPlayer = 1;
    public int playerCount;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();

    // Resource templates, used by Instantiate()
    private PlayerOverlay playerOverlay;
    private GameObject canvasTemplate;
    private GameObject simpleCanvasTemplate;
    private GameObject HUDTextLabelTemplate;
    private GameObject stretchedTextLabelTemplate;
    private GameObject fadeToColourTemplate;

    private GameObject canvas;

    // Draws on the canvas the current player & a list of players as sprites
    void MakeHUD()
    {
        canvas = Instantiate(canvasTemplate);
        GameObject playerListLabel = Instantiate(HUDTextLabelTemplate);
        playerListLabel.GetComponent<Text>().text = "Select character: ";
        playerListLabel.transform.SetParent(canvas.transform);
    }

    public void LevelEnd(bool win=true)
    {
        GameObject levelEndCanvas = Instantiate(simpleCanvasTemplate);
        GameObject levelEndText = Instantiate(stretchedTextLabelTemplate);
        Text text = levelEndText.GetComponent<Text>();
        text.text = "You win!";
        text.fontSize *= 4;  // Make the text bigger

        // Add the "game over" text, but make sure to keep the right world space position.
        // This can be done by setting the worldPositionStays option (second argument) in
        // setParent to false.
        levelEndText.transform.SetParent(levelEndCanvas.transform, false);
    }

    // Adds a player into the current scene.
    public void addPlayer(int id, Player player)
    {
        // Create a new instance of our player overlay prefab - this uses the
        // same sprite as the player but has no movement attached.
        PlayerOverlay newObj = Instantiate(playerOverlay);
        // Set the object name and sprite color
        newObj.name = "Player Overlay for Player " + id;
        newObj.GetComponent<Image>().color = player.getColor();
        // Move the object into the Canvas.
        newObj.transform.SetParent(canvas.transform);
        // Fix the position of the sprite within the character list
        // The index happens to equal the ID, since element 0 is the
        // "Character list" description text.
        newObj.transform.SetSiblingIndex(id);
        // Bind the new object to the player ID.
        newObj.playerID = id;

        // Finally, register the player into the player list. TODO: make sure
        // the key doesn't already exist.
        players[id] = player;
    }

    void Awake()
    {
        // TODO: make this thread safe
        instance = this;

        // Keep the game state code alive, even as we load different levels.
        DontDestroyOnLoad(gameObject);

        // Requirement for UI elements: Create an EventSystem object with a default input module,
        // if one doesn't already exist.
        EventSystem eventSystem;
        StandaloneInputModule sim;
        if (EventSystem.current)
        {
            Debug.Log("GameState: Using existing EventSystem");
            eventSystem = EventSystem.current;
            sim = eventSystem.gameObject.GetComponent<StandaloneInputModule>();
        } else
        {
            Debug.Log("GameState: No EventSystem found; creating a new one");
            GameObject eventSystemObject = new GameObject();
            eventSystemObject.name = "ShapesEventSystem";
            eventSystem = eventSystemObject.AddComponent<EventSystem>();
            sim = eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        // Use a separate input key for overlay buttons to not override keys like Enter and Space
        sim.submitButton = "ClickOnly";

        // Load our relevant resources
        playerOverlay = Resources.Load<PlayerOverlay>("PlayerOverlay");
        canvasTemplate = Resources.Load<GameObject>("HUDCanvas");
        simpleCanvasTemplate = Resources.Load<GameObject>("SimpleHUDCanvas");
        HUDTextLabelTemplate = Resources.Load<GameObject>("HUDTextLabel");
        stretchedTextLabelTemplate = Resources.Load<GameObject>("StretchedTextLabel");

        // Initialize the characters list HUD
        MakeHUD();
    }

    // Update is called once per frame
    void Update()
    {
        for (int btnNum = 1; btnNum <= playerCount; btnNum++)
        {
            string keyName = "Fire" + btnNum;

            try
            {
                if (Input.GetButtonDown(keyName))
                {
                    Debug.Log("Current player set to " + btnNum);
                    currentPlayer = btnNum;
                }
            }
            catch (System.ArgumentException)
            {
            }

        }
    }
}