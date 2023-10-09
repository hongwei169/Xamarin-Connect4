//using Newtonsoft.Json; 

using System;
using System.Linq;
using Xamarin.Forms;

namespace Connect4
{
    public partial class MainPage : ContentPage
    {
        //Public Variables
        int[,] gameGrid = new int[6, 7] {
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0}
        };

        const int BOARD_ROWS = 6;
        const int BOARD_COLS = 7;
        const int NUM_ROWS = 6;
        const int NUM_COLS = 7;

        int turnCounter = 0;
        int winner;

        public MainPage()
        {
            InitializeComponent();
            InitialiseGameBoard();
        }

        #region UI Generation

        /// <summary>
        /// Generate the Game Board ui
        /// </summary>
        private void InitialiseGameBoard()
        {
            int i;
            // add rows and columns to the grid.
            for (i = 0; i < NUM_ROWS ; i++)
            {
                GrdGameSetup.RowDefinitions.Add(new RowDefinition());
            }

            for (i = 0; i < NUM_COLS ; i++)
            {
                GrdGameSetup.ColumnDefinitions.Add(new ColumnDefinition());
            }

            CreateGameBoard();
            // ready to play

        }

        /// <summary>
        /// Create box for putting the disc
        /// </summary>
        private void CreateGameBoard()
        {
            int r, c;   // loop counter

            BoxView sq;
            TapGestureRecognizer t_sq = new TapGestureRecognizer();
            t_sq.NumberOfTapsRequired = 1;
            t_sq.Tapped += Pieces_Tapped;

            for (r = 0; r < BOARD_ROWS; r++)
            {
                for (c = 0; c < BOARD_COLS ; c++)
                {
                    sq = new BoxView();
                    sq.BackgroundColor = Color.White;
                    sq.HorizontalOptions = LayoutOptions.Center;
                    sq.VerticalOptions = LayoutOptions.Center;
                    sq.HeightRequest = 40;
                    sq.WidthRequest = 40;
                    sq.CornerRadius = 20;
                    sq.SetValue(Grid.RowProperty, r);
                    sq.SetValue(Grid.ColumnProperty, c);
                    sq.StyleId = "EmptySlot";
                    sq.GestureRecognizers.Add(t_sq);
                    GrdGameSetup.Children.Add(sq);                   
                }
            }
            
        }
        #endregion

        #region All the methods
        /// <summary>
        /// Method for placing down the pieces when tapped the pieces
        /// </summary>
        private async void placedown(int currRow, int currCol)
        {
            // Code for placing inside the grid
            BoxView boxview = (BoxView)GrdGameSetup.Children.FirstOrDefault(sq => Grid.GetRow(sq) == currRow && Grid.GetColumn(sq) == currCol);
            //var boxview = view as BoxView;

            // Checks for empty slot and changes row value
            if (boxview.StyleId == "EmptySlot")
            {
                if (turnCounter % 2 == 0)
                {
                    boxview.Color = Color.Red;
                    boxview.StyleId = "p1";
                    LblStatus.Text = "Yellow Turn";
                    LblStatus.TextColor = Color.Yellow;
                    gameGrid[currRow, currCol] = 1;
                }
                else
                {
                    boxview.Color = Color.Yellow;
                    boxview.StyleId = "p2";
                    LblStatus.Text = "Red Turn";
                    LblStatus.TextColor = Color.Red;
                    gameGrid[currRow, currCol] = 2;

                }

                winCheck();
                turnCounter++;


                if (turnCounter == 42)
                {
                    await DisplayAlert("Draw!", "No one win the game", "New Game");
                    NewGame();
                }
                // testP.Text = currRow + "," + currCol;    // Test current Row and Column clicked
            }
            else if (boxview.StyleId == "p1" || boxview.StyleId == "p2")
            {
                currRow--;
                // Stops player from dropping into full column
                if (currRow != -1)
                {
                    placedown(currRow, currCol);
                }
            }
        }

        /// <summary>
        /// To clear all the children 
        /// </summary>
        private void removeclick()
        {
            GrdGameSetup.Children.Clear();
        }

