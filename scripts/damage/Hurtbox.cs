using Godot;

public partial class Hurtbox : Area2D
{
	public void _on_area_entered(Hitbox Area2D)
	{
		
		var HitboxOwner = Area2D.Owner;

		if (Owner != HitboxOwner && Owner.GetType() != HitboxOwner.GetType())
		{
			var damage = Area2D.Get("Damage");
			float knockback = (float)Area2D.Get("Knockback");
			GD.Print("Hurtbox Owner: " + Owner.Name + " | " + "Hitbox Owner: " + HitboxOwner.Name);

			if (Owner.Name == "mengão")
			{
				GD.Print("Owner: " + Owner.Name);
				Owner.Call("Hurt", damage);
				return;
			}
			else
			{
				var hitboxlocation = Area2D.GlobalPosition;
				Owner.Call("Hurt", damage, hitboxlocation, knockback);
				return;
			}
		}
	}
}
