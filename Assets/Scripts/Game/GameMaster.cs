using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMaster : MonoBehaviour {

    private enum TurnMode
    {
        Nothing,
        PlayerMove,
        AIMove,
        GameOver
    }

    private GameInfo GI = null;

    [SerializeField]
    private Tween m_tweenScript;

    [SerializeField][ShowOnly]
    private int m_turns = 0;

    private Board m_board;

    public EasingFunction.Ease m_MarkerAnimationStyle = EasingFunction.Ease.EaseOutSine;
    public float m_MarkerAnimationDuration = 3.0f;

    public float m_AITurnDuration = 1.0f;

    public GameObject m_oMarker;

    public GameObject m_xMarker;

    public MarkerType playerMarker;
    public MarkerType AIMarker;

    [SerializeField][ShowOnly]
    private TurnMode currentTurnMove = TurnMode.Nothing;

    [SerializeField]
    private Transform[] m_slotPositions;

    [SerializeField]
    private Button[] m_slotButtons;

    private GameObject[] m_markers;

    // Determines if a player has won, returns 0 otherwise.
    private int[,] m_WinningStates = new int[8, 3]
    {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 },
            { 2, 4, 6 }
    };

    private bool m_turnOver = false;

	// Use this for initialization
	void Start () {
        // Get game info
        GI = GameInfo.GetInstance();

        m_board = new Board();
        m_markers = new GameObject[9];

        GI.SetPlayerMarker(MarkerType.O);
        GI.SetPlayerFirst(true);

        playerMarker = GI.GetPlayerMarker();
        AIMarker = GI.GetAIMarker();

        StartCoroutine(GameLoop());

    }

    /// <summary>
    /// Game loop
    /// </summary>
    /// <returns></returns>
    IEnumerator GameLoop()
    {
        int player = GI.GetPlayerFirst() ? 1 : 2;

        for (m_turns = 0; m_turns < 9 && Win(m_board) == 0; ++m_turns)
        {
            if ((m_turns + player) % 2 == 0)
            {
                Debug.Log("Computer's turn!");
                currentTurnMove = TurnMode.AIMove;
                yield return new WaitForSeconds(m_AITurnDuration);
                ComputerMove(m_board);
                Debug.Log("Computer has made their turn!");
                UpdateBoardVisuals();
            }
            else
            {
                Debug.Log("Player's turn!");
                currentTurnMove = TurnMode.PlayerMove;
                yield return new WaitUntil(() => m_turnOver);
                Debug.Log("Player has made their turn!");
                UpdateBoardVisuals();
            }

            m_turnOver = false;
        }

        currentTurnMove = TurnMode.GameOver;

        switch (Win(m_board).ToInt())
        {
            case 0:
                Debug.Log("A draw. How droll.\n");
                break;
            case 1:
                Debug.Log("You lose.\n");
                break;
            case -1:
                Debug.Log("You win.\n");
                break;
        }
    }

    Slots Win(Board currentBoard) 
    {
        Slots[] board = currentBoard.GetBoard();

	    for (int i = 0; i< 8; ++i)
        {

            if (
                board[m_WinningStates[i, 0]] != Slots.Blank &&
                board[m_WinningStates[i, 0]] == board[m_WinningStates[i, 1]] &&
                board[m_WinningStates[i, 0]] == board[m_WinningStates[i, 2]]
            )
            {
                return currentBoard.GetBoardSlot(m_WinningStates[i, 2]);
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
        int score = -2; //Losing moves are preferred to no move
        for (int i = 0; i < 9; ++i)
        {
            //For all moves,
            if (board[i] == Slots.Blank)
            {
                //If legal,
                board[i] = player; //Try the move

                int thisScore = -MinMax(currentBoard, player * -1);
                if (thisScore > score)
                {
                    score = thisScore;
                    move = i;
                } //Pick the one that's worst for the opponent

                board[i] = 0; //Reset board after try
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

    public void PlayerMove(int pos)
    {
        if (currentTurnMove != TurnMode.PlayerMove)
        {
            Debug.Log("It's not your turn!");
            return;
        }
        m_board.SetBoardSlot(Slots.Player, pos);
        m_slotButtons[pos].interactable = false;

        m_turnOver = true;
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
                    break;
                case 1:
                    m_slotButtons[i].interactable = false;
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
            //newMarker.transform.position = slotPositions[pos].position;
            newMarker.transform.position = new Vector3(0,0,-30);
            newMarker.transform.eulerAngles = new Vector3(Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f), Random.Range(-180.0f, 180.0f));

            m_tweenScript.TweenPositionAndRotation(newMarker.transform, m_slotPositions[pos].position, new Vector3(0.0f, 0.0f, 0.0f), m_MarkerAnimationStyle, m_MarkerAnimationDuration);

            newMarker.transform.SetParent(m_slotPositions[pos]);

            m_markers[pos] = newMarker;
        }

        return true; // Success
    }
}
