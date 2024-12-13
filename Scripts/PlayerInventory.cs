using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayerInventory : MonoBehaviour
{
    public int NumberOfCoins { get; private set; }

    public UnityEvent<PlayerInventory> OnCoinCollected;

    public void CoinCollected()
    {
        // Keep track of coins and add additional
        NumberOfCoins++;
        OnCoinCollected.Invoke(this);

    }
}