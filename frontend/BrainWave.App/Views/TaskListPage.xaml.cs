using BrainWave.App.ViewModels;

namespace BrainWave.App.Views;

public partial class TaskListPage : ContentPage
{
	public TaskListPage(TaskListViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}

	private void OnAddTaskClicked(object sender, EventArgs e)
	{
		if (BindingContext is TaskListViewModel vm)
		{
			vm.AddTaskCommand.Execute(null);
		}
	}
}