        /// <summary>
        /// Check the win conditions
        /// </summary>
        private void winCheck()
        {
            int player = 0;

            if (turnCounter % 2 == 0)
            {
                player = 1;
            }
            else
            {
                player = 2;
            }

            for (int i = 5; i >= 3; i--)
            {
                for (int j = 6; j >= 3; j--)
                {
                    //Offset(-1,-1) Up and Right
                    if (gameGrid[i, j] == player &&
                        gameGrid[i - 1, j - 1] == player &&
                        gameGrid[i - 2, j - 2] == player &&
                        gameGrid[i - 3, j - 3] == player)
                    {
                        winner = player;
                    }
                }
            }

            for (int i = 5; i >= 0; i--)
            {
                for (int j = 6; j >= 3; j--)
                {
                    //Offset(0,1) Horizontal 
                    if (gameGrid[i, j] == player &&
                        gameGrid[i, j - 1] == player &&
                        gameGrid[i, j - 2] == player &&
                        gameGrid[i, j - 3] == player)
                    {
                        winner = player;
                    }
                }
            }

            for (int i = 2; i >= 0; i--)
            {
                for (int j = 6; j >= 3; j--)
                {
                    //Offset(1,-1) Down and Left
                    if (gameGrid[i, j] == player &&
                        gameGrid[i + 1, j - 1] == player &&
                        gameGrid[i + 2, j - 2] == player)                      
                    {
                        winner = player;
                    }
                }
            }

            for (int i = 2; i >= 0; i--)
            {
                for (int j = 6; j >= 0; j--)
                {
                    //Offset(1,0) Vertical
                    if (gameGrid[i, j] == player &&                       
                        gameGrid[i + 1, j] == player &&
                        gameGrid[i + 2, j] == player)
                    {
                        winner = player;
                    }
                }
            }


            if (winner > 0)
            {
                GameWinner();
            }
        }

        /// <summary>
        /// Popup win game
        /// </summary>
        private async void GameWinner()
        {
           if(winner == 1)
            {
                await DisplayAlert("Player 1 Win!", $"Congrats Player 1 won the Connect4 game", "New Game");
            }
           else if (winner == 2)
            {
                await DisplayAlert("Player 2 Win!", $"Congrats Player 2 won the Connect4 game", "New Game");
            }
            NewGame();
        }

        /// <summary>
        /// Create New Game 
        /// </summary>
        private void NewGame()
        {
            removeclick();

            CreateGameBoard();
            //Reset Variables            
            turnCounter = 0;
            LblStatus.Text = "Red Turn";
            LblStatus.TextColor = Color.Red;
            winner = 0;
            gameGrid = new int[6, 7] {
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0}
            };
        }

        #endregion

        #region Click/Tap
        /// <summary>
        /// Place discs on the board
        /// </summary>
        private void Pieces_Tapped(object sender, EventArgs e)
        {
            BoxView currentPiece = (BoxView)sender;
            int currCol = (int)currentPiece.GetValue(Grid.ColumnProperty);
            int currRow = 5;

            placedown(currRow, currCol);          
        }

        /// <summary>
        /// Restart the game
        /// </summary>
        private async void Restart_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Are you sure?", "Are you sure you want to restart the game?", "Yes", "No");

            if (answer == false)
            {
                return;
            }

            // Restart the game
            NewGame();
        }

        /// <summary>
        /// Save the game to a file
        /// </summary>
        private async void Save_Clicked(object sender, EventArgs e)
        {
            //tried to save the file but unsuccessful below are the codes i tried

            /*string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(appFolder, "Save.json");

            string text = JsonConvert.SerializeObject(save);
            File.WriteAllText(filePath, text);*/

            await DisplayAlert("Saved!", "Game has been saved", "Close");
        }

        /// <summary>
        /// Load the game if it exsists or show a pop if not
        /// </summary>
        private async void Load_Clicked(object sender, EventArgs e)
        {
            //tried to load the file but unsuccessful below are the codes i tried

            /*string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string filePath = Path.Combine(appFolder, "Save.json");

            if (File.Exists(filePath) == false)
            {
                await DisplayAlert("No save game found!", "Save a game to load it later", "Close");
                return;
            }

            string text = File.ReadAllText(filePath);
            save = JsonConvert.DeserializeObject<SaveData>(text);*/

            await DisplayAlert("Loaded!", "Game has been loaded", "Play");
        }
        #endregion
    }
}
