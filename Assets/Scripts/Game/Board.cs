public struct Slots
{
    private int InternalValue { get; set; }

    public static readonly int Blank = 0;
    public static readonly int AI = 1;
    public static readonly int Player = -1;

    public override bool Equals(object obj)
    {
        Slots otherObj = (Slots)obj;
        return otherObj.InternalValue.Equals(InternalValue);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static bool operator ==(Slots val, Slots other)
    {
        return (val.InternalValue == other.InternalValue);
    }

    public static bool operator ==(Slots val, int other)
    {
        return (val.InternalValue == other);
    }

    public static bool operator !=(Slots val, Slots other)
    {
        return (val.InternalValue != other.InternalValue);
    }


    public static bool operator !=(Slots val, int other)
    {
        return (val.InternalValue != other);
    }

    public static int operator *(Slots val, int other)
    {
        return (val.InternalValue * other);
    }

    public static int operator *(Slots val, Slots other)
    {
        return (val.InternalValue * other.InternalValue);
    }

    public static implicit operator Slots(int otherType)
    {
        return new Slots
        {
            InternalValue = otherType
        };
    }

    public int ToInt()
    {
        return InternalValue;
    }
}

public class Board : GameMaster {

    // Board
    private Slots[] board;

    // Constructor
    public Board()
    {
        board = new Slots[9];
    }

    /// <summary>
    /// Get the board
    /// </summary>
    /// <returns>Board instance</returns>
    public Slots[] GetBoard()
    {
        return board;
    }

    public Slots GetBoardSlot(int pos)
    {
        return board[pos];
    }

    /// <summary>
    /// Replace entire board with new board
    /// </summary>
    /// <param name="newBoard">New board</param>
    public void SetBoard(Slots[] newBoard)
    {
        board = newBoard;
    }

    /// <summary>
    /// Set slot type
    /// </summary>
    /// <param name="slotType">Slot type</param>
    /// <param name="xPos">X Position</param>
    /// <param name="yPos">Y Position</param>
    public void SetBoardSlot(Slots slotType, int pos)
    {
        board[pos] = slotType;
    }
}
