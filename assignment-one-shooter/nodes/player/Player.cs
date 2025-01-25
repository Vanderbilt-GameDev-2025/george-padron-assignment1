using Godot;
using System;

public partial class Player : CharacterBody2D
{

    [Export]
    public float Speed = 300.0f;

    [Export]
    public PackedScene BulletScene;

    [Export]
    public float BulletSpawnOffset = 50.0f;

    // Plays sound effects
    private AudioStreamPlayer _audioStream;

    public override void _Ready() {
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

        // Handle movement and collision
        var col = MoveAndCollide(velocity * (float)delta);
        if (col?.GetCollider() is Enemy _enemy) {
            GetTree().ReloadCurrentScene();
        }

    }

    private void Shoot()
    {
        if (BulletScene == null) {
            GD.PushError("Bullet Scene Not Set!");
            return;
        }

        // Create bullet instance
        Bullet bullet = BulletScene.Instantiate<Bullet>();
        var direction = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        bullet.GlobalPosition = GlobalPosition + direction * BulletSpawnOffset;
        bullet.Direction = direction;
        bullet.LookAt(GetGlobalMousePosition());


        // Play the shoot sound effect
        _audioStream.Play();
        // Add bullet to the scene
        GetParent().AddChild(bullet);
    }
}
