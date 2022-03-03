namespace MauiTestApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;
		CounterLabel.Text = $"Count count count: {count}";

		SemanticScreenReader.Announce(CounterLabel.Text);
		var canvas = AGoodOlGraphicView.Drawable as MyFirstDrawable;
		canvas.DrawAnotherLine(count);
	}
}

