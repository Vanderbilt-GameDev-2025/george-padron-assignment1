using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    // How quickly the monster moves towards the player
    [Export]
    public float Speed = 100.0f;

    // The amount of hits the monster can take before dying
    [Export]
    public uint MaxHealth = 2;

    // Stores how many hits the player has taken. 
    private uint _health;

    private Node2D _player;

    // Initialize private variables
    public override void _Ready()
    {
        _player = GetNode<Node2D>("/root/World/Player");
        _health = MaxHealth;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 playerPos = _player.Position;
        Vector2 direction = (playerPos - Position).Normalized();
        LookAt(playerPos);

        MoveAndCollide(direction * Speed * (float)delta);
    }

    public void TakeDamage(uint damage)
    {
        _health -= damage;
        GD.Print($"Enemy Hit, {_health} hp remaining!");
        if (_health == 0) QueueFree();
    }
}

