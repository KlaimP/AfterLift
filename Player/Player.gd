extends CharacterBody2D

# Скорость движения игрока
@export var speed: float = 200.0;
@export var acceleration: float = 2000.0;
@export var friction: float = 3000.0;
@export var dash_speed: float = 600.0;

# Здоровье и выносливость игрока
@export var max_health: int = 100;
var current_health: int = max_health;

@export var max_stamina: int = 100;
var current_stamina: int = max_stamina;

# Длительность и перезарядка рывка
@export var dash_duration: float = 0.2;
@export var dash_cooldown: float = 0.3;

@onready var sprite: Sprite2D = $Sprite2D

# Приватные переменные для управления состоянием игрока
var is_dash: bool = false;
var dash_direction: Vector2 = Vector2.ZERO;
var is_invincible: bool = false;
var can_dash: bool = true;

func _physics_process(delta):
	var input_vector = Input.get_vector("move_left", "move_right", "move_up", "move_down");
	var target_velocity = input_vector * speed;

	# Поворот спрайта в зависимости от направления движения навайбкодил
	var target_rotation := 0.0;

	if velocity.x > 0:
		target_rotation = deg_to_rad(4);
	elif velocity.x < 0:
		target_rotation = deg_to_rad(-4);


	sprite.rotation = lerp(sprite.rotation, target_rotation, 12 * delta)

	if Input.is_action_just_pressed("dash"):
		dash(input_vector);
	
	if is_dash:
		velocity = dash_direction.normalized() * dash_speed;
		sprite.rotation = lerp(sprite.rotation, dash_direction.x * deg_to_rad(20), 20 * delta);
	elif input_vector != Vector2.ZERO:
		velocity = velocity.move_toward(target_velocity, acceleration * delta);
	else:
		velocity = velocity.move_toward(Vector2.ZERO, friction * delta);

	move_and_slide();

func dash(direction: Vector2) -> void:
	if is_dash or not can_dash:
		return;

	if direction == Vector2.ZERO:
		return;
	
	can_dash = false;
	is_dash = true;
	is_invincible = true;
	dash_direction = direction.normalized();

	print("Dashing in direction: ", dash_direction);
	await get_tree().create_timer(dash_duration).timeout;

	is_dash = false;
	is_invincible = false;

	await get_tree().create_timer(dash_cooldown).timeout;

	can_dash = true;

	# Я хз, нужна ли нам выносливость, но я оставлю это закомментированным на всякий случай
	#if current_stamina >= 20:
	#	current_stamina -= 20
	#else:
	#	print("Not enough stamina to dash.")

func take_damage(amount: float) -> void:
	if is_invincible:
		return;

	current_health -= amount;

	if current_health <= 0:
		die();

func die() -> void:
	queue_free();
