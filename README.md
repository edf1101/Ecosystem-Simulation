# Ecosystem-Simulation

<img width="1354" alt="Screenshot 2023-08-07 at 10 56 45" src="https://github.com/edf1101/Ecosystem-Simulation/assets/96292907/cb8236fa-e272-4eca-b793-e70908e665d0">


This was my Computer Science A Level NEA.
(Note this isnt the final version I submitted as I did a lot of refactoring and code cleanup, though the functionality is the same)
I created a 200m*200m map featuring multiple biomes, terrain types and water features in a procedurally generated low poly map using:
- Poisson Disk sampling
- Delaunay Triangulation
- Perlin noise
- Floodfill algorithms

Multiple species of animals are then spawned in and populate the map. The animals are all capable of:
- Eating
- Drinking
- Hunting
- Avoiding predators
- Reproducing

Data is saved along the course of the simulation and can be viewed at the end in a graph viewer or exported to a .csv file
