using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class CircleShot : BaseAttack
{
	public async override Task ExecuteAttack(Boss boss)
	{
		float angle = 0f;
		for (int i = 0; i < 20; i++)
		{
			Vector2 direction = Vector2.Right.Rotated(angle);
			boss.Shoot(direction);
			angle += 0.3f;
		}
	}
}
