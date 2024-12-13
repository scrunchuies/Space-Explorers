using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    private TextMeshProUGUI coinText;

    void Start()
    {
        coinText = GetComponent<TextMeshProUGUI>();
    }

    public void updateCoinText(PlayerInventory playerInventory)
    {

        coinText.text = playerInventory.NumberOfCoins.ToString();

        // Check to see if player has won the game
        if (coinText.text.Equals("3"))
        {

            SceneManager.LoadScene("EndScene");

        }
    }
}