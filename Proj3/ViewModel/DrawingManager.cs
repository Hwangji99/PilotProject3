using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;

namespace Proj3.ViewModel
{
    public enum DrawingMode
    {
        None,
        Line,
        Rectangle,
        Circle,
        Polygon,
        Eraser,
        Delete
    }

    public class DrawingManager
    {
        private Point _startPoint;
        private Shape _tempShape;
        private List<Point> _polygonPoints = new(); 
        private Polyline _currentPolyline;

        public DrawingMode CurrentDrawingMode { get; private set; } = DrawingMode.None;
        public Brush CurrentStroke { get; set; } = Brushes.Black;

        public void SetDrawingMode(DrawingMode mode)
        {
            CurrentDrawingMode = mode;
        }

        public void OnMouseDown(object parameter)
        {
            if (parameter is Canvas canvas)
            {
                _startPoint = Mouse.GetPosition(canvas);

                if (CurrentDrawingMode == DrawingMode.Eraser)
                {
                    var eraserCir = new Ellipse
                    {
                        Fill = Brushes.White, 
                        Width = 15,           
                        Height = 15,
                        Opacity = 1.0         
                    };

                    Canvas.SetLeft(eraserCir, _startPoint.X - 10); 
                    Canvas.SetTop(eraserCir, _startPoint.Y - 10);

                    canvas.Children.Add(eraserCir);
                    return;
                }

                if (CurrentDrawingMode == DrawingMode.Delete)
                {
                    var hitShapes = canvas.Children.OfType<Shape>().Where(s => s.IsMouseOver).ToList();
                    foreach (var shape in hitShapes)
                    {
                        canvas.Children.Remove(shape);
                    }
                    return;
                }

                switch (CurrentDrawingMode)
                {
                    case DrawingMode.Line:
                        _tempShape = new Line
                        {
                            Stroke = CurrentStroke,
                            StrokeThickness = 2,
                            X1 = _startPoint.X,
                            Y1 = _startPoint.Y,
                            X2 = _startPoint.X,
                            Y2 = _startPoint.Y
                        };
                        canvas.Children.Add(_tempShape);
                        break;

                    case DrawingMode.Rectangle:
                        _tempShape = new Rectangle
                        {
                            Stroke = CurrentStroke,
                            StrokeThickness = 2
                        };
                        Canvas.SetLeft(_tempShape, _startPoint.X);
                        Canvas.SetTop(_tempShape, _startPoint.Y);
                        canvas.Children.Add(_tempShape);
                        break;

                    case DrawingMode.Circle:
                        _tempShape = new Ellipse
                        {
                            Stroke = CurrentStroke,
                            StrokeThickness = 2
                        };
                        Canvas.SetLeft(_tempShape, _startPoint.X);
                        Canvas.SetTop(_tempShape, _startPoint.Y);
                        canvas.Children.Add(_tempShape);
                        break;

                    case DrawingMode.Polygon:
                        if (_currentPolyline == null)
                        {
                            _currentPolyline = new Polyline
                            {
                                Stroke = CurrentStroke,
                                StrokeThickness = 2
                            };
                            canvas.Children.Add(_currentPolyline);
                        }

                        _polygonPoints.Add(_startPoint);
                        _currentPolyline.Points = new PointCollection(_polygonPoints);
                        break;
                }
            }
        }

        public void OnMouseMove(object parameter)
        {
            if (parameter is Canvas canvas && _tempShape != null)
            {
                var currentPoint = Mouse.GetPosition(canvas);

                switch (_tempShape)
                {
                    case Line line:
                        line.X2 = currentPoint.X;
                        line.Y2 = currentPoint.Y;
                        break;

                    case Rectangle rectangle:
                        var rectWidth = Math.Abs(currentPoint.X - _startPoint.X);
                        var rectHeight = Math.Abs(currentPoint.Y - _startPoint.Y);
                        Canvas.SetLeft(rectangle, Math.Min(_startPoint.X, currentPoint.X));
                        Canvas.SetTop(rectangle, Math.Min(_startPoint.Y, currentPoint.Y));
                        rectangle.Width = rectWidth;
                        rectangle.Height = rectHeight;
                        break;

                    case Ellipse ellipse:
                        var radiusX = Math.Abs(currentPoint.X - _startPoint.X);
                        var radiusY = Math.Abs(currentPoint.Y - _startPoint.Y);
                        Canvas.SetLeft(ellipse, Math.Min(_startPoint.X, currentPoint.X));
                        Canvas.SetTop(ellipse, Math.Min(_startPoint.Y, currentPoint.Y));
                        ellipse.Width = radiusX;
                        ellipse.Height = radiusY;
                        break;

                    case Polygon polygon:
                        if (CurrentDrawingMode == DrawingMode.Polygon && _polygonPoints.Count > 0)
                        {
                            _polygonPoints[^1] = currentPoint;
                            polygon.Points = new PointCollection(_polygonPoints);
                        }
                        break;
                }
            }
        }

        public void OnMouseRightClick(object parameter)
        {
            if (CurrentDrawingMode == DrawingMode.Polygon && _polygonPoints.Count > 2 && parameter is Canvas canvas)
            {
                var polygon = new Polygon
                {
                    Stroke = CurrentStroke,
                    StrokeThickness = 2,
                    Points = new PointCollection(_polygonPoints),
                    Fill = Brushes.Transparent
                };

                canvas.Children.Remove(_currentPolyline); 
                canvas.Children.Add(polygon);

                _polygonPoints.Clear();
                _currentPolyline = null;
            }
        }

        public void OnMouseUp(object parameter)
        {
            if (CurrentDrawingMode != DrawingMode.Polygon)
            {
                _tempShape = null;
            }
        }
    }
}
