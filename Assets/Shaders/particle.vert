#version 330 core

layout(location = 0) in vec2 aPos;
layout(location = 1) in vec4 aColor;
layout(location = 2) in float aSize;

out vec4 vColor;

uniform mat4 u_ViewProj;

void main()
{
    vColor = aColor;
    gl_Position = u_ViewProj * vec4(aPos, 0.0, 1.0);
    gl_PointSize = aSize;
}