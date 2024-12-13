using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    private AudioSource audioSource;
    private Renderer coinRenderer;
    private Collider coinCollider;

    private void Start()
    {
        // Get the AudioSource component and Renderer attached to the same GameObject
        audioSource = GetComponent<AudioSource>();
        coinRenderer = GetComponent<Renderer>();
        coinCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory playerInventory = other.GetComponent<PlayerInventory>();
        if (playerInventory != null)
        {
            // Play the coinCollect sound
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Notify the player's inventory
            playerInventory.CoinCollected();

            // Make the coin invisible and disable its collider
            MakeInvisible();

            // Destroy the GameObject after the sound finishes playing
            StartCoroutine(DestroyAfterSound());
        }
    }

    private void MakeInvisible()
    {
        if (coinRenderer != null)
        {
            coinRenderer.enabled = false; // Hide the visual appearance of the coin
        }
        if (coinCollider != null)
        {
            coinCollider.enabled = false; // Disable the collider to prevent further interactions
        }
    }

    private IEnumerator DestroyAfterSound()
    {
        if (audioSource != null)
        {
            // Wait for the sound to finish
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        // Destroy the gameObject
        Destroy(gameObject);
    }
}
