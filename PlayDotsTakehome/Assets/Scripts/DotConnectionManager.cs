using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// This script deals with dots being connected and disconnected. It contains the logic for different dot interactions and how the score graphic should respond to them.

public class DotConnectionManager : MonoBehaviour
{
    // Static instance of this class.
    public static DotConnectionManager DotConnectionManagerInstance = null;

    /*
    The different line renderers in the game. The dot connector is the line that shows up on the game board when connecting dots,
    and the score graphics show up when new dots are connected or when a square is made. */
    public LineRenderer DotConnector, DotScoreGraphicTop, DotScoreGraphicBot, DotScoreGraphicLeft, DotScoreGraphicRight;

    // This float determines how much the score graphic grows when a new dot is connected. This felt right at 1.75.
    [SerializeField]
    private float dotScoreGraphicGrowAmount;
    // The length of the score graphic. This increases multiplicatively as new dots are connected.
    private float dotScoreGraphicXPos;
    // A serialized field for how big the score graphic is when a dot is initially selected.
    [SerializeField]
    private float dotScoreGraphicStartingValue;

    // This enum tracks which color is currently selected when we click on a dot.
    public enum SelectedDotColor { None, Blue, Green, Purple, Red, Yellow };
    public SelectedDotColor CurrentlySelectedDotColor;

    // Track whether a line is currently active or not.
    private bool lineActive = false;

    private Transform StartingDot, PreviousDot, TouchedDot;

