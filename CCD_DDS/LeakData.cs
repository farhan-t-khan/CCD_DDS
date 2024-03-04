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

        /*        public bool IsSelected
                {
                    get { return _isSelected; }
                    set
                    {
                        // Only allow setting IsSelected to true if Port is not 0
                        if (Port != "0" || value == true)
                        {
                            _isSelected = value;
                            OnPropertyChanged(nameof(IsSelected));
                        }
                    }
                }*/
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
