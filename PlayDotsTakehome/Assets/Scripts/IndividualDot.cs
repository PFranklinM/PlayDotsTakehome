using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualDot : MonoBehaviour
{
    public enum Color { None, Blue, Green, Purple, Red, Yellow };
    public Color ThisDotsColor;

    public Vector2 Coordinates;

    public IEnumerator FallIntoPlace(int yPos)
    {
        Vector3 DropPos = new Vector3(this.transform.position.x, yPos - 0.5f, 0);
        Vector3 FinalPos = new Vector3(this.transform.position.x, yPos, 0);

        while (Vector3.Distance(this.transform.position, DropPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, DropPos, 20f * Time.deltaTime);
            yield return null;
        }

        while(Vector3.Distance(this.transform.position, FinalPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, FinalPos, 20f * Time.deltaTime);
            yield return null;
        }

        transform.position = FinalPos;
    }

    private void OnMouseDown()
    {
        DotConnectionManager.DotConnectionManagerInstance.StartLine(transform);

        DotConnectionManager.DotConnectionManagerInstance.CurrentlySelectedDotColor = (DotConnectionManager.SelectedDotColor)(int)ThisDotsColor;
    }

    private void OnMouseEnter()
    {
        DotConnectionManager.DotConnectionManagerInstance.AnotherDotEntered(this);
    }

    private void OnMouseExit()
    {
        DotConnectionManager.DotConnectionManagerInstance.AnotherDotExited();
    }
}
