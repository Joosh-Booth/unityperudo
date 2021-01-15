using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string newGameScene;
    public string stringToEdit = "";
    public string playerName;
    public int cupColour;
    public int diceColour;
    public Material[] mats;

    public GameObject rules_window;
    public GameObject settings_window;
    public GameObject play_Window;
    public GameObject panel;
    public Text nameText;
    public InputField nameInput;
    public Image imgCheck;

    public Sprite tick;
    public Sprite cross;
    float[,] colours = new float[,] { { 1f, 0, 0 }, { 0,0,0},{1f,1f,0},{0,1f,0 },{0.1f,.57f,.69f },{1f,0,0.62f} };

    // Start is called before the first frame update
    void Start()
    {
        //Listener for the text field input to check if name is valid - Delegate used
        nameInput.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        //Enter name if one is saved in the playerPrefs into the textfield
        playerName = PlayerPrefs.GetString("name");
        if (playerName != null)
        {
            nameInput.text = playerName;
        }


        cupColour = PlayerPrefs.GetInt("cup");
        diceColour = PlayerPrefs.GetInt("dice");

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Play button is clicked - unhides name select window
    public void OnPlay() {
        play_Window.SetActive(true);
        panel.SetActive(true);
    }


    public void OnSetting() 
    {
        panel.SetActive(true);
        settings_window.SetActive(true);
        if (cupColour == 1)
        {

            GameObject.Find("CupColour").GetComponent<Outline>().enabled = false;

        }
        GameObject.Find("CupColour").GetComponent<Text>().color = new Color(colours[cupColour, 0], colours[cupColour, 1], colours[cupColour, 2]);
        GameObject.Find("Cup").GetComponent<Renderer>().material = mats[cupColour];

        if (diceColour == 1)
        {

            GameObject.Find("DiceColour").GetComponent<Outline>().enabled = false;

        }
        GameObject.Find("DiceColour").GetComponent<Text>().color = new Color(colours[diceColour, 0], colours[diceColour, 1], colours[diceColour, 2]);
        GameObject.Find("Dice").GetComponent<Renderer>().material = mats[diceColour];

    }
    public void OnRules() 
    {
        rules_window.SetActive(true);
        panel.SetActive(true);
    }


    public void ClosePanel() 
    {
        if (rules_window.activeSelf)
        {
            PageDecrease();
        }
        rules_window.SetActive(false);
        play_Window.SetActive(false);
        settings_window.SetActive(false);
        panel.SetActive(false);

    }
    public void PageDecrease() 
    {
        GameObject.Find("Rules2").GetComponent<Text>().enabled = false;
        GameObject.Find("Rules1").GetComponent<Text>().enabled = true;
        GameObject.Find("page").GetComponent<Text>().text = "1/2";
        GameObject.Find("increase").GetComponent<Image>().enabled = true;
        GameObject.Find("decrease").GetComponent<Image>().enabled = false;
    }
    public void PageIncrease()
    {
        GameObject.Find("Rules2").GetComponent<Text>().enabled = true;
        GameObject.Find("Rules1").GetComponent<Text>().enabled = false;
        GameObject.Find("page").GetComponent<Text>().text = "2/2";
        GameObject.Find("increase").GetComponent<Image>().enabled = false;
        GameObject.Find("decrease").GetComponent<Image>().enabled = true;
    }

    public void OnColourIncrease(bool c)
    {
        if (c) 
        {
            cupColour++;
            if (cupColour == 1)
            {
                GameObject.Find("CupColour").GetComponent<Outline>().enabled = false;
            }
            else 
            {
                GameObject.Find("CupColour").GetComponent<Outline>().enabled = true;
            }
            
            if (cupColour >= 6) 
            {
                cupColour = 0;
               

            }
            GameObject.Find("CupColour").GetComponent<Text>().color = new Color(colours[cupColour, 0], colours[cupColour, 1], colours[cupColour, 2]);
            GameObject.Find("Cup").GetComponent<Renderer>().material = mats[cupColour];
        }
        else
        {
            diceColour++;
            if (diceColour == 1)
            {
                GameObject.Find("DiceColour").GetComponent<Outline>().enabled = false;
            }
            else 
            {
                GameObject.Find("DiceColour").GetComponent<Outline>().enabled = true;

            }
            
            if (diceColour>= 6)
            {
                diceColour= 0;

            }
            GameObject.Find("DiceColour").GetComponent<Text>().color = new Color(colours[diceColour, 0], colours[diceColour, 1], colours[diceColour, 2]);
            GameObject.Find("Dice").GetComponent<Renderer>().material = mats[diceColour];

        }

    }

    public void OnColourDecrease(bool c)
    {
        if (c)
        {
            
            cupColour --;

            if (cupColour == 1)
            {
                GameObject.Find("CupColour").GetComponent<Outline>().enabled = false;
            }
            else 
            {
                GameObject.Find("CupColour").GetComponent<Outline>().enabled = true;

            }

            if (cupColour < 0)
            {
                cupColour = 5;

            }
            Debug.Log("CupCopl: " + cupColour);
            GameObject.Find("CupColour").GetComponent<Text>().color = new Color(colours[cupColour, 0], colours[cupColour, 1], colours[cupColour, 2]);
            GameObject.Find("Cup").GetComponent<Renderer>().material = mats[cupColour];

        }
        else
        {
            
            diceColour--;
            if (diceColour == 1)
            {
                GameObject.Find("DiceColour").GetComponent<Outline>().enabled = false;
            }
            else 
            {
               GameObject.Find("DiceColour").GetComponent<Outline>().enabled = true;
                
            }
            if (diceColour <0)
            {
                diceColour = 5;

            }
            GameObject.Find("DiceColour").GetComponent<Text>().color = new Color(colours[diceColour, 0], colours[diceColour, 1], colours[diceColour, 2]);
            GameObject.Find("Dice").GetComponent<Renderer>().material = mats[diceColour];

        }

    }

    public void SaveColour() 
    {
        PlayerPrefs.SetInt("cup", cupColour);
        PlayerPrefs.SetInt("dice", diceColour);
        settings_window.SetActive(false);
        panel.SetActive(false);
    }
    //Check if the name is valid, if it is apply tick or cross 
    public void ValueChangeCheck() {

        playerName = nameInput.text;

        if (playerName.Length > 0 && playerName.Length < 11 && !(playerName.Contains(" ")))
        {
            imgCheck.sprite = tick;

        }
        else if (playerName.Length == 0)
        {
            imgCheck.sprite = null;
        }
        else
        {
            imgCheck.sprite = cross;
        }


    }

    //If name is valid change scenes to the next one
    public void ChangeToReady() {
        if (playerName.Length > 0 && playerName.Length < 11 && !(playerName.Contains(" ")))
        {
            PlayerPrefs.SetString("name", playerName);
            SceneManager.LoadScene("ReadyScreen");
        }
        else 
        {
            nameText.fontStyle = FontStyle.Bold;
            nameText.color = Color.red;      
        }
    }

    public void QuitGame() {
        Application.Quit();
    }
}
