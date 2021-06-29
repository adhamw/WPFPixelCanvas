# The WPFPixelCanvas
Whenever learning a new programming language, I like to play with graphics. 
Learning the basics of a language constructs trhough building a Calculator app, is not fun at all.
Building a working fractal or plasma or some other graphical effect, now those have the potential to engage.

### The problem with Fast pixel plotting
The available API methods are too slow for anything more than working with a static image.
You may at some point want to look into 3rd party libraries ( e.g. SFML.net ), but wouldn't it be nice
if we could avoid that while still learning the basics of the language.

### The problem with WPF
WPF and XAML solved some issues, but they brought on some new headaches of their own.
Specifically, getting your head around MVVM can be quite the head-scratcher for someone just learning C#.

### To the rescue
Well not really. A little bit maybe.

This project solves two things.
- It provides a template for fast pixel plotting in WPF (Its main purpose) 
- It provides a WPF application example that abides (mostly) by MVVM principles 

I am not a MVVM puritan by any standard, but there's value in at least trying to follow well proven standards.
In the spirit of keeping things simple, I have not used any MVVM frameworks. There are MVVM topics that are not addressed 
at all, but it should provide a starting point for anyone trying to get started with WPF/MVVM.

Similarly, I have tried to keep it simple for for the pixel plotting part. It's all .NET/WPF, no 3rd party libraries.

# CONTENTS
```
├── Canvas  
│   │
│   ├── Models  
│   │   ├──── CCanvasManager.cs  ##   Manages the graphics buffer. Allows binding to <Image>  
│   │   ├──── CPattern1.cs       ##   Example plotting code module  
│   │   ├──── CPattern2.cs       ##   Ditto  
│   │   ├──── CPattern3.cs       ##   Ditto  
│   │   └──── ICanvasPlotter.cs  ##   Interface definition for the plotting module implementation  
│   │  
│   ├── ViewModels  
│   │   └──── ViewModel.cs       ##   Defines CanvasManager and Command objects  
│   │   
│   └── Views  
│       └──── MainWindow.cs      ##   The screen space accompanied by start/stop buttons.  
│   
└── Common  
    ├──── CRelayCommand.cs              ##   Used to create bindable commands properties in the ViewModel.  
    └──── CNotificationPropertyBase.cs  ##   Base for a property that can notify the UI when its value changes  
```

# REQUIREMENTS
The solution is built in Visual Studio Express 2019.
It is built and tested with .NET 5.0, but it should work with very few modifications in earlier .NET versions.

# USAGE ( Pixel plotting )
Let us assume you only want to try out some basic C# constructs ( but in a fun way ).
In this case, you can safely ignore everything but the plotting modules ( which is where the fun stuff takes place ).

## Selecting the plotting module
First have a look in the Canvas/Models/ folder in the project. I have provided a few examples ( CPattern1, CPattern2 etc. )

To make use of either of them, open "Canvas/ViewModels/Viewmode.cs".
Have a look at the constructor. You will find these lines:
```
    //Various plot component implementations
    //CPattern1 plotter = new CPattern1(width, height);   // Makes color patterns based on x,y position
    //CPattern2 plotter = new CPattern2(width, height);   // Makes wavy patterns 
    CPattern3 plotter = new CPattern3(width, height);     // Makes random dots
```
Comment out the current line ( CPattern3 ) and uncomment one of the others to change plotting module.
Recompile and run to see the plotting module in action.

# Building your own plotting module ( Pixel plotting )

## Setting up the scaffolding
- Create a new class in the Canvas/Models folder ( E.g. CMyPattern )
- Your class should implement the ICanvasPlotter inteface

It should look something like this:
```
    public class CMyPattern : ICanvasPlotter
    {
        //Public properties
        public int Width { get; private set; }
        public int Height { get; private set; }

        //Constructor
        public CPattern1(int width, int height)
        {
            Width = width;      // Defines width of plot area ( e.g. 800 pixels )
            Height = height;    // Defines height of plot area ( e.g. 600 pixels )
        }

        //Public interface
        public void plot(IntPtr buffer, int bytesperpixel, long refreshcounter = 0)
        {
            ... // Your code goes here
        }
    }
```

## Traversing image coordinates 
The plot function is where we want to write our plotting code.
For example; let us fill the whole screen with a blue color.
To do what we modify the plot() function as follows:
```
        public void plot(IntPtr buffer, int bytesperpixel, long refreshcounter = 0)
        {
            int pos = 0;
            unsafe
            {
                // Retrieve a byte pointer to our graphics buffer
                var bytes = (byte*)buffer.ToPointer();

                //Run through all lines in image
                for (int y = 0; y < Height; y++)
                {
                    //Run through every pixel on a line
                    for (int x = 0; x < Width; x++)
                    { ... our color assignment code goes here ... }
                }

            }


        }
```

This setup traverses every x and y value in our image.
There's a couple of mysterious things that begs explanation.

This line extracts a direct pointer to our graphcis buffer:
```
  var bytes = (byte*)buffer.ToPointer();
```
This is the very heart of the speed attributed to this solution.
It is not important that you understand exactly what it does, just realize that you can use the 'bytes' variable to directly access
the graphics buffer.

We need to notify .NET that we are operating in an unsafe manner ( we are using byte pointers ):
```
  unsafe
  {
     ... Code that involves pointer operations ...
  }
```

## The buffer layout

