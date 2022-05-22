#version 330 core

in vec4 vertexColor;
in vec2 vertexUV;
flat in uint vertexTexIndex;
flat in float vertexLifetime;
out vec4 outColor;

uniform sampler2DArray texture0;

mat4 thresholdMatrix = mat4(
1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
);


void main()
{
    if(vertexLifetime > 0.999)
        discard;
    
    if(vertexColor.w - thresholdMatrix[int(mod(gl_FragCoord.x , 4))][int(mod(gl_FragCoord.y , 4))] < 0)
       discard;

    vec4 col = texture(texture0, vec3(vertexUV, vertexTexIndex)) * vertexColor;
    
    if(col.w < 0.1)
        discard;
    outColor = col;
}