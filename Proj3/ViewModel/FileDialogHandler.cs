using Microsoft.Win32;

public class FileDialogHandler
{
    public string SelectFile()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Image and Video Files|*.jpg;*.png;*.bmp;"
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}
