using Godot;
using Godot.Collections;
using System;

public partial class Sequencer : Node
{
	[Export] public Player Player;
	[Export] public Boss Boss;
	[Export] public Array<BaseAttack> Attacks = new Array<BaseAttack>();

	int i = 0;
	public override void _Ready()
	{
	}

	public async void StartSequence()
	{
		while (i < Attacks.Count)
		{
			await Attacks[i].ExecuteAttack(Boss);
			i++;
		}
	}
}
