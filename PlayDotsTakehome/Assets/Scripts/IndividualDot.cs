using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualDot : MonoBehaviour
{
    public enum Color { None, Blue, Green, Purple, Red, Yellow };
    public Color ThisDotsColor;

    public Vector2 Coordinates;

    public IEnumerator FallIntoPlace(float yPos)
    {

        Vector3 FinalPos = new Vector3(this.transform.position.x, yPos, 0);

        while (Vector3.Distance(this.transform.position, FinalPos) > 0.1f)
        {
            Debug.Log("test");
            transform.position = Vector3.Lerp(transform.position, FinalPos, 20f * Time.deltaTime);
            yield return null;
        }

        transform.position = FinalPos;

        yield return null;
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
}
