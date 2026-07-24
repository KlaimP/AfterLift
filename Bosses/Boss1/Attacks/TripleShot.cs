using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class TripleShot : BaseAttack
{
	public async override Task ExecuteAttack(Boss boss)
	{
		if (!GodotObject.IsInstanceValid(boss.Player))
		return;
		Vector2 direction = (boss.Player.GlobalPosition - boss.GlobalPosition).Normalized();
		boss.Shoot(direction.Rotated(-0.2f));
		boss.Shoot(direction);
		boss.Shoot(direction.Rotated(0.2f));
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
	}
}
