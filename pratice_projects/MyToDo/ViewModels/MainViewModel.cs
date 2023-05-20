using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyToDo.Common.Models;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using Prism.Commands;
using Prism.Regions;
using MyToDo.Extentions;

namespace MyToDo.ViewModels
{
    public class MainViewModel : BindableBase,INavigationAware
    {
        private ObservableCollection<MenuBarModel> menuBars;
        public ObservableCollection<MenuBarModel> MenuBars
        {
            get { return menuBars; }
            set { menuBars = value; RaisePropertyChanged(); }
        }
        public MainViewModel(IRegionManager regionManager)
        {
            MenuBars= new ObservableCollection<MenuBarModel>();
            CreateMenuBar();
            NavigateCommand = new DelegateCommand<MenuBarModel>(Navigate);
            this.regionManager = regionManager;
            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoBack)
                {
                    journal.GoBack();
                }
            });

            GoForwardkCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                {
                    journal.GoForward();
                }
            });
        }
        private void Navigate(MenuBarModel obj) 
        {
            if(obj==null || string.IsNullOrWhiteSpace(obj.NameSpace))
            {
                return;
            }

            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(obj.NameSpace, back =>
            {
                journal = back.Context.NavigationService.Journal;
            });

           
        }
        public DelegateCommand<MenuBarModel> NavigateCommand{  get;private set; }
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardkCommand { get; private set; }
        public readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;
        private void CreateMenuBar()
        {
            MenuBars.Add(new MenuBarModel { Icon = "Home", Title = "首页", NameSpace = "IndexView" });
            MenuBars.Add(new MenuBarModel { Icon = "NotebookOutline", Title = "待办事项", NameSpace = "ToDoView" });
            MenuBars.Add(new MenuBarModel { Icon = "NotebookPlus", Title = "备忘录", NameSpace = "MemoView" });
            MenuBars.Add(new MenuBarModel { Icon = "Cog", Title = "设置", NameSpace = "SettingsView" });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            string uri = navigationContext.Uri.ToString();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }

}
