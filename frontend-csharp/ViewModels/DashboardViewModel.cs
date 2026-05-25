using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Linq;
using System.Collections.Generic;

namespace frontend_csharp.ViewModels
{
    public class DashboardViewModel
    {
        public ISeries[] LineSeries { get; set; }
        public Axis[] LineXAxes { get; set; }
        public Axis[] LineYAxes { get; set; }

        public ISeries[] BarSeries { get; set; }
        public Axis[] BarXAxes { get; set; }
        public Axis[] BarYAxes { get; set; }

        public DashboardViewModel()
        {
            PrepareLineChart();
            PrepareBarChart();
        }

        private void PrepareLineChart()
        {
            double[] dataValues = new double[] { 25, 26, 28, 35, 33, 38, 45, 50 };

            double rawMin = dataValues.Min();
            double rawMax = dataValues.Max();

            double targetMin = System.Math.Floor((rawMin - 0.1) / 5.0) * 5;
            double targetMax = System.Math.Ceiling((rawMax + 0.1) / 5.0) * 5;

            List<double> separators = new List<double>();
            for (double i = targetMin; i <= targetMax; i += 5)
            {
                separators.Add(i);
            }

            LineSeries = new ISeries[] {
                new LineSeries<double> {
                    Values = dataValues,
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Black, 2.5f),
                    GeometryFill = new SolidColorPaint(new SKColor(0x2D, 0xB8, 0x7F)),
                    GeometryStroke = new SolidColorPaint(SKColors.White, 2),
                    GeometrySize = 10,
                    LineSmoothness = 0
                }
            };

            LineXAxes = new Axis[] {
                new Axis {
                    Labels = new[] { "23", "24", "25", "26", "27", "28", "29", "30" },
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 10
                }
            };

            LineYAxes = new Axis[] {
                new Axis {
                    MinLimit = targetMin - 2,
                    MaxLimit = targetMax + 2,
                    CustomSeparators = separators.ToArray(),
                    Labeler = v => $"{v}tr",
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 9
                }
            };
        }

        private void PrepareBarChart()
        {
            double[] moSoValues = new double[] { 220, 280, 480, 300, 450, 420, 350, 440, 400, 460, 500, 350 };
            double[] dongSoValues = new double[] { 200, 250, 400, 270, 350, 330, 280, 300, 350, 420, 390, 250 };

            double maxVal = moSoValues.Concat(dongSoValues).Max();

            double magnitude = System.Math.Floor(System.Math.Log10(maxVal));
            double roundingStep = System.Math.Pow(10, magnitude);

            if (maxVal / roundingStep < 2.5)
            {
                roundingStep /= 10;
            }

            double targetMax = System.Math.Ceiling((maxVal + 1) / roundingStep) * roundingStep;

            List<double> ySeparators = new List<double>();
            for (double i = 0; i <= targetMax; i += roundingStep)
            {
                ySeparators.Add(i);
            }

            var moSoColor = new SKColor(0x2D, 0xB8, 0x7F);
            var dongSoColor = new SKColor(0x2D, 0x34, 0x36);

            BarSeries = new ISeries[] {
                new ColumnSeries<double> { Values = moSoValues, Name = "Mở sổ", Fill = new SolidColorPaint(moSoColor), MaxBarWidth = 18 },
                new ColumnSeries<double> { Values = dongSoValues, Name = "Đóng sổ", Fill = new SolidColorPaint(dongSoColor), MaxBarWidth = 18 }
            };

            BarXAxes = new Axis[] {
                new Axis {
                    Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                    CustomSeparators = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 },
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 8
                }
            };

            BarYAxes = new Axis[] {
                new Axis {
                    MinLimit = 0,
                    MaxLimit = targetMax,
                    CustomSeparators = ySeparators.ToArray(),
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 10
                }
            };
        }
    }
}