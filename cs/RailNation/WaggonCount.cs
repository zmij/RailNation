using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace RailNation
{
    public class WaggonCount : INotifyPropertyChanged
    {
        private User owner_;
        private ProductType type_;
        private int count_ = 0;
        private int maxHaul_ = 0;

        public WaggonCount(User owner, ProductType type)
        {
            owner_ = owner;
            type_ = type;
        }

        [Browsable(false)]
        public User Owner
        {
            get
            {
                return owner_;
            }
        }

        public int Era
        {
            get
            {
                return type_.era();
            }
        }

        public ProductType Type
        {
            get
            {
                return type_;
            }
        }

        public int Count
        {
            get
            {
                return count_;
            }
            set
            {
                if (count_ != value)
                {
                    count_ = value;
                    OnPropertyChanged("Count");
                }
            }
        }

        public int MaxHaul
        {
            get
            {
                return maxHaul_;
            }
            set
            {
                if (maxHaul_ != value)
                {
                    maxHaul_ = value;
                    OnPropertyChanged("MaxHaul");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                if (Thread.CurrentThread.IsBackground)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                else
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }
    }
}
