using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
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
    public sealed partial class MainWindow : Window
    {
        //Game board constraint constants
        private uint MIN_ROWS = Board.MIN_ROWS;
        private uint MAX_ROWS = Board.MAX_ROWS;
        private uint MIN_COLS = Board.MIN_COLS;
        private uint MAX_COLS = Board.MAX_COLS;

        //Window size constants
        private int WIDTH = 650;
        private int HEIGHT = 130;

        //Window setting variables
        private OverlappedPresenter _presenter;

        public MainWindow()
        {
            //Set the size of window taking into account DPI (https://stackoverflow.com/questions/67169712/winui-3-0-reunion-0-5-window-size)
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            var appWindow = AppWindow.GetFromWindowId(windowId);
            double dpi = (double)User32.GetDpiForWindow(hWnd);
            double scaling = dpi / 96;
            appWindow.Resize(new Windows.Graphics.SizeInt32 { Width = (int)(WIDTH * scaling), Height = (int)(HEIGHT * scaling) });

            //Fix the size of the window (https://github.com/microsoft/WindowsAppSDK/discussions/1694)
            _presenter = appWindow.Presenter as OverlappedPresenter;
            _presenter.IsResizable = false;
            _presenter.IsMaximizable = false;

            //Set the title of app
            Title = "Minesweeper Classic";

            this.InitializeComponent();
        }

        //Event handlers----------------------------------------------------------------------------------------------------------
        private void RowCount_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            //If the given row count is too small or not given at all, make the value its minimum
            if (RowCount.Value < MIN_ROWS ||  double.IsNaN(RowCount.Value))
            {
                RowCount.Value = MIN_ROWS;
            }
            
            //If the given row count is too big, make the value its maximum
            else if (RowCount.Value > MAX_ROWS)
            {
                RowCount.Value = MAX_ROWS;
            }

            //If the row is in the correct range, ensure it is an integer
            else
            {
                RowCount.Value = Math.Floor(RowCount.Value);
            }
        }

        private void ColCount_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            //If the given column count is too small or not given at all, make the value its minimum
            if (ColCount.Value < MIN_COLS || double.IsNaN(ColCount.Value))
            {
                ColCount.Value = MIN_COLS;
            }

            //If the given column count is too big, make the value its maximum
            else if (ColCount.Value > MAX_COLS)
            {
                ColCount.Value = MAX_COLS;
            }

            //If the column is in the correct range, ensure it is an integer
            else
            {
                ColCount.Value = Math.Floor(ColCount.Value);
            }
        }

        private void MineCount_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            EnsureProperMineCount();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            //Get rows and column numbers
            int rowCount = (int)RowCount.Value;
            int colCount = (int)ColCount.Value;
            int mineCount = (int)MineCount.Value;

            //Ensure that there is correct number of mines when game starts
            EnsureProperMineCount();

            //Pass the game board info into the game window and launch it
            GameWindow gameWindowInstance = new GameWindow(rowCount, colCount, mineCount);
            gameWindowInstance.Activate();

        }

        //Helpers-----------------------------------------------------------------------------------------------------------------
        private void EnsureProperMineCount()
        {
            //Calculate the total number of squares in the grid
            int totalSquares = (int)(RowCount.Value * ColCount.Value);

            //Ensure that there is at least one mine 
            if (MineCount.Value < 1 || double.IsNaN(MineCount.Value))
            {
                MineCount.Value = 1;
            }

            //Ensure that there is at least one free space
            else if (MineCount.Value >= totalSquares)
            {
                MineCount.Value = totalSquares - 1;
            }
        }
    }
}
