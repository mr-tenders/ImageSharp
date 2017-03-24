﻿// <copyright file="VonKriesChromaticAdaptation.cs" company="James Jackson-South">
// Copyright (c) James Jackson-South and contributors.
// Licensed under the Apache License, Version 2.0.
// </copyright>

namespace ImageSharp.Colors.Conversion
{
    using System.Numerics;

    using Implementation;
    using Spaces;

    /// <summary>
    /// Basic implementation of the von Kries chromatic adaptation model
    /// </summary>
    /// <remarks>
    /// Transformation described here:
    /// http://www.brucelindbloom.com/index.html?Eqn_ChromAdapt.html
    /// </remarks>
    public class VonKriesChromaticAdaptation : IChromaticAdaptation
    {
        private readonly IColorConversion<CieXyz, Lms> conversionToLms;

        private readonly IColorConversion<Lms, CieXyz> conversionToXyz;

        /// <summary>
        /// Initializes a new instance of the <see cref="VonKriesChromaticAdaptation"/> class.
        /// </summary>
        public VonKriesChromaticAdaptation()
            : this(new CieXyzAndLmsConverter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VonKriesChromaticAdaptation"/> class.
        /// </summary>
        /// <param name="transformationMatrix">
        /// The transformation matrix used for the conversion (definition of the cone response domain).
        /// <see cref="LmsAdaptationMatrix"/>
        /// </param>
        public VonKriesChromaticAdaptation(Matrix4x4 transformationMatrix)
            : this(new CieXyzAndLmsConverter(transformationMatrix))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VonKriesChromaticAdaptation"/> class.
        /// </summary>
        /// <param name="converter"></param>
        private VonKriesChromaticAdaptation(CieXyzAndLmsConverter converter)
            : this(converter, converter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VonKriesChromaticAdaptation"/> class.
        /// </summary>
        /// <param name="conversionToLms">The <see cref="Lms"/> color converter.</param>
        /// <param name="conversionToCieXyz">The <see cref="CieXyz"/> color converter.</param>
        public VonKriesChromaticAdaptation(IColorConversion<CieXyz, Lms> conversionToLms, IColorConversion<Lms, CieXyz> conversionToCieXyz)
        {
            Guard.NotNull(conversionToLms, nameof(conversionToLms));
            Guard.NotNull(conversionToCieXyz, nameof(conversionToCieXyz));

            this.conversionToLms = conversionToLms;
            this.conversionToXyz = conversionToCieXyz;
        }

        /// <inheritdoc/>
        public CieXyz Transform(CieXyz sourceColor, CieXyz sourceWhitePoint, CieXyz targetWhitePoint)
        {
            Guard.NotNull(sourceColor, nameof(sourceColor));
            Guard.NotNull(sourceWhitePoint, nameof(sourceWhitePoint));
            Guard.NotNull(targetWhitePoint, nameof(targetWhitePoint));

            if (sourceWhitePoint.Equals(targetWhitePoint))
            {
                return sourceColor;
            }

            Lms sourceColorLms = this.conversionToLms.Convert(sourceColor);
            Lms sourceWhitePointLms = this.conversionToLms.Convert(sourceWhitePoint);
            Lms targetWhitePointLms = this.conversionToLms.Convert(targetWhitePoint);

            Vector3 vector = new Vector3(targetWhitePointLms.L / sourceWhitePointLms.L, targetWhitePointLms.M / sourceWhitePointLms.M, targetWhitePointLms.S / sourceWhitePointLms.S);

            Lms targetColorLms = new Lms(Vector3.Multiply(vector, sourceColorLms.Vector));
            return this.conversionToXyz.Convert(targetColorLms);
        }
    }
}