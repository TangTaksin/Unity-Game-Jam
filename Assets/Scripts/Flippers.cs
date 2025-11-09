using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flippers : MonoBehaviour
{
    Rigidbody2D _rigidbody2D;

    public Side flipperSide;

    InputAction action_flip;
    bool isFlipping;

    public float travelTime = .1f;
    float targetAngle;
    public float flipperRestAngle = -45;
    public float flipperActiveAngle = 45;
    public AudioClip flipSound;


    private void Start()
    {
        targetAngle = flipperRestAngle;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, targetAngle);

        _rigidbody2D = GetComponent<Rigidbody2D>();

        switch (flipperSide)
        {
            case Side.Left:
                action_flip = InputSystem.actions.FindAction("Flipleft");
                break;
            case Side.Right:
                action_flip = InputSystem.actions.FindAction("FlipRight");
                break;
        }

        action_flip.started += Flip;
        action_flip.canceled += Flip;
    }

    private void OnDisable()
    {
        action_flip.started -= Flip;
        action_flip.canceled -= Flip;
    }

    void Flip(InputAction.CallbackContext context)
    {
        isFlipping = context.started;
        targetAngle = isFlipping ? flipperActiveAngle : flipperRestAngle;

        if (context.started && flipSound != null && AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(flipSound);
        }


        _rigidbody2D.DOComplete();
        _rigidbody2D.DORotate(targetAngle, travelTime).SetEase(Ease.Linear);

    }

}

public enum Side
{
    Left,
    Right
}
