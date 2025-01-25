// Made by George Padron
// Email: george.n.padron@vanderbilt.edu
// Vunetid: padrongn

using Godot;
using System;

public partial class Player : CharacterBody2D
{

    // How quickly the ship moves
    [Export]
    public float Speed = 300.0f;

    [Export]
    public PackedScene BulletScene;

    // How far the bullet is from the center of the player when created
    [Export]
    public float BulletSpawnOffset = 50.0f;

    // Plays sound effects
    private AudioStreamPlayer _audioStream;

    public override void _Ready() {
        // Get the audioStream as a child of this node
        _audioStream = GetChild<AudioStreamPlayer>(2); 
    }

    public override void _PhysicsProcess(double delta)
    {
        handleMovement(delta);

        // Aim towards mouse
        Vector2 mousePos = GetGlobalMousePosition();
        LookAt(mousePos);

        // Shoot on left click
        if (Input.IsActionJustPressed("shoot"))
        {
            Shoot();
        }
    }

    // Handles movement with the arrow keys/WASD
    private void handleMovement(double delta)
    {
        Vector2 velocity = Velocity;
        // Get the input direction and handle the movement/deceleration.
        Vector2 direction = Input.GetVector(
            "move_left",
            "move_right",
            "move_up",
            "move_down"
        );
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
            velocity.Y = direction.Y * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
        }

        // Finalize movement
        var col = MoveAndCollide(velocity * (float)delta);
        // Check for collisions with enemies
        if (col?.GetCollider() is Enemy _enemy) {
            // Reset the scene on collision
            GetTree().ReloadCurrentScene();
        }

    }

    private void Shoot()
    {
        // Do nothing if the bullet is not set 
        if (BulletScene == null) {
            GD.PushError("Bullet Scene Not Set!");
            return;
        }

        // Create bullet instance
        Bullet bullet = BulletScene.Instantiate<Bullet>();
        // Make sure the bullet moves towards the mouse cursor's position
        Vector2 mousePos = GetGlobalMousePosition();
        var direction = (mousePos - GlobalPosition).Normalized();
        // Make bullet appear in front of ship facing cursor  
        bullet.GlobalPosition = GlobalPosition + direction * BulletSpawnOffset;
        bullet.Direction = direction;
        bullet.LookAt(mousePos);

        // Play the shoot sound effect
        _audioStream.Play();

        // Add bullet to the scene
        GetParent().AddChild(bullet);
    }
}
