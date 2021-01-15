using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IncreaseDecreaseScript : MonoBehaviour
{
    ClientHolderObject client;
    // Start is called before the first frame update
    void Start()
    {
         client = GameObject.FindGameObjectWithTag("Client").GetComponent<ClientHolderObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Increase()
    {
        Text amount = GameObject.Find("Amount").GetComponent<Text>();
        int number = int.Parse(amount.text);
        number++;
        amount.text = number.ToString();
        client.guess[0] = number;

    }

    public void Decrease()
    {
        Text amount = GameObject.Find("Amount").GetComponent<Text>();
        int number = int.Parse(amount.text);
        if (number > 1)
        {
            number--;
        }
        amount.text = number.ToString();
        client.guess[0] = number;
    }
}
