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

    [Header("Info")]

    [SerializeField]
    [ShowOnly]
    private int m_turns = 0;

    [SerializeField]
    [ShowOnly]
    private TurnMode currentTurnMove = TurnMode.Nothing;

    [Header("Variables")]

    [SerializeField]
    private Tween m_tweenScript;
    [SerializeField]
    private LineRenderer m_WinStreakVisual;
    [SerializeField]
    private ParticleSystem m_WinStreakParticles;

    [SerializeField]
    private Animator m_TurnIndicator;

    [SerializeField]
    private Text m_TurnCurrentText;
    [SerializeField]
    private Text m_TurnFacadeText;
    [SerializeField]
    private Text m_TurnText;

    [SerializeField]
    private Camera m_currentCamera;

    private Board m_board;

    public EasingFunction.Ease m_MarkerAnimationStyle = EasingFunction.Ease.EaseOutSine;
    public float m_MarkerAnimationDuration = 3.0f;

    public EasingFunction.Ease m_WinAnimationStyle = EasingFunction.Ease.EaseOutSine;
    public float m_WinAnimationDuration = 3.0f;
    public Vector3 m_WinLineOffset = new Vector3(0.0f, 0.0f, -1.0f);

    public float m_BackgroundColorAnimationDamp = 1.0f;

    public float m_AITurnDuration = 1.0f;

    public GameObject m_oMarker;

    public GameObject m_xMarker;

    public MarkerType playerMarker;
    public MarkerType AIMarker;

    [SerializeField]
    private Transform[] m_slotPositions;

    [SerializeField]
    private Button[] m_slotButtons;

    private GameObject[] m_markers;

    private Color m_BackgroundColorGoTo = Color.black;
    private Vector3 m_backgroundVel = Vector3.zero;

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

    private int[] m_WinPositions = new int[3];

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

    void Update()
    {
        m_currentCamera.backgroundColor = ColorSmoothDamp(m_currentCamera.backgroundColor, m_BackgroundColorGoTo, ref m_backgroundVel, m_BackgroundColorAnimationDamp);
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
            m_TurnText.text = "Turns: " + m_turns.ToString();

            if ((m_turns + player) % 2 == 0)
            {
                Debug.Log("Computer's turn!");
                currentTurnMove = TurnMode.AIMove;
                UpdateTurnVisual();
                m_BackgroundColorGoTo = GetMarkerColor(AIMarker);

                yield return new WaitForSeconds(m_AITurnDuration);
                ComputerMove(m_board);
                Debug.Log("Computer has made their turn!");
                UpdateBoardVisuals();
            }
            else
            {
                Debug.Log("Player's turn!");
                currentTurnMove = TurnMode.PlayerMove;
                UpdateTurnVisual();
                m_BackgroundColorGoTo = GetMarkerColor(playerMarker);

                yield return new WaitUntil(() => m_turnOver);
                Debug.Log("Player has made their turn!");
                UpdateBoardVisuals();
            }

            m_turnOver = false;
        }

        currentTurnMove = TurnMode.GameOver;

        yield return new WaitForSeconds(1);

        switch (Win(m_board).ToInt())
        {
            case 0:
                Debug.Log("A draw. How droll.\n");
                m_BackgroundColorGoTo = Color.black;
                UpdateText("Tie game!", new Color(Convert8ToFloat(50.0f), Convert8ToFloat(50.0f), Convert8ToFloat(50.0f)));
                break;
            case 1:
                Debug.Log("You lose.\n");
                WinVisual(m_WinPositions);
                UpdateText("AI won!", GetMarkerColor(AIMarker));
                break;
            case -1:
                Debug.Log("You win.\n");
                WinVisual(m_WinPositions);
                UpdateText("You win!", GetMarkerColor(playerMarker));
                break;
        }
    }

    /// <summary>
    /// Checks for win state
    /// </summary>
    /// <param name="currentBoard">Active board</param>
    /// <returns>Winner</returns>
    Slots Win(Board currentBoard) 
    {
        Slots[] board = currentBoard.GetBoard();

        for (int i = 0; i < 8; ++i)
        {

            if (
                board[m_WinningStates[i, 0]] != Slots.Blank &&
                board[m_WinningStates[i, 0]] == board[m_WinningStates[i, 1]] &&
                board[m_WinningStates[i, 0]] == board[m_WinningStates[i, 2]]
            )
            {
                // Extract row from 2D array
                for (int p = 0; p < m_WinPositions.Length; p++)
                {
                    m_WinPositions[p] = m_WinningStates[i, p];
                }
                return currentBoard.GetBoardSlot(m_WinningStates[i, 2]);
            }
        }

	    return Slots.Blank;
    }

    /// <summary>
    /// Returns the score of the select position; determinds if it is a bad or good move
    /// </summary>
    /// <param name="currentBoard">Active board</param>
    /// <param name="player">Active user</param>
    /// <returns>Score</returns>
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

    /// <summary>
    /// AI move
    /// </summary>
    /// <param name="currentBoard">Active board</param>
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
                int tempScore = -MinMax(currentBoard, Slots.Player);
                board[i] = 0;
                if (tempScore > score)
                {
                    score = tempScore;
                    move = i;
                }
            }
        }
        //returns a score based on minimax tree at a given node.
        //board[move] = 1;

        currentBoard.SetBoardSlot(Slots.AI, move);
    }

    /// <summary>
    /// Player move
    /// </summary>
    /// <param name="pos">Selected position</param>
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

    /// <summary>
    /// Updates board visuals
    /// </summary>
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

    void UpdateTurnVisual()
    {
        switch(currentTurnMove)
        {
            case TurnMode.AIMove:
                UpdateText("AI", GetCurrentTurnColor(TurnMode.AIMove));
                break;

            case TurnMode.PlayerMove:
                UpdateText("Player", GetCurrentTurnColor(TurnMode.PlayerMove));
                break;
            default:
                UpdateText("");
                break;
        }
    }

    void UpdateText(string newText)
    {
        m_TurnFacadeText.text = newText;
        m_TurnFacadeText.color = new Color(50, 50, 50, 225);

        m_TurnIndicator.SetTrigger("Switch");
    }

    void UpdateText(string newText, Color newColor)
    {
        m_TurnFacadeText.text = newText;
        m_TurnFacadeText.color = newColor;

        m_TurnIndicator.SetTrigger("Switch");
    }

    Color GetCurrentTurnColor(TurnMode currentMode)
    {
        MarkerType selectedMarker;

        switch(currentMode)
        {
            case TurnMode.AIMove:
                selectedMarker = AIMarker;
                break;
            case TurnMode.PlayerMove:
                selectedMarker = playerMarker;
                break;
            default:
                selectedMarker = MarkerType.NULL;
                break;
        }

        switch (selectedMarker)
        {
            case MarkerType.O:
                //return m_oMarker.GetComponent<Shader>().GetColor("_Color");
                return m_oMarker.GetComponentInChildren<Renderer>().sharedMaterial.color;
            case MarkerType.X:
                //return m_xMarker.GetComponent<Material>().GetColor("_Color");
                return m_xMarker.GetComponentInChildren<Renderer>().sharedMaterial.color;
            default:
                return Color.black;
        }
    }

    Color GetMarkerColor(MarkerType selectedMarker)
    {
        switch (selectedMarker)
        {
            case MarkerType.O:
                //return m_oMarker.GetComponent<Shader>().GetColor("_Color");
                return m_oMarker.GetComponentInChildren<Renderer>().sharedMaterial.color;
            case MarkerType.X:
                //return m_xMarker.GetComponent<Material>().GetColor("_Color");
                return m_xMarker.GetComponentInChildren<Renderer>().sharedMaterial.color;
            default:
                return Color.black;
        }
    }

    Color ColorSmoothDamp(Color current, Color target, ref Vector3 velocity, float smoothTime)
    {
        Vector3 c = ColorToVector3(current);
        Vector3 t = ColorToVector3(target);

        Vector3 temp = Vector3.SmoothDamp(c, t, ref velocity, smoothTime);

        return Vector3ToColor(temp);
    }

    Vector3 ColorToVector3(Color newColor)
    {
        return new Vector3(newColor.r, newColor.g, newColor.b);
    }

    Color Vector3ToColor(Vector3 newVector3)
    {
        return new Color(newVector3.x, newVector3.y, newVector3.z);
    }

    float Convert8ToFloat(float new8Bit)
    {
        return new8Bit / 255.0f;
    }

    void WinVisual(int[] winPositions)
    {
        if (winPositions.Length > 3)
        {
            Debug.LogWarning("Array is too long!");
        }

        // Get the start position
        Vector3 startPos = m_slotPositions[winPositions[0]].position;

        // Set beginning and end of line to the start position
        m_WinStreakVisual.SetPosition(0, startPos + m_WinLineOffset);
        m_WinStreakVisual.SetPosition(1, startPos + m_WinLineOffset);

        m_WinStreakVisual.gameObject.SetActive(true);

        StartCoroutine(WinVisualAnimation(winPositions));
    }

    IEnumerator WinVisualAnimation(int[] winPositions)
    {
        Vector3 startPos = m_slotPositions[winPositions[0]].position + m_WinLineOffset;
        Vector3 endPos = m_slotPositions[winPositions[winPositions.Length - 1]].position + m_WinLineOffset;

        // Create new particles
        GameObject newParticlesGO = Instantiate(m_WinStreakParticles.gameObject) as GameObject;

        // Get main particle system from gameObject
        ParticleSystem.MainModule newParticles = newParticlesGO.GetComponent<ParticleSystem>().main;

        // Move particles to start position
        newParticlesGO.transform.position = startPos;

        for (float t = 0.0f; t < m_WinAnimationDuration; t+= Time.deltaTime)
        {
            float p = EasingFunction.GetEasingFunction(m_WinAnimationStyle)(0.0f, 1.0f, t / m_WinAnimationDuration);

            Vector3 newPos = Vector3.LerpUnclamped(startPos, endPos, p);

            m_WinStreakVisual.SetPosition(1, newPos);
            newParticlesGO.transform.position = newPos;
            yield return new WaitForEndOfFrame();
        }


        yield return new WaitForSeconds(newParticles.startLifetime.constant);

        Destroy(newParticlesGO);
    }

    /// <summary>
    /// Places selected marker to position
    /// </summary>
    /// <param name="selectedMarker">Selected marker</param>
    /// <param name="pos">Selected position</param>
    /// <returns>Success</returns>
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
