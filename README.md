# Game-Ai-Hw4

1) The weights of the three steering behaviors are as follows:

Cohesion: 3f (f is for float)
Separation: 1.5f
VelocityMatch/Align: 1.5f

Ultimately the way I arrived at these weights are: 

If I felt that the boids were too clumped I would either decrease cohesion and/or increase separation.

If I felt that they were not reacting enough to each other and swarming I would increase cohesion and/or Velocity match.

If I felt that the boids were too far apart I would either decrease Separation and/or increase cohesion.

Basically it was a balancing act between these goals and whatever made a more satisfying flocking behavior I kept.

Using reasoning it makes sense that cohesion and separation would be on a 2:1 ratio because every object is going to
apply separation so if two boids are near each other it is effectively double the separation and so on for as many are near
enough. Cohesion needs to be large to counteract how strong separation is. At the same time it is good that separation is strong
to ensure that they never form a full ball and retreat to a realistic distance between boids. Finally I treated velocitymatch as
like a small influencing force so there was no need for it to be all too large. While the weight of velocitymatch is the same as
separation, the effective influence is much smaller.

2) Part 2

To handle avoiding a group of agents for part 2 I just used the basic cone check and collision prediction algorithms on each boid 
in a flock while also maintaining the flocking behavior. The collision avoidance took the highest priority out of the vying
steering behaviors, but given how he flocking performed I was confident that after avoiding the boids would properly flock after.

The pathfinding weight is 1.5f
The collisionAvoidance weight is 15f

Cohesion, velocityMatch, and pathfinding synergize well. They Will ultimately pull in the same or similar direction pulling the 
flock together. To counteract this, collision is taken at the highest priority forcing its direction over anything else. Then, 
once the collision potential is over, the strong pull of the flocking and path bring it back into formation.

There was no real separation algorithm outside of regular flocking separation. Just individual avoidance.
