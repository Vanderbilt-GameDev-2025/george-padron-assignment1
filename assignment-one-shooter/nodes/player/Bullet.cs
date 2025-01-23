using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
    [Export]
    public float Speed = 600.0f;

    public Vector2 Direction { get; set; }

    public override void _PhysicsProcess(double delta)
    {
        var collision = MoveAndCollide(Direction * Speed * (float)delta);
        if (collision != null)
        {
            var collider = collision.GetCollider();
            if (collider is Enemy enemy)
            {
                enemy.TakeDamage(1);
            }
            QueueFree();
        }
    }
}
