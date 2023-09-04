using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Timers;
using System.Windows.Media.Media3D;
using System.Xml;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using ImageEncryptTCP.Views;
using ImageEncryptTCP.Events;

namespace SocketClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _username = "";
        private string _ipAddress = "127.0.0.1";
        private string _port = "8000";
        private string _errorMessage = "";

        private bool _isTimedOut = false;
        private bool _isConnected = false;

        public ICommand LoginCommand { get; }


        public string Username
        {
            get { return _username; }
            set
            {
                if (value.Length <= 13)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

        public string IPAddress
        {
            get { return FormatIPv4Address(_ipAddress); }
            set
            {
                _ipAddress = value ?? "";  
                OnPropertyChanged(nameof(IPAddress));
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _port = value;
                    OnPropertyChanged(nameof(Port));

                }
                else if (value.Length <= 5 && int.TryParse(value, out int numericValue))
                {
                    _port = value;
                    OnPropertyChanged(nameof(Port));
                }
            }
        }

        public string ErrorMessage
        {
            get{ return _errorMessage; }
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        
        public bool IsTimedOut
        {
            get { return _isTimedOut; }
            set
            {
                if (value)
                {
                    ErrorMessage = "No se pudo conectar después de 20 segundos.";
                }
                _isTimedOut = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(IsTimedOut));
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (value)
                {
                    Application.Current.MainWindow.Hide();
                    var chatView = new LoadImageView();
                    chatView.Show();
                    Application.Current.MainWindow = chatView;
                }
                else
                {
                    ErrorMessage = "No se pudo conectar.";
                }
                _isTimedOut = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }


        public LoginViewModel()
        {
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand);
        }

        private void ExecuteLoginCommand(object obj)
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Port) || string.IsNullOrEmpty(IPAddress))
            {
                _errorMessage = "Hay campos vacios.";
                OnPropertyChanged(nameof(ErrorMessage));
            }
            else
            {
                _errorMessage = "";
                OnPropertyChanged(nameof(ErrorMessage));

                // Create the connection and with an event we listen for any change on IsConnected so we update either the UI
                // or we do something else.


            }
        }

        private string FormatIPv4Address(string ipAddress)
        {
            string cleanIPAddress = new string(ipAddress.Where(c => char.IsDigit(c) || c == '.').ToArray());
            string[] segments = cleanIPAddress.Split('.');

            StringBuilder formattedAddress = new StringBuilder();
            for (int i = 0; i < segments.Length; i++)
            {
                if (segments[i].Length > 3)
                {
                    segments[i] = segments[i].Substring(0, 3);
                }

                if (int.TryParse(segments[i], out int segmentValue))
                {
                    if (segmentValue > 255)
                    {
                        segmentValue = 255;
                    }
                    formattedAddress.Append(segmentValue);
                }

                if (i < segments.Length - 1)
                {
                    formattedAddress.Append(".");
                }
            }

            return formattedAddress.ToString();
        }


        // Events

        private void ConnectionTimedOut(object? sender, ConnectionTimedOutEventArgs e) => IsTimedOut = e.IsTimedOut;
        private void ConnectionChanged(object? sender, ConnectionChangedEventArgs e) => IsConnected = e.IsConnected;
    }
}
