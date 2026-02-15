# Unity First Person Controller (Unity 6)

A complete **First Person Controller** for Unity 6 using `CharacterController`.

Includes:

* Mouse look (camera rotation)
* Walking movement
* Sprinting
* Jumping
* Gravity
* Smooth acceleration
* Crouching (smooth height change)
* Ground detection
* Cursor lock system

Designed to be simple, clean, and ready for FPS games.

---

## Controls

| Action              | Key                    |
| ------------------- | ---------------------- |
| Move Forward / Back | **W / S**              |
| Move Left / Right   | **A / D**              |
| Look Around         | **Mouse**              |
| Jump                | **Space**              |
| Sprint              | **Left Shift**         |
| Crouch (hold)       | **Left Ctrl** or **C** |
| Unlock Cursor       | **Escape**             |
| Lock Cursor Again   | **Left Mouse Click**   |

---

## Requirements

* Unity 6
* CharacterController component
* Camera attached to player

No extra packages required.

---

## Setup Guide (Step by Step)

### Create Player Object

In Unity:

```
Hierarchy → Right Click → Create Empty
Rename → Player
```

Reset transform to `(0, 0, 0)`.

---

### Add CharacterController

Select **Player**

```
Inspector → Add Component → Character Controller
```

Recommended settings:

```
Height: 1.8
Center Y: 0.9
Radius: 0.3
```

---

### Add Camera

Create a camera as child of Player:

```
Right Click Player → Camera
Rename → PlayerCamera
```

Position camera:

```
X: 0
Y: 1.6
Z: 0
```

This is the head position.

---

### Add the Script

Add your **FirstPersonController.cs** script to the Player object.

```
Player → Add Component → FirstPersonController
```

---

### Assign Camera Reference

In the script inspector:

```
Camera Transform → drag PlayerCamera here
```

Very important.

---

### Play

Press **Play**.

Click inside the game view to lock the mouse.

You now have full FPS movement.

---

## Script Features Explained

### Mouse Look

* Player rotates horizontally (yaw)
* Camera rotates vertically (pitch)
* Vertical rotation is clamped to prevent flipping

---

### Movement System

* Uses CharacterController.Move()
* Smooth acceleration
* Air control when jumping
* Speed changes when sprinting or crouching

---

### Jump System

Physics based jump:

```
jump velocity = √(jumpHeight × -2 × gravity)
```

Provides consistent jump height.

---

### Gravity

Manual gravity applied every frame for realistic falling.

---

### Crouching

Smooth height interpolation:

* Standing height → crouch height
* Player collider resizes smoothly

---

### Ground Detection

Uses:

* CharacterController.isGrounded
* SphereCast fallback

More stable on slopes.

---

### Cursor Lock

* Locks automatically when playing
* Escape unlocks
* Click to lock again

Perfect for FPS gameplay.

---

## 🔧 Inspector Settings You Can Customize

| Setting           | Purpose                 |
| ----------------- | ----------------------- |
| Mouse Sensitivity | Camera rotation speed   |
| Walk Speed        | Normal movement         |
| Sprint Speed      | Faster running          |
| Crouch Speed      | Movement while crouched |
| Jump Height       | Jump power              |
| Gravity           | Fall strength           |
| Stand Height      | Player height           |
| Crouch Height     | Crouched height         |

---

## 🏗 Recommended Player Structure

```
Player
├── CharacterController
├── FirstPersonController (script)
└── PlayerCamera
```

---

## ⚠ Common Problems

### Camera not moving

You forgot to assign Camera Transform in inspector.

---

### Player floating

Gravity value too small or ground check distance too high.

---

### Can't move

Input axes missing.

Check:

```
Edit → Project Settings → Input Manager
```

Must have:

```
Horizontal
Vertical
Mouse X
Mouse Y
Jump
```

---

## Optional Improvements

You can extend this controller with:

* Head bobbing
* Footstep sounds
* FOV sprint effect
* Leaning
* Weapon system
* Stamina system
* Slide mechanic
* Ladder climbing
* New Input System support

---

## Author

Created by **Yahia Saad**

Portfolio
https://yahiawork.github.io

---

## License

Free to use in personal and commercial projects.
