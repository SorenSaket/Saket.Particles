#version 330 core

in vec4 vertexColor;
in vec2 vertexUV;
flat in uint vertexTexIndex;

out vec4 outColor;

uniform sampler2DArray texture0;


void main()
{ 
    vec4 col = texture(texture0, vec3(vertexUV, vertexTexIndex)) * vertexColor;

    if(col.w < 0.01)
        discard;
    outColor = col;
}