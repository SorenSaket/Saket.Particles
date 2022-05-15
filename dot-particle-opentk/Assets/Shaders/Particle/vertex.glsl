#version 330 core

layout(location = 0) in vec2 quad;
layout(location = 1) in float posX;
layout(location = 2) in float posY;
layout(location = 3) in float posZ;

layout(location = 4) in float rotation;

layout(location = 5) in float scaleX;
layout(location = 6) in float scaleY;

layout(location = 7) in vec4 color;
layout(location = 8) in float lifetime;


uniform mat4 view;
uniform mat4 projection;

out vec4 vertexColor;

void main()
{
    // Billboarding
    // http://www.opengl-tutorial.org/intermediate-tutorials/billboards-particles/billboards/
    vec3 CameraRight_worldspace = vec3(view[0][0], view[0][1], view[0][2]);
    vec3 CameraUp_worldspace = vec3(view[1][0], view[1][1], view[1][2]);


    float vx = quad.x*scaleX;
    float vy = quad.y*scaleY;

    gl_Position = vec4( 
    vec3(posX,posY,posZ)  // World Space Position Of Particle
    + CameraRight_worldspace * (vx*cos(rotation) -vy*sin(rotation))  + // X position of quad
    CameraUp_worldspace * (vx*sin(rotation) + vy*cos(rotation)), // Y position of quad
    1.0) * view * projection; 
    

    /*
    gl_Position = vec4(
    posX+(quad.x*cos(rotation) - quad.y*sin(rotation)),
    posY+(quad.x*sin(rotation) + quad.y*cos(rotation)),
    posZ,
    1.0) * view * projection;*/
    
    // todo remove if statement by multipling with alpha
    if(lifetime < 0.999)
        vertexColor = color;
    else
        vertexColor = vec4(0,0,0,0);
}