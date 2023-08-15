#version 330 core
layout (location = 0) in vec3 aPosition;

uniform mat4 uViewMtx;
uniform mat4 uProjectionMtx;
uniform mat4 uModelMtx;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uModelMtx * uViewMtx * uProjectionMtx;
}