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
    vec3 lightTest = vec3(1,1,1);
    
    float ambientColourStrength = 0.3;
    vec3 ambientColour = ambientColourStrength * lightTest;
    
    vec3 normal = normalize(vertNormal);
    vec3 lightDir = normalize(lightPos - vertPos);
    
    // Diffuse lighting 
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = diff * lightTest * 0.5; // Adjusted diffuse strength
    
    // specular highlights
    float specularStrength = 1;
    vec3 viewDir = normalize(viewPos - vertPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = specularStrength * spec * lightTest;
    
    vec3 result = (ambientColour + diffuse + specular) * vertColour.rgb;
    FragColour = vec4(result, 1.0);
}