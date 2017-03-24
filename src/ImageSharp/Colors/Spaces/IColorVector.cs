﻿// <copyright file="IColorVector.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Colors.Spaces
{
    using System.Numerics;

    /// <summary>
    /// Color represented as a vector in its color space
    /// </summary>
    public interface IColorVector
    {
        /// <summary>
        /// The vector representation of the color
        /// </summary>
        Vector3 Vector { get; }
    }
}