using Godot;
using System;

namespace DodgeTheCreepsCS
{

public partial class Player : Area2D
{
    [Signal]
    public delegate void HitEventHandler();

    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).
    
    private Vector2 _screenSize; // Size of the game window.

    public void Start(Vector2 pos)
    {
        GD.Print("Player.Start: 初始化玩家位置");
        Position = pos;
        GD.Print("Player.Start: 显示玩家");
        Show();
        GD.Print("Player.Start: 启用碰撞检测");
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }

    public override void _Ready()
    {
        _screenSize = GetViewportRect().Size;
        Hide();
    }

    public override void _Process(double delta)
    {
        Vector2 velocity = Vector2.Zero; // The player's movement vector.
        
        if (Input.IsActionPressed("move_right"))
            velocity.X += 1;
        if (Input.IsActionPressed("move_left"))
            velocity.X -= 1;
        if (Input.IsActionPressed("move_down"))
            velocity.Y += 1;
        if (Input.IsActionPressed("move_up"))
            velocity.Y -= 1;

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();
        }
        else
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Stop();
        }

        Position += velocity * (float)delta;
        Position = Position.Clamp(Vector2.Zero, _screenSize);

        if (velocity.X != 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "walk";
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipV = false;
            // See the note below about boolean assignment.
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = velocity.X < 0;
        }
        else if (velocity.Y != 0)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Animation = "up";
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipV = velocity.Y > 0;
        }
    }

    private void _on_body_entered(Node2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
    }
}
}