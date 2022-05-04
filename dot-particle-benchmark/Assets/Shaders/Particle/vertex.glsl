#version 330 core

layout(location = 0) in vec2 quad;
layout(location = 1) in float posX;
layout(location = 2) in float posY;

void main()
{
    gl_Position = vec4(posX+quad.x,posY+quad.y,0.0,1.0);
}