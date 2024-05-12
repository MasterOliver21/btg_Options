namespace BTGOpcoes.Views;

public partial class OptionsCalcPage : ContentPage
{
	public OptionsCalcPage()
	{
		InitializeComponent();
	}

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue))
        {
            if(!double.TryParse(e.NewTextValue, out _)) ((Entry)sender).Text = e.OldTextValue;
        }
    }
}