using UnityEngine;

public class Jumpable : MonoBehaviour
{
    // How much force is applied with the jump, assigned in the inspector
    public float jumpForce = 10;

    // This method is called by the Character, when it lands on a jumpable object
    public void ApplyJumpToCharacter(bool negativeJump)
    {
        // Applies negative or positive jump force to the character, using the conditional operator (?:)
        Character.MoveDirection = negativeJump
                                      ? new Vector3(Character.MoveDirection.x, -jumpForce, 0)
                                      : new Vector3(Character.MoveDirection.x, jumpForce, 0);
    }
}