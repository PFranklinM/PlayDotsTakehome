using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndividualDot : MonoBehaviour
{
    public enum Color { None, Blue, Green, Purple, Red, Yellow };
    public Color ThisDotsColor;

    public Vector2 Coordinates;

    public IEnumerator FallIntoPlace()
    {
        while(Vector3.Distance(this.transform.position, Coordinates) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, Coordinates, 20f * Time.deltaTime);

            yield return null;
        }

        transform.position = Coordinates;
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
