using System;
using System.Collections.Generic;
using ReversiBackend;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Reversi
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int HUMAN_DROPDOWN_NUMBER = 0;
        private static readonly int AI_DROPDOWN_NUMBER = 1;
        private static readonly int MIN_MAX_DROPDOWN_NUMBER = 0;
        private static readonly int ALPHA_BETA_DROPDOWN_NUMBER = 1;
        private static readonly int FAST_ALPHA_BETA_DROPDOWN_NUMBER = 2;


        private static Dictionary<int, Func<Heuristic>> heuristicDictionary;

        private PlayersController aiPlayersController = null;
        private GameEngine gameEngine = null;

        private float timePassed;
        private bool shouldLogToFile;
        private string firstPlayerType;
        private string secondPlayerType;
   
        private string numberOfMovesTemplateText = "Turns: {0}";
        private string timerTemplateText = "Time[s]: {0}";
        private string currentMovingPlayerTemplateText = "Turn: Player {0}";
        private string firstPlayerScoreTemplateText = "Score: {0}";
        private string secondPlayerScoreTemplateText = "Score: {0}";
        private string winningPlayerTextTemplate = "Won: ";

        private string[] playerTypes = new string[] { "Człowiek", "AI" };
        private string[] algorithmTypes = new string[] { "Min-Max", "Alfa-beta", "Alfa-Beta H" };
        private int[] depthPossibilities = new int[] { 1, 2, 3, 4, 5, 6 };
        private string[] heuristicTypes = new string[] { "Pionki", "Pionki+Młynki", "Pionki+Ruchy" };

        int firstPlayerScore;
        int secondPlayerScore;

        System.Windows.Threading.DispatcherTimer dispatcherTimer;

        Stopwatch startTimeStopwatch;

        private GameField[] gameFieldButtons = null;
        public MainWindow()
        {
            InitializeComponent();
            InitDropdowns();
            InitPawnButtonHandlers();
            startTimeStopwatch = new Stopwatch();
        }
        static MainWindow()
        {
            heuristicDictionary = new Dictionary<int, Func<Heuristic>>();
            heuristicDictionary[0] = () => new SimplePawnNumberHeuristic();
            //heuristicDictionary[1] = () => new PawnMillNumberHeuristic();
            //heuristicDictionary[2] = () => new PawnMoveNumberHeuristic();
        }
        private void StartGame()
        {
            gameEngine = new GameEngine();
            AIPlayer firstPlayer = InitPlayer(PlayerNumber.FirstPlayer);
            AIPlayer secondPlayer = InitPlayer(PlayerNumber.SecondPlayer);
            aiPlayersController = new PlayersController(firstPlayer, secondPlayer);
            timePassed = 0;
            OnBoardUpdated(gameEngine.GameState.CurrentBoard);
            gameEngine.OnBoardChanged += OnBoardUpdated;
            gameEngine.OnGameFinished += OnGameFinished;
            gameEngine.GameState.UpdateScore(gameEngine.GameState.CurrentBoard);
            gameEngine.OnPlayerTurnChanged += OnPlayerTurnChanged;
            UpdatePlayersScore(gameEngine.GameState.firstPlayerScore, gameEngine.GameState.secondPlayerScore);
            gameEngine.OnPlayerTurnChanged += aiPlayersController.OnPlayerTurnChanged;
            //UpdateWinningPlayerText(PlayerNumber.None);
            //shouldLogToFile = log_file_checkbox.IsChecked ?? false;
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateAiSteps);
            dispatcherTimer.Interval = new TimeSpan(1000);
            dispatcherTimer.Start();
            startTimeStopwatch.Start();
        }

        private void InitPawnButtonHandlers()
        {
            this.gameFieldButtons = new GameField[]
           {
               A1, A2, A3, A4, A5, A6, A7, A8,
               B1, B2, B3, B4, B5, B6, B7, B8,
               C1, C2, C3, C4, C5, C6, C7, C8,
               D1, D2, D3, D4, D5, D6, D7, D8,
               E1, E2, E3, E4, E5, E6, E7, E8,
               F1, F2, F3, F4, F5, F6, F7, F8,
               G1, G2, G3, G4, G5, G6, G7, G8,
               H1, H2, H3, H4, H5, H6, H7, H8
           };
            for (int i = 0; i < gameFieldButtons.Length; i++)
            {
                gameFieldButtons[i].Initialize(this, i);
            }

        }
        private AIPlayer InitPlayer(PlayerNumber playerNumber)
        {
            ComboBox playerDropdown;
            ComboBox algorithmDropdown;
            ComboBox heuristicDropdown;
            ComboBox searchDepthDropdown;
            if (playerNumber == PlayerNumber.FirstPlayer)
            {
                playerDropdown = first_player_type_dropdown;
                algorithmDropdown = first_player_algorithm_dropdown;
                heuristicDropdown = first_player_heuristic_dropdown;
                searchDepthDropdown = first_player_depth_dropdown;
            }
            else
            {
                playerDropdown = second_player_type_dropdown;
                algorithmDropdown = second_player_algorithm_dropdown;
                heuristicDropdown = second_player_heuristic_dropdown;
                searchDepthDropdown = second_player_depth_dropdown;
            }
            if (playerDropdown.SelectedIndex == AI_DROPDOWN_NUMBER)
            {
                Heuristic heuristic = heuristicDictionary[heuristicDropdown.SelectedIndex]();
                int searchDepth = searchDepthDropdown.SelectedIndex + 1;
                if (algorithmDropdown.SelectedIndex == MIN_MAX_DROPDOWN_NUMBER)
                {
                    //SavePlayerType(playerNumber, "Min-Max: " + heuristic.GetType().Name);
                    return new MinMaxAIPlayer(gameEngine, heuristic, playerNumber, searchDepth);
                }
                else if (algorithmDropdown.SelectedIndex == ALPHA_BETA_DROPDOWN_NUMBER)
                {
                    //SavePlayerType(playerNumber, "Alfa-Beta: " + heuristic.GetType().Name);
                    //return new AlphaBetaAIPlayer(gameEngine, heuristic, playerNumber, searchDepth);
                }
                else
                {
                    //SavePlayerType(playerNumber, "Alfa-Beta H: " + heuristic.GetType().Name);
                    //Heuristic sortHeuristic = new SimplePawnNumberHeuristic();
                    //return new FastAlphaBetaAiPlayer(gameEngine, heuristic, playerNumber, searchDepth, sortHeuristic);
                }
            }
            else
            {
                //SavePlayerType(playerNumber, "Człowiek");
            }
            return null;
        }
        private void InitDropdowns()
        {
            first_player_type_dropdown.ItemsSource = playerTypes;
            first_player_type_dropdown.SelectedIndex = 0;
            second_player_type_dropdown.ItemsSource = playerTypes;
            second_player_type_dropdown.SelectedIndex = 0;

            first_player_algorithm_dropdown.ItemsSource = algorithmTypes;
            first_player_algorithm_dropdown.SelectedIndex = 0;
            second_player_algorithm_dropdown.ItemsSource = algorithmTypes;
            second_player_algorithm_dropdown.SelectedIndex = 0;

            first_player_depth_dropdown.ItemsSource = depthPossibilities;
            first_player_depth_dropdown.SelectedIndex = 0;
            second_player_depth_dropdown.ItemsSource = depthPossibilities;
            second_player_depth_dropdown.SelectedIndex = 0;

            first_player_heuristic_dropdown.ItemsSource = heuristicTypes;
            first_player_heuristic_dropdown.SelectedIndex = 0;
            second_player_heuristic_dropdown.ItemsSource = heuristicTypes;
            second_player_heuristic_dropdown.SelectedIndex = 0;
        }

        private void SetAIDropdownsActive(int playerType, PlayerNumber playerNumber)
        {
            if (playerNumber == PlayerNumber.FirstPlayer)
            {
                if (playerType == HUMAN_DROPDOWN_NUMBER)
                {
                    first_player_algorithm_dropdown.IsEnabled = false;
                    first_player_heuristic_dropdown.IsEnabled = false;
                    first_player_depth_dropdown.IsEnabled = false;
                }
                else
                {
                    first_player_algorithm_dropdown.IsEnabled = true;
                    first_player_heuristic_dropdown.IsEnabled = true;
                    first_player_depth_dropdown.IsEnabled = true;
                }
            }
            else
            {
                if (playerType == HUMAN_DROPDOWN_NUMBER)
                {
                    second_player_algorithm_dropdown.IsEnabled = false;
                    second_player_heuristic_dropdown.IsEnabled = false;
                    second_player_depth_dropdown.IsEnabled = false;
                }
                else
                {
                    second_player_algorithm_dropdown.IsEnabled = true;
                    second_player_heuristic_dropdown.IsEnabled = true;
                    second_player_depth_dropdown.IsEnabled = true;
                }
            }
        }
        private void OnBoardUpdated(Board newBoard)
        {
            for (int i = 0; i < gameFieldButtons.Length; i++)
            {
                Field field = newBoard.GetField(i);
                gameFieldButtons[i].RefreshImage(field.PawnPlayerNumber);
            }
        }
        private void First_player_type_dropdown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetAIDropdownsActive(first_player_type_dropdown.SelectedIndex, PlayerNumber.FirstPlayer);
        }

        private void Second_player_type_dropdown_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SetAIDropdownsActive(second_player_type_dropdown.SelectedIndex, PlayerNumber.SecondPlayer);
        }
        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            StartGame();
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        public void HandleButtonClick(int fieldIndex)
        {
            if (gameEngine != null)
            {
                gameEngine.HandleSelection(fieldIndex);
            }
        }
        private void OnGameFinished(PlayerNumber winningPlayer)
        {
            //UpdateWinningPlayerText(winningPlayer);
            gameEngine.OnBoardChanged -= OnBoardUpdated;
            gameEngine.OnGameFinished -= OnGameFinished;
            gameEngine.OnPlayerTurnChanged -= OnPlayerTurnChanged;
            //gameEngine.OnPlayerTurnChanged -= aiPlayersController.OnPlayerTurnChanged;
            gameEngine = null;
            //aiPlayersController = null;
            //dispatcherTimer.Stop();
            //dispatcherTimer = null;
            //startTimeStopwatch.Stop();
        }
        private void OnPlayerTurnChanged(PlayerNumber currentMovingPlayerNumber)
        {
            UpdatePlayersScore(gameEngine.GameState.firstPlayerScore, gameEngine.GameState.secondPlayerScore);
            if (currentMovingPlayerNumber == PlayerNumber.FirstPlayer)
            {
                UpdateTurnText(1);
   
            }
            else
            {
                UpdateTurnText(2);
            }
        }

        private void UpdateTurnText(int playerNumber)
        {
            turn_label.Content = string.Format(currentMovingPlayerTemplateText, playerNumber);
        }

        private void UpdatePlayersScore(int firstPlayerScore, int secondPlayerScore)
        {
            string firstScore = firstPlayerScore.ToString();
            string secondScore = secondPlayerScore.ToString();
            player_one_score_label.Content = string.Format(firstPlayerScoreTemplateText, firstScore);
            player_two_score_label.Content = string.Format(secondPlayerScoreTemplateText, secondScore);
        }

        private void UpdateAiSteps(object sender, EventArgs e)
        {
            MakeAiControllerStep();
            UpdateGameStateData();
        }

        private void MakeAiControllerStep()
        {
            if (aiPlayersController != null)
            {
                long timeMilis = aiPlayersController.CheckStep();
            }
        }

        private void UpdateGameStateData()
        {
            if (gameEngine != null)
            {
                UpdateMoveNumberText();
                UpdateTime();
            }
        }

        private void UpdateTime()
        {
            if (!gameEngine.GameState.GameFinished)
            {
                timePassed = startTimeStopwatch.ElapsedMilliseconds / 1000f;
                time_label.Content = string.Format(timerTemplateText, Math.Truncate(timePassed * 100) / 100);
            }
        }

        private void UpdateMoveNumberText()
        {
            turns_number_label.Content = string.Format(numberOfMovesTemplateText, gameEngine.GameState.MovesMade);
        }
    }
} 
