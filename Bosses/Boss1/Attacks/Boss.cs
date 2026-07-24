using Godot;
using System;

public partial class Boss : CharacterBody2D
{
	[Export] public Player Player;
	[Export] public Sequencer Sequencer;
	[Export] public float OrbitSpeed = 1f; // скорость вращения вокруг игрока

	//для таймера генератор случайного числа
	private float orbitDirection = 1f; // 1 = вправо, -1 = влево
	private RandomNumberGenerator rng = new RandomNumberGenerator();
	private Timer orbitTimer;

	[Export] public float MoveSpeed = 100f;
	[Export] public float DesiredDistance = 300f;
	
public override void _PhysicsProcess(double delta)
{
	// если это убрать босс не найдет игрока и будет срать ошибками
	if (!IsInstanceValid(Player))
		return;

	Vector2 toPlayer = Player.GlobalPosition - GlobalPosition;
	float distance = toPlayer.Length();
	Vector2 movement = Vector2.Zero;

	// держим дистанцию
	if (distance > DesiredDistance)
	{
		movement += toPlayer.Normalized();
	}
	else if (distance < DesiredDistance - 50)
	{
		movement -= toPlayer.Normalized();
	}

	// вращение вокруг игрока
	Vector2 orbit = toPlayer.Rotated(
		Mathf.Pi / 2 * orbitDirection
	);
	movement += orbit.Normalized() * OrbitSpeed;
	Velocity = movement.Normalized() * MoveSpeed;
	MoveAndSlide();
}
// таймер смены направления движения вокруг игрока
	private void StartOrbitTimer()
	{
		float time = rng.RandfRange(1f, 5f);
		orbitTimer.WaitTime = time;
		orbitTimer.Start();
	}
	// метод смены направления движения вокруг игрока
	private void ChangeOrbitDirection()
	{
		orbitDirection *= -1;
		StartOrbitTimer();
	}
	//переменная для сцены пули. PackedScene - скопировать сцену и использовать ее МНОГА
	[Export] private PackedScene Pupulka;
	
	// async добавил из за await
	public override async void _Ready()
	{
		//загрузить пулю
		Pupulka = GD.Load<PackedScene>("res://bullet.tscn");
		Sequencer.Player = Player;
		//игра не успевает и появляется ошибка Parent node is busy setting up children, add_child() failed.
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		Sequencer.StartSequence();
		//таймер для смены направления движения вокруг игрока
		orbitTimer = new Timer();
		AddChild(orbitTimer);
		orbitTimer.Timeout += ChangeOrbitDirection;
		StartOrbitTimer();
		//рандомайзер времени смены направления движения
		rng.Randomize();
	}
	// пиу пиу может принимать направление
	public void Shoot(Vector2 direction)
	{
		// была проблема, что Pupulka не успевала загрузиться из за слишком быстрого старта sequencer
	 	if (Pupulka == null)
		{
			GD.PrintErr("Bullet scene is not loaded!");
			return;
		}
		// Bullet тип переменной(то есть класса Bullet) которая называется bullet. 
		// Эта строчка создает пулю по шаблону(PackedScene) Instantiate отвечает за создать
		Bullet bullet = Pupulka.Instantiate<Bullet>();
		// родитель босс GetParent() рожает .AddChild(bullet)
		GetParent().AddChild(bullet);
		// пуля спавнится внутри босса
		bullet.GlobalPosition = GlobalPosition;
		// ?? пуля летит?
		// направление так сделано, чтобы можно было добавить погрешность для тройного выстрела
		bullet.Direction = direction;
		// всегда смотреть носом на игрока
		bullet.Rotation = direction.Angle();
	}
}
