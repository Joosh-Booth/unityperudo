using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubtButton : MonoBehaviour
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onClick() 
    {
        ClientHolderObject client = GameObject.FindWithTag("Client").GetComponent<ClientHolderObject>();
        client.client.SendMessage("DOUBT");
    }

}
