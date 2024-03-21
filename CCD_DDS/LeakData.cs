using System;
using System.ComponentModel;

namespace CCD_DDS
{
    public class LeakData : INotifyPropertyChanged, IEditableObject
    {
        private string _port;
        private string _leakDefinition;
        private string _concentration;
        private string _tankCapacity;
        private DateTime? _expiryDate;
        private string _lotNumber;
        private string _measuredConcentration;
        private string _tolerance;
        private bool _isSelected;
        private string _status;
        private double _tankLevel;
        private string _tankLevelColor;
        private int _tankPercent;
        private int _daysUntilExpiry=0;

        private DateTime? _precisionDate;
        private TimeSpan? _precisionTime;
        private string? _measurement1;
        private string? _measurement2;
        private string? _measurement3;
        private string? _precision;
        private bool _precisionIsSelected;

        private DateTime? _driftDate1;
        private TimeOnly? _driftTime1;
        private string? _driftConcentration1;
        private string? _driftConcentration2;
        private DateTime? _driftDate2;
        private TimeOnly? _driftTime2;
        private string? _driftPercentage;
        private bool _driftIsSelected;

        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                OnPropertyChanged(nameof(Port));

                // If Port is 0, set IsSelected to true
                if (_port == "0")
                {
                    IsSelected = true;
                }
            }
        }

        public string LeakDefinition
        {
            get { return _leakDefinition; }
            set
            {
                _leakDefinition = value;
                OnPropertyChanged(nameof(LeakDefinition));
            }
        }

        public string Concentration
        {
            get { return _concentration; }
            set
            {
                _concentration = value;
                OnPropertyChanged(nameof(Concentration));
            }
        }

        public string TankCapacity
        {
            get { return _tankCapacity; }
            set
            {
                _tankCapacity = value;
                OnPropertyChanged(nameof(TankCapacity));
            }
        }

        public double TankLevel
        {
            get { return _tankLevel; }
            set
            {
                if (_tankLevel != value && (value >= 0 || value <= 100))
                {
                    _tankLevel = value;
                    OnPropertyChanged(nameof(TankLevel));
                    UpdateTankLevelColor();
                }
            }
        }
        public int TankPercent
        {
            get { return _tankPercent; }
            set
            {
                _tankPercent = value;
                OnPropertyChanged(nameof(TankPercent));
            }
        }

        public DateTime? ExpiryDate
        {
            get { return _expiryDate; }
            set
            {
                _expiryDate = value;
                OnPropertyChanged(nameof(ExpiryDate));
                CalculateDaysUntilExpiry();
            }
        }
        public int DaysUntilExpiry
        {
            get { return _daysUntilExpiry; }
            set
            {
                if (_daysUntilExpiry != value)
                {
                    _daysUntilExpiry = value;
                    OnPropertyChanged(nameof(DaysUntilExpiry));
                }
            }
        }

        public string LotNumber
        {
            get { return _lotNumber; }
            set
            {
                _lotNumber = value;
                OnPropertyChanged(nameof(LotNumber));
            }
        }

        public string MeasuredConcentration
        {
            get { return _measuredConcentration; }
            set
            {
                _measuredConcentration = value;
                OnPropertyChanged(nameof(MeasuredConcentration));
            }
        }
        public string Tolerance
        {
            get { return _tolerance; }
            set
            {
                _tolerance = value;
                OnPropertyChanged(nameof(Tolerance));
            }
        }
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }


        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public DateTime? PrecisionDate
        {
            get { return _precisionDate; }
            set
            {
                _precisionDate = value;
                OnPropertyChanged(nameof(PrecisionDate));
            }
        }

        public TimeSpan? PrecisionTime
        {
            get { return _precisionTime; }
            set
            {
                _precisionTime = value;
                OnPropertyChanged(nameof(PrecisionTime));
            }
        }
        public string? Measurement1
        {
            get { return _measurement1; }
            set 
            {
                _measurement1 = value;
                OnPropertyChanged(nameof(Measurement1));
            }
        }
        public string? Measurement2
        {
            get { return _measurement2; }
            set
            {
                _measurement2 = value;
                OnPropertyChanged(nameof(Measurement2));
            }
        }
        public string? Measurement3
        {
            get { return _measurement3; }
            set
            {
                _measurement3 = value;
                OnPropertyChanged(nameof(Measurement3));
            }
        }
        public string? Precision
        {
            get { return _precision; }
            set
            {
                _precision = value;
                OnPropertyChanged(nameof(Precision));
            }
        }

        public bool PrecisionIsSelected
        {
            get { return _precisionIsSelected; }
            set
            {
                _precisionIsSelected = value;
                OnPropertyChanged(nameof(PrecisionIsSelected));
            }
        }

