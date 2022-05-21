
# Saket.Particles
![Alt text](banner.png?raw=true "Title")
"The worlds fastest and most versatile CPU accelerated particle system in .Net". Saket.Particles is a framework for developing particle systems.

#### Why use the CPU if the GPU is faster?
Running a particle system on the cpu gives a couple of benefits:
- **Easily Extendable**. Write your own particle behaviour with a couple of lines of C#
- **Interaction with other CPU bound system**. Easily intergrate with your physics system or anything else
- **Fast**. Modern CPUs are actually really fast if you use them correctly. I've found the upper limits of this system to be around 2 milion particles at 60 fps on my r7 2700X

#### How do you know this is the fastest C# particle system?
I don't. I just couldn't find anything remotely competitive that is also open sourced. It utilizes multithreading and SIMD to achieve high performance and I couldn't find anything else doing the same. This framework was a result of me repeatedly trying to implement a particle system in a monogame project for school. Learning more and more along the way. The resulting simulation went from around 20.000 particles to 2.000.000 on the same hardware. This framework is still young and I am new to high performance computing and haven't looked in depth into the disassembly or profiler to optimize it further. 

There are some caveats when trying to develop anything high performance in an interpreted language. The JIT compiler is slow and therefore high performance is only expected after the Jitting is complete. If you can't wait for warmup to occur, maybe look into native image compiling with crossgen.

#### Roadmap
- Make the renderer more robust. Support for module selectivity.
- 3D Model renderer and 3D rotation.
- Monogame Renderer
- Veldrid Renderer
- Documentation

#### How to use the particle System?

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
    
