#version 330 core

in vec2 vUV;

uniform sampler2D u_Scene;

uniform int u_Horizontal;

uniform vec2 u_TexelSize; 

out vec4 FragColor;

void main()
{
    vec2 dir = (u_Horizontal == 1) ? vec2(u_TexelSize.x, 0.0) : vec2(0.0, u_TexelSize.y);

    vec4 c = texture(u_Scene, vUV) * 0.20;
    c += texture(u_Scene, vUV + dir * 1.0) * 0.15;
    c += texture(u_Scene, vUV - dir * 1.0) * 0.15;
    c += texture(u_Scene, vUV + dir * 2.0) * 0.10;
    c += texture(u_Scene, vUV - dir * 2.0) * 0.10;
    c += texture(u_Scene, vUV + dir * 3.0) * 0.07;
    c += texture(u_Scene, vUV - dir * 3.0) * 0.07;
    c += texture(u_Scene, vUV + dir * 4.0) * 0.08;
    c += texture(u_Scene, vUV - dir * 4.0) * 0.08;

    FragColor = c;
}