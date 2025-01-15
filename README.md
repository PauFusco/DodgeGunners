# DodgeGunners
by Pau Fusco and Júlia Serra

## Links 
Link to GitHub: https://github.com/PauFusco/DodgeGunners
Link to Release: 

## Description
Dodge Gunners is a multiplayer online game developed for the subject "Networks and Online Games". 

Each game consists in 10-seconds rounds with one bullet per player. Each round ends after 10 seconds, or when one player recieves damage.

Once one player gets hit, the other one scores a point. The player that scores 3 points wins!

## Networking aspects of the game
Dodge Gunners is a peer-to-peer (P2P) videogame that sents the player's information to the other through packets using UDP connection.
To pass the information correctly, we have created two packets:
- Player, that we use to send and recieve position, rotation (quaternion), animation state, hp (health point) and score
- Projectile, that we use to send and recieve position, rotation (quaternion) 

## Contributions
We have worked together, so both names will appear on some of the tasks:

### Player
- Implementation of animations (animator/animations): Júlia
- Implementation of animations (scripting): Júlia
- Creation and Implementation of AnimationState: Júlia
- Implementing Animation State to packets: Júlia
- Balancing player behaviour: Pau and Júlia

### Menu
- Tutorial: Júlia
- Implementing assets: Júlia

### Level
- Winner's name: Pau
- Menu controller: Pau
- Implementing assets in level: Júlia
- Implementing assets to player: Júlia
- Implementing assets to projectiles: Pau
- Updating UI: Pau
- Restart after scoring: Pau

## Design
- Updating game's design: Pau

### Others:
- Optimization: Pau and Júlia
- Debugging: Pau and Júlia

## Improvements
### Design
We changed the game's design in order to improve it and made it more engaging. 

First, we changed the round's behaviour. We decided to change it so once one player recieves hit, the other score and the round starts. This made each round faster and engaging, as it depends in how fast each player shoots.

Then, we changed each round to last 10 seconds, and once it ends and if no player has killed the other, round restarts.

Finally we restricted the player's bullets to one per round, to add that component to engage more players.

Regarding the visual aspects, we have used assets from teh Unity Asset Store to add a visual component to the palyer behaviours, and create an aesthetic for the game.

### Bugfixes
Most of the bugs were related to gameplay:
- Bullets not detecting player correctly: Júlia
Issue when passing the health with healtbars. Collisions affected how the player and rounded behaved.
Fixed issue in projectile behaviour. 
Changes made: Added component rigidbody to bullet, made player's rigidbody collision detection continuos, passed through the net the health points and avoided calling the fucntion TakingDamage when detecting the trigger from the bullet (as once it detects one bullet the round restarts)

## Instructions
To move, use WASD:
- W: Jump, Double Jump
- A: Left
- S: Cancel Jump
- D: Right
- Space: Shoot

## Name of the main scene to run: 
MenuScene

## Notes
We tried to implement a code system to create rooms to avoid having to put their IP as input, as explained in the last delivery. 

The main idea was creating a server 