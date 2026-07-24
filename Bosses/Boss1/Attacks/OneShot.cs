using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class OneShot : BaseAttack
{
	public async override Task ExecuteAttack(Boss boss)
	{
		if (!GodotObject.IsInstanceValid(boss.Player))
		return;
		Vector2 direction = (boss.Player.GlobalPosition - boss.GlobalPosition).Normalized();
		boss.Shoot(direction);
		await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
	}
}
