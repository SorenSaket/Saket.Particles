#version 330 core

in vec4 vertexColor;

out vec4 outColor;

void main()
{ 
    if(vertexColor.a < 0.1)
        discard;
    outColor = vertexColor;
}