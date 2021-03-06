﻿using CognitiveLocator.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CognitiveLocator.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public IDependencyService DependencyService;
        protected readonly INavigationService NavigationService;

        private bool isBusy = false;

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        private string title = string.Empty;

        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public BaseViewModel(IDependencyService dependencyService)
        {
            DependencyService = dependencyService;
            NavigationService = DependencyService.Get<INavigationService>();
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName]string propertyName = "",
            Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        public virtual Task OnViewAppear()
        {
            return null;
        }

        public virtual Task OnViewDissapear()
        {
            return null;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChanged
    }
}