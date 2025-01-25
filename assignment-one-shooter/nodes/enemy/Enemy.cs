// Made by George Padron
// Email: george.n.padron@vanderbilt.edu
// Vunetid: padrongn

using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
    // Export variables 
    
    // How quickly the enemy moves towards the player
    [Export]
    public float Speed = 100.0f;

    // The amount of hits the monster can take before dying
    [Export]
    public uint MaxHealth = 2;

    // How much score the enemy gives when defeated
    [Export]
    public uint ScoreValue = 1;

    // Private variables 
    
    // Stores how many hits the enemy has taken. 
    private uint _health;

    // References to nodes in the scene
    private Node2D _player;
    private Score _score;
    private AudioStreamPlayer _audioStream;

    // Initialize references and private variables
    public override void _Ready()
    {
        _player = GetNode<Node2D>("/root/World/Player");
        _score = GetNode<Score>("/root/World/Score");
        _health = MaxHealth;

        _audioStream = GetNode<AudioStreamPlayer>("/root/World/EnemyDeathPlayer");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Face and move towards the player
        Vector2 playerPos = _player.Position;
        Vector2 direction = (playerPos - Position).Normalized();
        LookAt(playerPos);
        MoveAndCollide(direction * Speed * (float)delta);
    }

    public void TakeDamage(uint damage)
    {
        _health -= damage;
        // Play the hit sound effect 
        _audioStream.Play();
        
        // Kill if health is zero
        if (_health == 0) {
            QueueFree();
            _score.GainScore(ScoreValue);
        }
    }
}

