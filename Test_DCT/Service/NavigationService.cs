
using System.Windows.Controls;

namespace CoinTracker.Service;

    public interface INavigationService
    {
        void NavigateTo(Page page);
        void GoBack();
    }

    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateTo(Page page)
        {
            _frame.Navigate(page);
        }
        public void GoBack()
        {
            if (_frame.CanGoBack)
                _frame.GoBack();
        }
    }

