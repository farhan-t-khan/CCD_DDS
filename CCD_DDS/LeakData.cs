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
        private bool _isSelected;
        private string _status;
        private DateTime? _precisionDate;
        private TimeSpan? _precisionTime;
        private string? _measurement1;
        private string? _measurement2;
        private string? _measurement3;
        private string? _precision;

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

        public DateTime? ExpiryDate
        {
            get { return _expiryDate; }
            set
            {
                _expiryDate = value;
                OnPropertyChanged(nameof(ExpiryDate));
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
