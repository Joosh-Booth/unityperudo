using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeekButton : MonoBehaviour
{

    

    //The Time.time value when we started the interpolation
    private float _timeStartedLerping;

    private float timeTakenDuringLerp = .5f;
    private float timeTakenDuringLerpAng = 5f;


    ClientHolderObject client;

    public Camera mainCamera;
    public GameObject playerCup;
    public GameObject cupPeekPostion;
    public GameObject dice;

    Vector3 diceEnd;
    Vector3 diceCurrent;

    Vector3 cameraStartPosition;
    Vector3 cameraStartRotation;
    Vector3 cameraNewStartPosition;
    Vector3 cameraNewStartRotation;

    Vector3 cupStartPosition;
    Vector3 cupStartAngles;
    Vector3 cupNewStartPosition;
    Vector3 cupNewStartRotation;

    Vector3 cupEndPosition= new Vector3(0.2507444f,0.412f,-7.272f);
    Vector3 angles;

    bool peeking = false;
    bool peeked = false;
    bool reset = true;
    ShowDiceScript showDiceCheck;
    private void Start()
    {
        cameraStartPosition = mainCamera.transform.position;
        cameraStartRotation = mainCamera.transform.eulerAngles;

        cupStartPosition = playerCup.transform.position;
        cupStartAngles = playerCup.transform.eulerAngles;

        diceEnd = dice.transform.position;

        client = GameObject.FindGameObjectWithTag("Client").GetComponent<ClientHolderObject>();    
        showDiceCheck = GameObject.Find("Main Camera").GetComponent<ShowDiceScript>();
    }

    private void LateUpdate()
    {
        float percentageComplete = 0;

            if (peeking && !peeked)
            {
                float timeSinceStarted = Time.time - _timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;

                mainCamera.transform.position = Vector3.Lerp(cameraNewStartPosition, cupPeekPostion.transform.position, percentageComplete);

                angles = new Vector3(Mathf.LerpAngle(cameraNewStartRotation.x, 32.187f, percentageComplete), 0, 0);
                mainCamera.transform.eulerAngles = angles;



                playerCup.transform.position = Vector3.Lerp(cupNewStartPosition, cupEndPosition, percentageComplete);
                dice.transform.position = Vector3.Lerp(diceCurrent, diceEnd, percentageComplete);

                angles = new Vector3(Mathf.LerpAngle(cupNewStartRotation.x, cupStartAngles.x + 40.49f, percentageComplete), 0, 0);
                playerCup.transform.eulerAngles = angles;

                if (percentageComplete >= 1.0f)
                {
                    peeked = true;
                    percentageComplete = 0;
                }


            }
            else if (!showDiceCheck.showDice && (!peeking && (mainCamera.transform.position != cameraStartPosition)))
            {
                float timeSinceStarted = Time.time - _timeStartedLerping;
                percentageComplete = timeSinceStarted / timeTakenDuringLerp;

                mainCamera.transform.position = Vector3.Lerp(cameraNewStartPosition, cameraStartPosition, percentageComplete);
                angles = new Vector3(
                Mathf.LerpAngle(cameraNewStartRotation.x, cameraStartRotation.x, percentageComplete), 0, 0);
                mainCamera.transform.eulerAngles = angles;

                playerCup.transform.position = Vector3.Lerp(cupNewStartPosition, cupStartPosition, percentageComplete);
                angles = new Vector3(Mathf.LerpAngle(cupNewStartRotation.x, cupStartAngles.x, percentageComplete), 0, 0);
                playerCup.transform.eulerAngles = angles;

                peeked = false;
            }
        

    }


    public void press() 
    {
        cupNewStartPosition = playerCup.transform.position;
        cupNewStartRotation = playerCup.transform.eulerAngles;

        cameraNewStartPosition = mainCamera.transform.position;
        cameraNewStartRotation = mainCamera.transform.eulerAngles;

        diceCurrent = dice.transform.position;

        _timeStartedLerping = Time.time;

        Debug.Log("Pressed");
        peeking = true;
        client.client.SendMessage("SHAKE");
        client.shaken = true;
        GameObject.Find("GameObjects").GetComponent<GameSceneScript>().SetTurn();

        if (client.previousGuess[1] > 0)
        {
            for (int i = 0; i < client.players[client.id].DiceLeft; i++)
            {
                if (client.players[client.id].Dice[i] == client.previousGuess[1])
                {

                    GameObject.Find("Player/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer = false;

                }
                else if (GameObject.Find("Player/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer == false)

                {

                    GameObject.Find("Player/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer = true;

                }
            }
        }
        


    }
    public void release() 
    {
        cupNewStartPosition = playerCup.transform.position;
        cupNewStartRotation = playerCup.transform.eulerAngles;

        cameraNewStartPosition = mainCamera.transform.position;
        cameraNewStartRotation = mainCamera.transform.eulerAngles;
        _timeStartedLerping = Time.time;
        peeking = false;


        if (client.previousGuess[1] > 0)
        {
            for (int i = 0; i < client.players[client.id].DiceLeft; i++)
            {
                if (GameObject.Find("Player/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer == false)

                {

                    GameObject.Find("Player/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer = true;

                }
            }
        }

    }
}
