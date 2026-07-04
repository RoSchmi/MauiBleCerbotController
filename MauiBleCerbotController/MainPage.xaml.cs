
using MauiBleCerbotController.ViewModels;
using MauiBleCerbotController;
using MauiBleCerbotController.Infrastructure;

namespace MauiBleCerbotController
{
    public partial class MainPage : BaseContentPage<MainPageViewModel>
    {
        
      //  private readonly MainPageViewModel vm;
        

        public MainPage(MainPageViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
         
        }   
    }
}
