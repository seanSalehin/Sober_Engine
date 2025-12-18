#version 330 core
in vec4 vColor;
out vec4 FragColor;
uniform float u_Time;

void main()
{
    float pulse = abs(sin(u_Time));
    FragColor = vec4(vColor.rgb * pulse, vColor.a);
}
