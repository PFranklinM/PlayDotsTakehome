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

    private bool lineActive = false;
    private IndividualDot StartingDot;
    private IndividualDot PreviousDot;
    private IndividualDot TouchedDot;

    private void Awake()
    {
        DotConnectionManagerInstance = this;
    }

    public void StartLine(Transform ClickedDot)
    {
        StartingDot = ClickedDot.GetComponent<IndividualDot>();

        GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(ClickedDot);

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

    public void AnotherDotEntered(IndividualDot dot)
    {
        if (lineActive)
        {
            if (Mathf.Abs(PreviousDot.Coordinates.x - dot.Coordinates.x) == 1 && Mathf.Abs(PreviousDot.Coordinates.y - dot.Coordinates.y) == 0 ||
            Mathf.Abs(PreviousDot.Coordinates.x - dot.Coordinates.x) == 0 && Mathf.Abs(PreviousDot.Coordinates.y - dot.Coordinates.y) == 1)
            {
                if ((int)dot.ThisDotsColor == (int)PreviousDot.ThisDotsColor)
                {
                    if (dot == StartingDot)
                    {
                        Debug.Log("square made");
                    }
                    else
                    {
                        DotConnected(dot.transform);
                    }
                }
            }
        }
    }

    private void DotConnected(Transform NewDot)
    {
        GameBoardManager.GameBoardManagerInstance.ConnectedDots.Add(NewDot);

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

        StartCoroutine(GameBoardManager.GameBoardManagerInstance.DotRefill());

        lineActive = false;
    }
}
