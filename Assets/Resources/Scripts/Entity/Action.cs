using System;
using System.Collections.Generic;
using UnityEngine;

static public class Action 
{
  static public void WaitAction() 
  {
    GameManager.instance.EndTurn();
  }

  static public bool BumpAction(Actor actor, Vector2 direction) 
  {
    Actor target = GameManager.instance.GetActorAtLocation(actor.transform.position + (Vector3)direction);

    if (target) 
    {
      MeleeAction(actor, target);
      return false;
    } 
    else 
    {
      MovementAction(actor, direction);
      return true;
    }
  }

  static public void MovementAction(Actor actor, Vector2 direction) 
  {
    actor.Move(direction);
    actor.UpdateFieldOfView();
    GameManager.instance.EndTurn();
  }

  static public void MeleeAction(Actor actor, Actor target) 
  {
    int damage = actor.GetComponent<Fighter>().Power - target.GetComponent<Fighter>().Defense;

    string attackDesc = $"{actor.name} attacks {target.name}";

    string colorHex = "";

    if (actor.GetComponent<Player>()) 
    {
      colorHex = "#ffffff"; // white
    } 
    else 
    {
      colorHex = "#d1a3a4"; // light red
    }

    if (damage > 0) 
    {
      UIManager.instance.AddMessage($"{attackDesc} for {damage} hit points.", colorHex);
      target.GetComponent<Fighter>().Hp -= damage;
    } 
    else 
    {
      UIManager.instance.AddMessage($"{attackDesc} but does no damage.", colorHex);
    }
    GameManager.instance.EndTurn();
  }

  static public void PickupAction(Actor actor) 
  {
    for (int i = 0; i < GameManager.instance.Entities.Count; i++) 
    {
      if (GameManager.instance.Entities[i].GetComponent<Actor>() || actor.transform.position != GameManager.instance.Entities[i].transform.position) 
      {
        continue;
      }

      if (actor.Inventory.Items.Count >= actor.Inventory.Capacity) 
      {
        UIManager.instance.AddMessage($"Your inventory is full.", "#808080");
        return;
      }

      Item item = GameManager.instance.Entities[i].GetComponent<Item>();
      actor.Inventory.Add(item);

      UIManager.instance.AddMessage($"You picked up the {item.name}!", "#FFFFFF");
      GameManager.instance.EndTurn();
    }
  }

  static public void DropAction(Actor actor, Item item) 
  {
    actor.Inventory.Drop(item);

    UIManager.instance.ToggleDropMenu();
    GameManager.instance.EndTurn();
  }

  static public void UseAction(Actor consumer, Item item) 
  {
    bool itemUsed = false;

    if (item.GetComponent<Consumable>()) 
    {
      itemUsed = item.GetComponent<Consumable>().Activate(consumer);
    }

    UIManager.instance.ToggleInventory();

    if (itemUsed) 
    {
      GameManager.instance.EndTurn();
    }
  }

  static public void CastAction(Actor consumer, Actor target, Consumable consumable) 
  {
    bool castSuccess = consumable.Cast(consumer, target);

    if (castSuccess) 
    {
      GameManager.instance.EndTurn();
    }
  }
  static public void CastAction(Actor consumer, List<Actor> targets, Consumable consumable) 
  {
    bool castSuccess = consumable.Cast(consumer, targets);

    if (castSuccess) 
    {
      GameManager.instance.EndTurn();
    }
  }
}