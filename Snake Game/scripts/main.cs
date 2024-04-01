using Godot;
using System;
using System.Collections.Generic;


public partial class main : Node
{
	[Export]
	public PackedScene snakeScene;


	int score = 0;
	bool gameStarted = false;

	int cells = 20;
	int cellSize = 50;

	// Food variables
	private Vector2 foodPos;
	private bool regenFood = true;

	//Snake variables
	private Godot.Collections.Array oldData = new Godot.Collections.Array();
	private Godot.Collections.Array snakeData = new Godot.Collections.Array();
	private Godot.Collections.Array snake = new Godot.Collections.Array();

	private Vector2 startPos = new Vector2(9, 9);
	private Vector2 up = new Vector2(0, -1);
	private Vector2 down = new Vector2(0, 1);
	private Vector2 left = new Vector2(-1, 0);
	private Vector2 right = new Vector2(1, 0);
	private Vector2 moveDirection;
	private bool canMove = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		NewGame();

	}

	public void NewGame()
	{
		GetTree().Paused = false;
		GetNode<game_over_menu>("GameOverMenu").Hide();
		score = 0;
		GetTree().CallGroup("segments", "queue_free");
		GetNode<Label>("Hud/ScoreLabel").Text = "SCORE: " + score;
		moveDirection = up;
		canMove = true;

		MoveFood();
		GenerateSnake();
	}

	private void GenerateSnake()
	{
		oldData.Clear();
		snakeData.Clear();
		snake.Clear();
		for (int i = 0; i < 3; i++)
		{
			AddSegment(startPos + new Vector2(0, i));
		}
	}

	private void AddSegment(Vector2 pos)
	{
		snakeData.Add(pos);
		Sprite2D snakeSegment = (Sprite2D)snakeScene.Instantiate();
		snakeSegment.Position = (pos * cellSize) + new Vector2(0.5f, 0.5f) * cellSize;
		AddChild(snakeSegment);
		snake.Add(snakeSegment);
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		MoveSnake();
	}

	private void MoveSnake()
	{
		if (canMove)
		{
			if (Input.IsActionJustPressed("down") && moveDirection != up)
			{
				moveDirection = down;
				canMove = false;
				if (!gameStarted)
				{
					StartGame();
				}
			}
			if (Input.IsActionJustPressed("up") && moveDirection != down)
			{
				moveDirection = up;
				canMove = false;
				if (!gameStarted)
				{
					StartGame();
				}
			}
			if (Input.IsActionJustPressed("left") && moveDirection != right)
			{
				moveDirection = left;
				canMove = false;
				if (!gameStarted)
				{
					StartGame();
				}
			}
			if (Input.IsActionJustPressed("right") && moveDirection != left)
			{
				moveDirection = right;
				canMove = false;
				if (!gameStarted)
				{
					StartGame();
				}
			}
		}
	}

	private void StartGame()
	{
		gameStarted = true;
		Timer moveTimer = GetNode<Timer>("MoveTimer");
		if (moveTimer != null)
		{
			moveTimer.Start();
			GD.Print("Timer started. Time: ", moveTimer.WaitTime);
		}
		else
		{
			GD.Print("Timer not found.");
		}

	}

	private void _on_move_timer_timeout()
	{
		canMove = true;
		oldData = new Godot.Collections.Array(snakeData);
		snakeData[0] = (Vector2)snakeData[0] + moveDirection;
		for (int i = 0; i < snakeData.Count; i++)
		{
			if (i > 0)
			{
				snakeData[i] = oldData[i - 1];
			}
			((Sprite2D)snake[i]).Position = ((Vector2)snakeData[i] * cellSize) + new Vector2(0.5f, 0.5f) * cellSize;

		}
		CheckOutOfBounds();
		CheckSelfEaten();
		CheckFoodEaten();
	}

	private void CheckOutOfBounds()
	{
		Vector2 headPosition = (Vector2)snakeData[0];
		if (headPosition.X < 0 || headPosition.X >= cells || headPosition.Y < 0 || headPosition.Y >= cells)
		{
			GD.Print("Out of bounds");
			EndGame();
		}
	}



	private void CheckSelfEaten()
	{
		for (int i = 1; i < snakeData.Count; i++)
		{
			if ((Vector2)snakeData[0] == (Vector2)snakeData[i])
			{
				GD.Print("Self eaten");
				EndGame();
			}
		}
	}
	private void CheckFoodEaten()
	{
		if ((Vector2)snakeData[0] == foodPos)
		{
			score += 1;
			GetNode<Label>("Hud/ScoreLabel").Text = "SCORE: " + score;
			AddSegment((Vector2)oldData[oldData.Count - 1]);
			MoveFood();
		}
	}
	private void MoveFood()
	{
		while (regenFood)
		{
			regenFood = false;
			foodPos = new Vector2(GD.Randi() % cells, GD.Randi() % cells);
			foreach (Vector2 pos in snakeData)
			{
				if (foodPos == pos)
				{
					regenFood = true;
					break;
				}
			}
		}
		GetNode<Sprite2D>("Food").Position = (foodPos * cellSize) + new Vector2(0, cellSize);
		regenFood = true;
	}

	private void EndGame()
	{
		GetTree().Paused = true;
		GetNode<game_over_menu>("GameOverMenu").Show();
		GetNode<Timer>("MoveTimer").Stop();
		gameStarted = false;
	}

	private void _on_game_over_menu_restart()
	{
		NewGame();
	}
}