We now have code that traverses every coordinate (x,y) in our image, and we have direct access to the graphics buffer.
Our graphics buffer is laid out as follows:
```
    ( Buffer )       [b][g][r][a][b][g][r][a][b][g][r][a]...
    ( pointer value) [0][1][2][3][4][5][6][7][8][9]...
    ( pixel )        |- Pixel 1 -|- Pixel 2 -|- Pixel 3 -| ...
    ( x-coordinate ) |-   x=0   -|-   x=1   -|-   x=2   -| ...
```

## The pixel data
For every pixel, there are four values:
```
 [b] <= [0..255]  // Blue component
 [g] <= [0..255]  // Green component
 [r] <= [0..255]  // Red component
 [a] <= [0..255]  // Alpha component
```
The component values [r],[g] and [b] define how much each contributes to the final value. A value of '0' means the component does not contribute.
A value of '255' means the it contributes to the max.

For example, the color red would be defined as such:
```
 [b] = 0    // Blue component does not contribute at all
 [g] = 0    // Green  component does not contribute at all
 [r] = 255  // Blue component contribution at max.
```
The [a] component is the alpha channel. It defines the opacity of the color. A value of 255 means the color is completely opaque.
A value of 0 means it is completely transparent. Usually we will therefore leave it at 255.

## The pixel data
The variable 'pos' is the pointer into our buffer. If the value is '0' it points at the first byte in our buffer.
Let us now assume our 'pos' variable is pointing at the start of 'Pixel 2':
```
                                  ↓  ( pos = 4)
    ( Buffer )       [b][g][r][a][b][g][r][a][b][g][r][a]...
    ( pointer value) [0][1][2][3][4][5][6][7][8][9]...
    ( pixel )        |- Pixel 1 -|- Pixel 2 -|- Pixel 3 -| ...
    ( x-coordinate ) |-   x=0   -|-   x=1   -|-   x=2   -| ...
```

If we wish to set the color of Pixel 2 to blue, we would assign values as follows:
```
 Buffer at (pos+0) = 255    // Sets the [b] component value --> 100% contribution from Blue component
 Buffer at (pos+1) = 0      // Sets the [g] component value --> 0% contribution from Green component
 Buffer at (pos+2) = 0      // Sets the [r] component value --> 0% contribution from Red component
 Buffer at (pos+3) = 255    // Sets the [a] component value --> 100% opaque ( not transparent )
```

We do this in code as follows:
```
  // Setting the color to Black, for the pixel at (x,y):
  bytes[pos + 0] = 255;   //Blue component
  bytes[pos + 1] = 0;     //Green component 
  bytes[pos + 2] = 0;     //Red component 
  bytes[pos + 3] = 255;   //Alpha component

  pos += bytesperpixel;   // Moving buffer pointer forward
```
At the last line, we add the value of bytesperpixel to the pointer ( pos ).
In our case bytesperpixel = 4, so the pointer now points to the first color component of the next pixel.
```
                                              ↓  ( pos = 8)
    ( Buffer )       [b][g][r][a][b][g][r][a][b][g][r][a]...
    ( pointer value) [0][1][2][3][4][5][6][7][8][9]...
    ( pixel )        |- Pixel 1 -|- Pixel 2 -|- Pixel 3 -| ...
    ( x-coordinate ) |-   x=0   -|-   x=1   -|-   x=2   -| ...
```

If we add this code to our previous code for the plot function and compbine 
it with the class code, we get the final product:

```
  public class CMyPattern : ICanvasPlotter
  {
  |      //Public properties
  |     public int Width { get; private set; }
  |     public int Height { get; private set; }
  |
  |     //Constructor
  |     public CPattern1(int width, int height)
  |     {
  |         Width = width;      // Defines width of plot area ( e.g. 800 pixels )
  |         Height = height;    // Defines height of plot area ( e.g. 600 pixels )
  |     }
  |
  |     //Public interface
  |     public void plot(IntPtr buffer, int bytesperpixel, long refreshcounter = 0)
  |     {
  |     |   int pos = 0;
  |     |   unsafe
  |     |   {
  |     |       // Retrieve a byte pointer to our graphics buffer
  |     |       var bytes = (byte*)buffer.ToPointer();
  |     |
  |     |       //Run through all lines in image
  |     |       for (int y = 0; y < Height; y++)
  |     |       {
  |     |       |   //Run through every pixel on a line
  |     |       |   for (int x = 0; x < Width; x++)
  |     |       |   { 
  |     |       |       // Setting the color to Black, for the pixel at (x,y):
  |     |       |       bytes[pos + 0] = 255;   //Blue component
  |     |       |       bytes[pos + 1] = 0;     //Green component 
  |     |       |       bytes[pos + 2] = 0;     //Red component 
  |     |       |       bytes[pos + 3] = 255;   //Alpha component
  |     |       |
  |     |       |       pos += bytesperpixel;   // Moving buffer pointer forward
  |     |       |   }
  |     |       }
  |     |   }
  |     }
  }

```

If this new class was saved as "Canvas/Models/CMypattern.cs", you could now
head over to the "Canvas/ViewModels/ViewModel.cs" constructor.

Comment out the previous plotter object, and add your own:

```
    //Various plot component implementations
    //CPattern1 plotter = new CPattern1(width, height);   // Makes color patterns based on x,y position
    //CPattern2 plotter = new CPattern2(width, height);   // Makes wavy patterns 
    //CPattern3 plotter = new CPattern3(width, height);   // Makes random dots
    CMyPattern plotter = new CMyPattern(width, height);   // Your custom pattern
```

Compile and run it, press the "Run once" button and you should see a blue screen.








