﻿#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColour;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in uint aIndices;
layout (location = 4) in vec2 aTexCoords;

uniform mat4 uViewMtx;
uniform mat4 uProjectionMtx;
uniform mat4 uModelMtx;

out vec3 vertColour;
out vec3 vertNormal;
out vec3 vertPos;
out vec2 texCoords;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * uModelMtx * uViewMtx * uProjectionMtx;
    vertColour = aColour;
    vertNormal = aNormal * mat3(transpose(inverse(uModelMtx)));
    vertPos = (vec4(aPosition, 1.0) * uModelMtx).xyz ;
    texCoords = aTexCoords;
}