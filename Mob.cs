using Godot;
using System;

namespace DodgeTheCreepsCS
{

public partial class Mob : RigidBody2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] mobTypes = animatedSprite.SpriteFrames.GetAnimationNames();
        animatedSprite.Play(mobTypes[new Random().Next(0, mobTypes.Length)]);
    }

    private void _on_visible_on_screen_notifier_2d_screen_exited()
    {
        QueueFree();
    }
}
}