#version 330 core

in vec2 vTex;
out vec4 FragColor;

uniform sampler2D u_Tileset;
uniform float u_UvMinX;
uniform float u_UvMinY;
uniform float u_UvMaxX;
uniform float u_UvMaxY;

void main()
{
    vec2 uvMin = vec2(u_UvMinX, u_UvMinY);
    vec2 uvMax = vec2(u_UvMaxX, u_UvMaxY);
    vec2 uv = mix(uvMin, uvMax, vTex);
    FragColor = texture(u_Tileset, uv);
}