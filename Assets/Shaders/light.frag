#version 330 core
in vec2 vTex;
out vec4 FragColor;

// Change this from u_Scene to u_Texture to match standard bindings
uniform sampler2D u_Texture; 
uniform float u_AspectRatio;

const int MAX_LIGHTS = 16;
uniform int u_LightCount;
uniform vec2 u_LightUVs[MAX_LIGHTS];
uniform vec3 u_LightColors[MAX_LIGHTS];
uniform float u_LightRadii[MAX_LIGHTS];
uniform float u_LightIntensities[MAX_LIGHTS];

void main()
{
    vec4 sceneColor = texture(u_Texture, vTex);
    
    vec3 ambient = vec3(0.05, 0.05, 0.15); 
    vec3 totalLight = ambient;

    for(int i = 0; i < u_LightCount; i++)
    {
        vec2 diff = vTex - u_LightUVs[i];
        diff.x *= u_AspectRatio; 
        
        float dist = length(diff);
        float attenuation = smoothstep(u_LightRadii[i], 0.0, dist);
        
        totalLight += u_LightColors[i] * u_LightIntensities[i] * attenuation;
    }

    FragColor = vec4(sceneColor.rgb * totalLight, sceneColor.a);
}