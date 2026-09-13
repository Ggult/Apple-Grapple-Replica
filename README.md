# Apple Grapple Replica

## Technical Decisions

This README focuses on implementation decisions and extensions added beyond the core case requirements.

### Circular Country Flags

Character country flags are displayed above the characters. The provided flag images were rectangular, while the reference material used circular icons.

Instead of adding Unity UI `Mask` components, the project uses a custom shader (`AppleGrapple/UICircleMask`) that clips the rectangular texture into a circle in the fragment stage. This avoids the additional stencil pass and batching impact introduced by UI masks.

Another option would have been to permanently edit the provided PNG files in an external tool. The shader approach keeps the original assets unchanged and preserves the integrity of the supplied project files.

### Character Sprite Atlas

The character body and leg sprites are packed into a Sprite Atlas:

`Assets/UI/textures/character/New Sprite Atlas.spriteatlasv2`

This reduces texture switches and helps avoid unnecessary batches when the character parts are rendered together.

### Reusable Configuration Assets

Gameplay values are separated from MonoBehaviour logic and stored in ScriptableObject assets. This makes balancing possible from the Unity Inspector without changing code.

### Rendering and Feedback

- `SpriteFlash.shader` provides the hit-flash effect without replacing sprite textures.
- Character UI is hosted under a shared `CharacterInfoCanvas` instead of creating a separate canvas for every character.
- Character status UI is event-driven through health changes and damage events.
- Pickup and sword objects use pooling to reduce repeated Instantiate/Destroy operations during gameplay.
- The scratch-card package is used as a gameplay ground-reveal effect. Scratch input is disabled; character and sword positions drive the scratch operation instead.

## Configurable Settings

The following ScriptableObject configurations can be adjusted in the Unity Inspector.

### CharacterConfig

Location: `Assets/Scripts/Config/CharacterConfig.cs`

- `movementSpeed`: Character movement speed.
- `maxHealth`: Maximum health value.
- `knockbackForce`: Force applied after receiving a hit.
- `knockbackDuration`: Duration of the knockback reaction.
- `aiSmartValue`: AI decision-quality parameter.

### MapConfig

Location: `Assets/Scripts/Config/MapConfig.cs`

- `tileWorldSize`: World-space size of each map tile.
- `GridX`, `GridY`: Map dimensions.
- `mapTileVariants`: Available tile sprites and their selection chances.
- `defaultTileSprite`: Fallback tile sprite.
- `boundarySettings`: Fence sprites, fence offset, and stretched fence dimensions.

### WeaponConfig

Location: `Assets/Scripts/Config/WeaponConfig.cs`

- `weaponPrefab`: Sword prefab used by the weapon system.
- `damage`: Damage dealt by a sword hit.
- `rehitCooldown`: Minimum time before the same target can be hit again.
- `clashPriorityRadius`: Radius used when resolving sword clashes.

### PickupConfig

Location: `Assets/Scripts/Config/PickupConfig.cs`

- `prefab`: Pickup prefab to spawn.
- `poolSize`: Initial size of the pickup object pool.
- `maxAliveCount`: Maximum number of active pickups.
- `initialSpawnCount`: Number of pickups created at match start.
- `minSpawnInterval`, `maxSpawnInterval`: Random pickup spawn interval range.

## Project Notes

- The project uses Unity's New Input System.
- Character movement is shared between player and AI through an input-provider abstraction.
- The map is generated from configuration data and includes physical boundary colliders in addition to the visual fence.
- Character identities include a nickname and country code, which are used by the flag and character information UI.

## Running the Project

1. Open the project with the Unity version specified in `ProjectSettings/ProjectVersion.txt`.
2. Open the main scene under `Assets/Scenes/`.
3. Enter Play Mode.
4. Use mouse drag in the Unity Editor, or touch drag on a supported device, to move the player.
