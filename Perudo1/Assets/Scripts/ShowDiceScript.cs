using cakeslice;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Outline = cakeslice.Outline;

public class ShowDiceScript : MonoBehaviour
{

    public GameObject[] CameraPositions;
    Vector3 PlayerStartPosition;
    Vector3 PlayerStartAngles;
    public bool showDice = false;

    float transitionSpeed = 1f;

    ClientHolderObject client;

    int cameraCount;
    int counter = 0;

    float waitTime = 1.5f;


    public float timeTakenDuringLerp = 1f;
    public float timeTakenDuringLerpAng = 5f;
    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private Vector3 _startAngles;
    private Vector3 _endAngles;

    //The Time.time value when we started the interpolation
    private float _timeStartedLerping;

    // Start is called before the first frame update
    void Start()
    {
        PlayerStartPosition = transform.position;
        PlayerStartAngles = transform.eulerAngles;

        client = GameObject.FindGameObjectWithTag("Client").GetComponent<ClientHolderObject>();
        cameraCount = client.id+1;
        
    }

    public void ShowDice() 
    {
        _timeStartedLerping = Time.time;
        if (cameraCount >= client.players.Count)
        {
            cameraCount = 0;
            while (client.players[cameraCount].DiceLeft <= 0)
            {
                cameraCount++;
            }
        }

        if (cameraCount != client.id)
        {

            _endPosition = GameObject.Find(client.players[cameraCount].Cup + "CameraCup").transform.position;
            _endAngles = GameObject.Find(client.players[cameraCount].Cup + "CameraCup").transform.eulerAngles;
        }
        else
        {
            _endPosition = PlayerStartPosition;
            _endAngles = PlayerStartAngles;
        }

        
        _startPosition = transform.position;
        _startAngles = transform.eulerAngles;
        showDice = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (showDice) 
        {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            float percentageCompleteAng = timeSinceStarted / timeTakenDuringLerpAng;

            transform.position = Vector3.Lerp(_startPosition, _endPosition, percentageComplete);

            transform.eulerAngles = new Vector3(Mathf.LerpAngle(_startAngles.x, _endAngles.x, percentageComplete),
                                         Mathf.LerpAngle(_startAngles.y, _endAngles.y, percentageComplete),
                                         Mathf.LerpAngle(_startAngles.z, _endAngles.z, percentageComplete));
             


            if (percentageComplete >= 1.0f && cameraCount == client.id && counter>0)
            {
                //transform.position = PlayerStartPosition;
                //transform.eulerAngles = PlayerStartAngles;
                ShowCups();
                showDice = false;
            }

            if (percentageComplete >= 1.0f)
            {
                waitTime -= Time.deltaTime;
                if (waitTime <= 0)
                {
                    cameraCount++;
                    waitTime = 1.5f;
                    counter++;
                    ShowDice();
                }
            }
        }
    }

    public void HideCups() 
    {
        foreach (PlayerObject player in client.players)
        {
            if (player.Id != client.id)
            {
                for (int i = 0; i < player.DiceLeft; i++)
                {
                    Debug.Log("cup: " + player.Cup);
                    Debug.Log("i: "+i+"dice i: " + player.Dice[i]);
                    if (player.Dice[i] == client.previousGuess[1])
                    {

                        GameObject.Find(player.Cup + "/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer = false;

                    }
                    else if (GameObject.Find(player.Cup + "/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer == false)

                    {

                        GameObject.Find(player.Cup + "/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer = true;

                    }
                }
            }

            GameObject[] cups = GameObject.Find("GameObjects").GetComponent<GameSceneScript>().cups;
            if (player.DiceLeft > 0 ) 
            {
                if (player.Cup == "Player") 
                {
                    cups[0].transform.Find("Cup").gameObject.SetActive(false);
                }
                else
                {
                    cups[int.Parse(player.Cup.Substring(3))].transform.Find("Cup").gameObject.SetActive(false);
                }
            }
        }
    }

    void ShowCups() 
    {
        foreach (PlayerObject player in client.players)
        {
            GameObject[] cups = GameObject.Find("GameObjects").GetComponent<GameSceneScript>().cups;
            if (player.DiceLeft > 0)
            {
                if (player.Cup == "Player")
                {
                    cups[0].transform.Find("Cup").gameObject.SetActive(true);
                }
                else
                {
                    cups[int.Parse(player.Cup.Substring(3))].transform.Find("Cup").gameObject.SetActive(true);
                }
            }
            for (int i = 0; i < player.DiceLeft; i++)
            {
                Debug.Log("cup: " + player.Cup);
                Debug.Log("i: " + i + "dice i: " + player.Dice[i]);
                if (player.Dice[i] == client.previousGuess[1])
                {

                    GameObject.Find(player.Cup + "/dice/Dice" + (i + 1)).GetComponent<Outline>().eraseRenderer = true;

                }
                
            }
        }
        client.previousGuess[0] = 0;
        client.previousGuess[1] = 0;
        client.players[client.wrongId].DiceLeft--;
        if (client.wrongId == client.id) 
        {
            GameObject.Find("DiceLeft/Dice" + (client.players[client.id].DiceLeft + 1)).GetComponent<Image>().enabled = false;
        }
        GameObject.Find("GameObjects").GetComponent<GameSceneScript>().newRound = true;
    }
}
