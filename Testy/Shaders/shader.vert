#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColour;
layout (location = 2) in vec3 aNormals;
layout (location = 3) in vec3 aIndices;

uniform mat4 uViewMtx;
uniform mat4 uProjectionMtx;
uniform mat4 uModelMtx;

out vec3 vertColour;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uModelMtx * uViewMtx * uProjectionMtx;
    vertColour = aColour;
}