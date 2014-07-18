ConsoleNav
==========

A C# helper to easily manage navigation and menu's for Console Apps.



### Usage Example ###

![Example Menu][/demo/demo_1.png]

```
using System;
using JSI;

namespace SpecialToolsConsole
{
    class Program
    {
        private static ConsoleNav _console;

        static void Main(string[] args)
        {
            _console = new ConsoleNav();

            _console.DrawHeader("** Special Tools Console **");
            _console.RunMenu(CreateMainMenu());
            _console.DrawHeader("Exiting! (Press any key to close this window)");

            Console.ReadLine();
        }

        public static ConsoleMenu CreateMainMenu()
        {
            return new ConsoleMenu
            {
                Title = "Main Menu",
                MenuItems = new[]{
                    new ConsoleMenuItem {
                        Method = DisplayTest,
                        Label = "Display test text"
                    },
                    new ConsoleMenuItem {
                        Method = Submenu,
                        Label = "Goto submenu"
                    }}
            };
        }

        public static ConsoleMenu CreateSubMenu(string title)
        {
            return new ConsoleMenu
            {
                ExitMenuItem = new ConsoleMenuItem{
                    Label = "Back"
                },
                Title = title,
                MenuItems = new[]{
                    new ConsoleMenuItem {
                        Method = DisplayTest,
                        Label = "Test"
                    },
                    new ConsoleMenuItem {
                        Method = Submenu,
                        Label = "Goto submenu"
                    }}
            };
        }

        public static void DisplayTest(ConsoleMenu menu, ConsoleMenuItem selectedItem)
        {
            Console.WriteLine("TEST!");
        }

        public static void Submenu(ConsoleMenu menu, ConsoleMenuItem selectedItem)
        {
            _console.RunMenu(CreateSubMenu(menu.Title + ":Submenu"));
        }
    }
}
