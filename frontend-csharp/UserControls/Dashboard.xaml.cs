using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace frontend_csharp.UserControls
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
            SetupLineChart();
            SetupBarChart();
        }

        private void SetupLineChart()
        {
            LineChart.Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 25, 24, 28, 35, 33, 38, 45, 50 },
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Black, 2.5f),
                    GeometryFill = new SolidColorPaint(new SKColor(0x2D, 0xB8, 0x7F)),
                    GeometryStroke = new SolidColorPaint(SKColors.White, 2),
                    GeometrySize = 10,
                    LineSmoothness = 0
                }
            };

            LineChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = new[] { "23 Nov", "24", "25", "26", "27", "28", "29", "30" },
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 11
                }
            };

            LineChart.YAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit = 20,
                    MaxLimit = 55,
                    Labeler = v => $"{v}tr",
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 11
                }
            };
        }

        private void SetupBarChart()
        {
            var moSoColor = new SKColor(0x2D, 0xB8, 0x7F);
            var dongSoColor = new SKColor(0x2D, 0x34, 0x36);

            BarChart.Series = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { 220, 280, 480, 300, 450, 420, 350, 440, 400, 460, 500, 350 },
                    Name = "Mở sổ",
                    Fill = new SolidColorPaint(moSoColor),
                    MaxBarWidth = 14
                },
                new ColumnSeries<double>
                {
                    Values = new double[] { 200, 250, 400, 270, 350, 330, 280, 300, 350, 420, 390, 250 },
                    Name = "Đóng sổ",
                    Fill = new SolidColorPaint(dongSoColor),
                    MaxBarWidth = 14
                }
            };

            BarChart.XAxes = new Axis[]
            {
                new Axis
                {
                    Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" },
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 10
                }
            };

            BarChart.YAxes = new Axis[]
            {
                new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 650,
                    LabelsPaint = new SolidColorPaint(new SKColor(0x99, 0x99, 0x99)),
                    TextSize = 10
                }
            };
        }
    }
}
