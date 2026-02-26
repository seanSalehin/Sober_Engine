#version 330 core

in vec2 vUV;

uniform sampler2D u_Scene;

out vec4 FragColor;

void main()
{
    FragColor = texture(u_Scene, vUV);
}