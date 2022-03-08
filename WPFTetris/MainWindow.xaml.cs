using System;
using System.Collections.Generic;
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

namespace WPFTetris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Array of tile image assets. The order is significant. 0 = empty. The rest correspond to the tile IDs
        private readonly ImageSource[] tileImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/TileEmpty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileCyan.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileOrange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileYellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileGreen.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TilePurple.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/TileRed.png", UriKind.Relative))
        };

        // Array of block image assets.  The order is significant. 0 = empty. The rest correspond to the tile IDs
        private readonly ImageSource[] blockImages = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Block-Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-I.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-J.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-L.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-O.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-S.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-T.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Block-Z.png", UriKind.Relative))
        };

        // Image controls
        private readonly Image[,] imageControls;

        // max and min block speeds (in terms of delays in ms)
        // EXPERIMENT WITH THESE AT YOUR OWN RISK
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 100;
        // increments in speed (decrements in delay) in steps of 25
        private readonly int delayDecrease = 25;

        // Game state var
        private GameState gameState = new GameState();

        public MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(gameState.gameGrid);
        }

        // initialises the canvas
        private Image[,] SetupGameCanvas(GameGrid grid)
        {
            // create a grid of images
            Image[,] imageControls = new Image[grid.rows, grid.cols];
            // 25 px per cell
            int cellSize = 25;

            // for each row
            for (int r = 0; r < grid.rows; ++r)
            {
                // for each column
                for (int c = 0; c < grid.cols; ++c)
                {
                    // initialise a new image with the dimensions of the cell
                    Image imageControl = new Image
                    {
                        Width = cellSize, Height = cellSize
                    };

                    // offsets
                    // distance from the top of the canvas to the top of the image. (r - 2) hides the top two rows, which are used for spawning. The + 10 offset allows the player to see which block made them lose by giving a peek into the lower of the two hidden rows.
                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 10);
                    // distance from the left of the canvas to the left of the image
                    Canvas.SetLeft(imageControl, c * cellSize);

                    // make the image control a child of the canvas
                    GameCanvas.Children.Add(imageControl);
                    // and store it in the array
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        // draws the game grid
        private void drawGrid(GameGrid grid)
        {
            // for each row
            for (int r = 0; r < grid.rows; ++r)
            {
                // for each column
                for (int c = 0; c < grid.cols; ++c)
                {
                    // get the ID for the cell
                    int id = grid[r, c];
                    // reset the opacity of the grid (see the ghost image logic)
                    imageControls[r, c].Opacity = 1.0;
                    // and set its image accordingly
                    imageControls[r, c].Source = tileImages[id];
                }
            }
        }

        // draws a block
        private void drawBlock(Block block)
        {
            // for each position in the block
            foreach (Position p in block.tilePositions())
            {
                // draw fully opaque blocks (see ghost image logic)
                imageControls[p.row, p.col].Opacity = 1.0;
                // set the corresponding tile image
                imageControls[p.row, p.col].Source = tileImages[block.id];
            }
        }

        // selects the image for the next block
        private void drawNextBlock(BlockQueue blockQueue)
        {
            // get the next block from the queue
            Block next = blockQueue.nextBlock;
            // set the corresponding image as the source for NextImage (see XAML)
            NextImage.Source = blockImages[next.id];
        }

        private void drawHeldBlock(Block heldBlock)
        {
            // if not holding anything
            if (heldBlock == null)
            {
                // empty image
                HoldImage.Source = blockImages[0];
            }
            // if holding a block
            else
            {
                // set the corresponding image as the source for Hold Image
                HoldImage.Source = blockImages[heldBlock.id];
            }
        }

        // draws where the current block will land
        private void drawGhostBlock(Block block)
        {
            // get the drop distance (for calculating the coordinates of the ghost image)
            int dropDistance = gameState.blockDropDistance();

            // for each tile in the block
            foreach (Position p in block.tilePositions())
            {
                // set the opacity of the tile where the block would fit (if not moved) to 0.25
                imageControls[p.row + dropDistance, p.col].Opacity = 0.25;
                // and display the corresponding image there
                imageControls[p.row + dropDistance, p.col].Source = tileImages[block.id];
            }
        }

        // draws the grid and the current block
        private void draw(GameState gameState)
        {
            drawGrid(gameState.gameGrid);
            drawGhostBlock(gameState.CurrentBlock);
            drawBlock(gameState.CurrentBlock);
            drawNextBlock(gameState.blockQueue);
            ScoreText.Text = $"Score: {gameState.score}";
            drawHeldBlock(gameState.heldBlock);
        }

        // The game loop
        // async: wait without blocking the UI
        private async Task gameLoop()
        {
            draw(gameState);

            // as long as the game is not over
            while (!gameState.gameOver)
            {
                // set a delay as the larger of the minimum value and the maximum value minus the score times the step
                int delay = Math.Max(minDelay, maxDelay - (gameState.score * delayDecrease));
                // wait delay ms
                await Task.Delay(delay);
                // move the block down
                gameState.moveBlockDown();
                // draw the updated state
                draw(gameState);
            }

            // because we only break out of the loop when the game is over
            GameOverMenu.Visibility = Visibility.Visible;
            FinalScoreText.Text = $"Score: {gameState.score}";
        }

        // Key down event handler for the window
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.gameOver)
            {
                return;
            }

            /*
             * CONTROLS
             * A / Left : Move Left
             * D / Right: Move Right
             * S / Down : Move Down
             * Q / Z    : Rotate CCW
             * E / X    : Rotate CW
             * C / Up   : Hold Block
             * Spacebar : Drop Block
             */
            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                    gameState.moveBlockLeft();
                    break;
                case Key.D:
                case Key.Right:
                    gameState.moveBlockRight();
                    break;
                case Key.S:
                case Key.Down:
                    gameState.moveBlockDown();
                    break;
                case Key.Q:
                case Key.Z:
                    gameState.rotateBlockCCW();
                    break;
                case Key.E:
                case Key.X:
                    gameState.rotateBlockCW();
                    break;
                case Key.Up:
                case Key.H:
                    gameState.holdBlock();
                    break;
                case Key.Space:
                    gameState.dropBlock();
                    break;
                // default case to ensure we only redraw when the player presses a valid key
                default:
                    return;
            }

            draw(gameState);
        }

        // Load event for the canvas
        // async: to enable await (= suspending evaluation of the parent method until the awaited function returns)
        private async void GameCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            await gameLoop();
        }

        // Click event for the play again button on the game over menu
        private async void PlayAgain_Click(object sender, RoutedEventArgs e)
        {
            // reset the game state
            gameState = new GameState();
            // hide the game over popup
            GameOverMenu.Visibility=Visibility.Hidden;
            // start the game loop again
            await gameLoop();
        }
    }
}
