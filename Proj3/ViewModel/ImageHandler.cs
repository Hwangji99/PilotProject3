using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System.Windows.Media.Imaging;

public class ImageHandler
{
    private Mat _currentImage;

    public BitmapSource LoadImage(string filePath)
    {
        try
        {
            _currentImage = Cv2.ImRead(filePath);
            if (_currentImage.Empty())
            {
                throw new Exception("이미지를 로드하지 못했습니다.");
            }

            return MatToBitmapSource(_currentImage);
        }
        catch (Exception ex)
        {
            throw new Exception($"이미지 처리 중 오류 발생: {ex.Message}");
        }
    }

    public BitmapSource MatToBitmapSource(Mat mat)
    {
        return BitmapSourceConverter.ToBitmapSource(mat);
    }

    public void Clear()
    {
        _currentImage?.Dispose();
        _currentImage = null;
    }
}
