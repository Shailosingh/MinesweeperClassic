# MinesweeperClassic
![Logo](https://user-images.githubusercontent.com/52577016/202794944-553b5046-b75f-44d6-9958-1abe698eba9b.png)

Enjoy a free remake of the old Minesweeper you know and love from Windows XP. This remake is free of ads and offers hours of fun.

## Controls
Left-Click: Uncover a square.

Right-Click: Flag a square.

Middle Click: If the cell you middle click has a number that is equal to the number of flags surrounding it, it will uncover all the remaining covered squares surrounding it.
If it doesn't have the correct number or is just a covered square, it will simply highlight the surrounding covered squares. While this control is optional, it is
essential for advanced strategies such as chording.

## Technologies
This application was written using C# and WinUI 3. The backend code for the logic of Minesweeper, is handled for me by my NuGET package 
[Minesweeper Library](https://github.com/Shailosingh/MinesweeperLibrary).
