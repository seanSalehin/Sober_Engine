#version 330 core
in vec2 vUV;
out vec4 FragColor;

uniform sampler2D u_Font;
uniform vec4 u_Color;
uniform int u_UseTexture; // 0 = solid rect, 1 = font

void main(){
    if(u_UseTexture == 0){
        FragColor = u_Color;
        return;
    }

    // font atlas assumed white on transparent
    vec4 tex = texture(u_Font, vUV);
    FragColor = vec4(u_Color.rgb, u_Color.a * tex.a);
}