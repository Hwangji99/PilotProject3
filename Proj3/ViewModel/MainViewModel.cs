using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Proj3.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _filePath;
        private DrawingManager _drawingManager;
        private BitmapSource _displayImage;
        private Brush _currentStroke = Brushes.Black;
        private bool _imageVisible;
        private readonly ImageHandler _imageHandler = new();
        private readonly FileDialogHandler _fileDialogHandler = new();

        public MainViewModel()
        {
            _drawingManager = new DrawingManager();

            DrawingLine = new Command.Command(SetLineMode);
            DrawingCircle = new Command.Command(SetCircleMode);
            DrawingRectangle = new Command.Command(SetRectangleMode);
            DrawingPolygon = new Command.Command(SetPolygonMode);
            EraserBtn = new Command.Command(SetErasingMode);
            DeleteBtn = new Command.Command(SetDeleteMode);
            AllDel = new Command.Command(CanvasUtilities.ClearCanvas);
            SaveBtn = new Command.Command(parameter => CanvasUtilities.CaptureCanvas(parameter));
            MouseDownCommand = new Command.Command(_drawingManager.OnMouseDown);
            MouseMoveCommand = new Command.Command(_drawingManager.OnMouseMove);
            MouseUpCommand = new Command.Command(_drawingManager.OnMouseUp);
            OpenFileBtn = new Command.Command(OpenFile);
            RightClickCommand = new Command.Command(_drawingManager.OnMouseRightClick);
        }

        public Brush CurrentStroke
        {
            get => _currentStroke;
            set
            {
                _currentStroke = value;
                OnPropertyChanged(nameof(CurrentStroke));
                _drawingManager.CurrentStroke = value;
            }
        }

        public ICommand OpenFileBtn { get; }
        public ICommand DrawingLine { get; }
        public ICommand DrawingCircle { get; }
        public ICommand DrawingRectangle { get; }
        public ICommand DrawingPolygon { get; }
        public ICommand EraserBtn { get; }
        public ICommand DeleteBtn { get; }
        public ICommand AllDel { get; }
        public ICommand SaveBtn { get; }
        public ICommand MouseDownCommand { get; }
        public ICommand MouseMoveCommand { get; }
        public ICommand MouseUpCommand { get; }
        public ICommand RightClickCommand { get; }

        private void SetLineMode(object parameter) => _drawingManager.SetDrawingMode(DrawingMode.Line);
        private void SetCircleMode(object parameter) => _drawingManager.SetDrawingMode(DrawingMode.Circle);
        private void SetRectangleMode(object parameter) => _drawingManager.SetDrawingMode(DrawingMode.Rectangle);
        private void SetPolygonMode(object parameter) => _drawingManager.SetDrawingMode(DrawingMode.Polygon);
        private void SetErasingMode(object parameter) => _drawingManager.SetDrawingMode(DrawingMode.Eraser);
        private void SetDeleteMode(object parameter) => _drawingManager.SetDrawingMode(DrawingMode.Delete);

        private void DisplayImageOnCanvas(Canvas canvas, BitmapSource image)
        {

            var imageControl = new Image
            {
                Source = image,
                Stretch = Stretch.None 
            };

            canvas.Children.Add(imageControl); 
        }

        private void OpenFile(object parameter)
        {
            if (parameter is not Canvas canvas)
            {
                MessageBox.Show("캔버스가 전달되지 않았습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string filePath = _fileDialogHandler.SelectFile();
            if (filePath == null) return;

            var image = _imageHandler.LoadImage(filePath);

            DisplayImageOnCanvas(canvas, image);
        }

        public BitmapSource DisplayImage
        {
            get => _displayImage;
            set
            {
                _displayImage = value;
                OnPropertyChanged(nameof(DisplayImage));
            }
        }

        public bool ImageVisible
        {
            get => _imageVisible;
            set
            {
                _imageVisible = value;
                OnPropertyChanged(nameof(ImageVisible));
            }
        }

        public string FilePath
        {
            get => _filePath;
            set
            {
                _filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        private Color _selectedColor = Colors.Black;

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
                CurrentStroke = new SolidColorBrush(_selectedColor);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
