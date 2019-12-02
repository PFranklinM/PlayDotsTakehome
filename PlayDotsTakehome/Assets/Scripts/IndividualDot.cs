using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is attached to the dot prefab. It stores relevant information such as color and position and calls the mouse events that drive gameplay.

public class IndividualDot : MonoBehaviour
{
    // Enum that tracks each dots color. The None category is added in case a situation would arise where a dot would need to be colorless.
    public enum Color { None, Blue, Green, Purple, Red, Yellow };
    public Color ThisDotsColor;

    // The coordinates of each dot in a grid.
    public Vector2 Coordinates;

    // Change the fall speed of the dots. I set this to 20 which felt good while playtesting.
    [SerializeField]
    private float dotFallSpeed;

    // This coroutine moves the dots into their assigned coordinates. This is called on game start and after dots are cleared from the board and respawned.
    public IEnumerator FallIntoPlace()
    {

        /* This Vector3 deals with the dropping animation. Rather than using an animator which would change all 3 parameters of a dots position
         and which would require parenting all the dots to a dot container, I thought it would be more flexible to animate the dots through code.
         So the target position that the dots drop to is actually 0.5 less than their actual y coordinate. */
        Vector3 DropDotPos = new Vector3(Coordinates.x, Coordinates.y - 0.5f, 0);

        // Lerp the dot to its lower position.
        while(Vector3.Distance(transform.position, DropDotPos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, DropDotPos, dotFallSpeed * Time.deltaTime);

            yield return null;
        }

        // Arbitrary length of time to wait that helps sell the bounce animation.
        yield return new WaitForSeconds(0.05f);

        // Once it reaches the lower position, it snaps to its final position. When this is done at full speed, it creates a bounce effect.
        transform.position = Coordinates;
    }

    // This event fires when we click on a dot. We start drawing the connection line and assign the currently selected dot color to the current dots color.
    private void OnMouseDown()
    {
        DotConnectionManager.DotConnectionManagerInstance.StartLine(transform);

        // Both individual dots and currently selected dots color enums are identical. This makes it easy to set one equal to the other.
        DotConnectionManager.DotConnectionManagerInstance.CurrentlySelectedDotColor = (DotConnectionManager.SelectedDotColor)(int)ThisDotsColor;
    }

    // This event fires when our mouse pointer enters a new dot.
    private void OnMouseEnter()
    {
        DotConnectionManager.DotConnectionManagerInstance.AnotherDotEntered(transform);
    }
}
