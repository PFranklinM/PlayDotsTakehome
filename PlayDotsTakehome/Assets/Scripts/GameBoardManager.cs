using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script manages the game board. Clearing and repopulating the board as well as assigning colors to dots. 
   We use object pooling for the dots to avoid repeated destroy/instantiate calls.  */

public class GameBoardManager : MonoBehaviour
{
    // Static instance of this class.
    public static GameBoardManager GameBoardManagerInstance = null;

    // Transforms for the game board and the area where dots go when they have been scored (we use pooling for the dots to avoid constantly destroying and instantiating them).
    public Transform GameBoardParent, DotScoredZone;

    // The list of dots that have been connected in a chain.
    public List<Transform> ConnectedDots;

    // The dot prefab.
    public GameObject DotPrefab;

    // The possible colors for a dot.
    public Material Blue, Green, Purple, Red, Yellow;

    // These are used when a square has been made and the dots should ignore a certain color when being reassigned.
    private bool shouldIgnoreColor;
    private int colorToIgnore;

    // The size of the game board. For now just set to 6x6 but could expand or shrink for more advanced levels.
    [SerializeField]
    private int rows, columns;

    private void Awake()
    {
        // Singleton for this class. This manager is intended to be used in all levels.
        if (GameBoardManagerInstance == null)
        {
            GameBoardManagerInstance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    // Spawn dots on application start.
    private void Start()
    {
        StartCoroutine(SpawnDots());
    }

    // Here is where we spawn the dots. Since we are using object pooling, we only need to instantiate them once.
    private IEnumerator SpawnDots()
    {
        // Spawn the dots row by row and have them start offscreen.
        int numberOfRows = 0;

        while (numberOfRows < rows)
        {
            for (int i = 0; i < columns; i++)
            {
                Vector3 SpawnPos = new Vector3(i, rows * 2, 0);
                GameObject IndividualDot = Instantiate(DotPrefab, SpawnPos, Quaternion.identity);
                AssignDots(IndividualDot.transform, numberOfRows);
            }

            numberOfRows++;

            yield return null;
        }
    }

    // This is called when we release a click from connecting dots.
    public IEnumerator DotsScored()
    {
        // If we have not connected enough dots to score, we simply clear the list of connected dots.
        if (ConnectedDots.Count < 2)
        {
            ConnectedDots.Clear();
        }
        else
        {
            // If we have connected enough dots, we loop through each connected dot...
            int clearedDotCounter = 0;

            while (clearedDotCounter <= ConnectedDots.Count)
            {
                foreach (Transform ClearedDots in ConnectedDots)
                {
                    // ...and move them to the dot scored zone (which is offscreen).
                    ClearedDots.SetParent(DotScoredZone);
                    ClearedDots.position = DotScoredZone.position;

                    foreach (Transform RemainingDots in GameBoardParent)
                    {
                        // We then change the coordinate values of both the scored dots and remaining dots that are above them which would be affected by "gravity".
                        if (RemainingDots.GetComponent<IndividualDot>().Coordinates.x == ClearedDots.GetComponent<IndividualDot>().Coordinates.x &&
                        RemainingDots.GetComponent<IndividualDot>().Coordinates.y > ClearedDots.GetComponent<IndividualDot>().Coordinates.y)
                        {
                            RemainingDots.GetComponent<IndividualDot>().Coordinates.y--;

                            ClearedDots.GetComponent<IndividualDot>().Coordinates.y++;
                        }
                    }
                }

                clearedDotCounter++;

                yield return null;
            }

            // Have the remaining dots fall into place if their position does not match their coordinates.
            foreach (Transform RemainingDots in GameBoardParent)
            {
                if (RemainingDots.transform.position != new Vector3(RemainingDots.GetComponent<IndividualDot>().Coordinates.x,
                    RemainingDots.GetComponent<IndividualDot>().Coordinates.y,
                    0))
                {
                    StartCoroutine(RemainingDots.GetComponent<IndividualDot>().FallIntoPlace());
                }
            }

            RepopulateDots();
        }
    }

    // We then add the cleared dots back to the grid.
    private void RepopulateDots()
    {
        // For all of the connected dots, we add them back to the game board and set their position above the gameboard.
        foreach (Transform ClearedDots in ConnectedDots)
        {
            ClearedDots.SetParent(GameBoardParent);
            ClearedDots.position = new Vector3(ClearedDots.GetComponent<IndividualDot>().Coordinates.x, rows * 2, 0);

            // We then assign these dots a new color.
            AssignDots(ClearedDots, (int)ClearedDots.GetComponent<IndividualDot>().Coordinates.y);
        }

        // Clear the list of connected dots.
        ConnectedDots.Clear();

        StartCoroutine(ReorganizeHierarchy());
    }

    // This reorganizes the hierarchy of the dots based on their positions.
    private IEnumerator ReorganizeHierarchy()
    {
        int childTracker = 0;

        // We set the sibling index of the dotsby row (horizontal progression through the children instead of vertical).
        while (childTracker < GameBoardParent.childCount)
        {
            foreach (Transform dot in GameBoardParent)
            {
                dot.SetSiblingIndex((int)dot.position.x + ((int)dot.position.y * rows));
            }

            childTracker++;
            yield return null;
        }

        // Finally we check to see if a certain color of dot is being excluded from making a square. If so, set color exclusion to false.
        if (shouldIgnoreColor)
        {
            shouldIgnoreColor = false;
        }
    }

    // This int is called in the event of a square being made and a certain color needing to be excluded from the random color generator.
    private int RandomColor(int minimum, int maximum, int exception)
    {
        // First we generate a random color.
        int randomColor = Random.Range(0, 5);

        // Then if the color is equal to the excluded color, we set it equal to a different color.
        if(randomColor == exception)
        {
            randomColor = (randomColor + 1) % maximum;
        }

        return randomColor;
    }
    
    // This assigns each dot a random color before it falls into place as well as its coordinate values.
    public void AssignDots(Transform Dot, int finalYPos)
    {
        int randomColor = 0;

        // Check if we should ignore a color for this particular dot assignment.
        if (shouldIgnoreColor)
        {
            randomColor = RandomColor(0, 5, colorToIgnore);
        }
        else
        {
            randomColor = Random.Range(0, 5);
        }

        // Store the Individual dot and renderer components in local variables.
        IndividualDot DotScript = Dot.GetComponent<IndividualDot>();
        Renderer DotRenderer = Dot.GetComponent<Renderer>();

        // Pick a random color.
        switch (randomColor)
        {
            case 0:
                DotScript.ThisDotsColor = IndividualDot.Color.Blue;
                DotRenderer.material = Blue;
                break;

            case 1:
                DotScript.ThisDotsColor = IndividualDot.Color.Green;
                DotRenderer.material = Green;
                break;

            case 2:
                DotScript.ThisDotsColor = IndividualDot.Color.Purple;
                DotRenderer.material = Purple;
                break;

            case 3:
                DotScript.ThisDotsColor = IndividualDot.Color.Red;
                DotRenderer.material = Red;
                break;

            case 4:
                DotScript.ThisDotsColor = IndividualDot.Color.Yellow;
                DotRenderer.material = Yellow;
                break;
        }

        // Set the coordinates of each dot.
        DotScript.Coordinates.x = Dot.transform.position.x;
        DotScript.Coordinates.y = finalYPos;

        // Parent each dot to the game board.
        Dot.transform.SetParent(GameBoardParent);

        // Start the dots falling into place.
        StartCoroutine(DotScript.FallIntoPlace());
    }

    // If a square is made, flag a certain color to be ignored in reassignment.
    public void DotReassignmentAvoidColor(int color)
    {
        shouldIgnoreColor = true;
        colorToIgnore = color;
    }
}
