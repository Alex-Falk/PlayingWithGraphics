#version 330 core
in vec3 vertColour;

out vec4 FragColour;

uniform vec3 uObjectColour;

void main()
{
    FragColour = vec4(vertColour, 1.0);
}