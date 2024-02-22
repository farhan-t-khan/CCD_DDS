using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows.Input;

using Prism.Commands;
using Prism.Mvvm;
using System.Media;
using System.Windows;

namespace CCD_DDS
{
    public class CalibrationStartViewModel : BindableBase
    {
        private string _displayText;
        private bool _isButtonVisible;
        private bool _isAutoZeroInProgress;
        private bool _isCalibrating;
        private SoundPlayer _soundPlayer;
        private DelegateCommand _nextCommand;

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

        public bool IsAutoZeroInProgress
        {
            get { return _isAutoZeroInProgress; }
            set { SetProperty(ref _isAutoZeroInProgress, value); }
        }
        public bool IsCalibrating
        {
            get { return _isCalibrating; }
            set { SetProperty(ref _isCalibrating, value); }
        }

        public DelegateCommand NextCommand
        {
            get { return _nextCommand; }
            set { SetProperty(ref _nextCommand, value); }
        }
        public CalibrationStartViewModel()
        {
            //Initialize Sound Player
            _soundPlayer = new SoundPlayer("Resource\\click.wav");
            // Initialize properties
            DisplayText = "Connect Zero Air and press Yes on the detector";
            IsButtonVisible = true;

            _nextCommand = new DelegateCommand(ExecuteAutoZeroCommand);
        }

        private async void ExecuteAutoZeroCommand()
        {
            _soundPlayer.Play();
            DisplayText = "Running AutoZero in Zero Air...";
            IsButtonVisible = false;

            // Show loading wheel
            IsAutoZeroInProgress = true;

/*            // Simulate auto zero process
            Task.Run(() =>
            {
                // Your auto zero logic here...

                // Hide loading wheel when process is complete
                IsAutoZeroInProgress = false;
            });*/


            //await Task.Delay(18000); //Actual
            await Task.Delay(2000); //Test
            NextCommand = new DelegateCommand(ExecuteGasCalibrationCommand);
            DisplayText = "AutoZero Successful";
            IsButtonVisible = true;
            IsAutoZeroInProgress = false;
            IsCalibrating = false;
        }
        private async void ExecuteGasCalibrationCommand()
        {
            DisplayText = "Connect Calibration Gas 10000 PPM";
            IsButtonVisible = true;
            NextCommand = new DelegateCommand(ExecuteCalibrating);
        }

        private async void ExecuteCalibrating()
        {
            DisplayText = "Calibration in progress";
            IsButtonVisible = false;
            IsCalibrating = true;
            
            //await Task.Delay(28000); //Actual
            await Task.Delay(5000); //Test
            CalibrationGasDetected();

        }
        private void CalibrationGasDetected()
        {
            DisplayText = "Done. Continue to AutoZero";
            IsButtonVisible = true;
            IsCalibrating = false;
            NextCommand = new DelegateCommand(ExecuteAutoZeroCommand);
        }
    }
}
