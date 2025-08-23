using Godot;
using System;
using System.Threading.Tasks;

public partial class Hurtbox : Area2D
{

    public void _on_area_entered(Hitbox Area2D)
    {
        var hitboxlocation = Area2D.GlobalPosition;

        var HitboxOwner = Area2D.Owner;

        if (Owner != HitboxOwner && Owner.GetType() != HitboxOwner.GetType())
        {
            var damage = Area2D.Get("Damage");
            float knockback = (float)Area2D.Get("Knockback");

            if (Owner.HasMethod("Hurt"))
            {
                GD.Print($"Hurtbox owner: {Owner.Name} hurt by {HitboxOwner.Name} ");
                Owner.Call("Hurt", damage, hitboxlocation, knockback);
                return;
            }
            else
            {
                GD.PrintErr($"{Owner.Name} não possui o método Hurt!");
            }
        }
    }
}