    private void Awake()
    {
        // Singleton for this class. This manager is intended to be used in all levels.
        if(DotConnectionManagerInstance == null)
        {
            DotConnectionManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    // This is called when a dot is clicked to begin a line.
    public void StartLine(Transform ClickedDot)
    {
        // Keep track of the starting dot of the line as well as the previous dot for the next dot in the line.
        StartingDot = ClickedDot;
        PreviousDot = StartingDot;

        // Add the starting dot to the list of connected dots.
        GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(ClickedDot);


        // This sets the connection lines position and color equal to the starting dot.
        Vector3 Pos = new Vector3(ClickedDot.position.x,
            ClickedDot.position.y,
            -1);

        DotConnector.SetPosition(0, Pos);

        DotConnector.material = ClickedDot.GetComponent<Renderer>().material;

        // When no dot is selected, the line renderer has only 1 position so that it disappears. Here we increase the position count to be able to see the line.
        DotConnector.positionCount++;

        // We set the score graphic equal to it's starting value. I found that 0.1 felt right for this.
        dotScoreGraphicXPos = dotScoreGraphicStartingValue;

        DotScoreGraphicTop.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotScoreGraphicTop.material = ClickedDot.GetComponent<Renderer>().material;
        DotScoreGraphicBot.material = ClickedDot.GetComponent<Renderer>().material;

        // Start the coroutine which has the connecting line follow the mouse.
        StartCoroutine(MouseDragLineRenderer());
    }

    // This is called when we enter a new dot while drawing a line.
    public void AnotherDotEntered(Transform EnteredDot)
    {
        if (lineActive)
        {
            // Script references for the previous dot in the chain and the current one.
            IndividualDot PreviousDotScript = PreviousDot.GetComponent<IndividualDot>();
            IndividualDot EnteredDotScript = EnteredDot.GetComponent<IndividualDot>();

            // Check if the dots are adjacent. No diagonal matching.
            if (Mathf.Abs(PreviousDotScript.Coordinates.x - EnteredDotScript.Coordinates.x) == 1 && Mathf.Abs(PreviousDotScript.Coordinates.y - EnteredDotScript.Coordinates.y) == 0 ||
                       Mathf.Abs(PreviousDotScript.Coordinates.x - EnteredDotScript.Coordinates.x) == 0 && Mathf.Abs(PreviousDotScript.Coordinates.y - EnteredDotScript.Coordinates.y) == 1)
            {
                // Check if the dots are the same color.
                if ((int)EnteredDotScript.ThisDotsColor == (int)PreviousDotScript.ThisDotsColor)
                {
                    // If the new dot is a valid dot, we call a number of different actions for the new dot.
                    NewDotActions(EnteredDot);
                }
            }
        }
    }

    // This is called when a valid new dot is entered.
    private void NewDotActions(Transform Dot)
    {
        // If the dot we just entered is the starting dot and we have enough connected dots to form a square.
        if(Dot == StartingDot && GameBoardManager.GameBoardManagerInstance.ConnectedDots.Count > 2)
        {
            MadeASquare();
        }
        // else if the dot we are entering is already in the chain (used to disconnect dots).
        else if (GameBoardManager.GameBoardManagerInstance.ConnectedDots.Contains(Dot))
        {
            DotDisconnected(Dot);
        }
        // else we add the new dot to the chain.
        else
        {
            DotConnected(Dot);
        }
    }

    // This is called when we connect the dots in a square.
    private void MadeASquare()
    {
        // Add all dots of the chains color to the chain of connected dots.
        foreach(Transform Dot in GameBoardManager.GameBoardManagerInstance.GameBoardParent)
        {
            if ((int)Dot.GetComponent<IndividualDot>().ThisDotsColor == (int)CurrentlySelectedDotColor)
            {
                GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(Dot);
            }
        }

        // Set the flag to avoid the chosen color when respawning dots.
        // Since there is a none color option, we want to subtract 1 from the enum int to account for only colors.
        GameBoardManager.GameBoardManagerInstance.DotReassignmentAvoidColor((int)CurrentlySelectedDotColor - 1);

        // Trigger the square shapped score graphics in celebration.
        DotScoreGraphicTop.SetPosition(0, new Vector3(-4.5f, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(4.5f, 0, 0));

        DotScoreGraphicBot.SetPosition(0, new Vector3(-4.5f, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(4.5f, 0, 0));

        DotScoreGraphicLeft.gameObject.SetActive(true);
        DotScoreGraphicLeft.material = StartingDot.GetComponent<Renderer>().material;

        DotScoreGraphicRight.gameObject.SetActive(true);
        DotScoreGraphicRight.material = StartingDot.GetComponent<Renderer>().material;
    }

    // This is called when we confirm that a dot should be added to the chain.
    private void DotConnected(Transform Dot)
    {
        // Update the previous dot.
        PreviousDot = Dot;

        // Add the dot to the connected dots list.
        GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(Dot);

        // The position of the new dot.
        Vector3 Pos = new Vector3(Dot.position.x,
            Dot.position.y,
            -1);

        // Update the dot connector line and increase the score graphic.
        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.positionCount++;

        dotScoreGraphicXPos *= dotScoreGraphicGrowAmount;

        DotScoreGraphicTop.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
    }

    // This is called when we want to disconnect a dot (the undo line option).
    private void DotDisconnected(Transform Dot)
    {
        // Update the previous dot.
        PreviousDot = Dot;

        // This is basically the reverse of adding a dot. We shrink the score graphic and remove the dot from the connected list.
        Vector3 Pos = new Vector3(Dot.position.x,
            Dot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.positionCount--;

        dotScoreGraphicXPos /= dotScoreGraphicGrowAmount;

        DotScoreGraphicTop.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        GameBoardManager.GameBoardManagerInstance.ConnectedDots.RemoveAt(GameBoardManager.GameBoardManagerInstance.ConnectedDots.Count - 1);
    }

    // This coroutine has the connector line follow the mouse position.
    private IEnumerator MouseDragLineRenderer()
    {
        // Keep track of the line being active.
        lineActive = true;

        // The end of the line will follow the mouse position while the mouse left click is held down.
        while (Input.GetMouseButton(0))
        {
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 9;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

            DotConnector.SetPosition(DotConnector.positionCount - 1, worldPoint);

            yield return null;
        }

        // Once the click has been released, we turn off the score graphic and check if any dots have been scored.
        dotScoreGraphicXPos = 0.0f;

        DotScoreGraphicTop.SetPosition(0, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicTop.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(0, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphicBot.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotConnector.positionCount = 1;

        DotScoreGraphicLeft.gameObject.SetActive(false);
        DotScoreGraphicRight.gameObject.SetActive(false);

        StartCoroutine(GameBoardManager.GameBoardManagerInstance.DotsScored());

        lineActive = false;
    }
}
