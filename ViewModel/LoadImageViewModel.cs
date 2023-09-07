using ImageEncryptTCP.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageEncryptTCP.ViewModel
{
    public class LoadImageViewModel : ViewModelBase
    {
        private string? _filePath;
        public ICommand LoadImageCommand { get; }

        public string ? Filepath
        {
            get
            {
                return _filePath;
            }
            set 
            { 
                _filePath = value;
                OnPropertyChanged(nameof(Filepath));
            }
        }

        public LoadImageViewModel()
        {
            _filePath = "No ha seleccionado ningún archivo.";
            LoadImageCommand = new ViewModelCommand(LoadImage);    
        }

        private void LoadImage(object obj)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
			openFileDlg.Filter = "Archios de imagen (*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF";

			Nullable<bool> result = openFileDlg.ShowDialog();            

            if (result == true)
            {
                Filepath = openFileDlg.FileName;
            }
        }
    }
}