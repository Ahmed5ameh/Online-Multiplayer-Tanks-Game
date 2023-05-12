# Online-Multiplayer-Tanks-Game
This is a very basic online multiplayer game created in Unity3D using PhotonPUN2 for networking.

Some rules about this game:
- Only the Master client sees the "Start Game" button (dimmed at first). Other clients have a "Ready" button.
- When all clients press "Ready", the "Start Game" button gets enabled for the host, so he can start the game.
- Players can select their desired team from a list in the main menu.
- TeamId is synced across the network.
- Bullets can go through players of the same team.
- Bullets damage players of other teams only.

Classes:
- Players can select (Tank / DPS / Healer) from a list in the main menu.

- Tank has:
    - High max health.
    - Low movement speed.
    - Low damage.
        
- DPS has:
    - Very low max health.
    - High movement speed.
    - High damage.

- Healer:
    - His bullets heal teammates with a very low value.
    - He can spawn a healing range.

Healing Range Behavior:
- Only a healer can spawn a healing range.
- It heals teammates only.
- Lifetime: it's spawned for a duration then it gets destroyed.
- Healing interval: heals players in range at a very slow rate.
- Healing value: heals with a relatively high value.

PS: All the above is handled in Photon.Realtime.Player.CustomProperties

https://youtu.be/gUOXMVt9Ls8
