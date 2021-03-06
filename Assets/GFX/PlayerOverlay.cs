﻿/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * PlayerOverlay.cs: Implements clickable player sprites for the Shapes Game.
 */

using UnityEngine;
using UnityEngine.UI;

public class PlayerOverlay : ClickableOverlay
{
    [Tooltip("Sets whether a textbox showing the player ID should be inscribed on this overlay.")]
    public bool showPlayerID = true;
    [Tooltip("Sets whether this overlay should match the player's sprite.")]
    public bool copySprite = true;
    public Player player { get; set; }
    private GameObject label;

    protected override void Start()
    {
        base.Start(); // Initialize the base ClickableOverlay class

        // Make our sprite match the player's
        if (copySprite)
            GetComponent<Image>().sprite = player.sprite;

        if (showPlayerID)
        {
            // Label which player this overlay corresponds to
            label = Instantiate(GameState.Instance.textLabelTemplate);
            Text labeltext = label.GetComponent<Text>();
            labeltext.text = player.playerID.ToString();

            // Use the same colour as the player object's overlay (for consistency)
            labeltext.color = Player.textLabelColor;

            // Move the label under the player overlay
            label.transform.SetParent(transform);
        }
    }

    // When clicked, set the current player to this one.
    public override void OnClick()
    {
        if (player.playerID != 0)
        {
            Debug.Log("Clicked player overlay of player ID " + player.playerID);
            GameState.Instance.currentPlayer = player.playerID;
        }
        else
        {
            Debug.LogWarning("PlayerOverlay clicked but not bound to a player ID!");
        }
    }

    void Update() {
        if (player == null)
            // If the player object disappears, remove the overlay.
            Destroy(gameObject);
    }
}
