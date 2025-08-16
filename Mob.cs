using Godot;
using System;

namespace DodgeTheCreepsCS
{
/// <summary>
/// 敌人类，负责敌人的行为和外观
/// </summary>
public partial class Mob : RigidBody2D
{
    #region Lifecycle Methods
    /// <summary>
    /// 初始化敌人，随机选择敌人类型并播放相应动画
    /// </summary>
    public override void _Ready()
    {
        GD.Print("Mob._Ready: 初始化敌人");
        var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] mobTypes = animatedSprite.SpriteFrames.GetAnimationNames();
        
        // 随机选择敌人类型
        int randomIndex = new Random().Next(0, mobTypes.Length);
        string selectedType = mobTypes[randomIndex];
        GD.Print($"Mob._Ready: 选择敌人类型 '{selectedType}'");
        
        animatedSprite.Play(selectedType);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// 当敌人离开屏幕时被调用，负责清理敌人实例
    /// </summary>
    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        GD.Print("Mob.OnVisibleOnScreenNotifier2DScreenExited: 敌人离开屏幕，准备销毁");
        QueueFree();
    }
    #endregion
}
}