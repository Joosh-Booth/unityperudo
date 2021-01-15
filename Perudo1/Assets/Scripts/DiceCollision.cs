using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCollision : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision with "+ collision.gameObject.tag);
        if (collision.gameObject.tag == "Finish") 
        {
            Debug.Log("Tag is Dice");
            gameObject.transform.position=new Vector3((gameObject.transform.position.x)+1, gameObject.transform.position.y, gameObject.transform.position.z);
        }
        if (collision.gameObject.tag == "PlayerCup")
        {
            Debug.Log("Tag is Cup");

        }
    }
}
