using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DotConnectionManager : MonoBehaviour
{
    public static DotConnectionManager DotConnectionManagerInstance;

    public LineRenderer DotConnector, DotScoreGraphic;

    private float dotScoreGraphicXPos;

    public enum SelectedDotColor { None, Blue, Green, Purple, Red, Yellow };
    public SelectedDotColor CurrentlySelectedDotColor;

    public Transform DotGameBoard;
    public Transform DotScoredZone;

    private bool lineActive = false;
    private IndividualDot PreviousDot;
    private IndividualDot TouchedDot;

    public List<Transform> ConnectedDots;

    private void Awake()
    {
        DotConnectionManagerInstance = this;
    }

    public void AnotherDotEntered(IndividualDot dot)
    {
        if (lineActive)
        {
            if (Mathf.Abs(PreviousDot.Coordinates.x - dot.Coordinates.x) == 1 && Mathf.Abs(PreviousDot.Coordinates.y - dot.Coordinates.y) == 0 ||
            Mathf.Abs(PreviousDot.Coordinates.x - dot.Coordinates.x) == 0 && Mathf.Abs(PreviousDot.Coordinates.y - dot.Coordinates.y) == 1)
            {
                if((int)dot.ThisDotsColor == (int)PreviousDot.ThisDotsColor)
                {
                    DotConnected(dot.transform);
                }
            }
        }
    }

    public void StartLine(Transform ClickedDot)
    {
        ConnectedDots.Add(ClickedDot);

        PreviousDot = ClickedDot.GetComponent<IndividualDot>();

        Vector3 Pos = new Vector3(ClickedDot.position.x,
            ClickedDot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.material = ClickedDot.GetComponent<Renderer>().material;

        DotConnector.positionCount++;

        dotScoreGraphicXPos = 0.1f;

        DotScoreGraphic.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphic.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotScoreGraphic.material = ClickedDot.GetComponent<Renderer>().material;

        StartCoroutine(MouseDragLineRenderer());
    }

    private void DotConnected(Transform NewDot)
    {
        ConnectedDots.Add(NewDot);

        PreviousDot = NewDot.GetComponent<IndividualDot>();

        Vector3 Pos = new Vector3(NewDot.position.x,
            NewDot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.positionCount++;

        dotScoreGraphicXPos *= 2f;

        DotScoreGraphic.SetPosition(0, new Vector3(-dotScoreGraphicXPos, 0, 0));
        DotScoreGraphic.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));
    }

    private IEnumerator MouseDragLineRenderer()
    {
        lineActive = true;

        while (Input.GetMouseButton(0))
        {
            Vector3 screenPoint = Input.mousePosition;
            screenPoint.z = 9;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);

            DotConnector.SetPosition(DotConnector.positionCount - 1, worldPoint);

            yield return null;
        }

        dotScoreGraphicXPos = 0.0f;

        DotScoreGraphic.SetPosition(0, new Vector3(dotScoreGraphicXPos, 0, 0));
        DotScoreGraphic.SetPosition(1, new Vector3(dotScoreGraphicXPos, 0, 0));

        DotConnector.positionCount = 1;

        DotRefill();

        lineActive = false;
    }

    private void DotRefill()
    {
        if (ConnectedDots.Count < 2)
        {
            ConnectedDots.Clear();
        }
        else
        {

            foreach (Transform clearedDots in ConnectedDots)
            {
                foreach (Transform remainingDots in DotGameBoard)
                {
                    if (remainingDots.GetComponent<IndividualDot>().Coordinates.x == clearedDots.GetComponent<IndividualDot>().Coordinates.x &&
                    remainingDots.GetComponent<IndividualDot>().Coordinates.y > clearedDots.GetComponent<IndividualDot>().Coordinates.y)
                    {
                        remainingDots.GetComponent<IndividualDot>().Coordinates.y--;
                        StartCoroutine(remainingDots.GetComponent<IndividualDot>().FallIntoPlace());
                    }
                }

                //clearedDots.SetParent(DotScoredZone);
                //clearedDots.position = DotScoredZone.position;

                clearedDots.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator RepopulateDots()
    {
        foreach (Transform dot in ConnectedDots)
        {
            dot.position = new Vector3(dot.GetComponent<IndividualDot>().Coordinates.x, 6, 0);
            dot.SetParent(DotGameBoard);
        }

        yield return new WaitForSeconds(2);

        int startColumn = (int)ConnectedDots[0].GetComponent<IndividualDot>().Coordinates.x;
        int endColumn = (int)ConnectedDots[ConnectedDots.Count - 1].GetComponent<IndividualDot>().Coordinates.x;

        while (startColumn <= endColumn)
        {
            int amountToDrop = 0;

            foreach (Transform clearedDots in ConnectedDots)
            {
                if (clearedDots.GetComponent<IndividualDot>().Coordinates.x == startColumn)
                {
                    amountToDrop++;
                }

                //StartCoroutine(clearedDots.GetComponent<IndividualDot>().FallIntoPlace(clearedDots.GetComponent<IndividualDot>().Coordinates.y - amountToDrop));
            }
            yield return null;
        }
    }

    private void ReassignDots()
    {
        foreach(Transform dot in DotGameBoard)
        {
            IndividualDot DotScript = dot.GetComponent<IndividualDot>();

            if(DotScript.Coordinates != new Vector2(dot.transform.position.x, dot.transform.position.y))
            {
                DotScript.Coordinates = new Vector2(Mathf.RoundToInt(dot.transform.position.x), Mathf.RoundToInt(dot.transform.position.y));
            }
        }
    }
}
