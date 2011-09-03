BattleShip Challenge!
=====================

Intro
-----

This game is an automated game of BattleShip (sänka skepp) which pits
two 3rd party player AI's against each other.

Game usage
----------
Here are the command line switches that you can use:
-i         - Runs the game with interactive pauses after the ship setup
             phase and after the game finishes. Press any key to continue.
-r <nr>    - Runs <nr> amount of matches in a row.
-t         - Prints every players turn. (Overrides config setting)
--noturns  - Mutes the output of player turns. (Overrides config setting)

The configuration allows you to enable/disable the print out of all
player shots. Also it's in the configuration you specify your own 3rd
party AI.

Implementing your own AI
------------------------

To create your own super smart AI you only need 1 assembly, BattleShip.Interface.
In it are all the interfaces and small classes you need to create an AI.
Just follow these simple steps to get going:
  1. Start Visual Studio.
  2. Create a new Class Library project.
  3. Copy BattleShip.Interface.dll to the project.
  4. Add the dll as a reference.
  5. Create a class that implements the IPlayer interface. Tip:
     Write the class definition like this: public class MyAI : IPlayer
     and then have visual studio implement the interface for you by hovering
     the mouse over IPlayer and choosing that little popup thing.
     (Choose to implement implicitly, explicit implementation is messy)
  6. Implement all methods so that they do something smart!! :D

When you think you are ready you can simply copy the dll you get when you build
to the game location and edit the configuration for player1 or player2 (or both!)
and then run the game.

Good luck :D

Game Rules
----------

The game is basically the classic battleship game. The grid you play on is 10x10
in size. And the coordinate system starts at 1 and goes to 10. Here is a small
ascii table showing how the game views the board:

10
 9
 8
 7
 6
 5
 4
 3
 2
 1
   1  2  3  4  5  6  7  8  9 10

As you can see the game starts the origo at the bottom left and has the Y axis
going up and the X axis going to the right. This is important as you place your
ships.

- Ship Placement -

To place ships you use the Placement class, which basically contains a coordinate
and an orientation. To understand how that works look at the following example:

10   Coordinate ->X->Horizontal
 9                |
 8                v
 7               Ver
 6               tic
 5               al
 4
 3
 2
 1
   1  2  3  4  5  6  7  8  9 10

The coordinate specifies the, let's say, bow of the ship and the orientation specifies
in which direction the stern is from that point.

 * Horizontal means the stern goes to the right (or X+)
 * Vertical means the stern goes down (or Y-)

Obviously the ship length decides how far away the stern is from the bow.

- Placement Rules -

These are rules that must be followed when placing ships:
 * All parts of the ship must be within the 10x10 board.
   E.g. An invalid placement is to place a size 3 ship at (9,2) horizontal.
 * A ship must be separated from any other ship by at least 1 square.
   E.g. An invalid placement would be one ship at (3,4) vertical and one ship at (3,5)
   horizontal.