using Godot;
using System;
//создание класса пули public доступ всем, унаследован от Node2D, partial godot добавляет часть движка
public partial class Bullet : Area2D
{
[Export] public float Damage = 25f;

public override void _Ready()
{
	// получение урона
	BodyEntered += OnBodyEntered;
}
	public float Speed = 400f;
	//переменная Direction направление движения Vector2 объект из 2х чисел .Right по умолчанию вправо
	public Vector2 Direction = Vector2.Right;
	//дефолт метод godot, каждый цикл, было public override я убрал. double delta чтобы скорость не зависела от fps
	public override void _Process(double delta)
	{
		//новая позиция = старая позиция + направление * скрорсть * время (float) смена типа данных delta
		Position += Direction * Speed * (float)delta;
	}
	private void OnBodyEntered(Node body)
{
	// получение урона
	if (body is Player player)
	{
		player.TakeDamage(Damage);

		QueueFree();
	}
}

}
