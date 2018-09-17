using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    public float maxSpeed = 1f;
    public float maxForce = .03f;
    public float neighbourhoodRadius = 6f;

    public bool calculateSeparation = true;
    public bool calculateCohesion = true;
    public bool calculateAlignment = true;
    public Vector2 acceleration;
    public Vector2 velocity;

    private Vector2 Position {
        get {
            return gameObject.transform.position;
        }
        set {
            gameObject.transform.position = value;
        }
    }

    private void Start()
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void Update()
    {
        var boids = FindObjectsOfType<Flocking>();

        Flock(boids);
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
        WrapAround();
    }

    private void Flock(IEnumerable<Flocking> boids)
    {
        var alignment = calculateAlignment ? Alignment(boids) : Vector2.zero;
        var separation = calculateSeparation ? Separation(boids) : Vector2.zero;
        var cohesion = calculateCohesion ? Cohesion(boids) : Vector2.zero;

        acceleration = alignment + cohesion + 1.5f * separation;
    }

    public void UpdateVelocity()
    {
        velocity += acceleration;
        velocity = velocity.normalized * maxSpeed; // limit to max speed
    }

    private void UpdatePosition()
    {
        Position += velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 58));
    }

    private float DistanceTo(Flocking boid)
    {
        return Vector3.Distance(boid.transform.position, Position);
    }

    private Vector2 Alignment(IEnumerable<Flocking> boids)
    {
        var velocity = Vector2.zero;
        boids = boids.Where(o => DistanceTo(o) <= neighbourhoodRadius && o != this);
        if (!boids.Any()) return velocity;

        foreach (var boid in boids)
        {
            velocity += boid.velocity;
        }
        velocity /= boids.Count();

        var steer = Steer(velocity.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Cohesion(IEnumerable<Flocking> boids)
    {
        boids = boids.Where(o => DistanceTo(o) <= neighbourhoodRadius && o != this);
        if (!boids.Any()) return Vector2.zero;

        var sumPositions = Vector2.zero;
        foreach (var boid in boids)
        {
            sumPositions += boid.Position;
        }
        var average = sumPositions / boids.Count();
        var direction = average - Position;

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Separation(IEnumerable<Flocking> boids)
    {
        var direction = Vector2.zero;
        boids = boids.Where(o => DistanceTo(o) <= neighbourhoodRadius / 2 && o != this);
        if (!boids.Any()) return direction;

        foreach (var boid in boids)
        {
            var difference = Position - boid.Position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= boids.Count();

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - velocity;
        if (steer.sqrMagnitude > maxForce * maxForce)
        {
            steer = steer.normalized * maxForce;
        }

        return steer;
    }

    private void WrapAround()
    {
        if (Position.x < -14) Position = new Vector2(14, Position.y);
        if (Position.y < -8) Position = new Vector2(Position.x, 8);
        if (Position.x > 14) Position = new Vector2(-14, Position.y);
        if (Position.y > 8) Position = new Vector2(Position.x, -8);
    }
}