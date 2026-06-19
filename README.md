# Looga Blackboard

Looga Blackboard is a small typed runtime state system for Unity. It lets systems share gameplay state without string keys, static one-off fields, or direct references between unrelated systems.

Use it for state that many systems may need to read, such as scene type, menu state, player status, interaction availability, raid flags, or other lightweight game conditions.

## Core Idea

The blackboard has three parts:

- **Blackboard Definition**: a ScriptableObject asset that organizes all available keys.
- **Blackboard Key**: a typed ScriptableObject key, such as `IsRaidScene`, `PlayerIsDead`, or `CurrentSceneType`.
- **Runtime Blackboard**: the in-memory value store used while the game is running.

Keys are assets, not strings. This keeps references strong, refactorable, and designer-friendly.

## Main Types

`LoogaBlackboardDefinition`
: Defines the available keys for a project or feature. Keys are grouped by type: bool, int, float, and string.

`LoogaBlackboardKey`
: A typed key asset. Each key has a display name, locked value type, and optional description.

`LoogaBlackboard`
: Runtime storage for key/value pairs. Implements both read and write interfaces.

`ILoogaBlackboardReader`
: Read-only access. Use this when a system should inspect state but not mutate it.

`ILoogaBlackboardWriter`
: Write access. Use this when a system publishes state.

`LoogaBlackboardRegistry`
: Static access to the active runtime blackboard. This is the easiest path for normal game code.

## Creating Keys

1. Create a blackboard definition:
   `Create > LoogaSoft > Blackboard > Definition`

2. Select the blackboard definition asset.

3. Use the typed key lists to add keys:
   - `Add Bool Key`
   - `Add Int Key`
   - `Add Float Key`
   - `Add String Key`

4. Save the key near the blackboard asset when prompted.

5. Give the key a clear name and description.

Example keys:

- `Is Raid Scene`
- `Is Station Scene`
- `Any Menu Open`
- `Player Can Move`
- `Current Inventory Mode`

Key creation and deletion are disabled in Play Mode.

## Runtime Setup

Create a runtime blackboard and register it as active:

```csharp
using LoogaSoft.Blackboard;
using UnityEngine;

public sealed class BlackboardInstaller : MonoBehaviour
{
    private readonly LoogaBlackboard _blackboard = new();

    private void Awake()
    {
        LoogaBlackboardRegistry.SetActive(_blackboard);
    }

    private void OnDestroy()
    {
        LoogaBlackboardRegistry.ClearActive(_blackboard);
    }
}
```

Only one blackboard should normally be active for the current gameplay session.

## Writing Values

For most gameplay code, use the registry shortcuts:

```csharp
using LoogaSoft.Blackboard;
using UnityEngine;

public sealed class SceneStatePublisher : MonoBehaviour
{
    [SerializeField] private LoogaBlackboardKey _isRaidSceneKey;

    private void Start()
    {
        LoogaBlackboardRegistry.SetBool(_isRaidSceneKey, true);
    }
}
```

Typed setters:

```csharp
LoogaBlackboardRegistry.SetBool(key, true);
LoogaBlackboardRegistry.SetInt(key, 3);
LoogaBlackboardRegistry.SetFloat(key, 0.75f);
LoogaBlackboardRegistry.SetString(key, "Station");
```

If the active blackboard is missing, the key is null, or the value type does not match the key type, writes safely do nothing.

## Reading Values

Use `GetBool`, `GetInt`, `GetFloat`, and `GetString` for almost everything:

```csharp
using LoogaSoft.Blackboard;
using UnityEngine;

public sealed class RaidOnlyObject : MonoBehaviour
{
    [SerializeField] private LoogaBlackboardKey _isRaidSceneKey;

    private void OnEnable()
    {
        bool isRaidScene = LoogaBlackboardRegistry.GetBool(_isRaidSceneKey);
        gameObject.SetActive(isRaidScene);
    }
}
```

Default values:

- Bool: `false`
- Int: `0`
- Float: `0f`
- String: `""`

These defaults are returned if the key is unset, null, the wrong type, or no active blackboard exists.

Use `TryGet...` only when the difference between "unset" and "set to default" matters:

```csharp
if (LoogaBlackboardRegistry.TryGetInt(_difficultyKey, out int difficulty))
{
    ApplyDifficulty(difficulty);
}
```

## Reader And Writer Interfaces

Use interfaces when a system should receive blackboard access through dependency injection or explicit setup.

Read-only system:

```csharp
using LoogaSoft.Blackboard;

public sealed class MenuRequirementChecker
{
    private readonly ILoogaBlackboardReader _blackboard;

    public MenuRequirementChecker(ILoogaBlackboardReader blackboard)
    {
        _blackboard = blackboard;
    }

    public bool CanOpen(LoogaBlackboardKey key)
    {
        return _blackboard.GetBool(key);
    }
}
```

Writer system:

```csharp
using LoogaSoft.Blackboard;

public sealed class PlayerStatePublisher
{
    private readonly ILoogaBlackboardWriter _blackboard;

    public PlayerStatePublisher(ILoogaBlackboardWriter blackboard)
    {
        _blackboard = blackboard;
    }

    public void SetAlive(LoogaBlackboardKey key, bool alive)
    {
        _blackboard.SetBool(key, alive);
    }
}
```

Prefer `ILoogaBlackboardReader` for UI, rules, and query-only systems. Prefer `ILoogaBlackboardWriter` only for systems that own and publish a value.

## Menu Framework Integration

Looga Menu Framework evaluates requirements through blackboard keys.

A menu rule compares:

- A blackboard key
- A comparison operation
- An expected value

Example:

- Key: `Is Raid Scene`
- Comparison: `Equals`
- Value: `true`

The menu can open only when the rule evaluates to true.

This keeps menu access rules designer-authored while letting gameplay systems publish the actual runtime state.

## Common Patterns

### Publish Scene Type

```csharp
LoogaBlackboardRegistry.SetBool(_isRaidSceneKey, sceneType == SceneType.Raid);
LoogaBlackboardRegistry.SetBool(_isStationSceneKey, sceneType == SceneType.Station);
```

### Gate Gameplay Logic

```csharp
if (!LoogaBlackboardRegistry.GetBool(_playerCanInteractKey))
    return;
```

### Clear Temporary State

```csharp
LoogaBlackboardRegistry.RemoveValue(_currentLootContainerKey);
```

### Debug Runtime Values

Select the blackboard definition in Play Mode. The custom inspector shows each key's current runtime value from the active blackboard.

## Best Practices

- Use blackboard keys for shared state, not private component internals.
- Keep key names explicit, such as `Is Raid Scene` instead of `Raid`.
- Let the owning system write a value; let other systems read it.
- Avoid using string keys in game code.
- Use `Get...` methods by default and `TryGet...` only when unset state matters.
- Do not store large data, inventories, or object graphs in the blackboard. Use dedicated systems for those.

## When Not To Use It

Do not use the blackboard for:

- High-frequency per-frame data that only one system needs.
- Large save data.
- Lists of items or complex objects.
- Direct commands or events.

For one-time notifications, use an event. For persistent ownership data, use a dedicated service. For lightweight shared state, use the blackboard.
