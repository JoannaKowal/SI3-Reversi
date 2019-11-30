using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiBackend
{
    public enum PlayerNumber
    {
        FirstPlayer,
        SecondPlayer,
        None
    }
    public class Field
    {
        public static int FIELD_INDEX_UNSET = -1;

        public PlayerNumber PawnPlayerNumber { get; set; }
        public int FieldIndex { get; private set; }
        public int LastFieldIndex { get; set; }
        public bool Empty
        {
            get
            {
                return this.PawnPlayerNumber == PlayerNumber.None;
            }
        }
        public Field(int fieldIndex)
        {
            PawnPlayerNumber = PlayerNumber.None;
            FieldIndex = fieldIndex;
        }
        public Field(Field other)
        {
            PawnPlayerNumber = other.PawnPlayerNumber;
            LastFieldIndex = other.LastFieldIndex;
            FieldIndex = other.FieldIndex;
        }
        public bool BelongsTo(PlayerNumber player)
        {
            return PawnPlayerNumber == player;
        }
        public bool CanMoveTo(Field other)
        {
            return true;
            //if((LastFieldIndex == FIELD_INDEX_UNSET || LastFieldIndex != other.FieldIndex) && other.PawnPlayerNumber == PlayerNumber.None)
            //{
            //    MoveOK(other);
            //}
           // return (LastFieldIndex == FIELD_INDEX_UNSET || LastFieldIndex != other.FieldIndex) && other.PawnPlayerNumber == PlayerNumber.None;
            //return other.PawnPlayerNumber == PlayerNumber.None;
        }

    }
}
