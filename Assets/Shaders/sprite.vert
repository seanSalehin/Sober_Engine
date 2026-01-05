#version 330 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTex;

out vec2 vTex;

uniform mat4 u_Model;

void main()
{
    gl_Position = u_Model * vec4(aPos, 1.0);
    vTex = vec2(aTex.x, 1.0 - aTex.y); 
}
