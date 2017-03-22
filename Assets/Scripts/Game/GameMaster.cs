using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MarkerType
{
    X = 0,
    O = 1
}

public class GameMaster : MonoBehaviour {

    [SerializeField][ShowOnly]
    private Board m_board;

    [SerializeField][ShowOnly]
    private int m_turns = 0;

    public int m_totalTurns = 9;

    public GameObject m_oMarker;

    public GameObject m_xMarker;

    public MarkerType playerMarker;
    public MarkerType AIMarker;

    [SerializeField]
    private Transform[] slotPositions;

    [SerializeField]
    private GameObject[] slotButtons;

    private GameObject[] m_markers;

	// Use this for initialization
	void Start () {
        m_board = new Board();
        m_markers = new GameObject[9];

        ComputerMove(m_board);

        playerMarker = MarkerType.O;
        AIMarker = MarkerType.O;

        UpdateBoardVisuals();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    Slots Win(Board currentBoard) 
    {
        Slots[] board = currentBoard.GetBoard();

        // Determines if a player has won, returns 0 otherwise.
        int[,] wins = new int[8,3]{ { 0, 1, 2 }, { 3, 4, 5 }, { 6, 7, 8 }, { 0, 3, 6 }, { 1, 4, 7 }, { 2, 5, 8 }, { 0, 4, 8 }, { 2, 4, 6 } };
	    for (int i = 0; i< 8; ++i)
        {

            if (
                board[wins[i, 0]] != Slots.Blank &&
                board[wins[i, 0]] == board[wins[i, 1]] &&
                board[wins[i, 0]] == board[wins[i, 2]]
            )
            {
                return currentBoard.GetBoardSlot(wins[i, 2]);
            }
	    }

	    return Slots.Blank;
    }

    int MinMax(Board currentBoard, Slots player)
    {
        Slots[] board = currentBoard.GetBoard();

        //How is the position like for player (their turn) on board?
        Slots winner = Win(currentBoard);
        if (winner != 0) return winner * player;

        int move = -1;
        int score = -2;//Losing moves are preferred to no move
        int i;
        for (i = 0; i < 9; ++i)
        {
            //For all moves,
            if (board[i] == Slots.Blank)
            {
                //If legal,
                board[i] = player;//Try the move

                int thisScore = -MinMax(currentBoard, player * -1);
                if (thisScore > score)
                {
                    score = thisScore;
                    move = i;
                }//Pick the one that's worst for the opponent

                board[i] = 0;//Reset board after try
            }
        }
        if (move == -1) return 0;
        return score;
    }

    void ComputerMove(Board currentBoard)
    {
        Slots[] board = currentBoard.GetBoard();

        int move = -1;
        int score = -2;
        int i;
        for (i = 0; i < 9; ++i)
        {
            if (board[i] == 0)
            {
                board[i] = 1;
                int tempScore = -MinMax(currentBoard, Slots.AI);
                board[i] = 0;
                if (tempScore > score)
                {
                    score = tempScore;
                    move = i;
                }
            }
        }
        //returns a score based on minimax tree at a given node.
        board[move] = 1;
    }

    void UpdateBoardVisuals()
    {
        Slots[] board = m_board.GetBoard();



        for (int i = 0; i < m_markers.Length; i++)
        {
            switch(board[i].ToInt())
            {
                case -1:
                    PlaceMarker(playerMarker, i);
                    //GameObject newMarker = Instantiate()
                    break;
                case 1:
                    PlaceMarker(AIMarker, i);
                    break;
                default:
                    if (m_markers[i])
                    {
                        Destroy(m_markers[i]);
                        m_markers[i] = null;
                    }
                    break;
            }
        }
    }

    bool PlaceMarker(MarkerType selectedMarker, int pos)
    {
        // If marker already exists in place. Return as fail
        if (m_markers[pos])
        {
            return false;
        }

        GameObject newMarker;

        switch (selectedMarker)
        {
            case MarkerType.O:
                newMarker = Instantiate(m_oMarker) as GameObject;
                break;
            case MarkerType.X:
                newMarker = Instantiate(m_xMarker) as GameObject;
                break;
            default:
                return false;
        }

        // New marker needs it be placed in the right spot if exists
        if (newMarker)
        {
            newMarker.transform.position = slotPositions[pos].position;
            newMarker.transform.SetParent(slotPositions[pos]);
        }

        return true; // Success
    }
}
