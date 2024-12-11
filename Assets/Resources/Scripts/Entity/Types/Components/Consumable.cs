using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class Consumable : MonoBehaviour 
{
    // Activate the consumable (could be something like healing, etc.)
    public virtual bool Activate(Actor actor) 
    {
        // Default behavior, can be overridden for specific consumables
        return false;
    }

    // Cast the consumable on a single target (e.g., a healing potion on an enemy or ally)
    public virtual bool Cast(Actor actor, Actor target) 
    {
        // Default behavior for casting on a single target
        return false;
    }

    // Cast the consumable on multiple targets (e.g., an AoE healing or damaging consumable)
    public virtual bool Cast(Actor actor, List<Actor> targets) 
    {
        // Default behavior for casting on multiple targets
        return false;
    }

    // Consume the consumable (remove from inventory and destroy object)
    public void Consume(Actor consumer) 
    {
        // If this consumable is selected, unselect it in the inventory
        if (consumer.GetComponent<Inventory>().SelectedConsumable == this) 
        {
            consumer.GetComponent<Inventory>().SelectedConsumable = null;
        }

        // Remove the consumable from the inventory
        consumer.GetComponent<Inventory>().Items.Remove(GetComponent<Item>());
        
        // Provide feedback on the UI that the consumable was used
        UIManager.instance.AddMessage($"You used {GetComponent<Item>().name}!", "#00FF00");

        // Destroy the consumable object from the world
        Destroy(gameObject);
    }
}
