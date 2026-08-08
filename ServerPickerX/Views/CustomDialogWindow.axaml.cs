using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ServerPickerX.Views
{
    public partial class CustomDialogWindow : Window
    {
        public string TitleText { get; set; } = "";
        public string Message { get; set; } = "";
        public string IconValue { get; set; } = "fa-info-circle";
        public string IconColor { get; set; } = "#00f2ff";
        public bool ShowCancel { get; set; } = false;
        public string OkText { get; set; } = "OK";
        public string CancelText { get; set; } = "Cancel";

        public CustomDialogWindow()
        {
            InitializeComponent();
        }

        protected override void OnOpened(System.EventArgs e)
        {
            base.OnOpened(e);
            DataContext = this;
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            Close("Ok");
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close("Cancel");
        }
    }
}
