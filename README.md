# Unions

## Easy to use discriminated unions for C#

The `OneOf` data structure is used to hold a single value of one of a fixed set of Types.

###  Example

    Rectangle rect = new Rectangle(10, 10, 10, 10);  
    OneOf<Rectangle, Circle, Prism> shape = rect;

You can then access the value using .IsT0 and .AsT0

    Assert.True(shape.IsT0);  
    Assert.True(shape.AsT0 is Rectangle);

You use the `match(func f0, ... func fn)` method to perform an action e.g.

    OneOf<Rectangle, Circle, Prism> shape = ...;
    shape.Match(
      rect => gfx.DrawRect(rect.X, rect.Y, rect.Width, rec.Height), 
      circle => gfx.DrawCircle(circle.X, circle.Y, circle.Radius),
      prism => ... 
    ); 

... or convert to another value

    OneOf<Rectangle, Circle, Prism> shape = ...;
    Int32 area = shape.Match(
      rect => rect.Width * rec.Height, 
      circle => 2 * Math.Pi * circle.Radius,
      prism => ... 
    ); 

*/
