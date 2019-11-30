using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiBackend
{
    public class Board
    {
        public static int DEFAULT_NUMBER_OF_FIELDS = 64;
        private List<Field> fields;

        public List<Field> Fields
        {
            get
            {
                return fields;
            }
        }
        public Board()
        {
            this.fields = new List<Field>(DEFAULT_NUMBER_OF_FIELDS);
            for (int i = 0; i < DEFAULT_NUMBER_OF_FIELDS; i++)
            {
                this.fields.Add(new Field(i));
            }
            fields[27].PawnPlayerNumber = PlayerNumber.FirstPlayer;
            fields[28].PawnPlayerNumber = PlayerNumber.SecondPlayer;
            fields[35].PawnPlayerNumber = PlayerNumber.SecondPlayer;
            fields[36].PawnPlayerNumber = PlayerNumber.FirstPlayer;
        }
        public Board(Board other)
        {
            this.fields = new List<Field>(other.fields.Count);
            for (int i = 0; i < other.fields.Count; i++)
            {
                this.fields.Add(new Field(other.fields[i]));
            }
        }
        public List<Field> GetPlayerFields(PlayerNumber playerNumber)
        {
            List<Field> playerFields = new List<Field>();
            foreach (var field in fields)
            {
                if (field.PawnPlayerNumber == playerNumber)
                {
                    playerFields.Add(field);
                }
            }
            return playerFields;
        }
        public Field GetField(int index)
        {
            return fields[index];
        }
        public List<Field> GetEmptyFields()
        {
            return GetPlayerFields(PlayerNumber.None);
        }
       
    }
}
