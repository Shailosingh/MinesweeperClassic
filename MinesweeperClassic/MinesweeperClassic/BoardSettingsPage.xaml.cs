﻿using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MinesweeperLibrary;
using PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MinesweeperClassic
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BoardSettingsPage : Page
    {
        //Game board constraint constants
        private uint MIN_ROWS = Board.MIN_ROWS;
        private uint MAX_ROWS = Board.MAX_ROWS;
        private uint MIN_COLS = Board.MIN_COLS;
        private uint MAX_COLS = Board.MAX_COLS;
        private uint MAX_MINES = 999;

        //Window size constants
        private int WIDTH = 650;
        private int HEIGHT = 130;

        //Window setting variables
        private OverlappedPresenter _presenter;
        private MainWindow WindowObj;

        public BoardSettingsPage()
        {
            //Grab the Window instance variable
            WindowObj = MainWindow.CurrentInstance;

            //Set the size of window taking into account DPI (https://stackoverflow.com/questions/67169712/winui-3-0-reunion-0-5-window-size)
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(WindowObj);
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
            WindowObj.Title = "Minesweeper Classic";

            this.InitializeComponent();
        }

        //Event handlers----------------------------------------------------------------------------------------------------------
        private void RowCount_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            //If the given row count is too small or not given at all, make the value its minimum
            if (RowCount.Value < MIN_ROWS || double.IsNaN(RowCount.Value))
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
            MainWindow.Rows = (int)RowCount.Value;
            MainWindow.Cols = (int)ColCount.Value;

            //Ensure that there is correct number of mines when game starts
            EnsureProperMineCount();
            MainWindow.Mines = (int)MineCount.Value;

            //Get the frame controlling this page and navigate to the Game Page (https://learn.microsoft.com/en-us/windows/apps/design/basics/navigate-between-two-pages)
            this.Frame.Navigate(typeof(GamePage));
            
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

            //Ensure that there aren't more mines than the three digit display can support
            else if (MineCount.Value > MAX_MINES)
            {
                MineCount.Value = MAX_MINES;
            }
        }
    }
}
