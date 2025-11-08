using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System;

public class Flippers : MonoBehaviour
{
    HingeJoint2D _joint;
    Rigidbody2D _rigidbody2D;

    public KeyCode flipperKey;
    bool isFlipping;

    public float travelTime = .1f;
    float targetAngle;
    public float flipperRestAngle = -45;
    public float flipperActiveAngle = 45;


    private void Start()
    {
        targetAngle = flipperRestAngle;
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, targetAngle);

        _rigidbody2D = GetComponent<Rigidbody2D>();
        _joint = GetComponent<HingeJoint2D>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(flipperKey))
        {
            Flip(true);
        }
        else if (Input.GetKeyUp(flipperKey))
        {
            Flip(false);
        }
    }

    void Flip(bool isFlipping)
    {
        targetAngle = isFlipping ? flipperActiveAngle : flipperRestAngle;

        _rigidbody2D.DOComplete();
        _rigidbody2D.DORotate(targetAngle, travelTime).SetEase(Ease.Linear);
    }

}

public enum Side
{
    Left,
    Right
}
