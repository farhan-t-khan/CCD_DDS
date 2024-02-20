﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows.Input;

using Prism.Commands;
using Prism.Mvvm;

namespace CCD_DDS
{
    public class CalibrationStartViewModel : BindableBase
    {
        private string _displayText;
        private bool _isButtonVisible;

        public string DisplayText
        {
            get { return _displayText; }
            set { SetProperty(ref _displayText, value); }
        }

        public bool IsButtonVisible
        {
            get { return _isButtonVisible; }
            set { SetProperty(ref _isButtonVisible, value); }
        }

        public DelegateCommand YesCommand { get; }

        public CalibrationStartViewModel()
        {
            // Initialize properties
            DisplayText = "Connect Zero Air and press Yes on the detector";
            IsButtonVisible = true;

            // Initialize command using Prism's DelegateCommand
            YesCommand = new DelegateCommand(ExecuteYesCommand);
        }

        private void ExecuteYesCommand()
        {
            // Handle logic when the "Yes" button is clicked
            // For example, change the display text and hide the button
            DisplayText = "Something else...";
            IsButtonVisible = false;
        }
    }
}
