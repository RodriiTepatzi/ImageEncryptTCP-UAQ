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
        private string? _fromServerFilePath;
        private string? _equalMessage;
        private ConnectionManager _connectionManager;

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

        public string? FromServerFilePath
        {
            get
            {
                return _fromServerFilePath;
            }
            set
            {
                _fromServerFilePath = value;
                OnPropertyChanged(nameof(FromServerFilePath));
            }
        }

        public string? EqualMessage
        {
            get
            {
                return _equalMessage;
            }
            set
            {
                _equalMessage = value;
                OnPropertyChanged(nameof(EqualMessage));
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
            _connectionManager = ConnectionManager.Instance;
            _filePath = "No ha seleccionado ningún archivo.";
            _equalMessage = "Datos no enviados.";
            LoadImageCommand = new ViewModelCommand(LoadImage);
            SendData = new ViewModelCommand(SendImage);
			_connectionManager.MessageChanged += _connectionManager_MessageChanged;
			_connectionManager.PathChanged += _connectionManager_PathChanged;
        }

		private void _connectionManager_PathChanged(object? sender, Events.PathChangedEventArgs e)
		{
			this.FromServerFilePath = e.Path;
		}

		private void _connectionManager_MessageChanged(object? sender, Events.MessageChangedEventArgs e)
		{
			this.EqualMessage = e.Message.ToString();
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