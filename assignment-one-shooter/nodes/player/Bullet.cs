using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
    [Export]
    public float Speed = 600.0f;

    public Vector2 Direction;

    public override void _PhysicsProcess(double delta)
    {
        var col = MoveAndCollide(Direction * Speed * (float)delta);
        if (col != null)
        {
            if (col.GetCollider() is Enemy enemy)
            {
                enemy.TakeDamage(1);
            }
            QueueFree();
        }
    }
}
