using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class SpiralAttack : BaseAttack
{
	public async override Task ExecuteAttack(Boss boss)
	{
		GD.Print("SpiralAttack executed!");
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
		GD.Print("SpiralAttack finished!");
	}
}
