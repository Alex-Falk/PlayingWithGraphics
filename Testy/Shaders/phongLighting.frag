#version 330 core
out vec4 FragColour;

uniform vec3 lightColour;
uniform vec3 lightPos;
uniform vec3 viewPos;

in vec3 vertNormal;
in vec3 vertPos;
in vec4 vertColour;

void main()
{
    float ambientColourStrength = 0.2;
    vec3 ambientColour = ambientColourStrength * lightColour;
    
    vec3 normal = normalize(fragNormal);
    vec3 lightDir = normalize(lightPos - fragPos);
    
    // Diffuse lighting 
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = diff * lightColour;
    
    // specular highlights
    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - fragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightColour;
    FragColor = vec4(result, 1.0);
}