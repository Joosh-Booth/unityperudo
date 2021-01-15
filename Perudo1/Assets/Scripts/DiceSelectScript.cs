using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceSelectScript : MonoBehaviour
{
    public GameObject[] outlines;


    public void onClicked(Button button)
    {
        ClientHolderObject client = GameObject.FindGameObjectWithTag("Client").GetComponent<ClientHolderObject>();
        foreach (GameObject obj in outlines)
        {
            obj.SetActive(false);
        }
        
        if (int.Parse(button.name) >= client.previousGuess[1]) 
        { 
        outlines[int.Parse(button.name) - 1].SetActive(true);

        client.guess[1] = int.Parse(button.name);
        }
    }
}
