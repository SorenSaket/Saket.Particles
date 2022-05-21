# Dot Particle
## The worlds fastest and most versatile CPU accelerated particle system in .Net  

### Why use the CPU if the GPU is faster?
Running a particle system on the cpu gives a couple of benefits:
- **Easily Extendable**. Write your own particle behaviour with a couple of lines of C#
- **Interaction with other CPU bound system**. Easily intergrate with your physics system or anything else
- **Fast**. Modern CPUs are actually really fast if you use them correctly. I've found the upper limits of this system to be around 2 milion particles at 60 fps
    
### How do you know this is the fastest C# particle system?
I don't. I just couldn't find anything remotely competitive that is also open sourced.




### How to use the particle System?

    // ---- Startup ----
    // Create a new particle system
    // The number of particles must be predetermined by artist
    _particleSystem = new SimulatorCPU(256, new IModule[]
    {
        // Add the wanted Modules
        new ModuleLifetime(),
        new ModulePosition(),
        new ModuleRotation(),
        new ModuleVelocity(),
        new ModuleColor()
    });

    // 
    var emitterSettings = new EmitterSettings()
    {
        RateOverTime = 16,
    };
    // Create a new emitters
    emitter = new MyCustomEmitter(emitterSettings, _particleSystem);

    renderer = new RendererOpenTK(_particleSystem);

    _particleSystem.Play();
    emitter.Start();


    // ---- Update ----
    emitter.Update(DeltaTime);

    // ---- Render ----
    renderer.Draw();
    



