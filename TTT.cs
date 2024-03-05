using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TTT : MonoBehaviour
{
    //Holds who the winner is
    [SerializeField] string winner;

    //Holds whether the game is over or not
    [SerializeField] bool gameOver;

    //Ensures that the game end only happens once 
    [SerializeField] bool disabled;

    //Holds the index of the winning lines
    [SerializeField] int line;


    //Holds the image of the TTT board (that also holds everything else in the canvas)
    [SerializeField] GameObject image;

    //Holds the text that announces who is the winner
    [SerializeField] TextMeshProUGUI announcementText;

    //Holds the text for the scoreboard
    [SerializeField] TextMeshProUGUI scoreText;

    //Holds the manual TTT canvas
    [SerializeField] Canvas manTTT;

    //Holds the AI TTT canvas
    [SerializeField] Canvas aiTTT;

    
    //Holds all of the buttons (for the sections of the TTT board)
    [SerializeField] Button[] buttons = new Button[9];

    //Holds all of the spaces where XO can be placed
    [SerializeField] TextMeshProUGUI[] texts = new TextMeshProUGUI[9];

    //Holds the line that shows up to mark where the winning 3-in-a-row was made
    [SerializeField] Image[] winningLines = new Image[8];


    //Holds whether X or O is the next player
    bool x;

    //Holds the original position of the TTT board (image)
    Vector3 imageOriginalPosition;

    //Holds the original position of the announcementText
    Vector3 annTextOriginalPosition;

    //Holds the scores
    int[] scores = new int[3];

    // Start is called before the first frame update
    void Start()
    {
        //Get image's original position
        imageOriginalPosition = image.transform.position;

        //Get the announcementText's original position
        annTextOriginalPosition = announcementText.transform.position;

        //Make the button call click() when pressed
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => click(index));
        }

        //Initialize the variables
        restart();
    }

    // Update is called once per frame
    void Update()
    {
        //Find out if the game is over or not
        gameOver = isGameOver();

        //If the game is over ...
        if (gameOver && !disabled)
        {
            //Make sure endGame() is only called once
            disabled = true;

            //End the game
            endGame();
        }
    }

    public void click(int i)
    {
        //If the game is over, nothing happens
        if (gameOver) return;

        //Get the specified TMPro 
        TextMeshProUGUI t = texts[i];

        //If the text isn't empty, nothing happens
        if (t.text != "") return;

        //If the player is X ...
        if (x)
        {
            //Turn the X to red
            t.color = Color.red;

            //Place the X in the proper space
            t.text = "X";
        }

        //If the player is O ...
        else
        {
            //Turn the O to white
            t.color = Color.white;

            //Place the O in the proper space
            t.text = "O";
        }

        //Make the player into the other player
        x = !x;
    }

    private void updateScores()
    {
        scoreText.text = string.Format("Score: {0:0}:{1:0}:{2:0} (X:O:Draw)", scores[0], scores[1], scores[2]);
    }

    //Find the winner of the game. 
    private string findWinner()
    {
        for (int i = 0; i < 3; i++)
        {
            //Look for horizontal win
            if (texts[i * 3].text == texts[i * 3 + 1].text && texts[i * 3].text == texts[i * 3 + 2].text && texts[i * 3].text != "")
            {
                //Get the index for the winning line
                if (i == 0) line = 3;
                else if (i == 1) line = 4;
                else line = 5;

                //Return the winner
                return texts[i * 3].text;
            }

            //Look for vertical win
            else if (texts[i].text == texts[i + 3].text && texts[i].text == texts[i + 6].text && texts[i].text != "")
            {
                //Get the index for the winning line
                if (i == 0) line = 0;
                else if (i == 1) line = 1;
                else line = 2;

                //Return the winner
                return texts[i].text;
            }

            //Look for diagonal win
            else if (texts[0].text == texts[4].text && texts[0].text == texts[8].text && texts[0].text != "")
            {
                //Get the index for the winning line
                line = 6;

                //Return the winner
                return texts[0].text;
            }

            //Look for other diagonal win
            else if (texts[2].text == texts[4].text && texts[2].text == texts[6].text && texts[2].text != "")
            {
                //Get the index for the winning line
                line = 7;

                //Return the winner
                return texts[2].text;
            }
        }

        //If there is a draw
        if (isDraw())
        {
            //There is no winning line
            line = -2;

            //Return that no one won
            return "draw";
        }

        //No one won
        return "n";
    }

    //Returns if there is no space left in the board
    private bool isDraw()
    {
        for (int i = 0; i < texts.Length; i++)
        {
            //If there is a space ...
            if (texts[i].text == "") 
                return false;
        }

        //There is no space left
        return true;
    }

    //Find if the game is over or not
    private bool isGameOver()
    {
        //Get the winner
        string str = findWinner();

        //If there is no winner / no draw
        if (str != "n")
        {
            //Assign the winner
            winner = str;

            //The game is over
            return true;
        }

        //Game is not over
        return false;
    }

    private void endGame()
    {
        //If X wins ... 
        if (winner == "X")
        {
            //Get the right winning line
            winningLines[line].gameObject.SetActive(true);

            //Set the color of the winning line to red
            winningLines[line].color = Color.red;

            //Set the announcement text to red
            announcementText.color = Color.red;

            //Announce X as the winner
            announcementText.text = winner + " is the winner!";

            //Add to scores
            scores[0]++;
        }

        //If O wins ...
        else if (winner == "O")
        {
            //Get the right winning line
            winningLines[line].gameObject.SetActive(true);

            //Set the color of the winning line to white
            winningLines[line].color = Color.white;

            //Set the announcement text to white
            announcementText.color = Color.white;

            //Announce O as the winner
            announcementText.text = winner + " is the winner!";

            //Add to scores
            scores[1]++;
        }

        //If there is a draw ...
        else
        {
            //Set the announcement text to black
            announcementText.color = Color.black;

            //Announce there is a draw
            announcementText.text = "Draw!";

            //Add to scores
            scores[2]++;
        }
        
        //Update the scoreboard
        updateScores();

        //Move the board
        StartCoroutine(moveBoard());
    }

    private IEnumerator moveBoard()
    {
        //Wait for 1 second
        yield return new WaitForSeconds(1f);

        //Slowly move the board to the side to be able to see the winning announcement
        for (int i = 0; i < 325; i++)
        {
            image.transform.Translate(-1f, 0f, 0f);
            announcementText.gameObject.transform.Translate(-1f, 0f, 0f);
            yield return new WaitForSeconds(0.005f);
        }
    }

    public void switchToRandomAI()
    {
        //Disable current canvas (manual TTT)
        manTTT.gameObject.SetActive(false);

        //Enable other canvas (AI TTT)
        aiTTT.gameObject.SetActive(true);

        //Play against the smart AI
        aiTTT.GetComponent<AITTT>().smart = false;

        //Call restart for other canvas
        aiTTT.GetComponent<AITTT>().restart();
    }

    public void switchToSmartAI()
    {
        //Disable current canvas (manual TTT)
        manTTT.gameObject.SetActive(false);

        //Enable other canvas (AI TTT)
        aiTTT.gameObject.SetActive(true);

        //Play against the smart AI
        aiTTT.GetComponent<AITTT>().smart = true;

        //Call restart for other canvas
        aiTTT.GetComponent<AITTT>().restart();
    }

    public void restart()
    {
        //There is no winner
        winner = "n";

        //The game is still ongoing
        gameOver = false;

        //The player(s) can still place XO down
        disabled = false;

        //There is no index for the winning line
        line = -1;


        //The starting char is X
        x = true;

        //Place the image in its starting position
        image.transform.position = imageOriginalPosition;

        //Place the announcementText in its starting position
        announcementText.transform.position = annTextOriginalPosition;

        //Disable the winning lines
        for (int i = 0; i < winningLines.Length; i++)
        {
            winningLines[i].gameObject.SetActive(false);
        }

        //Clear all chars in texts
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = "";
        }

        //Stops the board from moving
        StopAllCoroutines();
    }
}
