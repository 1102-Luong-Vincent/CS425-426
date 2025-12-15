Between Cure and Chaos

By Team 4 -- Vincent Luong, Yuhan Tang, Shawn Meng and Sean Masterson

LINK TO DEMO BUILD FROM 12/5/25:
https://www.dropbox.com/scl/fi/8u43bgeq2lsavacjgo3d5/test-build.zip?rlkey=iasfkfutbl4rk9bkrbbezvc9g&dl=0

Features implemented for prototype build:

1. Core Turn-Based Card Battle System
What we implemented:
We built the foundational combat loop where the player can draw cards, view card details, use cards, apply effects, and end turns. This includes:
Dynamic UI for card hovering, selecting, enlarging, and discarding. This was done using our BattleCardControl.cs and BattlePlayerUIManager.cs.
Card effect parsing via the CardEffectParser enabling healing, buffs, curing debuffs, etc.
Turn progression handled through BattleManage and tested through the player test UI.
Why this was chosen for P4:
 This represents the core gameplay loop of the entire game. Since the final product relies heavily on card-based combat, implementing this system early was essential. It demonstrates meaningful end-to-end functionality: drawing → selecting → activating card effects → turn switching. It also validates the card data pipeline and UI interactions, which are high-risk components highlighted in your project document.

2. Scene Transitions, Interactables, and Exploration Foundations
What we implemented:
 We implemented interactable objects and region transitions, including:
A functional interact-prompt system, Interactable.cs, showing an “E” pop-up when the player approaches.
Three region layouts from Player’s house→ City → Hospital, allowing the player to walk, explore, and trigger events.
Scene transition logic used for the player’s house, city and hospital, where by walking close to a specific door, it will transition the player to the next region.
Player movement, collision, and map traversal.


Why this was chosen for P4:
Exploration and interaction are part of our Level 1 functional requirements. By implementing interactables early supports tutorials, introductions, and the narrative flow. It also allows teammates working on story and UI to build on top of a stable exploration loop. This showcases that the world is functional and not just the battle scene.
3. Inventory Management
What we implemented:
We built an in-game menu that allows the player to look at their inventory of cards and set their weapon, deck, and starting hand.
Dynamic UI for card hovering, selecting and discarding. This was done using our MenuCardControl script
Inventory screen buttons controlled by the InventoryUIControl script
Organization of player’s cards handled by the InventoryManager script.
Why this was chosen for P4:
 The game’s battle mechanics depend directly on the player’s card library. It was essential to get this feature functional so that the player is able to choose which cards they bring into battle.

4. Card Combination System
What we implemented:
We have implemented a card combination system, which allows the player to use their existing cards for combine. This includes:
Players can choose any two cards from their card collection and combine them into a new card. Each pair of cards has a predefined result and a fail chance by CardCombination.cs.
A functional combine process that checks if a selected pair is valid and then generates either the upgraded card or a failed result.
CardCombination.cs allows designers to update or expand combinations easily without changing the code.
Why this was chosen for P4:
The card combination mechanism is the key to supporting the growth of players' strength. As card battles are the core gameplay, allowing players to upgrade cards increases the depth of strategy and optimizes deck construction. Implementing this system as early as possible also helps verify our card data structure and demonstrate how players interact with the core role-playing mechanisms outside of battles.

