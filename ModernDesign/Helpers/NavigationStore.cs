using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernDesign.Helpers
{
    internal class NavigationStore
    {
        public event Action? currentViewModelChanged;

        private BaseValueModel? _currentView;

        public BaseValueModel CurrentViewModel
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnCurrentViewModelChanged();
            }
        }

        private void OnCurrentViewModelChanged()
        {
            currentViewModelChanged = ;
        }
    }
}
