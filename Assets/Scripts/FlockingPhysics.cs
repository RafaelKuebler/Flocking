using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: look up https://processing.org/examples/flocking.html
// current implementation from https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444
public class FlockingPhysics : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 6f;

    [SerializeField]
    private float neighbourhoodRadius = 4f;

    [SerializeField]
    private bool separation = true;

    [SerializeField]
    private bool cohesion = true;

    [SerializeField]
    private bool alignment = true;

    public Vector2 separationVelocity;
    public Vector2 cohesionVelocity;
    public Vector2 alignmentVelocity;

    private Vector2 Velocity {
        get {
            return GetComponent<Rigidbody2D>().velocity;
        }
        set {
            GetComponent<Rigidbody2D>().velocity = value;
        }
    }

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
        //float angle = Random.Range(0, 2 * Mathf.PI);
        //transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //Velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Velocity = Vector2.one;
    }

    // Update is called once per frame
    private void Update()
    {
        Wraparound();
        var neighbourColliders = Physics2D.OverlapCircleAll(Position, neighbourhoodRadius).ToList();
        neighbourColliders.Remove(GetComponent<CapsuleCollider2D>());
        if (!neighbourColliders.Any())
        {
            return;
        }

        var neighbourRigidBodies = neighbourColliders.Select(o => o.GetComponent<Rigidbody2D>());
        alignmentVelocity = alignment ? SetAlignment(neighbourRigidBodies) : Vector2.zero;
        cohesionVelocity = cohesion ? SetCohesion(neighbourRigidBodies) : Vector2.zero;
        separationVelocity = separation ? SetSeparation(neighbourRigidBodies) : Vector2.zero;

        //Velocity = Velocity + alignmentVelocity + cohesionVelocity + 2f * separationVelocity;
        //if (Velocity.sqrMagnitude > maxSpeed * maxSpeed)
        //{
        //    Velocity = Velocity.normalized * maxSpeed;
        //}
        var acceleration = alignmentVelocity + cohesionVelocity + separationVelocity;
        GetComponent<Rigidbody2D>().AddForce(acceleration);
        if (Velocity.sqrMagnitude > maxSpeed * maxSpeed)
        {
            Velocity = Velocity.normalized * maxSpeed;
        }

        var angle = Mathf.Atan2(Velocity.y, Velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    // Wraparound
    private void Wraparound()
    {
        if (Position.x < -14) Position = new Vector2(14, Position.y);
        if (Position.y < -8) Position = new Vector2(Position.x, 8);
        if (Position.x > 14) Position = new Vector2(-14, Position.y);
        if (Position.y > 8) Position = new Vector2(Position.x, -8);
    }

    private Vector2 SetAlignment(IEnumerable<Rigidbody2D> neighbours)
    {
        var velocity = Vector2.zero;
        if (!neighbours.Any()) return velocity;

        foreach (var neighbour in neighbours)
        {
            velocity += neighbour.velocity;
        }
        velocity /= neighbours.Count();

        return velocity.normalized;
    }

    private Vector2 SetCohesion(IEnumerable<Rigidbody2D> neighbours)
    {
        var velocity = Vector2.zero;
        if (!neighbours.Any()) return velocity;

        foreach (var neighbour in neighbours)
        {
            velocity += neighbour.position;
        }
        velocity /= neighbours.Count();
        velocity = velocity - Position;

        return velocity.normalized;
    }

    private Vector2 SetSeparation(IEnumerable<Rigidbody2D> neighbours)
    {
        var velocity = Vector2.zero;
        if (!neighbours.Any()) return velocity;

        foreach (var neighbour in neighbours)
        {
            //velocity = Position - neighbour.position;
            var distance = Position - neighbour.position;
            velocity = distance.normalized / distance.magnitude;
        }
        velocity /= neighbours.Count();

        return velocity;
    }
}