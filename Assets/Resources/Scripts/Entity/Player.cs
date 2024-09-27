using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Actor))]
sealed class Player : MonoBehaviour, Controls.IPlayerActions {
  private Controls controls;

  [SerializeField] private bool moveKeyHeld; //read-only

  private void Awake() => controls = new Controls();

  private void OnEnable() {
    controls.Player.SetCallbacks(this);
    controls.Player.Enable();
  }

  private void OnDisable() {
    controls.Player.SetCallbacks(null);
    controls.Player.Disable();
  }

  void Controls.IPlayerActions.OnMovement(InputAction.CallbackContext context) {
    if (context.started)
      moveKeyHeld = true;
    else if (context.canceled)
      moveKeyHeld = false;
  }

// Handle exiting the game (Escape key)
  void Controls.IPlayerActions.OnExit(InputAction.CallbackContext context) {
    if (context.performed) {
      Debug.Log("Exiting the game...");
      Application.Quit(); // This will close the application. Won't work in the editor.
    }
  }

  // Handle reloading the level (R key)
  void Controls.IPlayerActions.OnReload(InputAction.CallbackContext context) {
    if (context.performed) {
      Debug.Log("Reloading the level...");
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Reload the current scene
    }
  }

  public void OnView(InputAction.CallbackContext context) {
    if (context.performed)
      UIManager.instance.ToggleMessageHistory();
  }

  private void FixedUpdate() {
    if (!UIManager.instance.IsMessageHistoryOpen) {
      if (GameManager.instance.IsPlayerTurn && moveKeyHeld && GetComponent<Actor>().IsAlive) {
        MovePlayer();
      }
    }
  }

  private void MovePlayer() {
    Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
    Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
    Vector3 futurePosition = transform.position + (Vector3)roundedDirection;

    if (IsValidPosition(futurePosition))
      moveKeyHeld = Action.BumpAction(GetComponent<Actor>(), roundedDirection); //If we bump into an entity, moveKeyHeld is set to false.
  }

  private bool IsValidPosition(Vector3 futurePosition) {
    Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(futurePosition);
    if (!MapManager.instance.InBounds(gridPosition.x, gridPosition.y) || MapManager.instance.ObstacleMap.HasTile(gridPosition) || futurePosition == transform.position)
      return false;

    return true;
  }

    public void OnReload(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }
}