/*        
        
        
        private string? _driftPercentage;*/


        public DateTime? DriftDate1
        {
            get { return _driftDate1; }
            set
            {
                _driftDate1 = value;
                OnPropertyChanged(nameof(DriftDate1));
            }
        }

        public TimeOnly? DriftTime1
        {
            get { return _driftTime1; }
            set
            {
                _driftTime1 = value;
                OnPropertyChanged(nameof(DriftTime1));
            }
        }

        public string? DriftConcentration1
        {
            get { return _driftConcentration1; }
            set
            {
                _driftConcentration1 = value;
                OnPropertyChanged(nameof(DriftConcentration1));
            }
        }
        public DateTime? DriftDate2
        {
            get { return _driftDate2; }
            set
            {
                _driftDate2 = value;
                OnPropertyChanged(nameof(DriftDate2));
            }
        }
        public TimeOnly? DriftTime2
        {
            get { return _driftTime2; }
            set
            {
                _driftTime2 = value;
                OnPropertyChanged(nameof(DriftTime2));
            }
        }
        public string? DriftConcentration2
        {
            get { return _driftConcentration2; }
            set
            {
                _driftConcentration2 = value;
                OnPropertyChanged(nameof(DriftConcentration2));
            }
        }
        public string? DriftPercentage
        {
            get { return _driftPercentage; }
            set
            {
                _driftPercentage = value;
                OnPropertyChanged(nameof(DriftPercentage));
            }
        }

        public bool DriftIsSelected
        {
            get { return _driftIsSelected; }
            set
            {
                _precisionIsSelected = value;
                OnPropertyChanged(nameof(DriftIsSelected));
            }
        }
        public string TankLevelColor
        {
            get { return _tankLevelColor; }
            set
            {
                if (_tankLevelColor != value)
                {
                    _tankLevelColor = value;
                    OnPropertyChanged(nameof(TankLevelColor));
                }
            }
        }


        private void UpdateTankLevelColor()
        {
            if (TankLevel < 20)
            {
                TankLevelColor = "Red";
            }
            else if (TankLevel < 50)
            {
                TankLevelColor = "Yellow";
            }
            else if (TankLevel <= 100)
            {
                TankLevelColor = "Green";
            }
            else
            {
                // Set a default color if TankLevel is out of the specified ranges
                TankLevelColor = "Black"; // You can set any default color here
            }
        }



        private void CalculateDaysUntilExpiry()
        {
            if (ExpiryDate.HasValue)
            {
                TimeSpan difference = ExpiryDate.Value - DateTime.Today;
                DaysUntilExpiry = (int)difference.TotalDays;
                if (DaysUntilExpiry < 5) {
                    DaysUntilExpiry = 0;
                } 
                else if (DaysUntilExpiry < 60) {
                    DaysUntilExpiry = 1;
                } else { DaysUntilExpiry = 2; }
            }
            else
            {
                DaysUntilExpiry = 0;
            }
        }



        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // IEditableObject implementation
        public void BeginEdit() { }

        public void CancelEdit() { }

        public void EndEdit() { }
    }
}
