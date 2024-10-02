using System.ComponentModel;
using System.Windows.Input;

namespace HttpTask;

public class RelayCommand : ICommand
{
	private Action<object> execute;
	private Func<object, bool> executeFunc;

	public event EventHandler CanExecuteChanged
	{
	add { CommandManager.RequerySuggested+=value;}
    remove {  CommandManager.RequerySuggested -= value;}
		
	}
   

	public RelayCommand(Action<object> execute, Func<object, bool> executeFunc=null)
	{
		this.execute=execute;
		this.executeFunc=executeFunc;
	}

	public bool CanExecute(object parameter)=> this.executeFunc==null||this.executeFunc(parameter);

	public void Execute(object parameter)
	{
		this.execute(parameter);
	}

}
