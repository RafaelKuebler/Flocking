# Unity3D Flocking using Craig Reynolds' Boids


2D implementation of [Craig Reynolds' boids](http://www.cs.toronto.edu/~dt/siggraph97-course/cwr87/) in the Unity3D game engine.

<img alt="150 boids" src="https://user-images.githubusercontent.com/9216979/45744864-d3d7f400-bbff-11e8-9e3e-0bee1d2f5865.gif" width="500">

The boid's emergent flocking behaviour is caused by [3 rules](http://www.red3d.com/cwr/boids/):

* **Alignment**: steer towards the direction nearby boids are heading
* **Cohesion**: steer to move toward the average position of local flockmates
* **Separation**: steer to avoid colliding or getting too close to another boid

## Built With

* [Unity3D](https://unity3d.com/) - Game Engine

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

* Inspiration: [Processing Flocking implementation](https://processing.org/examples/flocking.html)
