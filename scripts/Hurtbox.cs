using Godot;
using System;

public partial class Hurtbox : Area2D
{
    public void _on_area_entered(Hitbox Area2D){
        var damage = Area2D.Get("Damage");
        if (Owner.HasMethod("Hurt"))
        {
            Owner.Call("Hurt", damage);
        }
    }
}
