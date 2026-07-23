using Godot;
using System;
using System.Threading.Tasks;

public abstract partial class BaseAttack : Node
{
	public virtual int CountBullets { get; set; }
	public async virtual Task ExecuteAttack(Boss boss)
	{
		
	}
}
