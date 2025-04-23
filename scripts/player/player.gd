extends CharacterBody2D

#os @export são para permitir a edição das variaveis pelo editor e outros arquivos
@export var walk_speed = 150.0
@export_range(0, 1) var acceleration = 0.1
@export_range(0, 1) var deceleration = 0.1

@export var jump_force = -400.0
@export_range(0, 1) var decelerate_on_jump_release = 0.5

@export var dash_speed = 1000.0
@export var dash_max_distance = 300.0
@export var dash_curve: Curve # Jeito que o dash funciona
@export var dash_cooldown = 1.0

var gravity = ProjectSettings.get_setting("physics/2d/default_gravity")


#coisas para o dash
var is_dashing = false
var dash_start_position = 0 # para descobrir se atravesou a distancia maxima
var dash_direction = 0
var dash_timer = 0


func _physics_process(delta):
	#estados do jogador
	#var is_falling := velocity.y > 0.0 and not is_on_floor()
	#var is_jumping := Input.is_action_just_pressed("jump") and is_on_floor()
	##var is_idling := is_on_floor() and is_zero_approx(velocity.x)
	#var is_walking := is_on_floor() and not is_zero_approx(velocity.x)

	var sprite = $Sprite2D
	assert(sprite != null, "Esta bosta não foi encontrada.")
	print("Sprite: ",sprite)

	# Adiciona a gravidade
	if not is_on_floor():
		velocity.y += gravity * delta

	if is_on_floor():
		print("Chão")
	
	#Pulo
	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = jump_force
		print("Pulando")
	
	if Input.is_action_just_released("jump") and velocity.y < 0:
		velocity.y *= decelerate_on_jump_release
	
	#direção
	var direction = Input.get_axis("left", "right")
	
	if direction > 0:
		sprite.flip_h = false
	elif direction < 0:
		sprite.flip_h = true

	#Andar 
	if direction:
		print("Moving")
		print(direction)
		velocity.x = move_toward(velocity.x, direction * walk_speed, walk_speed * acceleration)
	else:
		velocity.x = move_toward(velocity.x, 0, walk_speed * deceleration)
	
	#Ativação do dash
	if Input.is_action_just_pressed("dash") and direction and not is_dashing and dash_timer <= 0:
		is_dashing = true
		dash_start_position = position.x
		dash_direction = direction
		dash_timer = dash_cooldown
		print("Dash")
	
	#dash em si
	if is_dashing:
		var current_distance = abs(position.x - dash_start_position)
		if current_distance >= dash_max_distance or is_on_wall():
			is_dashing = false
		#mover o personagem
		else:
			velocity.x = dash_direction * dash_speed * dash_curve.sample(current_distance / dash_max_distance)
			velocity.y = 0
			is_dashing = false
	
	#cooldown do dash
	if dash_timer > 0:
		dash_timer -= delta
	
	move_and_slide()
