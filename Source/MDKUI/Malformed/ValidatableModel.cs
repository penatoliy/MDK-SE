using System;
using System.Collections;
using System.ComponentModel;

namespace Malware.MDKUI.Malformed
{
    public abstract class ValidatableModel : Model, INotifyDataErrorInfo
    {
        protected ValidatableModel()
        {
            var validation = new ModelValidationManager();
            validation.ErrorsChanged += OnErrorsChanged;
            Validation = validation;
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected ModelValidationManager Validation { get; }

        bool INotifyDataErrorInfo.HasErrors => Validation.HasErrors();

        void OnErrorsChanged(object sender, DataErrorsChangedEventArgs e) => OnErrorsChanged(e);

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName) => Validation.GetErrors(propertyName);
    }
}
