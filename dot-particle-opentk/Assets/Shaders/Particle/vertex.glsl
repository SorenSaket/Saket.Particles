#version 330 core

layout(location = 0) in vec2 quad;
layout(location = 1) in float posX;
layout(location = 2) in float posY;
layout(location = 3) in float rotation;
layout(location = 4) in vec4 color;
layout(location = 5) in float lifetime;

out vec4 vertexColor;

void main()
{
    gl_Position = vec4(
    posX+(quad.x*cos(rotation) - quad.y*sin(rotation)),
    posY+(quad.x*sin(rotation) + quad.y*cos(rotation)),0.0,1.0);
    
    // todo remove if statement by multipling with alpha
    if(lifetime < 1)
        vertexColor = color;
    else
        vertexColor = vec4(0,0,0,0);
}