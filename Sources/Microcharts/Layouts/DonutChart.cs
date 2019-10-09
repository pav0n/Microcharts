﻿// Copyright (c) Aloïs DENIEL. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Microcharts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SkiaSharp;

    /// <summary>
    /// ![chart](../images/Donut.png)
    /// 
    /// A donut chart.
    /// </summary>
    public class DonutChart : Chart
    {
        #region Properties

        /// <summary>
        /// Gets or sets the radius of the hole in the center of the chart.
        /// </summary>
        /// <value>The hole radius.</value>
        public float HoleRadius { get; set; } = 0.5f;
        private IList<SKPath> paths;
        #endregion

        #region Methods

        public override void DrawContent(SKCanvas canvas, int width, int height)
        {
            if (this.Entries != null)
            {
                paths = new List<SKPath>();
                this.DrawCaption(canvas, width, height);
                using (new SKAutoCanvasRestore(canvas))
                {
                    //canvas.Translate(width / 2, height / 2);
                    var sumValue = this.Entries.Sum(x => Math.Abs(x.Value));
                    var radius = (Math.Min(width, height) - (2 * Margin)) / 2;

                    var start = 0.0f;
                    for (int i = 0; i < this.Entries.Count(); i++)
                    {
                        var entry = this.Entries.ElementAt(i);
                        var end = start + ((Math.Abs(entry.Value) / sumValue) * this.AnimationProgress);

                        // Sector

                        var path = RadialHelpers.CreateSectorPath(start, end, radius, radius * this.HoleRadius);
                        using (var paint = new SKPaint
                        {
                            Style = SKPaintStyle.Fill,
                            Color = entry.Color,
                            IsAntialias = true,
                        })
                        {
                            paths.Add(path);

                            SKPoint MidPoint = new SKPoint(width / 2, height / 2);    // CanvasMidPoint and MidPoint both are equal. So I use MidPoint here.
                            SKRect computeRect = new SKRect();
                            path.GetTightBounds(out computeRect);
                            SKMatrix pathMatrix = SKMatrix.MakeTranslation(width / 2, height / 2);
                            //SKMatrix.PostConcat(ref pathMatrix, SKMatrix.MakeTranslation(MidPoint.X - (computeRect.Width / 2), MidPoint.Y - (computeRect.Height / 2)));
                            path.Transform(pathMatrix);
                            canvas.DrawPath(path, paint);
                        }

                        start = end;
                    }
                }
            }
            this.DrawMarkerView(canvas);
        }

        private void DrawCaption(SKCanvas canvas, int width, int height)
        {
            var sumValue = this.Entries.Sum(x => Math.Abs(x.Value));
            var rightValues = new List<ChartEntry>();
            var leftValues = new List<ChartEntry>();

            int i = 0;
            var current = 0.0f;
            while (i < this.Entries.Count() && (current < sumValue / 2))
            {
                var entry = this.Entries.ElementAt(i);
                rightValues.Add(entry);
                current += Math.Abs(entry.Value);
                i++;
            }

            while (i < this.Entries.Count())
            {
                var entry = this.Entries.ElementAt(i);
                leftValues.Add(entry);
                i++;
            }

            leftValues.Reverse();

            this.DrawCaptionElements(canvas, width, height, rightValues, false);
            this.DrawCaptionElements(canvas, width, height, leftValues, true);
        }




        public override SKPoint PointToMarkerView(SKPoint point)
        {
            foreach (var item in this.paths)
            {
                if(this.PointInPath(point,item))
                {
                    this.index = this.paths.IndexOf(item);
                    this.pointMarkerView = new SKPoint(item.Bounds.MidX, item.Bounds.MidY);
                    return point;
                }
            }


            this.index = -1;
            return default(SKPoint);
        }

        #endregion
    }
}