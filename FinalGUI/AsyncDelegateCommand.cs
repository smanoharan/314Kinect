using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace FinalGUI
{
    /// <summary>
    /// An async version for delegate command
    /// </summary>
    public class AsyncDelegateCommand : ICommand
    {
        readonly BackgroundWorker worker = new BackgroundWorker();
        readonly Func<bool> canExecute;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="action">The action to be executed</param>
        /// <param name="canExecute">Will be used to determine if the action can be executed</param>
        /// <param name="completed">Will be invoked when the action is completed</param>
        /// <param name="error">Will be invoked if the action throws an error</param>
        public AsyncDelegateCommand(Action action,
                                    Func<bool> canExecute = null,
                                    Action<object> completed = null,
                                    Action<Exception> error = null
                                    )
        {

            worker.DoWork += (s, e) =>
            {
                CommandManager.InvalidateRequerySuggested();
                action();
            };

            worker.RunWorkerCompleted += (s, e) =>
            {

                if (completed != null && e.Error == null)
                    completed(e.Result);

                if (error != null && e.Error != null)
                    error(e.Error);

                CommandManager.InvalidateRequerySuggested();
            };

            this.canExecute = canExecute;
        }


        /// <summary>
        /// To cancel an ongoing execution
        /// </summary>
        public void Cancel()
        {
            if (worker.IsBusy)
                worker.CancelAsync();
        }

        /// <summary>
        /// Note that this will return false if the worker is already busy
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return (canExecute == null) ?
                    !(worker.IsBusy) : !(worker.IsBusy)
                        && canExecute();
        }

        /// <summary>
        /// Let us use command manager for thread safety
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Here we'll invoke the background worker
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            worker.RunWorkerAsync();
        }
    }
}
