namespace MauiTestApp;

public class MyFirstDrawable : IDrawable 
{
    int countThatISaved = 0;

    public void DrawAnotherLine(int count) {
        countThatISaved = count;
    }
    public void Draw(ICanvas canvas, RectangleF dirtyRect) 
    {
        canvas.StrokeColor = Colors.Red;
		canvas.StrokeSize = 6;
		canvas.DrawLine(countThatISaved * 10, 100, 400, 400);
    }
}