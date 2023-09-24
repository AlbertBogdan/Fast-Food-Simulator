using FastFoodSimulatorLibrary.Order;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Employee_
{
    public class Person : INotifyPropertyChanged
    {
        private bool isWork;

        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }

        public bool IsWork
        {
            get { return isWork; }
            set
            {
                if (isWork != value)
                {
                    isWork = value;
                    OnPropertyChanged("IsWork"); // Передаем имя свойства "IsWork" строкой
                }
            }
        }

        public Person(string Name)
        {
            this.Name = Name;
            IsWork = false;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
