using SkiaSharp;
#if __IOS__
namespace Microcharts.iOS
{
    using UIKit;
    using SkiaSharp.Views.iOS;
    using System.Diagnostics;
    using System.Linq;
#else
namespace Microcharts.macOS
{
    using SkiaSharp.Views.Mac;
#endif

    public class ChartView : SKCanvasView
    {
        #region Constructors

        public ChartView()
        {
#if __IOS__
            this.BackgroundColor = UIColor.Clear;
#endif
            this.PaintSurface += OnPaintCanvas;
        }

        #endregion

        #region Fields

        private InvalidatedWeakEventHandler<ChartView> handler;

        private Chart chart;

        #endregion

        #region Properties

        public Chart Chart
        {
            get => this.chart;
            set
            {
                if (this.chart != value)
                {
                    if (this.chart != null)
                    {
                        handler.Dispose();
                        this.handler = null;
                    }

                    this.chart = value;
                    this.InvalidateChart();

                    if (this.chart != null)
                    {
                        this.handler = this.chart.ObserveInvalidate(this, (view) => view.InvalidateChart());
                    }
                }
            }
        }

        #endregion

        #region Methods

        private void InvalidateChart() => this.SetNeedsDisplayInRect(this.Bounds);

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            if (this.chart != null)
            {
                this.chart.Draw(e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
            else
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
            }
            this.UserInteractionEnabled = true;
        }

        public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            foreach (UITouch touch in touches)
            {
                var position = touch.LocationInView(this).ToSKPoint();
                chart.PointToMarkerView(position);
            }
            //var r = chart.PointToMarkerView(e.Location);
            /*var touch = touches.ToArray<UITouch>().First();
            var r = chart.PointToMarkerView(l.ToSKPoint());
            this.InvalidateChart();*/

        }
        private float Density => 2;

        private SKPoint ToPlatform(SKPoint point)
        {
            return new SKPoint(point.X * Density, point.Y * Density);
        }



        #endregion
    }
}
