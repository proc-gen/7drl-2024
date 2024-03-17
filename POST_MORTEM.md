# Post-Mortem

## Project Info

Start Time: 3-3-2024 7:30PM EST
End Time: 3-10-2024 1:00PM EST
Status: Success

## Things Left Out Entirely

- Mission Selection
  - Not enough time
- Pause Menu
  - Seemed unnecessary, but would be nice to have an option to end the run without having to start a new game
- Settings Menu
  - No sound/music and no need for display options eliminated any reason to include this
- Sound/Music
  - Partially a time issue
  - I also wanted to keep this entire project open source, including any assets I used, so the time issue of making these on my own was self inflicted
- Mage Characteristics
  - Another time issue
  - I probably could have included a small number of these had I pushed through the next 6.5 hours I had, but I was mentally done after having written 7.5k-ish lines of code in the last week

## Things Changed From Original Idea

- Magic Elements
  - Originally there were 6 elements planned, but only 3 made it into the final version due to time constraints
  - Earth and Air elements would have added skill types that could create/destroy walls, which would add a whole new way to play along with requiring much better enemy logic than just doing something if the player is in sight
  - Similarly, Water would have added a status effect and a variation to AOE
  - Fire, Ice, and Lightning had the best cross-over for implementation and allowed me to be more efficient with my time
- Element-Styled Enemies
  - Tombs do have elemental enemies based on the mage's element, but they only affect the base stats of the enemy
  - Ideally, they would have had a simple skill of the element type, even if it was only imbuement
- Non-Elemental Enemies
  - Originally not considered, but became necessary due to a lack of thought/preparation around making all enemies based on the mage's element
- Resistances
  - All damage reduction comes from armor, so when a fire mage should be resistant to fire, they don't get any bonus reduction
- Stat usage
  - Dexterity 
	- This stat as a whole feels like it's wasted. It does increase critical hit chance, but there's no dodge mechanic or penalty for wearing heavier armor. There's no chance to hit. Once it's high enough to equip the highest bow, it's a waste to continue adding points to it
  - Mana
	- Given enough time, much more balancing needed to happen here. Starting as a mage, there was so much starting mana that the player would be able to increase their intelligence plenty to not worry about regen at all by the time they acquired more expensive skills. Mana potions are wasted inventory space
  - Health
	- Same thing here with balancing. The mage originally had so little health that they could get insta-killed on the first level. There's no form of regen outside of health potions and leveling up, so it makes the gameplay style extremely cautious until the player can deal or take more damage
- Starting Choices
  - Weapons and skills were originally going to be separate choices
  - Cards for a starting class that had both already chosen looked better overall

## Things That Went Well

- Pathfinding
  - No issues at all with monsters getting stuck
  - A future iteration would make it so they can choose between a longer path to get to the player or trying to go through another enemy and waiting in line
- "Real-Time" Effects
  - Seeing a character show up and then fade away for damage or skill usage made it very clear what was being used and by whom. Felt like a fluff feature at first
- Save/Load
  - Having prior knowledge of the Arch ECS library made working with loading and saving data much easier than expected, though there were a few bumps to work through due to some of the data structures used
- Datasets
  - JSON datasets made all the data intensive stuff much easier, especially with handling different types of items

## Final Thoughts

This was a great way to force me into finishing a project without all the features I originally planned. Even just planning in general since I typically like a more "go with the flow" style. This was a bit of a hybrid of both paths in that I had planned features and I let the programming flow to those through the path of least resistance.

I'd like to make a rogue-like as an official side-project now that I've completed 7DRL. I'll probably start with making all the reusable pieces into a library instead of copy/pasting them everywhere. Things like my Arch ECS extensions, systems, mapping, pathfinding, etc. Maybe after that I can spend the next 7DRL focused on the game itself instead of all the other parts building it up as well.