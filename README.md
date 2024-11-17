# DodgeGunners
by Pau Fusco and Júlia Serra

## Links 
Link to GitHub: https://github.com/PauFusco/DodgeGunners
Link to Release: https://github.com/PauFusco/DodgeGunners/releases/tag/v1.0

## Contributions
We have worked on some tasks together, so both names will appear on some of the tasks

### Menu
- UI: Pau and Júlia
- Menu Flow: Pau
- Menu Script: Pau
- Create Lobby: Pau
- Join Lobby: Pau
- Show Host IP: Júlia
- Enable Starting game when both players have joined: Júlia
- Copy IP: Júlia

### Level
- Score: Júlia
- Timer: Júlia
- UI: Júlia
- Passing Usernames: Pau
- Player Movement: Júlia
- Updating and Recieving Movement: Pau
- Sending Movement: Pau
- Solving Bugs: Pau and Júlia

### Others:
- Optimization: Pau and Júlia
- Debugging: Pau and Júlia

## Instructions
To move, use WASD:
- W: Up
- A: Left
- S: Down
- D: Right

## Name of the main scene to run: 
MenuScene

## List of difficulties, comments and bugs
### Known bugs
- Build does not take in account the window size, so depending on the proportions you may not see one or both characters as they will be outside the window 

### Difficulties
Here is a list of solved bugs:

#### Player appearing in incorrect position
Solved by Júlia. 
The position that was sent as a string and converted to a Vector3 had an issue, as the float.Parse() was changing decimals into integers.
Solved by adding "CultureInfo.InvariantCulture" to the function.

#### Both players appearing in the same position
Solved by Pau.
Issue in Start() of the PlayerManager

### For next delivery
We have agreed on leaving this features for next delivery:
- Shoot: Although we started code to shoot and there is a prefab created already, we have agreed to implement it once packets work correctly
- Score: We have stated that score will be implemented after implementing shooting, as score will be determined by bullet collision