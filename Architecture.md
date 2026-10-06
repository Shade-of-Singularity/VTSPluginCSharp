# End-points
End-point in our case - is a VTube Studio instance.

# Ticking
Some systems might require ticking.
To check for statuses for example.
Making every system manage their own Tick handler would be troublesome.
They will all need a ITickFactory supplied, so developers (including us) can change when a tick occurs.

We can solve this by providing another service - `ITickService`.
By default, it will just tick every system 10 times a second.
In Unity, we can automatically replace it with FPS-based/physics-based ticking, with some frames being skipped.
In Godot - same.
In WPF - based on a Dispatcher timer? tbh ITickService probably shouldn't tick things on a UI thread.
In Console (and probably WPF) - simple timer-based ticking.