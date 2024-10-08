﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Windows.Input;

using Prism.Commands;
using Prism.Mvvm;
using System.Media;
using System.Windows;
using System.Windows.Navigation;

namespace CCD_DDS
{
    public class CalibrationStartViewModel : BindableBase
    {
        private string _displayText;
        private string _cancelText;
        private bool _isButtonVisible;
        private bool _isAutoZeroInProgress;
        private bool _isCalibrating;
        private SoundPlayer _soundPlayer;
        private DelegateCommand _nextCommand;
        private List<GasData> _selectedGases;
        private int tracker;
        public string DisplayText
        {
            get { return _displayText; }
            set { SetProperty(ref _displayText, value); }
        }

        public string CancelText
        {
            get { return _cancelText; }
            set { SetProperty(ref _cancelText, value); }
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
            DisplayText = "";
            IsButtonVisible = false;
            CancelText = "Cancel";

            // Initialize list with the selected gases and the tracker
            _selectedGases = ReadSetupData("SetupData.csv");
            tracker = 0;

            //AutoZero before anything
            _nextCommand = new DelegateCommand(ExecuteAutoZeroCommand);
            ExecuteAutoZeroCommand();
        }

        private async void ExecuteAutoZeroCommand()
        {
            //_soundPlayer.Play();
            DisplayText = "Reading zero gas...";
            //IsButtonVisible = false;

            // Show loading wheel
            IsAutoZeroInProgress = true;

            //await Task.Delay(18000); //Actual
            await Task.Delay(4000); //Test

            DisplayText = "AutoZero Successful";
            //IsButtonVisible = true;
            IsAutoZeroInProgress = false;
            IsCalibrating = false;
            await Task.Delay(2000);

            if (tracker < _selectedGases.Count)
            {
                //NextCommand = new DelegateCommand(ExecuteGasCalibrationCommand);
                ExecuteGasCalibrationCommand();
            } else
            {
                //NextCommand = new DelegateCommand(NavigateToHome);
                DisplayText = "Successful";
                IsButtonVisible = true;
                CancelText = "Cancel";

            }
            
/*            DisplayText = "AutoZero Successful";
            IsButtonVisible = true;
            IsAutoZeroInProgress = false;
            IsCalibrating = false;*/
        }
        private async void ExecuteGasCalibrationCommand()
        {

            DisplayText = $"Reading Gas with {_selectedGases[tracker].Concentration} PPM";
            //IsButtonVisible = true;
            //NextCommand = new DelegateCommand(ExecuteCalibrating);
            await Task.Delay(2000);
            ExecuteCalibrating();

        }

        private async void ExecuteCalibrating()
        {

            DisplayText = "Calibration in progress";
            //IsButtonVisible = false;
            IsCalibrating = true;

            //await Task.Delay(28000); //Actual
            await Task.Delay(5000); //Test
            CalibrationGasDetected();

        }
        private async void CalibrationGasDetected()
        {
            DisplayText = "Done. Continuing to AutoZero";
            //IsButtonVisible = true;
            IsCalibrating = false;
            tracker++;

            await Task.Delay(2000);

            //NextCommand = new DelegateCommand(ExecuteAutoZeroCommand);
            ExecuteAutoZeroCommand();
        }



        private List<GasData> ReadSetupData(string filePath)
        {
            var selectedGases = new List<GasData>();

            try
            {
                // Read all lines from the CSV file
                string[] lines = File.ReadAllLines(filePath);

                // Skip the header line
                for (int i = 1; i < lines.Length; i++)
                {
                    // Split the line into fields
                    string[] fields = lines[i].Split(',');

                    // Ensure that the line has the correct number of fields
                    if (fields.Length >= 5)
                    {
                        // Parse the 'Selected' field to determine if the gas is selected
                        if (bool.TryParse(fields[4], out bool isSelected) && isSelected)
                        {
                            // Create a new GasData object and add it to the list
                            selectedGases.Add(new GasData
                            {
                                Gas = fields[0],
                                Concentration = fields[1]
                            });
                        }
                    }
                }
                //Sort the selectedGases List 
                selectedGases = selectedGases.OrderBy(g => Convert.ToInt32(g.Concentration)).ToList();
                var lastItem = selectedGases.Last();
                selectedGases.Insert(0, lastItem);

            }
            catch (Exception ex)
            {
                //MessageBox.Show($"Error reading setup data: {ex.Message}");
            }
            return selectedGases;
        }
        public void NavigateToHome()
        {
             
            
        }
    }
}
