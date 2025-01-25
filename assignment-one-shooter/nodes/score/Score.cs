using Godot;
using System;

public partial class Score : Label
{
    // The Basic Text of the Label
    [Export]
    public String ScoreText = "Score: ";

    private uint _score = 0;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        // Initializes the score 
        GainScore(0);
    }

    // Adds score and updates label
    public void GainScore(uint score) {
        _score += score;
        Text = ScoreText + _score;
    }



}
