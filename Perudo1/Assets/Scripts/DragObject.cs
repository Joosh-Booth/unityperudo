using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragObject : MonoBehaviour
{

    ClientHolderObject client;

   

    private Touch touch;
    private bool moving;
    
    private Vector3 mousePos;
    private Vector3 startPos;
    private float factor = 1.0f;
    private float distZ;

    private int direction;
    private int moves=0;
    private Vector3 shakeCheckVec;

    private void Start()
    {
        client = GameObject.FindGameObjectWithTag("Client").GetComponent<ClientHolderObject>();
    }
    
    void Update()
    {
        //Only work when one finger is touching
        if (Input.touchCount != 1)
        {
            moving = false;
            return;
        }

        //Set Touch object the first touch input
        touch = Input.GetTouch(0);
        Vector3 v3;
        
        //If it is the initial run
        if (touch.phase == TouchPhase.Began)
        { 
            
            
            RaycastHit hitObject;
            Ray ray = Camera.main.ScreenPointToRay(touch.position);

            //Check if object hit through ray cast from input
            if (Physics.Raycast(ray, out hitObject) && (hitObject.collider.tag == "PlayerCup")) 
            {
                Debug.Log("Hit");
                //Set startPos to the initial position of the object 
                startPos = GameObject.Find("Player").transform.position;

                //Find z in terms of screenCoords
                v3 = Camera.main.WorldToScreenPoint(startPos);
                distZ = v3.z;

                //Set mouse position with z of object, then set it in terms of worldCoords
                mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distZ);
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                //Set bool for one drag movement
                moving = true;
                shakeCheckVec = startPos;
            }

        }

        //Condition met after touch has been detected on a player object
        if (moving && touch.phase == TouchPhase.Moved)
        {
            //Set vector to mouse position relative to the world
            v3 = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distZ);
            v3 = Camera.main.ScreenToWorldPoint(v3);

            //New vector to find the difference from the starting coordinates
            Vector3 v32 = startPos;
            v32.x = v32.x + (v3.x - mousePos.x);

            //Translate z by the difference in y, multiplied by the factor (higher factor increases rate at which object moves back)
            v32.z = v32.z + (v3.y - mousePos.y) * factor;
            transform.position = v32;

            //Check if shaken
            if (!client.shaken && Vector3.Distance(shakeCheckVec, v32) >  0.3)
            {
                if (shakeCheckVec.x < v32.x) 
                {
                    if (direction != 0) 
                    {
                        Debug.Log("changed direction right");
                        

                        direction = 0;
                        moves++;
                        shakeCheckVec = v32;
                    }
                }
                else if(shakeCheckVec.x > v32.x)
                {
                    if (direction != 1)
                    {
                        Debug.Log("changed direction left");
                        
                        direction = 1;
                        moves++;
                        shakeCheckVec = v32;
                    }
                }
                if (!client.shaken &&  moves > 3) 
                {
                    Debug.Log("Sending message");
                    client.client.SendMessage("SHAKE");
                    client.shaken = true;
                    GameObject.Find("GameObjects").GetComponent<GameSceneScript>().SetTurn();
                    moves = 0;

                }
                shakeCheckVec = v32;
            }
        }

        //Condition once the user removes finger from phone
        if (moving && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            moving = false;

        }

    }
}
