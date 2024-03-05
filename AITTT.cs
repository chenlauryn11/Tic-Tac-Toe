using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AITTT : MonoBehaviour
{
    //Player is -1, computer is 1


    //Holds the TTT board
    private int[] board = new int[9];

    //Holds who the winner is
    [SerializeField] int winner;

    //Holds whether the game is over or not
    [SerializeField] bool gameOver;

    //Ensures that the game end only happens once 
    [SerializeField] bool disabled;

    //Holds whether the move the AI is responding to was the first move
    [SerializeField] bool firstMove;

    //Holds whether the player's first move is in the middle
    [SerializeField] bool putInMiddle;

    //Holds whether the AI is smart or not
    public bool smart;

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

    //Holds the sprite image to be changed
    [SerializeField] Image aiMode;

    //Holds the two sprites
    [SerializeField] Sprite randomAI, smartAI;


    //Holds all of the buttons (for the sections of the TTT board)
    [SerializeField] Button[] buttons = new Button[9];

    //Holds all of the spaces where XO can be placed
    [SerializeField] TextMeshProUGUI[] texts = new TextMeshProUGUI[9];

    //Holds the line that shows up to mark where the winning 3-in-a-row was made
    [SerializeField] Image[] winningLines = new Image[8];


    //Holds whether it is the player's turn or not
    bool player;

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

    public void click(int i)
    {
        //If the game is over, nothing happens
        if (gameOver) return;

        //If it is the computer's turn, do nothing
        if (!player) return;

        //Get the specified TMPro 
        TextMeshProUGUI t = texts[i];

        //If the text isn't empty, nothing happens
        if (t.text != "") return;

        //Turn the X to red
        t.color = Color.red;

        //Place the X in the proper space
        t.text = "X";

        //Put -1 into the board
        board[i] = -1;

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

        //If the game is still going ...
        else
        {
            //Make it the computer's turn
            player = false;

            int move;

            //If playing against the smart AI ...
            if (smart)
            {
                //Get the best move
                move = getBestMove(board, 1);
            }

            else
            {
                //Get a random move
                move = getRandomMove();
            }

            //If there are no available moves, don't do anything
            if (move < 0) return;

            StartCoroutine(placeMove(move));
        }
    }

    private int getBestMove(int[] b, int player)
    {
        //If there is a move that lets the computer win, return it
        if (canWin(1) != -100) return canWin(1);


        //If there is a move that will block the player
        if (canWin(-1) != -100) return canWin(-1);

        //First move is used to not fall into the usual corner trap
        //If it is the first move ...
        if (firstMove)
        {
            //No longer the first move
            firstMove = false;

            //If the first move was put in the middle
            if (b[4] != 0)
            {
                //Player's first move is in the middle
                putInMiddle = true;

                //Get all of the available corners
                int[] possibleMoves = getAllAvailableCorners(board);

                //Randomly return one of the corners
                return possibleMoves[q.getRandI(0, possibleMoves.Length - 1)];
            }
        }

        //putInMiddle is used to not fall into the secondary corner trap
        //If the player's first move was in the middle ...
        if (putInMiddle)
        {
            //Don't call this line of code anymore
            putInMiddle = false;

            //Get all of the available corners
            int[] pm = getAllAvailableCorners(board);

            //If there is an open corner ...
            if (pm.Length != 0)
            {
                //Randomly return one of the corners
                return pm[q.getRandI(0, pm.Length - 1)];
            }
        }

        //Use alpha-beta algorithm to choose the next move
        return getBestMove_ab(b, player);
    }

    private int canWin(int play)
    {
        int[] possibleMoves = getAllMoves(board);

        //Look at hor 1
        if (q.inArr(possibleMoves, lineWin(0, 1, 2, play)))
        {
            return lineWin(0, 1, 2, play);
        }

        //Look at hor 2
        if (q.inArr(possibleMoves, lineWin(3, 4, 5, play)))
        {
            return lineWin(3, 4, 5, play);
        }

        //Look at hor 3
        if (q.inArr(possibleMoves, lineWin(6, 7, 8, play)))
        {
            return lineWin(6, 7, 8, play);
        }


        //Look at ver 1
        if (q.inArr(possibleMoves, lineWin(0, 3, 6, play)))
        {
            return lineWin(0, 3, 6, play);
        }

        //Look at ver 2
        if (q.inArr(possibleMoves, lineWin(1, 4, 7, play)))
        {
            return lineWin(1, 4, 7, play);
        }

        //Look at ver 3
        if (q.inArr(possibleMoves, lineWin(2, 5, 8, play)))
        {
            return lineWin(2, 5, 8, play);
        }


        //Look at dia 1
        if (q.inArr(possibleMoves, lineWin(0, 4, 8, play)))
        {
            return lineWin(0, 4, 8, play);
        }

        //Look at dia 2
        if (q.inArr(possibleMoves, lineWin(2, 4, 6, play)))
        {
            return lineWin(2, 4, 6, play);
        }


        //No one can win in the next move
        return -100;
    }

    private int lineWin(int a, int b, int c, int play)
    {
        if (board[a] == board[b] && board[c] == 0 && board[a] == play) return c;
        if (board[a] == board[c] && board[b] == 0 && board[a] == play) return b;
        if (board[b] == board[c] && board[a] == 0 && board[b] == play) return a;

        return -1;
    }

    private int getBestMove_ab(int[] b, int player)
    {
        //Get all possible moves
        int[] possibleMoves = q.randomize(getAllMoves(b));

        int tScore = -1;

        //Assign a default best move
        int bestMove = possibleMoves[0];

        int[] tBoard;
        int score;

        for (int i = 0; i < possibleMoves.Length; i++)
        {
            //Copy the board, so changes are not reflected on actual board
            tBoard = q.copyArr(b);

            //Add a move to the copied board
            tBoard[i] = player;

            score = alphaBeta(tBoard, 0, -2, 2, -player) * player; //+- 2 are arbitrary values (represent infinity)

            if (score > tScore)
            {
                bestMove = possibleMoves[i];
                tScore = score;
            }
        }

        return bestMove;
    }

    private int alphaBeta(int[] node, int depth, int a, int b, int player)
    {
        //Get all possible moves
        int[] possibleMoves = q.randomize(getAllMoves(node));

        //Break out of recursive method (No moves; someone won; too deep)
        if (possibleMoves.Length == 0 || findWinner(node) != 0 || depth == 6)
        {
            return findWinner(node);
        }

        int value;
        int[] tBoard;

        //If computer is playing ... 
        if (player == 1)
        {
            value = -3; //Is an arbitrary value; it must be below -1

            //Try all moves
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                //Copy the board so anything done isn't on the actual board
                tBoard = q.copyArr(node);

                //Add move to the copied board
                tBoard[i] = player;

                //Use the minimax algorithm
                value = Math.Max(value, alphaBeta(tBoard, depth + 1, a, b, -player));

                //Prune the tree
                if (value > b) break;

                a = Math.Max(a, value);
            }

            return value;
        }

        else
        {
            value = 3; //Arbitrary value again

            //Try all moves
            for (int i = 0; i < possibleMoves.Length; i++)
            {
                //Copy the board, so anything done isn't reflected on the actual board
                tBoard = q.copyArr(node);

                //Add move to the copied board
                tBoard[i] = player;

                //Use the minimax algorithm
                value = Math.Min(value, alphaBeta(tBoard, depth + 1, a, b, -player));

                //Prune the tree
                if (value < a) break;

                b = Math.Min(b, value);
            }

            return value;
        }
    }

    private int getRandomMove()
    {
        int[] moves = getAllMoves(board);

        if (moves.Length == 0) 
            return -1;

        return moves[q.getRandI(0, moves.Length)];
    }

    private IEnumerator placeMove(int i)
    {
        yield return new WaitForSeconds(1f);

        //Get the specified TMPro 
        TextMeshProUGUI t = texts[i];

        //Turn the O to white
        t.color = Color.white;

        //Place the O in the proper space
        t.text = "O";

        //Put 1 into the board
        board[i] = 1;

        //Find out if the game is over or not
        gameOver = isGameOver();

        //If the game is over ...
        if (gameOver && !disabled)
        {
            //Make sure endGame() is only called once
            disabled = true;

            //End the game
            endGame();

            yield break;
        }

        //Make it the player's turn
        player = true;
    }

    private void updateScores()
    {
        scoreText.text = string.Format("Score: {0:0}:{1:0}:{2:0} (X:O:Draw)", scores[0], scores[1], scores[2]);
    }

    //Find the winner of the game. 
    private int findWinner(int[] b)
    {
        for (int i = 0; i < 3; i++)
        {
            //Look for horizontal win
            if (b[i * 3] == b[i * 3 + 1] && b[i * 3] == b[i * 3 + 2] && b[i * 3] != 0)
            {
                //Get the index for the winning line
                if (i == 0) line = 3;
                else if (i == 1) line = 4;
                else line = 5;

                //Return the winner
                return b[i * 3];
            }

            //Look for vertical win
            else if (b[i] == b[i + 3] && b[i] == b[i + 6] && b[i] != 0)
            {
                //Get the index for the winning line
                if (i == 0) line = 0;
                else if (i == 1) line = 1;
                else line = 2;

                //Return the winner
                return b[i];
            }

            //Look for diagonal win
            else if (b[0] == b[4] && b[0] == b[8] && b[0] != 0)
            {
                //Get the index for the winning line
                line = 6;

                //Return the winner
                return b[0];
            }

            //Look for other diagonal win
            else if (b[2] == b[4] && b[2] == b[6] && b[2] != 0)
            {
                //Get the index for the winning line
                line = 7;

                //Return the winner
                return b[2];
            }
        }

        //If there is a draw
        if (isDraw())
        {
            //There is no winning line
            line = -2;

            //Return that no one won
            return -2;
        }

        //No one one
        return 0;
    }

    //Returns all availabe moves
    private int[] getAllMoves(int[] b)
    {
        List<int> m = new List<int>();

        for (int i = 0; i < b.Length; i++)
        {
            //If the space is empty, add the move to the list
            if (b[i] == 0)
                m.Add(i);
        }

        //Return the list as an array
        return q.copyList(m);
    }

    private int[] getAllAvailableCorners(int[] b)
    {
        //Get all of the corners
        int[] allCorners = new int[4] { 0, 2, 6, 8 };

        //Get all possible moves
        int[] possibleMoves = getAllMoves(b);

        List<int> result = new List<int>();

        for (int i = 0; i < allCorners.Length; i++)
        {
            if (q.inArr(possibleMoves, allCorners[i]))
            {
                result.Add(allCorners[i]);
            }
        }

        return q.copyList(result);
    }

    //Returns if there is no space left in the board
    private bool isDraw()
    {
        for (int i = 0; i < board.Length; i++)
        {
            //If there is a space ...
            if (board[i] == 0)
                return false;
        }

        //There is no space left
        return true;
    }

    //Find if the game is over or not
    private bool isGameOver()
    {
        //Get the winner
        int num = findWinner(board);

        //If there is no winner / no draw
        if (num != 0)
        {
            //Assign the winner
            winner = num;

            //The game is over
            return true;
        }

        //Game is not over
        return false;
    }

    private void endGame()
    {
        //If the player wins ... 
        if (winner == -1)
        {
            //Get the right winning line
            winningLines[line].gameObject.SetActive(true);

            //Set the color of the winning line to red
            winningLines[line].color = Color.red;

            //Set the announcement text to red
            announcementText.color = Color.red;

            //Announce the player as the winner
            announcementText.text = "Player (X) is the winner!";

            //Add to scores
            scores[0]++;
        }

        //If the computer wins ...
        else if (winner == 1)
        {
            //Get the right winning line
            winningLines[line].gameObject.SetActive(true);

            //Set the color of the winning line to white
            winningLines[line].color = Color.white;

            //Set the announcement text to white
            announcementText.color = Color.white;

            //Announce the computer as the winner
            announcementText.text = "Computer (O) is the winner!";

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
            yield return new WaitForSeconds(0.00001f);
        }
    }

    public void switchToManual()
    {
        //Disable current canvas (AI TTT)
        aiTTT.gameObject.SetActive(false);

        //Enable other canvas (Manual TTT)
        manTTT.gameObject.SetActive(true);

        //Call restart for other canvas
        manTTT.GetComponent<TTT>().restart();
    }

    public void switchToOtherAIMode()
    {
        //Play against the other AI mode
        smart = !smart;

        //Restart the game
        restart();
    }

    public void restart()
    {
        //There is no winner
        winner = 0;

        //The game is still ongoing
        gameOver = false;

        //The player(s) can still place XO down
        disabled = false;

        //It was the first move
        firstMove = true;

        //Player hasn't put a move down yet
        putInMiddle = false;

        //There is no index for the winning line
        line = -1;


        //The player starts
        player = true;

        //Place the image in its starting position
        image.transform.position = imageOriginalPosition;

        //Place the announcementText in its starting position
        announcementText.transform.position = annTextOriginalPosition;

        //Places the correct sprite on the button
        if (smart)
        {
            aiMode.sprite = randomAI;
        }

        else
        {
            aiMode.sprite = smartAI;
        }

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

        //Clear the board
        for (int i = 0; i < board.Length; i++)
        {
            board[i] = 0;
        }

        //Stops the board from moving
        StopAllCoroutines();
    }
}
