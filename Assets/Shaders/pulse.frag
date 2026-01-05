#version 330 core
out vec4 FragColor;

uniform float u_Time;

void main()
{
    float pulse = abs(sin(u_Time));
    FragColor = vec4(pulse, pulse, 0.0, 1.0);
}
