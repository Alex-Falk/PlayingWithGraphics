#version 330 core

struct Material {
    vec3 ambientColour;
    vec3 diffuseColour; 
    vec3 specularColour; 

    float shininess; //Shininess is the power the specular light is raised to
};

struct Light {
    vec3 position;

    // note - vec3s so that we can basically adjust the strength for each colour channel
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

out vec4 FragColour;

uniform Light light;
uniform Material material;
uniform vec3 viewPos;

in vec3 vertNormal;
in vec3 vertPos;
in vec4 vertColour;

void main()
{
    vec3 ambientColour = light.ambient * material.ambientColour;
    
    vec3 normal = normalize(vertNormal);
    vec3 lightDir = normalize(light.position - vertPos);
    
    // Diffuse lighting 
    float diff = max(dot(normal, lightDir), 0.0);
    vec3 diffuse = diff * light.diffuse * material.diffuseColour; // Adjusted diffuse strength
    
    // specular highlights
    vec3 viewDir = normalize(viewPos - vertPos);
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec3 specular = material.specularColour * spec * light.specular; // Adjusted specular strength
    
    vec3 result = (ambientColour + diffuse + specular);
    FragColour = vec4(result, 1.0);
}