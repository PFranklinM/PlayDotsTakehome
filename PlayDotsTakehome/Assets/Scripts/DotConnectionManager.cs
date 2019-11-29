using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotConnectionManager : MonoBehaviour
{
    public static DotConnectionManager DotConnectionManagerInstance;

    public LineRenderer DotConnector;

    public enum SelectedDotColor { None, Blue, Green, Purple, Red, Yellow };
    public SelectedDotColor CurrentlySelectedDotColor;

    private bool lineActive = false;
    private IndividualDot PreviousDot;
    private IndividualDot TouchedDot;

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
                    AddLineNode(dot.transform);
                }
            }
        }
    }

    public void StartLine(Transform ClickedDot)
    {
        PreviousDot = ClickedDot.GetComponent<IndividualDot>();

        Vector3 Pos = new Vector3(ClickedDot.position.x,
            ClickedDot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.material = ClickedDot.GetComponent<Renderer>().material;

        DotConnector.positionCount++;

        StartCoroutine(MouseDragLineRenderer());
    }

    private void AddLineNode(Transform NewDot)
    {
        PreviousDot = NewDot.GetComponent<IndividualDot>();

        Vector3 Pos = new Vector3(NewDot.position.x,
            NewDot.position.y,
            -1);

        DotConnector.SetPosition(DotConnector.positionCount - 1, Pos);

        DotConnector.positionCount++;
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

        DotConnector.positionCount = 1;
        lineActive = false;
    }
}
