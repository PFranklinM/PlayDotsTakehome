# PlayDotsTakehome

This is a recreation of Playdots classic mobile game dots. Players connect like colored dots to clear them off the board.

There are three major scripts which run this game:

1) IndividualDot.cs:
	This script is attached to the dot prefab. It stores relevant information such as color and position and calls the mouse events that drive gameplay.

2) GameBoardManager.cs:
	This script manages the game board. Clearing and repopulating the board as well as assigning colors to dots. We use object pooling for the dots to avoid repeated destroy/instantiate calls.

3) DotConnectionManager.cs:
	This script deals with dots being connected and disconnected. It contains the logic for different dot interactions and how the score graphic should respond to them.