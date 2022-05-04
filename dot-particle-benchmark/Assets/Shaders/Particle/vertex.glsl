#version 330 core

layout(location = 0) in float posX;
layout(location = 1) in float posY;


void main()
{
    gl_Position = vec4(posX,posY,0.0,1.0);
}