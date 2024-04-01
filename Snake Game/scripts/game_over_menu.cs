using Godot;
using System;

public partial class game_over_menu : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	[Signal]
	public delegate void RestartEventHandler();

	public void _on_restart_button_pressed()
	{
		EmitSignal(nameof(Restart));
	}
}
