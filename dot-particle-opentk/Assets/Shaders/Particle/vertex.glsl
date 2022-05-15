#version 330 core

layout(location = 0) in vec2 quad;
layout(location = 1) in float posX;
layout(location = 2) in float posY;
layout(location = 3) in float posZ;

layout(location = 4) in float rotation;
layout(location = 5) in vec4 color;
layout(location = 6) in float lifetime;


uniform mat4 view;
uniform mat4 projection;

out vec4 vertexColor;

void main()
{/*
    mat4 a = mat4(
   1,0,0,view[1][3],
    0,1,0,view[1][3],
   0,0,1,view[2][3],
    view[3][0],view[3][1],view[3][2],view[3][3]);*/


    gl_Position = vec4(
    posX+(quad.x*cos(rotation) - quad.y*sin(rotation)),
    posY+(quad.x*sin(rotation) + quad.y*cos(rotation)),posZ,1.0) * view * projection;
    
    // todo remove if statement by multipling with alpha
    if(lifetime < 1)
        vertexColor = color;
    else
        vertexColor = vec4(0,0,0,0);
}