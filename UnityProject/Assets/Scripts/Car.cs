using System;
using UnityEngine;

public abstract class CarController : MonoBehaviour
{
    [SerializeField] private float Acceleration = 2.5f;
    [SerializeField] private float Steering = 150;

    public string ID { get; private set; }

    private Rigidbody2D Rigidbody;

    protected virtual void Awake()
    {
        ID = Guid.NewGuid().ToString();
        Rigidbody = GetComponent<Rigidbody2D>();
	}

    protected virtual void FixedUpdate()
    {
        float vertical, horizontal;
        GetInput(out vertical, out horizontal);
        Move(vertical, horizontal);
    }

    protected abstract void GetInput(out float vertical, out float horizontal);

    public abstract void Kill();

    private void Move(float vertical, float horizontal)
    {
        Rigidbody.velocity = transform.up * vertical * Acceleration;
        Rigidbody.angularVelocity = -horizontal * Steering;
    }
}
