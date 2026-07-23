using Godot;
using System;

public partial class Boss : CharacterBody2D
{
	[Export]
	public Player Player;

	[Export]
	public Sequencer Sequencer;
	public override void _Ready()
	{
		Sequencer.Player = Player;
	}
}
