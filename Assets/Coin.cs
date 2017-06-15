﻿/* Shapes Game (c) 2017 James Lu. All rights reserved.
 * Coin.cs: Provides a coin object that increments the score when taken.
 */

using UnityEngine;

public class Coin : Collidable {
    // How much is this coin worth?
    public int value = 100;

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // When a player hits the coin, increment the score and destroy the coin.
        if (other.gameObject.GetComponent<Player>() != null)
        {
            // Add to the score and coin count
            GameState.Instance.AddScore(value);
            GameState.Instance.coinCount++;

            // Update the display for the amount of coins already taken.
            Utils.SetText(GameState.Instance.coinCountText, GameState.Instance.coinCount.ToString());

            if (GameState.Instance.coinsNeeded > 0 && GameState.Instance.coinCount >= GameState.Instance.coinsNeeded)
            {
                // End the level when we've reached the amount of coins needed.
                GameState.Instance.LevelEnd();
            }

            // Remove the coin; they're single use.
            Destroy(gameObject);
        }
    }
}
