using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HealthInstitution.TableClass
{
    class EquipmentReviewFilterObject : INotifyPropertyChanged
    {
        private bool _isChecked;
        private string _title;
        public bool IsChecked {
            get { return _isChecked; }
            set { _isChecked = value; NotifyPropertyChanged(); }
        }
        public string Title {
            get { return _title; }
            set { _title = value; NotifyPropertyChanged(); }
        }

        public EquipmentReviewFilterObject(string title) {
            IsChecked = true;
            Title = title;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
