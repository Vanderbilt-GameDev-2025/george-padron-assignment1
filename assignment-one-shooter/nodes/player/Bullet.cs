// Made by George Padron
// Email: george.n.padron@vanderbilt.edu
// Vunetid: padrongn

using Godot;
using System;

public partial class Bullet : CharacterBody2D
{
    [Export]
    public float Speed = 600.0f;

    // To be set by whomever spawns the bullet 
    public Vector2 Direction;

    public override void _PhysicsProcess(double delta)
    {
        // Move bullet towards direction 
        var col = MoveAndCollide(Direction * Speed * (float)delta);
        
        // Handle collisions
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
