using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Malware.MDKUI.Malformed
{
    public class ModelValidationManager : IEnumerable<ModelValidationError>
    {
        Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public void AddError(string propertyName, string error)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            lock (_errors)
            {
                if (!_errors.TryGetValue(propertyName, out var errorList))
                    _errors[propertyName] = errorList = new List<string>();
                errorList.Add(error ?? "");
            }
            OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
        }

        public bool HasErrors(string propertyName = null)
        {
            lock (_errors)
            {
                if (string.IsNullOrEmpty(propertyName))
                    return _errors.Values.Any(l => l.Count > 0);
                if (_errors.TryGetValue(propertyName, out var errorList))
                    return errorList.Count > 0;
                return false;
            }
        }

        public IEnumerable<string> GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            lock (_errors)
            {
                if (string.IsNullOrEmpty(propertyName))
                    return _errors.Values.SelectMany(l => l).ToArray();
                if (_errors.TryGetValue(propertyName, out var errorList))
                    return errorList.ToArray();
                return new string[0];
            }
        }

        public void AddErrors(string propertyName, params string[] errors) => AddErrors(propertyName, (IEnumerable<string>)errors);

        public void AddErrors(string propertyName, IEnumerable<string> errors)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            lock (_errors)
            {
                if (!_errors.TryGetValue(propertyName, out var errorList))
                    _errors[propertyName] = errorList = new List<string>();
                foreach (var error in errors)
                    errorList.Add(error ?? "");
                OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
            }
        }

        public void Clear(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException(nameof(propertyName));
            lock (_errors)
            {
                if (_errors.TryGetValue(propertyName, out var errorList))
                {
                    errorList.Clear();
                    OnErrorsChanged(new DataErrorsChangedEventArgs(propertyName));
                }
            }
        }

        protected virtual void OnErrorsChanged(DataErrorsChangedEventArgs e)
        {
            ErrorsChanged?.Invoke(this, e);
        }

        public IEnumerator<ModelValidationError> GetEnumerator()
        {
            ModelValidationError[] errors;
            lock (_errors)
                errors = _errors.SelectMany(e => e.Value.Select(v => new ModelValidationError(e.Key, v))).ToArray();
            return (IEnumerator<ModelValidationError>)errors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}