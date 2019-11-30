using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiBackend
{
    public class GameState
    {
        public Board CurrentBoard { get; }
        private static int[] possibleMoveIndices;
        private static Dictionary<PlayerNumber, string> playerNames;

        public delegate void GameStateChanged();
        public event GameStateChanged OnGameStateChanged = delegate { };

        public delegate void LastSelectedFieldChanged();
        public event LastSelectedFieldChanged OnLastSelectedFieldChanged = delegate { };
        public PlayerNumber WinningPlayer { get; private set; }
        public PlayerNumber CurrentMovingPlayer { get; private set; }
        public int firstPlayerScore;
        public int secondPlayerScore;
        public PlayerNumber OtherPlayer
        {
            get
            {
                return CurrentMovingPlayer == PlayerNumber.FirstPlayer ? PlayerNumber.SecondPlayer : PlayerNumber.FirstPlayer;
            }
        }
        public int MovesMade
        {
            get
            {
                return FirstPlayerMovesMade + SecondPlayerMovesMade;
            }
        }
        public int FirstPlayerMovesMade { get; private set; }
        public int SecondPlayerMovesMade { get; private set; }

        public GameState()
        {
            CurrentBoard = new Board();
            WinningPlayer = PlayerNumber.None;
            CurrentMovingPlayer = PlayerNumber.FirstPlayer;
            firstPlayerScore = 2;
            secondPlayerScore = 2;
            FirstPlayerMovesMade = 0;
            SecondPlayerMovesMade = 0;
            //MovesUntilNow = "";
        }
        public GameState(GameState other)
        {
            CurrentBoard = new Board(other.CurrentBoard);
            WinningPlayer = other.WinningPlayer;
            CurrentMovingPlayer = other.CurrentMovingPlayer;
            LastSelectedField = null;
        }

        private Field _lastSelectedField = null;
        public Field LastSelectedField
        {
            get
            {
                return _lastSelectedField;
            }
            set
            {
                _lastSelectedField = value;
                OnLastSelectedFieldChanged();
            }
        }
        public double Evaluation { get; set; }
        public List<GameState> GetAllPossibleNextStates(PlayerNumber playerNumber, GameState state)
        {
            List<GameState> gameStates = new List<GameState>();
            List<Field> possibleMoves = IsMovePossible(state.CurrentBoard);
            for(int i = 0; i < possibleMoves.Count; i++)
            {
                GameState newGameState = new GameState(state);
                CanPlacePawn(possibleMoves[i].FieldIndex, false, newGameState.CurrentBoard);
                HandlePawnPlacing(possibleMoves[i].FieldIndex, newGameState.CurrentBoard);
                UpdateScore(newGameState.CurrentBoard);
                gameStates.Add(newGameState);
            }
            return gameStates;
            //return GetNextPossibleStates(playerNumber);
        }
        private List<GameState> GetNextPossibleStates(PlayerNumber playerNumber)
        {
            List<GameState> gameStates = new List<GameState>();
            return gameStates;
        }
        public void SwitchPlayer()
        {
            if (CurrentMovingPlayer == PlayerNumber.FirstPlayer)
            {
                CurrentMovingPlayer = PlayerNumber.SecondPlayer;
            }
            else
            {
                CurrentMovingPlayer = PlayerNumber.FirstPlayer;
            }
        }
        public void HandleSelection(int fieldIndex)
        {
           List<Field> possibleMoves = IsMovePossible(CurrentBoard);
            if(possibleMoves.Count != 0)
            {
                if (CanPlacePawn(fieldIndex, false, CurrentBoard))
                {
                    HandlePawnPlacing(fieldIndex,CurrentBoard);
                    CheckGameStateChanged();
                    UpdateScore(CurrentBoard);
                    OnGameStateChanged();

                }

            }
            else
            {
                SwitchPlayer();
                OnGameStateChanged();
            }
            

        }
        private List<Field> IsMovePossible(Board board)
        {
            List<Field> possibleMoves = new List<Field>();
            for(int i = 0; i < board.Fields.Count; i++)
            {
                if(board.Fields[i].PawnPlayerNumber == PlayerNumber.None)
                {
                   if(CanPlacePawn(i, true, board))
                    {
                        possibleMoves.Add(board.Fields[i]);
                    }

                }
            }
            return possibleMoves;
        }
        private bool CanPlacePawn(int fieldIndex, bool test, Board board)
        {
            bool result = false;
            int columnIndex = fieldIndex % 8;
            int rowIndex = fieldIndex / 8;
            
            Field field = board.GetField(fieldIndex);
           
            if(field.PawnPlayerNumber == PlayerNumber.None)
            {
                if(columnIndex >= 2) //sprawdzenie w lewo
                {
                    Field other = board.GetField(fieldIndex - 1);
                    if(other.PawnPlayerNumber == OtherPlayer)
                    {
                        int nextColumnIndex = columnIndex - 1;
                        other = board.GetField(rowIndex * 8 + nextColumnIndex);
                        while(nextColumnIndex >= 0 && other.BelongsTo(OtherPlayer))
                        {
                           nextColumnIndex--;
                            if(nextColumnIndex >= 0)
                            {
                                other = board.GetField(rowIndex * 8 + nextColumnIndex);
                            }
                            
                        }
                        if(nextColumnIndex >= 0) //brak miejsca, gdy -1
                        {
                            if(other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextColumnIndex++;
                                    other = board.GetField(rowIndex * 8 + nextColumnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextColumnIndex++;
                                        other = board.GetField(rowIndex * 8 + nextColumnIndex);
                                    }
                                }
                              
                                result = true;
                            }
                        }

                    }
                }
                if (columnIndex <= 5) //sprawdzenie w prawo
                {
                    Field other = board.GetField(fieldIndex + 1);
                    if (other.PawnPlayerNumber == OtherPlayer)
                    {
                        int nextColumnIndex = columnIndex + 1;
                        other = board.GetField(rowIndex * 8 + nextColumnIndex);
                        while (nextColumnIndex <= 7 && other.BelongsTo(OtherPlayer))
                        {
                            nextColumnIndex++;
                            if (nextColumnIndex <= 7)
                            {
                                other = board.GetField(rowIndex * 8 + nextColumnIndex);
                            }

                        }
                        if (nextColumnIndex <= 7) //brak miejsca, gdy -1
                        {
                            if (other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextColumnIndex--;
                                    other = board.GetField(rowIndex * 8 + nextColumnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextColumnIndex--;
                                        other = board.GetField(rowIndex * 8 + nextColumnIndex);
                                    }
                                }
                                result = true;
                            }
                        }

                    }
                }
                if (rowIndex >= 2) // w górę
                {
                    Field other = board.GetField(fieldIndex - 8);
                    if (other.PawnPlayerNumber == OtherPlayer)
                    {
                        int nextRowIndex = rowIndex - 1;
                        other = board.GetField(nextRowIndex * 8 + columnIndex);
                        while (nextRowIndex >= 0 && other.BelongsTo(OtherPlayer))
                        {
                            nextRowIndex--;
                            if (nextRowIndex >= 0)
                            {
                                other = board.GetField(nextRowIndex * 8 + columnIndex);
                            }

                        }
                        if (nextRowIndex >= 0) //brak miejsca, gdy -1
                        {
                            if (other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextRowIndex++;
                                    other = board.GetField(nextRowIndex * 8 + columnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextRowIndex++;
                                        other = board.GetField(nextRowIndex * 8 + columnIndex);
                                    }
                                }
                               
                                result = true;
                            }
                        }

                    }
                }
                if (rowIndex <= 5) // w dół
                {
                    Field other = board.GetField(fieldIndex + 8);
                    if (other.PawnPlayerNumber == OtherPlayer)
                    {
                        int nextRowIndex = rowIndex + 1;
                        other = board.GetField(nextRowIndex * 8 + columnIndex);
                        while (nextRowIndex <= 7 && other.BelongsTo(OtherPlayer))
                        {
                            nextRowIndex++;
                            if (nextRowIndex <= 7)
                            {
                                other = board.GetField(nextRowIndex * 8 + columnIndex);
                            }

                        }
                        if (nextRowIndex <= 7) //brak miejsca, gdy -1
                        {
                            if (other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextRowIndex--;
                                    other = board.GetField(nextRowIndex * 8 + columnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextRowIndex--;
                                        other = board.GetField(nextRowIndex * 8 + columnIndex);
                                    }
                                }
                                
                                result = true;
                            }
                        }

                    }
                }
                if (Math.Min(columnIndex, rowIndex) >= 2) //przekątna w lewo w górę
                {
                    int nextColumnIndex = columnIndex - 1;
                    int nextRowIndex = rowIndex - 1;

                    Field other = board.GetField(8 * nextRowIndex + nextColumnIndex);
                    if (other.PawnPlayerNumber == OtherPlayer)
                    {
                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                        while (Math.Min(nextColumnIndex,nextRowIndex) >= 0 && other.BelongsTo(OtherPlayer))
                        {
                            nextColumnIndex--;
                            nextRowIndex--;
                            if (Math.Min(nextColumnIndex, nextRowIndex) >= 0)
                            {
                                other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                            }

                        }
                        if (Math.Min(nextColumnIndex, nextRowIndex) >= 0) //brak miejsca, gdy -1
                        {
                            if (other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextColumnIndex++;
                                    nextRowIndex++;
                                    other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextColumnIndex++;
                                        nextRowIndex++;
                                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    }
                                }
                               
                                result = true;
                            }
                        }

                    }
                }
                if (Math.Max(columnIndex, rowIndex) <= 5) //przekątna w prawo w dół
                {
                    int nextColumnIndex = columnIndex + 1;
                    int nextRowIndex = rowIndex + 1;

                    Field other = board.GetField(8 * nextRowIndex + nextColumnIndex);
                    if (other.PawnPlayerNumber == OtherPlayer)
                    {
                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                        while (Math.Max(nextColumnIndex, nextRowIndex) <= 7 && other.BelongsTo(OtherPlayer))
                        {
                            nextColumnIndex++;
                            nextRowIndex++;
                            if (Math.Max(nextColumnIndex, nextRowIndex) <= 7)
                            {
                                other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                            }

                        }
                        if (Math.Max(nextColumnIndex, nextRowIndex) <= 7) //brak miejsca, gdy -1
                        {
                            if (other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextColumnIndex--;
                                    nextRowIndex--;
                                    other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextColumnIndex--;
                                        nextRowIndex--;
                                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    }
                                }
                               
                                result = true;
                            }
                        }

                    }
                }
                if (rowIndex >= 2 && columnIndex <= 5) //przekątna w prawo w górę
                {
                    int nextColumnIndex = columnIndex + 1;
                    int nextRowIndex = rowIndex - 1;

                    Field other = board.GetField(8 * nextRowIndex + nextColumnIndex);
                    if (other.PawnPlayerNumber == OtherPlayer)
                    {
                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                        while (nextRowIndex >= 0 && nextColumnIndex <= 7 && other.BelongsTo(OtherPlayer))
                        {
                            nextColumnIndex++;
                            nextRowIndex--;
                            if (nextRowIndex >= 0 && nextColumnIndex <= 7)
                            {
                                other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                            }

                        }
                        if (nextRowIndex >= 0 && nextColumnIndex <= 7) //brak miejsca, gdy -1
                        {
                            if (other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextColumnIndex--;
                                    nextRowIndex++;
                                    other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextColumnIndex--;
                                        nextRowIndex++;
                                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    }
                                }
                               
                                result = true;
                            }
                        }

                    }
                }
                if (rowIndex <= 5 && columnIndex >= 2) //przekątna w lewo w dół
                {
                    int nextColumnIndex = columnIndex - 1;
                    int nextRowIndex = rowIndex + 1;

                    Field other = board.GetField(8 * nextRowIndex + nextColumnIndex);
                    if (other.PawnPlayerNumber == OtherPlayer)
                    {
                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                        while (nextRowIndex <= 7 && nextColumnIndex >= 0 && other.BelongsTo(OtherPlayer))
                        {
                            nextColumnIndex--;
                            nextRowIndex++;
                            if (nextRowIndex <= 7 && nextColumnIndex >= 0)
                            {
                                other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                            }

                        }
                        if (nextRowIndex <= 7 && nextColumnIndex >= 0) //brak miejsca, gdy -1
                        {
                            if (other.BelongsTo(CurrentMovingPlayer))
                            {
                                if(!test)
                                {
                                    nextColumnIndex++;
                                    nextRowIndex--;
                                    other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    while (other.BelongsTo(OtherPlayer))
                                    {
                                        other.PawnPlayerNumber = CurrentMovingPlayer;
                                        nextColumnIndex++;
                                        nextRowIndex--;
                                        other = board.GetField(nextRowIndex * 8 + nextColumnIndex);
                                    }
                                }

                                result = true;
                            }
                        }

                    }
                }
            }

            return result;
        }

        private void HandlePawnPlacing(int fieldIndex, Board board)
        {
            Field selectedField = board.GetField(fieldIndex);
            if (selectedField.Empty)
            {
                CurrentPlayerPlacePawn(fieldIndex, board);
               
                SwitchPlayer();
            }
        }
        private void CurrentPlayerPlacePawn(int fieldIndex, Board board)
        {
            PlayerPlacePawn(fieldIndex, CurrentMovingPlayer, board);
        }

        private void PlayerPlacePawn(int fieldIndex, PlayerNumber playerNumber, Board board)
        {
            board.Fields[fieldIndex].PawnPlayerNumber = playerNumber;
            //IncreaseMovesMade();
        }
        private void CheckGameStateChanged()
        {
            RecalculateWinningPlayer();
        }
        private void RecalculateWinningPlayer()
        {
            //if(CurrentMovingPlayer == PlayerNumber.FirstPlayer && GetAllPossibleMoves(PlayerNumber.FirstPlayer, CurrentBoard).Count == 0)
            //{
            //    WinningPlayer = PlayerNumber.SecondPlayer;
            //}
            //else if (CurrentMovingPlayer == PlayerNumber.SecondPlayer && GetAllPossibleMoves(PlayerNumber.SecondPlayer, CurrentBoard).Count == 0)
            //{
            //    WinningPlayer = PlayerNumber.FirstPlayer;
            //}

        }
        private List<Move> GetAllPossibleMoves(PlayerNumber playerNumber, Board board)
        {
            List<Move> allMoves = new List<Move>();
            List<Field> playersFields = board.GetPlayerFields(playerNumber);
        
            foreach (var playersField in playersFields)
            {
                List<Field> toFields = GetPossibleNewFields(playersField, board);
                foreach (var toField in toFields)
                {
                    allMoves.Add(new Move(toField.FieldIndex));
                }
            }
            return allMoves;
        }
        private List<Field> GetPossibleNewFields(Field playersField, Board board)
        {
            List<Field> possibleFields = new List<Field>();
            for(int i = 0; i < possibleMoveIndices.Length; i++)
            {
                Field toField = board.GetField(i);
                if (playersField.CanMoveTo(toField))
                {
                    possibleFields.Add(board.GetField(i));
                }
            }
            return possibleFields;
        }
        public HashSet<int> GetCurrentPlayerPossibleMoveIndices()
        {
            if (LastSelectedField == null)
            {
                return null;
            }
            List<Field> fields;
            fields = GetPossibleNewFields(LastSelectedField, CurrentBoard);
            HashSet<int> indices = new HashSet<int>();
            foreach (var field in fields)
            {
                indices.Add(field.FieldIndex);
            }
            return indices;
        }
        public bool GameFinished
        {
            get
            {
                return WinningPlayer != PlayerNumber.None;
            }
        }
        static GameState()
        {
          
            InitializePossibleMoveIndices();
            //InitializeFieldNames();
            InitializePlayerNames();
        }

        private static void InitializePossibleMoveIndices()
        {
            possibleMoveIndices = new int[64];
            for(int i = 0; i < possibleMoveIndices.Length; i++)
            {
                possibleMoveIndices[i] = i;
            }
        }

        private static void InitializePlayerNames()
        {
            playerNames = new Dictionary<PlayerNumber, string>();
            playerNames[PlayerNumber.FirstPlayer] = "White";
            playerNames[PlayerNumber.SecondPlayer] = "Black";
        }
        public void UpdateScore(Board board)
        {
            firstPlayerScore = 0;
            secondPlayerScore = 0;
            for(int i = 0; i < board.Fields.Count; i++)
            {
                if(board.Fields[i].PawnPlayerNumber == PlayerNumber.FirstPlayer)
                {
                    firstPlayerScore++;
                }
                else if(board.Fields[i].PawnPlayerNumber == PlayerNumber.SecondPlayer)
                {
                    secondPlayerScore++;
                }
            }
        }

    }
}
