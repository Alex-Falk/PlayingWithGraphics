#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

uniform mat4 uViewMtx;
uniform mat4 uProjectionMtx;
uniform mat4 uModelMtx;

out vec3 vertColor;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uModelMtx * uViewMtx * uProjectionMtx;
    vertColor = aColor;
}