using Godot;
using System.Threading.Tasks;

public partial class Player : CharacterBody2D
{
    // Скорость движения игрока
    [Export] public float Speed = 200.0f;
    [Export] public float Acceleration = 2000.0f;
    [Export] public float Friction = 3000.0f;
    [Export] public float DashSpeed = 600.0f;

    // Здоровье и выносливость игрока
    [Export] public float MaxHealth = 100.0f;
    private float _currentHealth;

    [Export] public int MaxStamina = 100;
    private int _currentStamina;

    // Длительность и перезарядка дэша
    [Export] public float DashDuration = 0.2f;
    [Export] public float DashCooldown = 0.3f;
    [Export] public float BulletInvincibilityDuration = 0.5f;

    private Sprite2D _sprite;

    // Состояния игрока
    private bool _isDash = false;
    private Vector2 _dashDirection = Vector2.Zero;
    private bool _isDashInvincible = false;
    private bool _isBulletInvincible = false;
    private bool _canDash = true;

    public override void _Ready()
    {
        _sprite = GetNode<Sprite2D>("Sprite2D");

        _currentHealth = MaxHealth;
        _currentStamina = MaxStamina;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float)delta;

        Vector2 inputVector = Input.GetVector(
            "move_left",
            "move_right",
            "move_up",
            "move_down"
        );

        Vector2 targetVelocity = inputVector * Speed;

        // Поворот спрайта
        float targetRotation = 0.0f;

        if (Velocity.X > 0)
            targetRotation = Mathf.DegToRad(4);
        else if (Velocity.X < 0)
            targetRotation = Mathf.DegToRad(-4);

        _sprite.Rotation = Mathf.Lerp(_sprite.Rotation, targetRotation, 12 * dt);

        // Дэш
        if (Input.IsActionJustPressed("dash"))
            Dash(inputVector);

        // Движение
        if (_isDash)
        {
            Velocity = _dashDirection.Normalized() * DashSpeed;
            _sprite.Rotation = Mathf.Lerp(
                _sprite.Rotation,
                _dashDirection.X * Mathf.DegToRad(20),
                20 * dt
            );
        }
        else if (inputVector != Vector2.Zero)
        {
            Velocity = Velocity.MoveToward(targetVelocity, Acceleration * dt);
        }
        else
        {
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * dt);
        }

        MoveAndSlide();
    }

    // Выполнение дэша
    private async void Dash(Vector2 direction)
    {
        if (_isDash || !_canDash)
            return;

        if (direction == Vector2.Zero)
            return;

        _canDash = false;
        _isDash = true;
        _isDashInvincible = true;
        _dashDirection = direction.Normalized();

        GD.Print($"Dashing in direction: {_dashDirection}");

        await ToSignal(GetTree().CreateTimer(DashDuration), SceneTreeTimer.SignalName.Timeout);

        _isDash = false;
        _isDashInvincible = false;

        await ToSignal(GetTree().CreateTimer(DashCooldown), SceneTreeTimer.SignalName.Timeout);

        _canDash = true;

        /*
        if (_currentStamina >= 20)
            _currentStamina -= 20;
        else
            GD.Print("Not enough stamina to dash.");
        */
    }

    // Получение урона
    public async void TakeDamage(float amount)
    {
        if (_isDashInvincible || _isBulletInvincible)
            return;

        _currentHealth -= amount;

        if (_currentHealth <= 0)
        {
            Die();
            return;
        }

        _isBulletInvincible = true;

        await ToSignal(
            GetTree().CreateTimer(BulletInvincibilityDuration),
            SceneTreeTimer.SignalName.Timeout
        );

        _isBulletInvincible = false;
    }

    private void Die()
    {
        // TODO: Добавить анимацию смерти
        QueueFree();
    }
}