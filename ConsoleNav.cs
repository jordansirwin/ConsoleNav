using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace JSI
{
    public class ConsoleNav
    {
        static readonly object LockUiObject = new object();

        public void DrawError(string errorMessage)
        {
            lock (LockUiObject)
            {
                WriteLine(errorMessage, ConsoleColor.DarkGray, ConsoleColor.DarkRed);
                Console.WriteLine();
            }
        }

        public void DrawHeader(string title)
        {
            WriteLine(title, ConsoleColor.Yellow);
        }

        public void DrawSubtleText(string text)
        {
            WriteLine(text, ConsoleColor.DarkGray);
        }

        public void DrawSystemText(string text)
        {
            WriteLine(text, ConsoleColor.White, ConsoleColor.DarkGray);
        }

        public void DrawText(string text)
        {
            WriteLine(text, ConsoleColor.Gray);
        }

        public void RunMenu(ConsoleMenu menu)
        {
            while (true)
            {
                DrawMenu(menu);
                var selection = WaitForMenuSelection(menu);
                if (selection == menu.ExitMenuItem)
                    break;
            }
        }

        #region Private Parts
        protected void WriteLine(string formatText, ConsoleColor foreColor, params object[] args)
        {
            Write(formatText, foreColor, Console.BackgroundColor, true, args);
        }

        protected void WriteLine(string formatText, ConsoleColor foreColor, ConsoleColor backColor, params object[] args)
        {
            Write(formatText, foreColor, backColor, true, args);
        }

        protected void Write(string formatText, ConsoleColor foreColor, params object[] args)
        {
            Write(formatText, foreColor, Console.BackgroundColor, false, args);
        }

        protected void Write(string formatText, ConsoleColor foreColor, ConsoleColor backColor, params object[] args)
        {
            Write(formatText, foreColor, backColor, false, args);
        }

        protected void Write(string formatText, ConsoleColor foreColor, ConsoleColor backColor, bool withNewline, params object[] args)
        {
            lock (LockUiObject)
            {
                var origBgColor = Console.BackgroundColor;
                var origColor = Console.ForegroundColor;

                Console.ForegroundColor = foreColor;
                Console.BackgroundColor = backColor;

                Console.Write(formatText, args);
                if (withNewline)
                    Console.WriteLine();

                Console.ForegroundColor = origColor;
                Console.BackgroundColor = origBgColor;
            }
        }

        protected
            void DrawMenuTitle(string title)
        {
            title = string.Format("{0}\r\n{1}", title, "".PadRight(title.Length, '-'));
            WriteLine(title, ConsoleColor.Green);
        }

        protected void DrawMenu(ConsoleMenu menu)
        {
            lock (LockUiObject)
            {
                Console.WriteLine();
                DrawMenuTitle(menu.Title);

                if (menu.MenuItems == null)
                    menu.MenuItems = new List<ConsoleMenuItem>();

                // loop through items (adjusted for special items like exit)
                for(var i = 0; i <= menu.MenuItems.Count; i++)
                {
                    // exit item is first
                    var menuItem = i == 0
                        ? menu.ExitMenuItem
                        : menu.MenuItems[i - 1];

                    // default to index value if no selection is provided
                    if (string.IsNullOrWhiteSpace(menuItem.MenuSelection))
                        menuItem.MenuSelection = i.ToString();

                    DrawMenuItem(menuItem);
                }
            }
        }

        protected void DrawMenuItem(ConsoleMenuItem menuItem)
        {
            lock (LockUiObject)
            {
                Write("{0}) ", ConsoleColor.Green, menuItem.MenuSelection);
                WriteLine(menuItem.Label, ConsoleColor.DarkGreen);
            }
        }
    
        protected ConsoleMenuItem WaitForMenuSelection(ConsoleMenu menu)
        {
            lock (LockUiObject)
            {
                Console.WriteLine();
                Write("{0}> ", ConsoleColor.DarkGray, menu.Title);
            }

            var selection = (Console.ReadLine() ?? string.Empty).Trim();

            var matchingItem = menu.MenuItems
                .FirstOrDefault(m => m.MenuSelection.Equals(selection, StringComparison.InvariantCultureIgnoreCase));

            if(matchingItem != null)
            {
                DrawSystemText(string.Format("Selected: {0}", matchingItem.Label));
                Console.WriteLine();

                if (matchingItem.Method != null)
                    matchingItem.Method(menu, matchingItem);

                return matchingItem;
            }

            if(selection.Equals(menu.ExitMenuItem.MenuSelection, StringComparison.InvariantCultureIgnoreCase))
            {
                DrawSystemText(string.Format("Selected: {0}", menu.ExitMenuItem.Label));
                Console.WriteLine();
                return menu.ExitMenuItem;
            }

            DrawError("Invalid menu option.");
            return null;
        }
        #endregion
    }

    public class ConsoleMenu
    {
        public ConsoleMenuItem ExitMenuItem { get; set; }
        public string Title { get; set; }
        public IList<ConsoleMenuItem> MenuItems { get; set; }

        public ConsoleMenu()
        {
            ExitMenuItem = new ConsoleMenuItem
            {
                Label = "Quit"
            };
        }
    }

    public class ConsoleMenuItem
    {
        public string MenuSelection { get; set; }
        public string Label { get; set; }
        public Action<ConsoleMenu, ConsoleMenuItem> Method { get; set; }
    }
}
