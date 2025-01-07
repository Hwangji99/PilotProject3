using System.IO;
using System.Windows.Controls;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace Proj3.ViewModel
{
    public static class CanvasUtilities
    {
        public static void ClearCanvas(object parameter)
        {
            if (parameter is Canvas canvas)
            {
                canvas.Children.Clear();
            }
        }

        public static void CaptureCanvas(object parameter)
        {
            if (parameter is Canvas canvas)
            {
                if (canvas.ActualWidth == 0 || canvas.ActualHeight == 0)
                {
                    MessageBox.Show("캔버스가 비어 있습니다. 저장할 내용이 없습니다.", "저장 오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                try
                {
                    var renderTarget = new RenderTargetBitmap(
                        (int)canvas.ActualWidth,
                        (int)canvas.ActualHeight,
                        96, 
                        96,
                        PixelFormats.Pbgra32);

                    renderTarget.Render(canvas);

                    var encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(renderTarget));

                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "PNG Files|*.png",
                        DefaultExt = "png",
                        FileName = "CanvasImage.png"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                        {
                            encoder.Save(fileStream); 
                        }
                        MessageBox.Show("캔버스 이미지가 성공적으로 저장되었습니다.", "저장 완료", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"이미지 저장 중 오류가 발생했습니다: {ex.Message}", "저장 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("잘못된 캔버스 참조입니다.", "저장 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
