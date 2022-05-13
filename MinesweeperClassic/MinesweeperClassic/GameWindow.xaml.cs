using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using MinesweeperLibrary;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MinesweeperClassic
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameWindow : Window
    {
        //Constants
        public static int GRID_SQUARE_LENGTH { get; private set; } = 16;
        public static int RESET_SQUARE_LENGTH { get; private set; } = 24;

        //Window properties
        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

        //Utility variables for window
        public Dictionary<string, BitmapImage> ResetImageMap { get; private set; }
        public Dictionary<int, BitmapImage> GridImageMap { get; private set; }
        public bool LeftClickHeld { get; private set; } = false;
        public bool RightClickHeld { get; private set; } = false;
        public bool MiddleClickHeld { get; private set; } = false;
        public int SelectedGridCellIndex { get; private set; } = -1;

        //Board backend object
        private Board BoardObject;

        //Window setting variables
        private OverlappedPresenter _presenter;

        //Constructor for the game window
        public GameWindow(int rows, int columns, int mines)
        {
            //Start up game
            BoardObject = new Board(rows, columns, mines);

            //Preload all images into dictionary, so the program doesn't constantly have to access the files
            PreloadImages();

            //Setup window width (accomodating for grid squares and border where each side will be half a grid square length)
            int gridWidth = GRID_SQUARE_LENGTH * columns;
            int windowLeftRightBorderWidth = 2 * GRID_SQUARE_LENGTH;
            WindowWidth = gridWidth + windowLeftRightBorderWidth;

            //Setup window height (grid squares, reset button panel and bottom border)
            int gridHeight = GRID_SQUARE_LENGTH * rows;
            int resetPanelHeight = RESET_SQUARE_LENGTH + GRID_SQUARE_LENGTH;
            int bottomBorderHeight = 3 * GRID_SQUARE_LENGTH;
            WindowHeight = gridHeight + resetPanelHeight + bottomBorderHeight;

            //Set the size of window taking into account DPI (https://stackoverflow.com/questions/67169712/winui-3-0-reunion-0-5-window-size)
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            double dpi = (double)User32.GetDpiForWindow(hWnd);
            double scaling = dpi / 96;
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)(WindowWidth * scaling), Height = (int)(WindowHeight * scaling) });

            //Fix the size of the window (https://github.com/microsoft/WindowsAppSDK/discussions/1694)
            _presenter = appWindow.Presenter as OverlappedPresenter;
            _presenter.IsResizable = false;
            _presenter.IsMaximizable = false;
            _presenter.IsMinimizable = false;

            //Set the title of app
            Title = "Minesweeper Classic";

            this.InitializeComponent();

            //Setup size of main UI stack
            MainUIStack.Height = WindowHeight;
            MainUIStack.Width = WindowWidth;

            //Setup size of reset panel
            ResetPanelStack.Height = resetPanelHeight;
            ResetPanelStack.Width = WindowWidth;

            //Setup ResetIcon
            ResetIcon.Width = RESET_SQUARE_LENGTH;
            ResetIcon.Height = RESET_SQUARE_LENGTH;
            ResetIcon.Source = ResetImageMap["smile"];
            ResetIcon.Margin = new Thickness(0, GRID_SQUARE_LENGTH / 2, 0, GRID_SQUARE_LENGTH / 2);

            //Setup the Game Board canvas to proper sizes and margins
            GameBoardCanvas.Width = gridWidth;
            GameBoardCanvas.Height = gridHeight;
            GameBoardCanvas.Margin = new Thickness(GRID_SQUARE_LENGTH / 2, 0, GRID_SQUARE_LENGTH / 2, 0);

            //Setup the Game Board's border
            GameBoardBorder.Width = gridWidth+1;
            GameBoardBorder.Height = gridHeight+2;

            //Add grid cells as children to the canvas
            for (int cellIndex = 0; cellIndex < rows * columns; cellIndex++)
            {
                //Create new image to put into the canvas 
                Image currentGridCell = new Image();
                currentGridCell.Width = GRID_SQUARE_LENGTH;
                currentGridCell.Height = GRID_SQUARE_LENGTH;
                currentGridCell.Source = GridImageMap[12];
                currentGridCell.Margin = new Thickness(0, 0, 0, 0);
                Canvas.SetLeft(currentGridCell, (cellIndex % columns) * GRID_SQUARE_LENGTH);
                Canvas.SetTop(currentGridCell, (cellIndex / columns) * GRID_SQUARE_LENGTH);

                //Add it to canvas
                GameBoardCanvas.Children.Add(currentGridCell);
            }
        }

        private void PreloadImages()
        {
            //The map for the Reset Button Icon
            ResetImageMap = new Dictionary<string, BitmapImage>();
            ResetImageMap.Add("dead", new BitmapImage(new Uri("ms-appx:///Images/Reset/dead.bmp")));
            ResetImageMap.Add("oFace", new BitmapImage(new Uri("ms-appx:///Images/Reset/oFace.bmp")));
            ResetImageMap.Add("smile", new BitmapImage(new Uri("ms-appx:///Images/Reset/smile.bmp")));
            ResetImageMap.Add("smileClicked", new BitmapImage(new Uri("ms-appx:///Images/Reset/smileClicked.bmp")));
            ResetImageMap.Add("sunglasses", new BitmapImage(new Uri("ms-appx:///Images/Reset/sunglasses.bmp")));

            //The map that takes the library codes for each grid square and maps them to an icon
            GridImageMap = new Dictionary<int, BitmapImage>();
            GridImageMap.Add(0, new BitmapImage(new Uri("ms-appx:///Images/Grid/highlight.bmp")));
            GridImageMap.Add(1, new BitmapImage(new Uri("ms-appx:///Images/Grid/1.bmp")));
            GridImageMap.Add(2, new BitmapImage(new Uri("ms-appx:///Images/Grid/2.bmp")));
            GridImageMap.Add(3, new BitmapImage(new Uri("ms-appx:///Images/Grid/3.bmp")));
            GridImageMap.Add(4, new BitmapImage(new Uri("ms-appx:///Images/Grid/4.bmp")));
            GridImageMap.Add(5, new BitmapImage(new Uri("ms-appx:///Images/Grid/5.bmp")));
            GridImageMap.Add(6, new BitmapImage(new Uri("ms-appx:///Images/Grid/6.bmp")));
            GridImageMap.Add(7, new BitmapImage(new Uri("ms-appx:///Images/Grid/7.bmp")));
            GridImageMap.Add(8, new BitmapImage(new Uri("ms-appx:///Images/Grid/8.bmp")));
            GridImageMap.Add(9, new BitmapImage(new Uri("ms-appx:///Images/Grid/flag.bmp")));
            GridImageMap.Add(10,new BitmapImage(new Uri("ms-appx:///Images/Grid/highlight.bmp")));
            GridImageMap.Add(11,new BitmapImage(new Uri("ms-appx:///Images/Grid/mine.bmp")));
            GridImageMap.Add(12,new BitmapImage(new Uri("ms-appx:///Images/Grid/cell.bmp")));
        }

        private void RepaintGameWindow()
        {
            //Update the reset button
            if(BoardObject.GameIsWon())
            {
                ResetIcon.Source = ResetImageMap["sunglasses"];
            }

            else if(BoardObject.GameIsLost())
            {
                ResetIcon.Source = ResetImageMap["dead"];
            }

            else
            {
                ResetIcon.Source = ResetImageMap["smile"];
            }

            //Update grid
            for (int rowIndex = 0; rowIndex < BoardObject.NumberOfRows; rowIndex++)
            {
                for (int colIndex = 0; colIndex < BoardObject.NumberOfColumns; colIndex++)
                {
                    ((Image)GameBoardCanvas.Children[(rowIndex * (int)(BoardObject.NumberOfRows)) + colIndex]).Source = GridImageMap[BoardObject.CellVisualStatus(rowIndex, colIndex)];
                }
            }
        }

        //Event handlers----------------------------------------------------------------------------------------------------------
        private void ResetIcon_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //Get the current point that caused the event
            PointerPoint eventPoint = e.GetCurrentPoint(ResetIcon);

            //Ensure it is the left mouse button and then change the reset button to a clicked version
            if(eventPoint.Properties.IsLeftButtonPressed)
            {
                ResetIcon.Source = ResetImageMap["smileClicked"];
                LeftClickHeld = true;
            }
        }

        private void ResetIcon_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //Ensure it is the left mouse button clicking the button, then reset the board and repaint it
            if (LeftClickHeld)
            {
                BoardObject.Reset();
                RepaintGameWindow();
                LeftClickHeld = false;
            }

        }

        private void ResetIcon_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            //Ensure that left click was already held, before putting the reset button back to normal
            if(LeftClickHeld)
            {
                ResetIcon.Source = ResetImageMap["smile"];
                LeftClickHeld = false;
            }
        }

        //https://www.youtube.com/watch?v=8eTWq27h4sY
        private void GameBoardCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //Don't do anything if someone tries to click the grid when the game is no longer running
            if(!BoardObject.GameStillRunning())
            {
                return;
            }

            //Get the current point that caused the event
            PointerPoint eventPoint = e.GetCurrentPoint(ResetIcon);

            //Get the index of the currently pressed square so it can be used by the release/cancel events
            SelectedGridCellIndex = GameBoardCanvas.Children.IndexOf((Image)e.OriginalSource);

            //Get coordinates of current cell
            int row = SelectedGridCellIndex / (int)BoardObject.NumberOfColumns;
            int col = SelectedGridCellIndex % (int)BoardObject.NumberOfColumns;

            //Handle the left click holds
            if (eventPoint.Properties.IsLeftButtonPressed)
            { 
                //Ensure that the cell to be clicked isn't already clicked in some way
                if(BoardObject.CellVisualStatus(row, col) == 12)
                {
                    //Highlight the held square
                    ((Image)e.OriginalSource).Source = GridImageMap[10];

                    //Give the reset button the o face while holding down the square
                    ResetIcon.Source = ResetImageMap["oFace"];
                }

                LeftClickHeld = true;
            }

            //Handle the right click holds
            else if(eventPoint.Properties.IsRightButtonPressed)
            {
                RightClickHeld = true;
            }

            //Handle the middle click holds
            else if(eventPoint.Properties.IsMiddleButtonPressed)
            {
                //Hold down the board at this position
                BoardObject.BothClickHoldDown(row, col);

                //Repaint the board to highlight surrounding squares
                RepaintGameWindow();

                MiddleClickHeld = true;
            }
        }

        //Work on this next
        private void GameBoardCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            //Don't do anything if someone tries to click the grid when the game is no longer running
            if (!BoardObject.GameStillRunning())
            {
                return;
            }

            //If where the mouse is now does not match up where the mouse was when pressed, presume the click was cancelled
            if (SelectedGridCellIndex != GameBoardCanvas.Children.IndexOf((Image)e.OriginalSource))
            {
                GameBoardCanvas_PointerCanceled(sender, e);
                return;
            }

            //Get the current coordinates of the selected cell
            int row = SelectedGridCellIndex / (int)BoardObject.NumberOfColumns;
            int col = SelectedGridCellIndex % (int)BoardObject.NumberOfColumns;

            //Handle the left clicks
            if (LeftClickHeld)
            {
                BoardObject.LeftClick(row,col);
                LeftClickHeld = false;
            }

            //Handle the right clicks
            else if (RightClickHeld)
            {
                BoardObject.RightClick(row,col);
                RightClickHeld = false;
            }

            //Handle the middle clicks
            else if (MiddleClickHeld)
            {
                BoardObject.BothClickRelease(row,col); 
                MiddleClickHeld = false;
            }

            //Update and repaint the whole board
            RepaintGameWindow();
        }

        private void GameBoardCanvas_PointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            //Get the coordinate of the square whose click is cancelled
            int oldRow = SelectedGridCellIndex / (int)BoardObject.NumberOfColumns;
            int oldCol = SelectedGridCellIndex % (int)BoardObject.NumberOfColumns;

            //Don't do anything if someone tries to click the grid when the game is no longer running
            if (!BoardObject.GameStillRunning())
            {
                return;
            }

            //Cancel the left click
            if (LeftClickHeld)
            {
                //Reset the square that was clicked and subsequently cancelled
                ((Image)GameBoardCanvas.Children[SelectedGridCellIndex]).Source = GridImageMap[BoardObject.CellVisualStatus(oldRow, oldCol)];

                //Put the reset button icon back to smiles
                ResetIcon.Source = ResetImageMap["smile"];

                LeftClickHeld = false;
            }

            //Cancel the right click
            else if (RightClickHeld)
            {
                RightClickHeld = false;
            }

            //Cancel the middle click
            else if (MiddleClickHeld)
            {
                //Cancel the hold on the board and repaint it
                BoardObject.BothClickHoldDownCancel(oldRow, oldCol);
                RepaintGameWindow();

                MiddleClickHeld = false;
            }
        }
    }
}
