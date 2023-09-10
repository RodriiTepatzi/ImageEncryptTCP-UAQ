using ImageEncryptTCP.Manager;
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
        private string? _key;

        public ICommand LoadImageCommand { get; }
        public ICommand SendData { get; }

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

        public string? Key
        {
            get { return _key; }
            set
            {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        public LoadImageViewModel()
        {
            _filePath = "No ha seleccionado ningún archivo.";
            LoadImageCommand = new ViewModelCommand(LoadImage);
            SendData = new ViewModelCommand(SendImage);
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

        private void SendImage(object obj)
        {
            if (!string.IsNullOrEmpty(Key) && !string.IsNullOrEmpty(Filepath))
            {
				ConnectionManager.Instance.ImagePath = Filepath;
                ConnectionManager.Instance.EncryptKey = Key;
			}
		}
    }
}