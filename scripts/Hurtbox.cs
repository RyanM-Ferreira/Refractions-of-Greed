using Godot;
using System;

public partial class Hurtbox : Area2D
{
    public void _on_area_entered(Hitbox Area2D)
    {
        var HitboxOwner = Area2D.Owner;
        if (HitboxOwner == null)
        {
            GD.PrintErr("Hitbox owner is null.");
            return;
        }
        if (Owner != HitboxOwner)
        {
            var damage = Area2D.Get("Damage");
            float knockback = (float)Area2D.Get("Knockback");
            if (Owner.Name == "Player")
            {
                var hitboxlocation = Area2D.GlobalPosition;
                Owner.Call("Hurt", damage, hitboxlocation, knockback);
                return;
            }
            if (Owner.HasMethod("Hurt"))
            {
                Owner.Call("Hurt", damage);
            }
        }
    }
}
