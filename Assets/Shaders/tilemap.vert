#version 330 core

layout (location = 0) in vec2 aPos;
layout (location = 1) in vec2 aTex;

uniform mat4 u_Model;
uniform mat4 u_ViewProj;

out vec2 vTex;

void main()
{
    vTex = aTex;
    gl_Position = u_ViewProj * u_Model * vec4(aPos, 0.0, 1.0);
}