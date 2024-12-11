using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour 
{
  [SerializeField] private int capacity = 0;
  [SerializeField] private Consumable selectedConsumable = null;
  [SerializeField] private List<Item> items = new List<Item>();
  
  
  public int Capacity { get => capacity; }
  public Consumable SelectedConsumable { get => selectedConsumable; set => selectedConsumable = value; }
  public List<Item> Items { get => items; }

  public void Add(Item item) 
  {
    items.Add(item);
    item.transform.SetParent(transform);
    GameManager.instance.RemoveEntity(item);
  }

  public void Drop(Item item) 
  {
    items.Remove(item);
    item.transform.SetParent(null);
    GameManager.instance.AddEntity(item);
    UIManager.instance.AddMessage($"You dropped the {item.name}.", "#FF0000");
  }
}

/******************************************************************************
[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour 
{
    [SerializeField] private int capacity = 10;  // Default capacity of the inventory
    [SerializeField] private Consumable selectedConsumable = null;
    [SerializeField] private List<Item> items = new List<Item>();

    public int Capacity { get => capacity; }
    public Consumable SelectedConsumable { get => selectedConsumable; set => selectedConsumable = value; }
    public List<Item> Items { get => items; }

    // Add an item to the inventory
    public void Add(Item item) 
    {
        if (items.Count < capacity) 
        {
            items.Add(item);
            item.transform.SetParent(transform);
            GameManager.instance.RemoveEntity(item);
            UIManager.instance.AddMessage($"You picked up {item.name}.", "#00FF00");
        } 
        else 
        {
            UIManager.instance.AddMessage("Your inventory is full.", "#FF0000");
        }
    }

    // Drop an item from the inventory
    public void Drop(Item item) 
    {
        if (items.Contains(item)) 
        {
            items.Remove(item);
            item.transform.SetParent(null);
            GameManager.instance.AddEntity(item);
            UIManager.instance.AddMessage($"You dropped the {item.name}.", "#FF0000");
        }
    }

    // Use the selected consumable (if applicable)
    public void UseConsumable() 
    {
        if (selectedConsumable != null) 
        {
            selectedConsumable.Use();  // Assuming the Consumable class has a Use() method
            Remove(selectedConsumable);  // Remove the consumable after use
            selectedConsumable = null;  // Clear the selected consumable
        }
        else 
        {
            UIManager.instance.AddMessage("No consumable selected!", "#FF0000");
        }
    }

    // Remove an item from the inventory without dropping it in the world
    private void Remove(Item item) 
    {
        items.Remove(item);
        Destroy(item.gameObject);  // Optionally destroy the item object
    }

    // A helper method to check if an item exists in the inventory
    public bool Contains(Item item) 
    {
        return items.Contains(item);
    }
}
*/